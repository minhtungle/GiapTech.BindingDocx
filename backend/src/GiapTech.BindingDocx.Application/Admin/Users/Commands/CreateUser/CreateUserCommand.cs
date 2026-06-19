using MediatR;

namespace GiapTech.BindingDocx.Application.Admin.Users.Commands.CreateUser;

public record CreateUserCommand(
    string Username,
    string Email,
    string Password,
    string Role,
    bool IsActive
) : IRequest<Guid>;
