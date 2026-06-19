using GiapTech.BindingDocx.Application.Admin.Tokens.DTOs;
using GiapTech.BindingDocx.Application.Common.Models;
using GiapTech.BindingDocx.Domain.Interfaces;
using MediatR;

namespace GiapTech.BindingDocx.Application.Admin.Tokens.Queries.GetAllTransactions;

public class GetAllTransactionsQueryHandler(ITokenRepository tokenRepository)
    : IRequestHandler<GetAllTransactionsQuery, PagedResult<AdminTokenTransactionDto>>
{
    public async Task<PagedResult<AdminTokenTransactionDto>> Handle(GetAllTransactionsQuery request, CancellationToken cancellationToken)
    {
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var skip = (page - 1) * pageSize;

        var (rows, total) = await tokenRepository.GetAllTransactionsWithUsernamePagedAsync(request.UserId, skip, pageSize, cancellationToken);

        var dtos = rows.Select(r => new AdminTokenTransactionDto(
            r.Transaction.Id, r.UserId, r.Username,
            r.Transaction.Type, r.Transaction.Amount, r.Transaction.Description, r.Transaction.CreatedAt))
            .ToList();

        return PagedResult<AdminTokenTransactionDto>.Create(dtos, total, page, pageSize);
    }
}
