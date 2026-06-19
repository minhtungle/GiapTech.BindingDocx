using GiapTech.BindingDocx.Application.Auth.DTOs;
using GiapTech.BindingDocx.Application.Common.Exceptions;
using GiapTech.BindingDocx.Domain.Interfaces;
using MediatR;

namespace GiapTech.BindingDocx.Application.Auth.Commands.Login;

public class LoginCommandHandler(
    IUserRepository userRepository,
    IJwtService jwtService)
    : IRequestHandler<LoginCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByUsernameAsync(request.Username, cancellationToken)
            ?? throw new UnauthorizedException("Invalid username or password.");

        if (!user.IsActive)
            throw new UnauthorizedException("Account is deactivated.");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedException("Invalid username or password.");

        var accessToken = jwtService.GenerateAccessToken(user);
        var refreshToken = jwtService.GenerateRefreshToken();
        var expiry = DateTime.UtcNow.AddDays(7);

        await userRepository.UpdateRefreshTokenAsync(user.Id, refreshToken, expiry, cancellationToken);

        return new AuthResponse(
            accessToken,
            refreshToken,
            DateTime.UtcNow.AddHours(1),
            user.Id,
            user.Username,
            user.Email
        );
    }
}
