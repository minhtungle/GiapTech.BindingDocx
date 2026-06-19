using GiapTech.BindingDocx.Domain.Interfaces;
using MediatR;

namespace GiapTech.BindingDocx.Application.Auth.Commands.Logout;

public class LogoutCommandHandler(IUserRepository userRepository) : IRequestHandler<LogoutCommand>
{
    public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        await userRepository.UpdateRefreshTokenAsync(request.UserId, null, null, cancellationToken);
    }
}
