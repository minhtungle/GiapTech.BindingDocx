using Dapper;
using GiapTech.BindingDocx.Domain.Entities;
using GiapTech.BindingDocx.Domain.Interfaces;

namespace GiapTech.BindingDocx.Infrastructure.Repositories;

public class UserRepository(IDbConnectionFactory connectionFactory)
    : BaseRepository<User>(connectionFactory), IUserRepository
{
    protected override string TableName => "Users";

    public async Task<Guid> AddAsync(User entity, CancellationToken ct = default)
    {
        using var conn = ConnectionFactory.CreateConnection();
        await conn.ExecuteAsync(@"
            INSERT INTO Users (Id, Username, Email, PasswordHash, RefreshToken, RefreshTokenExpiryTime, IsActive, Role, CreatedAt, UpdatedAt)
            VALUES (@Id, @Username, @Email, @PasswordHash, @RefreshToken, @RefreshTokenExpiryTime, @IsActive, @Role, @CreatedAt, @UpdatedAt)",
            entity);
        return entity.Id;
    }

    public async Task UpdateAsync(User entity, CancellationToken ct = default)
    {
        using var conn = ConnectionFactory.CreateConnection();
        await conn.ExecuteAsync(@"
            UPDATE Users SET Username=@Username, Email=@Email, PasswordHash=@PasswordHash,
            RefreshToken=@RefreshToken, RefreshTokenExpiryTime=@RefreshTokenExpiryTime,
            IsActive=@IsActive, Role=@Role, UpdatedAt=@UpdatedAt WHERE Id=@Id",
            entity);
    }

    public async Task<Guid> CreateAsync(User entity, CancellationToken ct = default)
        => await AddAsync(entity, ct);

    public async Task UpdateProfileAsync(User entity, CancellationToken ct = default)
    {
        using var conn = ConnectionFactory.CreateConnection();
        await conn.ExecuteAsync(@"
            UPDATE Users SET Username=@Username, Email=@Email, PasswordHash=@PasswordHash,
            IsActive=@IsActive, Role=@Role, UpdatedAt=GETUTCDATE() WHERE Id=@Id",
            entity);
    }

    public async Task<(IEnumerable<User> Items, int Total)> GetPagedAsync(string? search, int skip, int take, CancellationToken ct = default)
    {
        using var conn = ConnectionFactory.CreateConnection();
        var whereClause = string.IsNullOrWhiteSpace(search)
            ? string.Empty
            : "WHERE Username LIKE @Search OR Email LIKE @Search";
        var param = new { Search = $"%{search}%", Skip = skip, Take = take };

        var total = await conn.QuerySingleAsync<int>($"SELECT COUNT(1) FROM Users {whereClause}", param);
        var items = await conn.QueryAsync<User>(
            $"SELECT * FROM Users {whereClause} ORDER BY CreatedAt DESC OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY",
            param);
        return (items, total);
    }

    public async Task<bool> UsernameExistsAsync(string username, Guid? excludeId, CancellationToken ct = default)
    {
        using var conn = ConnectionFactory.CreateConnection();
        var sql = excludeId.HasValue
            ? "SELECT COUNT(1) FROM Users WHERE Username=@Username AND Id<>@ExcludeId"
            : "SELECT COUNT(1) FROM Users WHERE Username=@Username";
        return await conn.QuerySingleAsync<int>(sql, new { Username = username, ExcludeId = excludeId }) > 0;
    }

    public async Task<bool> EmailExistsAsync(string email, Guid? excludeId, CancellationToken ct = default)
    {
        using var conn = ConnectionFactory.CreateConnection();
        var sql = excludeId.HasValue
            ? "SELECT COUNT(1) FROM Users WHERE Email=@Email AND Id<>@ExcludeId"
            : "SELECT COUNT(1) FROM Users WHERE Email=@Email";
        return await conn.QuerySingleAsync<int>(sql, new { Email = email, ExcludeId = excludeId }) > 0;
    }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default)
    {
        using var conn = ConnectionFactory.CreateConnection();
        return await conn.QuerySingleOrDefaultAsync<User>(
            "SELECT * FROM Users WHERE Username = @Username AND IsActive = 1",
            new { Username = username });
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        using var conn = ConnectionFactory.CreateConnection();
        return await conn.QuerySingleOrDefaultAsync<User>(
            "SELECT * FROM Users WHERE Email = @Email AND IsActive = 1",
            new { Email = email });
    }

    public async Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        using var conn = ConnectionFactory.CreateConnection();
        return await conn.QuerySingleOrDefaultAsync<User>(
            "SELECT * FROM Users WHERE RefreshToken = @RefreshToken AND IsActive = 1",
            new { RefreshToken = refreshToken });
    }

    public async Task UpdateRefreshTokenAsync(Guid userId, string? refreshToken, DateTime? expiry, CancellationToken ct = default)
    {
        using var conn = ConnectionFactory.CreateConnection();
        await conn.ExecuteAsync(@"
            UPDATE Users SET RefreshToken=@RefreshToken, RefreshTokenExpiryTime=@Expiry, UpdatedAt=GETUTCDATE()
            WHERE Id=@UserId",
            new { UserId = userId, RefreshToken = refreshToken, Expiry = expiry });
    }
}
