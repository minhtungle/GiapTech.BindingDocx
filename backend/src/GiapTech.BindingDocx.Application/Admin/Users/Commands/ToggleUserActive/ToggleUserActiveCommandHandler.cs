using GiapTech.BindingDocx.Application.Common.Exceptions;
using GiapTech.BindingDocx.Domain.Interfaces;
using MediatR;

namespace GiapTech.BindingDocx.Application.Admin.Users.Commands.ToggleUserActive;

public class ToggleUserActiveCommandHandler(IUserRepository userRepository)
    : IRequestHandler<ToggleUserActiveCommand, bool>
{
    public async Task<bool> Handle(ToggleUserActiveCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"User {request.Id} not found.");

        user.IsActive = !user.IsActive;
        await userRepository.UpdateProfileAsync(user, cancellationToken);
        return user.IsActive;
    }
}
