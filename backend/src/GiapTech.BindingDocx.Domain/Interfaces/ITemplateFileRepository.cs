using GiapTech.BindingDocx.Domain.Entities;

namespace GiapTech.BindingDocx.Domain.Interfaces;

public interface ITemplateFileRepository : IRepository<TemplateFile>
{
    Task<IEnumerable<TemplateFile>> GetByGroupIdAsync(Guid groupId, CancellationToken ct = default);
    Task<IEnumerable<TemplateFile>> GetActiveByGroupIdAsync(Guid groupId, CancellationToken ct = default);
}
