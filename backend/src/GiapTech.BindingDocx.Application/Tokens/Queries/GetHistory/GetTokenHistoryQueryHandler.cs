using GiapTech.BindingDocx.Application.Common.Models;
using GiapTech.BindingDocx.Application.Tokens.DTOs;
using GiapTech.BindingDocx.Domain.Interfaces;
using MediatR;

namespace GiapTech.BindingDocx.Application.Tokens.Queries.GetHistory;

public class GetTokenHistoryQueryHandler(ITokenRepository tokenRepository)
    : IRequestHandler<GetTokenHistoryQuery, PagedResult<TokenTransactionDto>>
{
    public async Task<PagedResult<TokenTransactionDto>> Handle(GetTokenHistoryQuery request, CancellationToken cancellationToken)
    {
        var skip = (request.PageNumber - 1) * request.PageSize;
        var transactions = await tokenRepository.GetTransactionHistoryAsync(request.UserId, skip, request.PageSize + 1, cancellationToken);

        var list = transactions.ToList();
        var hasMore = list.Count > request.PageSize;
        if (hasMore) list.RemoveAt(list.Count - 1);

        var dtos = list.Select(t => new TokenTransactionDto(
            t.Id,
            t.Type,
            t.Amount,
            t.Description,
            t.CreatedAt
        )).ToList();

        return PagedResult<TokenTransactionDto>.Create(dtos, dtos.Count, request.PageNumber, request.PageSize);
    }
}
