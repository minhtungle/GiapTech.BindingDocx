using GiapTech.BindingDocx.Domain.Entities;

namespace GiapTech.BindingDocx.Domain.Interfaces;

public interface ITokenRepository
{
    Task<UserToken?> GetUserTokenAsync(Guid userId, CancellationToken ct = default);
    Task<int> GetCurrentBalanceAsync(Guid userId, CancellationToken ct = default);
    Task DeductTokensAsync(Guid userId, int amount, string description, CancellationToken ct = default);
    Task AddTokensAsync(Guid userId, int amount, string description, string? referenceId, CancellationToken ct = default);
    Task<IEnumerable<TokenTransaction>> GetTransactionHistoryAsync(Guid userId, int skip, int take, CancellationToken ct = default);
    Task<IEnumerable<TokenPackage>> GetActivePackagesAsync(CancellationToken ct = default);
}
