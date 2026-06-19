using GiapTech.BindingDocx.Application.Common.Exceptions;
using GiapTech.BindingDocx.Domain.Interfaces;
using MediatR;

namespace GiapTech.BindingDocx.Application.Admin.Users.Commands.DeleteUser;

public class DeleteUserCommandHandler(IUserRepository userRepository)
    : IRequestHandler<DeleteUserCommand>
{
    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"User {request.Id} not found.");

        if (user.Role == "admin")
            throw new ValidationException(new Dictionary<string, string[]>
                { ["id"] = ["Cannot delete admin accounts."] });

        await userRepository.DeleteAsync(request.Id, cancellationToken);
    }
}
