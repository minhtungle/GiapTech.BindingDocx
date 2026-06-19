using GiapTech.BindingDocx.Domain.Entities;

namespace GiapTech.BindingDocx.Domain.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    Guid? GetUserIdFromToken(string token);
}
