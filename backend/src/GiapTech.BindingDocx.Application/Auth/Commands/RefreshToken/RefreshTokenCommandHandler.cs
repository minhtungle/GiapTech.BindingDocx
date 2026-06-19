using GiapTech.BindingDocx.Application.Auth.DTOs;
using GiapTech.BindingDocx.Application.Common.Exceptions;
using GiapTech.BindingDocx.Domain.Interfaces;
using MediatR;

namespace GiapTech.BindingDocx.Application.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler(
    IUserRepository userRepository,
    IJwtService jwtService)
    : IRequestHandler<RefreshTokenCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByRefreshTokenAsync(request.RefreshToken, cancellationToken)
            ?? throw new UnauthorizedException("Invalid refresh token.");

        if (user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            throw new UnauthorizedException("Refresh token has expired.");

        var accessToken = jwtService.GenerateAccessToken(user);
        var newRefreshToken = jwtService.GenerateRefreshToken();
        var expiry = DateTime.UtcNow.AddDays(7);

        await userRepository.UpdateRefreshTokenAsync(user.Id, newRefreshToken, expiry, cancellationToken);

        return new AuthResponse(
            accessToken,
            newRefreshToken,
            DateTime.UtcNow.AddHours(1),
            user.Id,
            user.Username,
            user.Email
        );
    }
}
