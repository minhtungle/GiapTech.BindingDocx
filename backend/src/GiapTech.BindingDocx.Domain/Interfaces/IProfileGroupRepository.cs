using GiapTech.BindingDocx.Domain.Entities;

namespace GiapTech.BindingDocx.Domain.Interfaces;

public interface IProfileGroupRepository : IRepository<ProfileGroup>
{
    Task<IEnumerable<ProfileGroup>> GetActiveGroupsAsync(CancellationToken ct = default);
}
