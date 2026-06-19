using MediatR;

namespace GiapTech.BindingDocx.Application.Admin.Users.Commands.DeleteUser;

public record DeleteUserCommand(Guid Id) : IRequest;
