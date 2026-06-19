using GiapTech.BindingDocx.Domain.Entities;

namespace GiapTech.BindingDocx.Domain.Interfaces;

public interface ITokenRepository
{
    Task<UserToken?> GetUserTokenAsync(Guid userId, CancellationToken ct = default);
    Task<int> GetCurrentBalanceAsync(Guid userId, CancellationToken ct = default);
    Task DeductTokensAsync(Guid userId, int amount, string description, CancellationToken ct = default);
    Task AddTokensAsync(Guid userId, int amount, string description, string? referenceId, CancellationToken ct = default);
    Task<IEnumerable<TokenTransaction>> GetTransactionHistoryAsync(Guid userId, int skip, int take, CancellationToken ct = default);
    Task<int> GetTransactionCountAsync(Guid userId, CancellationToken ct = default);
    Task<IEnumerable<TokenPackage>> GetActivePackagesAsync(CancellationToken ct = default);

    Task<IDictionary<Guid, int>> GetTokensForUsersAsync(IEnumerable<Guid> userIds, CancellationToken ct = default);

    // Admin
    Task<(IEnumerable<TokenTransaction> Items, int Total)> GetAllTransactionsPagedAsync(Guid? userId, int skip, int take, CancellationToken ct = default);
    Task<(IEnumerable<(Guid UserId, string Username, TokenTransaction Transaction)> Items, int Total)> GetAllTransactionsWithUsernamePagedAsync(Guid? userId, int skip, int take, CancellationToken ct = default);
    Task<IEnumerable<TokenPackage>> GetAllPackagesAsync(CancellationToken ct = default);
    Task<Guid> CreatePackageAsync(TokenPackage package, CancellationToken ct = default);
    Task UpdatePackageAsync(TokenPackage package, CancellationToken ct = default);
    Task DeletePackageAsync(Guid id, CancellationToken ct = default);
    Task TogglePackageActiveAsync(Guid id, CancellationToken ct = default);
}
