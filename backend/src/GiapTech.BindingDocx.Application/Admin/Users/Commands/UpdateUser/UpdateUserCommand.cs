using MediatR;

namespace GiapTech.BindingDocx.Application.Admin.Users.Commands.UpdateUser;

public record UpdateUserCommand(
    Guid Id,
    string Username,
    string Email,
    string? Password,
    string Role,
    bool IsActive
) : IRequest;
