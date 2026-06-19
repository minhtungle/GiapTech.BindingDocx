using GiapTech.BindingDocx.Application.Common.Exceptions;
using GiapTech.BindingDocx.Domain.Interfaces;
using MediatR;

namespace GiapTech.BindingDocx.Application.Admin.Users.Commands.AdjustUserTokens;

public class AdjustUserTokensCommandHandler(IUserRepository userRepository, ITokenRepository tokenRepository)
    : IRequestHandler<AdjustUserTokensCommand>
{
    public async Task Handle(AdjustUserTokensCommand request, CancellationToken cancellationToken)
    {
        _ = await userRepository.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new NotFoundException($"User {request.UserId} not found.");

        if (request.Amount > 0)
            await tokenRepository.AddTokensAsync(request.UserId, request.Amount, request.Description, null, cancellationToken);
        else if (request.Amount < 0)
            await tokenRepository.DeductTokensAsync(request.UserId, Math.Abs(request.Amount), request.Description, cancellationToken);
    }
}
