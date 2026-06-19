using GiapTech.BindingDocx.Application.Tokens.DTOs;
using MediatR;

namespace GiapTech.BindingDocx.Application.Tokens.Queries.GetBalance;

public record GetTokenBalanceQuery(Guid UserId) : IRequest<TokenBalanceDto>;
