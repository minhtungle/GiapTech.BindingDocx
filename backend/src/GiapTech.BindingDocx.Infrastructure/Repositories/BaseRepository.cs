using Dapper;
using GiapTech.BindingDocx.Domain.Common;
using GiapTech.BindingDocx.Domain.Interfaces;

namespace GiapTech.BindingDocx.Infrastructure.Repositories;

public abstract class BaseRepository<T>(IDbConnectionFactory connectionFactory) where T : BaseEntity
{
    protected IDbConnectionFactory ConnectionFactory { get; } = connectionFactory;

    protected abstract string TableName { get; }

    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        using var conn = ConnectionFactory.CreateConnection();
        return await conn.QuerySingleOrDefaultAsync<T>(
            $"SELECT * FROM {TableName} WHERE Id = @Id",
            new { Id = id });
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default)
    {
        using var conn = ConnectionFactory.CreateConnection();
        return await conn.QueryAsync<T>($"SELECT * FROM {TableName}");
    }

    public virtual async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        using var conn = ConnectionFactory.CreateConnection();
        await conn.ExecuteAsync($"DELETE FROM {TableName} WHERE Id = @Id", new { Id = id });
    }
}
