using GiapTech.BindingDocx.Application.Auth.DTOs;
using MediatR;

namespace GiapTech.BindingDocx.Application.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(string RefreshToken) : IRequest<AuthResponse>;
