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
            INSERT INTO Users (Id, Username, Email, PasswordHash, RefreshToken, RefreshTokenExpiryTime, IsActive, CreatedAt, UpdatedAt)
            VALUES (@Id, @Username, @Email, @PasswordHash, @RefreshToken, @RefreshTokenExpiryTime, @IsActive, @CreatedAt, @UpdatedAt)",
            entity);
        return entity.Id;
    }

    public async Task UpdateAsync(User entity, CancellationToken ct = default)
    {
        using var conn = ConnectionFactory.CreateConnection();
        await conn.ExecuteAsync(@"
            UPDATE Users SET Username=@Username, Email=@Email, PasswordHash=@PasswordHash,
            RefreshToken=@RefreshToken, RefreshTokenExpiryTime=@RefreshTokenExpiryTime,
            IsActive=@IsActive, UpdatedAt=@UpdatedAt WHERE Id=@Id",
            entity);
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
