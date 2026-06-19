using GiapTech.BindingDocx.Application.Common.Models;
using GiapTech.BindingDocx.Application.Tokens.DTOs;
using MediatR;

namespace GiapTech.BindingDocx.Application.Tokens.Queries.GetHistory;

public record GetTokenHistoryQuery(Guid UserId, int PageNumber = 1, int PageSize = 20)
    : IRequest<PagedResult<TokenTransactionDto>>;
