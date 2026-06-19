namespace GiapTech.BindingDocx.Application.Auth.DTOs;

public record AuthResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    Guid UserId,
    string Username,
    string Email,
    string Role
);
