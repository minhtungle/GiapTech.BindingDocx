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

    public async Task<int> GetTransactionCountAsync(Guid userId, CancellationToken ct = default)
    {
        using var conn = connectionFactory.CreateConnection();
        return await conn.QuerySingleAsync<int>(
            "SELECT COUNT(1) FROM TokenTransactions WHERE UserId = @UserId", new { UserId = userId });
    }

    public async Task<IEnumerable<TokenPackage>> GetActivePackagesAsync(CancellationToken ct = default)
    {
        using var conn = connectionFactory.CreateConnection();
        return await conn.QueryAsync<TokenPackage>(
            "SELECT * FROM TokenPackages WHERE IsActive = 1 ORDER BY SortOrder ASC");
    }

    public async Task<IDictionary<Guid, int>> GetTokensForUsersAsync(IEnumerable<Guid> userIds, CancellationToken ct = default)
    {
        var ids = userIds.ToList();
        if (ids.Count == 0) return new Dictionary<Guid, int>();
        using var conn = connectionFactory.CreateConnection();
        var rows = await conn.QueryAsync<(Guid UserId, int CurrentToken)>(
            "SELECT UserId, CurrentToken FROM UserTokens WHERE UserId IN @Ids", new { Ids = ids });
        return rows.ToDictionary(r => r.UserId, r => r.CurrentToken);
    }

    // Admin methods
    public async Task<(IEnumerable<TokenTransaction> Items, int Total)> GetAllTransactionsPagedAsync(Guid? userId, int skip, int take, CancellationToken ct = default)
    {
        using var conn = connectionFactory.CreateConnection();
        var where = userId.HasValue ? "WHERE UserId = @UserId" : string.Empty;
        var param = new { UserId = userId, Skip = skip, Take = take };
        var total = await conn.QuerySingleAsync<int>($"SELECT COUNT(1) FROM TokenTransactions {where}", param);
        var items = await conn.QueryAsync<TokenTransaction>(
            $"SELECT * FROM TokenTransactions {where} ORDER BY CreatedAt DESC OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY", param);
        return (items, total);
    }

    public async Task<(IEnumerable<(Guid UserId, string Username, TokenTransaction Transaction)> Items, int Total)>
        GetAllTransactionsWithUsernamePagedAsync(Guid? userId, int skip, int take, CancellationToken ct = default)
    {
        using var conn = connectionFactory.CreateConnection();
        var where = userId.HasValue ? "WHERE t.UserId = @UserId" : string.Empty;
        var param = new { UserId = userId, Skip = skip, Take = take };
        var total = await conn.QuerySingleAsync<int>($"SELECT COUNT(1) FROM TokenTransactions t {where}", param);
        var rows = await conn.QueryAsync<(Guid Id, Guid UserId, string Type, int Amount, string? Description, string? ReferenceId, DateTime CreatedAt, string Username)>(@$"
            SELECT t.Id, t.UserId, t.Type, t.Amount, t.Description, t.ReferenceId, t.CreatedAt, u.Username
            FROM TokenTransactions t
            INNER JOIN Users u ON u.Id = t.UserId
            {where}
            ORDER BY t.CreatedAt DESC
            OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY", param);

        var items = rows.Select(r => (r.UserId, r.Username,
            new TokenTransaction { Id = r.Id, UserId = r.UserId, Type = r.Type, Amount = r.Amount, Description = r.Description, ReferenceId = r.ReferenceId, CreatedAt = r.CreatedAt }));
        return (items, total);
    }

    public async Task<IEnumerable<TokenPackage>> GetAllPackagesAsync(CancellationToken ct = default)
    {
        using var conn = connectionFactory.CreateConnection();
        return await conn.QueryAsync<TokenPackage>("SELECT * FROM TokenPackages ORDER BY SortOrder ASC");
    }

    public async Task<Guid> CreatePackageAsync(TokenPackage package, CancellationToken ct = default)
    {
        using var conn = connectionFactory.CreateConnection();
        await conn.ExecuteAsync(@"
            INSERT INTO TokenPackages (Id, Name, TokenAmount, PricePerToken, TotalPrice, IsActive, SortOrder, CreatedAt)
            VALUES (@Id, @Name, @TokenAmount, @PricePerToken, @TotalPrice, @IsActive, @SortOrder, @CreatedAt)",
            package);
        return package.Id;
    }

    public async Task UpdatePackageAsync(TokenPackage package, CancellationToken ct = default)
    {
        using var conn = connectionFactory.CreateConnection();
        await conn.ExecuteAsync(@"
            UPDATE TokenPackages SET Name=@Name, TokenAmount=@TokenAmount, PricePerToken=@PricePerToken,
            TotalPrice=@TotalPrice, IsActive=@IsActive, SortOrder=@SortOrder WHERE Id=@Id",
            package);
    }

    public async Task DeletePackageAsync(Guid id, CancellationToken ct = default)
    {
        using var conn = connectionFactory.CreateConnection();
        await conn.ExecuteAsync("DELETE FROM TokenPackages WHERE Id=@Id", new { Id = id });
    }

    public async Task TogglePackageActiveAsync(Guid id, CancellationToken ct = default)
    {
        using var conn = connectionFactory.CreateConnection();
        await conn.ExecuteAsync(
            "UPDATE TokenPackages SET IsActive = CASE WHEN IsActive=1 THEN 0 ELSE 1 END WHERE Id=@Id",
            new { Id = id });
    }
}
