using MediatR;

namespace GiapTech.BindingDocx.Application.Auth.Commands.Logout;

public record LogoutCommand(Guid UserId) : IRequest;
