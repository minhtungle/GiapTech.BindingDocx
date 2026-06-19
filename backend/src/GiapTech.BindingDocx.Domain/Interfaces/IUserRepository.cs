using GiapTech.BindingDocx.Domain.Entities;

namespace GiapTech.BindingDocx.Domain.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken ct = default);
    Task UpdateRefreshTokenAsync(Guid userId, string? refreshToken, DateTime? expiry, CancellationToken ct = default);

    // Admin
    Task<(IEnumerable<User> Items, int Total)> GetPagedAsync(string? search, int skip, int take, CancellationToken ct = default);
    Task<bool> UsernameExistsAsync(string username, Guid? excludeId, CancellationToken ct = default);
    Task<bool> EmailExistsAsync(string email, Guid? excludeId, CancellationToken ct = default);
    Task<Guid> CreateAsync(User entity, CancellationToken ct = default);
    Task UpdateProfileAsync(User entity, CancellationToken ct = default);
}
