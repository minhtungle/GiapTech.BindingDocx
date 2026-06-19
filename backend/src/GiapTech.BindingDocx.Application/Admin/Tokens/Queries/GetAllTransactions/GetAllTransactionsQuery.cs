using GiapTech.BindingDocx.Application.Admin.Tokens.DTOs;
using GiapTech.BindingDocx.Application.Common.Models;
using MediatR;

namespace GiapTech.BindingDocx.Application.Admin.Tokens.Queries.GetAllTransactions;

public record GetAllTransactionsQuery(Guid? UserId, int Page, int PageSize) : IRequest<PagedResult<AdminTokenTransactionDto>>;
