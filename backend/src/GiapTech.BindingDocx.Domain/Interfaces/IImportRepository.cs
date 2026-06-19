using GiapTech.BindingDocx.Domain.Entities;

namespace GiapTech.BindingDocx.Domain.Interfaces;

public interface IImportRepository
{
    Task<ImportBatch?> GetBatchByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<ImportBatch>> GetBatchesByGroupIdAsync(Guid groupId, CancellationToken ct = default);
    Task<Guid> AddBatchAsync(ImportBatch batch, CancellationToken ct = default);
    Task UpdateBatchStatusAsync(Guid batchId, string status, CancellationToken ct = default);
    Task<IEnumerable<ImportRecord>> GetRecordsByBatchIdAsync(Guid batchId, CancellationToken ct = default);
    Task AddRecordsAsync(IEnumerable<ImportRecord> records, CancellationToken ct = default);
}
