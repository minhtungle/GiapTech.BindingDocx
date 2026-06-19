using System.Data;
using Dapper;
using GiapTech.BindingDocx.Domain.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GiapTech.BindingDocx.Infrastructure.Data;

public class DatabaseMigrator(IDbConnectionFactory connectionFactory, IConfiguration configuration, ILogger<DatabaseMigrator> logger)
{
    public async Task MigrateAsync()
    {
        await EnsureDatabaseCreatedAsync();

        using var connection = connectionFactory.CreateConnection();
        connection.Open();

        logger.LogInformation("Running database migrations...");

        await CreateMigrationsTableAsync(connection);
        await RunMigrationsAsync(connection);

        logger.LogInformation("Database migrations completed.");
    }

    private async Task EnsureDatabaseCreatedAsync()
    {
        var originalCs = configuration.GetConnectionString("DefaultConnection")!;
        var builder = new SqlConnectionStringBuilder(originalCs);
        var dbName = builder.InitialCatalog;
        builder.InitialCatalog = "master";

        using var masterConnection = new SqlConnection(builder.ConnectionString);
        await masterConnection.OpenAsync();

        var exists = await masterConnection.ExecuteScalarAsync<int>(
            "SELECT COUNT(1) FROM sys.databases WHERE name = @name", new { name = dbName });

        if (exists == 0)
        {
            logger.LogInformation("Creating database {DbName}...", dbName);
            await masterConnection.ExecuteAsync($"CREATE DATABASE [{dbName}]");
            logger.LogInformation("Database {DbName} created.", dbName);
        }
    }

    private static async Task CreateMigrationsTableAsync(IDbConnection connection)
    {
        await connection.ExecuteAsync(@"
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='__Migrations' AND xtype='U')
            CREATE TABLE __Migrations (
                Id INT IDENTITY(1,1) PRIMARY KEY,
                Name NVARCHAR(200) NOT NULL UNIQUE,
                AppliedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
            )");
    }

    private async Task RunMigrationsAsync(IDbConnection connection)
    {
        var applied = (await connection.QueryAsync<string>("SELECT Name FROM __Migrations")).ToHashSet();

        foreach (var (name, sql) in GetMigrations())
        {
            if (applied.Contains(name)) continue;

            logger.LogInformation("Applying migration: {Name}", name);
            using var transaction = connection.BeginTransaction();
            try
            {
                foreach (var statement in SplitBatches(sql))
                {
                    if (!string.IsNullOrWhiteSpace(statement))
                        await connection.ExecuteAsync(statement, transaction: transaction);
                }

                await connection.ExecuteAsync(
                    "INSERT INTO __Migrations (Name) VALUES (@Name)",
                    new { Name = name },
                    transaction);

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }

    private static IEnumerable<string> SplitBatches(string sql)
        => sql.Split(["\nGO\n", "\r\nGO\r\n", "\nGO\r\n", "\r\nGO\n"],
               StringSplitOptions.RemoveEmptyEntries);

    private static IEnumerable<(string Name, string Sql)> GetMigrations()
    {
        yield return ("001_InitialSchema", Migration001_InitialSchema);
        yield return ("002_SeedData", Migration002_SeedData);
        yield return ("003_AddRoleToUsers", Migration003_AddRoleToUsers);
    }

    private const string Migration001_InitialSchema = @"
CREATE TABLE Users (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    Username NVARCHAR(100) NOT NULL UNIQUE,
    Email NVARCHAR(200) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(300) NOT NULL,
    RefreshToken NVARCHAR(500) NULL,
    RefreshTokenExpiryTime DATETIME2 NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
)
GO
CREATE TABLE ProfileGroups (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(500) NULL,
    TemplatePath NVARCHAR(500) NULL,
    SortOrder INT NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
)
GO
CREATE TABLE TemplateFiles (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    GroupId UNIQUEIDENTIFIER NOT NULL REFERENCES ProfileGroups(Id),
    Name NVARCHAR(200) NOT NULL,
    FilePath NVARCHAR(1000) NOT NULL,
    FileType NVARCHAR(20) NOT NULL,
    FileSize BIGINT NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
)
GO
CREATE TABLE ImportBatches (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    GroupId UNIQUEIDENTIFIER NOT NULL REFERENCES ProfileGroups(Id),
    Name NVARCHAR(200) NOT NULL,
    Type NVARCHAR(50) NOT NULL,
    TotalRecords INT NOT NULL DEFAULT 0,
    Status NVARCHAR(50) NOT NULL DEFAULT 'pending',
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
)
GO
CREATE TABLE ImportRecords (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    BatchId UNIQUEIDENTIFIER NOT NULL REFERENCES ImportBatches(Id),
    JsonData NVARCHAR(MAX) NOT NULL,
    Status NVARCHAR(50) NOT NULL DEFAULT 'pending',
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
)
GO
CREATE TABLE UserTokens (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    UserId UNIQUEIDENTIFIER NOT NULL REFERENCES Users(Id) UNIQUE,
    CurrentToken INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
)
GO
CREATE TABLE TokenTransactions (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    UserId UNIQUEIDENTIFIER NOT NULL REFERENCES Users(Id),
    Type NVARCHAR(50) NOT NULL,
    Amount INT NOT NULL,
    Description NVARCHAR(500) NULL,
    ReferenceId NVARCHAR(200) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
)
GO
CREATE TABLE TokenPackages (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    Name NVARCHAR(200) NOT NULL,
    TokenAmount INT NOT NULL,
    PricePerToken DECIMAL(18,2) NOT NULL,
    TotalPrice DECIMAL(18,2) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    SortOrder INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
)
";

    private const string Migration003_AddRoleToUsers = @"
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Users') AND name = 'Role')
    ALTER TABLE Users ADD Role NVARCHAR(20) NOT NULL DEFAULT 'user'
GO
UPDATE Users SET Role = 'admin' WHERE Username = 'admin'
";

    private const string Migration002_SeedData = @"
DECLARE @AdminId UNIQUEIDENTIFIER = NEWID()
INSERT INTO Users (Id, Username, Email, PasswordHash, IsActive)
VALUES (@AdminId, 'admin', 'admin@giaptech.vn', '$2a$12$AqAevrmF12xCxFCQg5MPV.R9XHWCfK9aPLHjrFxQZpbx.zb2TtyLC', 1)
GO
INSERT INTO UserTokens (UserId, CurrentToken)
SELECT TOP 1 Id, 100 FROM Users WHERE Username = 'admin'
GO
INSERT INTO ProfileGroups (Name, Description, SortOrder)
VALUES
    (N'Nhóm hồ sơ 1', N'Nhóm hồ sơ mặc định 1', 1),
    (N'Nhóm hồ sơ 2', N'Nhóm hồ sơ mặc định 2', 2),
    (N'Nhóm hồ sơ 3', N'Nhóm hồ sơ mặc định 3', 3),
    (N'Nhóm hồ sơ 4', N'Nhóm hồ sơ mặc định 4', 4),
    (N'Nhóm hồ sơ 5', N'Nhóm hồ sơ mặc định 5', 5),
    (N'Nhóm hồ sơ 6', N'Nhóm hồ sơ mặc định 6', 6)
GO
INSERT INTO TokenPackages (Name, TokenAmount, PricePerToken, TotalPrice, SortOrder)
VALUES
    (N'Gói 1 Token', 1, 10000, 10000, 1),
    (N'Gói 500 Token', 500, 7000, 3500000, 2),
    (N'Gói 1000 Token', 1000, 5000, 5000000, 3)
";
}
