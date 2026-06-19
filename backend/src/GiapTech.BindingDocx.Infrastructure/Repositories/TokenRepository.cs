using Dapper;
using GiapTech.BindingDocx.Domain.Entities;
using GiapTech.BindingDocx.Domain.Interfaces;

namespace GiapTech.BindingDocx.Infrastructure.Repositories;

public class TokenRepository(IDbConnectionFactory connectionFactory) : ITokenRepository
{
    public async Task<UserToken?> GetUserTokenAsync(Guid userId, CancellationToken ct = default)
    {
        using var conn = connectionFactory.CreateConnection();
        return await conn.QuerySingleOrDefaultAsync<UserToken>(
            "SELECT * FROM UserTokens WHERE UserId = @UserId", new { UserId = userId });
    }

    public async Task<int> GetCurrentBalanceAsync(Guid userId, CancellationToken ct = default)
    {
        using var conn = connectionFactory.CreateConnection();
        return await conn.QuerySingleOrDefaultAsync<int>(
            "SELECT ISNULL(CurrentToken, 0) FROM UserTokens WHERE UserId = @UserId",
            new { UserId = userId });
    }

    public async Task DeductTokensAsync(Guid userId, int amount, string description, CancellationToken ct = default)
    {
        using var conn = connectionFactory.CreateConnection();
        conn.Open();
        using var transaction = conn.BeginTransaction();
        try
        {
            var balance = await conn.QuerySingleOrDefaultAsync<int>(
                "SELECT CurrentToken FROM UserTokens WHERE UserId = @UserId",
                new { UserId = userId }, transaction);

            if (balance < amount)
                throw new InvalidOperationException($"Insufficient tokens. Available: {balance}, Required: {amount}");

            await conn.ExecuteAsync(
                "UPDATE UserTokens SET CurrentToken = CurrentToken - @Amount, UpdatedAt = GETUTCDATE() WHERE UserId = @UserId",
                new { UserId = userId, Amount = amount }, transaction);

            await conn.ExecuteAsync(@"
                INSERT INTO TokenTransactions (Id, UserId, Type, Amount, Description, CreatedAt)
                VALUES (NEWID(), @UserId, 'use', @Amount, @Description, GETUTCDATE())",
                new { UserId = userId, Amount = amount, Description = description }, transaction);

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task AddTokensAsync(Guid userId, int amount, string description, string? referenceId, CancellationToken ct = default)
    {
        using var conn = connectionFactory.CreateConnection();
        conn.Open();
        using var transaction = conn.BeginTransaction();
        try
        {
            var exists = await conn.QuerySingleOrDefaultAsync<int>(
                "SELECT COUNT(1) FROM UserTokens WHERE UserId = @UserId",
                new { UserId = userId }, transaction);

            if (exists == 0)
            {
                await conn.ExecuteAsync(@"
                    INSERT INTO UserTokens (Id, UserId, CurrentToken, CreatedAt, UpdatedAt)
                    VALUES (NEWID(), @UserId, @Amount, GETUTCDATE(), GETUTCDATE())",
                    new { UserId = userId, Amount = amount }, transaction);
            }
            else
            {
                await conn.ExecuteAsync(
                    "UPDATE UserTokens SET CurrentToken = CurrentToken + @Amount, UpdatedAt = GETUTCDATE() WHERE UserId = @UserId",
                    new { UserId = userId, Amount = amount }, transaction);
            }

            await conn.ExecuteAsync(@"
                INSERT INTO TokenTransactions (Id, UserId, Type, Amount, Description, ReferenceId, CreatedAt)
                VALUES (NEWID(), @UserId, 'purchase', @Amount, @Description, @ReferenceId, GETUTCDATE())",
                new { UserId = userId, Amount = amount, Description = description, ReferenceId = referenceId },
                transaction);

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<IEnumerable<TokenTransaction>> GetTransactionHistoryAsync(Guid userId, int skip, int take, CancellationToken ct = default)
    {
        using var conn = connectionFactory.CreateConnection();
        return await conn.QueryAsync<TokenTransaction>(@"
            SELECT * FROM TokenTransactions WHERE UserId = @UserId
            ORDER BY CreatedAt DESC
            OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY",
            new { UserId = userId, Skip = skip, Take = take });
    }

    public async Task<IEnumerable<TokenPackage>> GetActivePackagesAsync(CancellationToken ct = default)
    {
        using var conn = connectionFactory.CreateConnection();
        return await conn.QueryAsync<TokenPackage>(
            "SELECT * FROM TokenPackages WHERE IsActive = 1 ORDER BY SortOrder ASC");
    }
}
