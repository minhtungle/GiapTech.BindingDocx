using Dapper;
using GiapTech.BindingDocx.Domain.Entities;
using GiapTech.BindingDocx.Domain.Interfaces;

namespace GiapTech.BindingDocx.Infrastructure.Repositories;

public class ImportRepository(IDbConnectionFactory connectionFactory) : IImportRepository
{
    public async Task<ImportBatch?> GetBatchByIdAsync(Guid id, CancellationToken ct = default)
    {
        using var conn = connectionFactory.CreateConnection();
        return await conn.QuerySingleOrDefaultAsync<ImportBatch>(
            "SELECT * FROM ImportBatches WHERE Id = @Id", new { Id = id });
    }

    public async Task<IEnumerable<ImportBatch>> GetBatchesByGroupIdAsync(Guid groupId, CancellationToken ct = default)
    {
        using var conn = connectionFactory.CreateConnection();
        return await conn.QueryAsync<ImportBatch>(
            "SELECT * FROM ImportBatches WHERE GroupId = @GroupId ORDER BY CreatedAt DESC",
            new { GroupId = groupId });
    }

    public async Task<Guid> AddBatchAsync(ImportBatch batch, CancellationToken ct = default)
    {
        using var conn = connectionFactory.CreateConnection();
        await conn.ExecuteAsync(@"
            INSERT INTO ImportBatches (Id, GroupId, Name, Type, TotalRecords, Status, CreatedAt)
            VALUES (@Id, @GroupId, @Name, @Type, @TotalRecords, @Status, @CreatedAt)",
            batch);
        return batch.Id;
    }

    public async Task UpdateBatchStatusAsync(Guid batchId, string status, CancellationToken ct = default)
    {
        using var conn = connectionFactory.CreateConnection();
        await conn.ExecuteAsync(
            "UPDATE ImportBatches SET Status = @Status WHERE Id = @Id",
            new { Id = batchId, Status = status });
    }

    public async Task<IEnumerable<ImportRecord>> GetRecordsByBatchIdAsync(Guid batchId, CancellationToken ct = default)
    {
        using var conn = connectionFactory.CreateConnection();
        return await conn.QueryAsync<ImportRecord>(
            "SELECT * FROM ImportRecords WHERE BatchId = @BatchId ORDER BY CreatedAt ASC",
            new { BatchId = batchId });
    }

    public async Task AddRecordsAsync(IEnumerable<ImportRecord> records, CancellationToken ct = default)
    {
        using var conn = connectionFactory.CreateConnection();
        conn.Open();
        using var transaction = conn.BeginTransaction();
        try
        {
            foreach (var record in records)
            {
                await conn.ExecuteAsync(@"
                    INSERT INTO ImportRecords (Id, BatchId, JsonData, Status, CreatedAt)
                    VALUES (@Id, @BatchId, @JsonData, @Status, @CreatedAt)",
                    record, transaction);
            }
            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}
