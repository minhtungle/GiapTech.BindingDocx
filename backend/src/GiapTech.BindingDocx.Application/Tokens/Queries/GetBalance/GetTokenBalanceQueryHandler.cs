using GiapTech.BindingDocx.Application.Common.Exceptions;
using GiapTech.BindingDocx.Application.Tokens.DTOs;
using GiapTech.BindingDocx.Domain.Interfaces;
using MediatR;

namespace GiapTech.BindingDocx.Application.Tokens.Queries.GetBalance;

public class GetTokenBalanceQueryHandler(ITokenRepository tokenRepository)
    : IRequestHandler<GetTokenBalanceQuery, TokenBalanceDto>
{
    public async Task<TokenBalanceDto> Handle(GetTokenBalanceQuery request, CancellationToken cancellationToken)
    {
        var token = await tokenRepository.GetUserTokenAsync(request.UserId, cancellationToken)
            ?? throw new NotFoundException("UserToken", request.UserId);

        return new TokenBalanceDto(token.UserId, token.CurrentToken, token.UpdatedAt);
    }
}
