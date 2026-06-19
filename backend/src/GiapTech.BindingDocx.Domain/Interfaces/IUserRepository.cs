using GiapTech.BindingDocx.Domain.Entities;

namespace GiapTech.BindingDocx.Domain.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken ct = default);
    Task UpdateRefreshTokenAsync(Guid userId, string? refreshToken, DateTime? expiry, CancellationToken ct = default);
}
