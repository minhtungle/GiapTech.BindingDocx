using GiapTech.BindingDocx.Application.Auth.DTOs;
using MediatR;

namespace GiapTech.BindingDocx.Application.Auth.Commands.Login;

public record LoginCommand(string Username, string Password) : IRequest<AuthResponse>;
