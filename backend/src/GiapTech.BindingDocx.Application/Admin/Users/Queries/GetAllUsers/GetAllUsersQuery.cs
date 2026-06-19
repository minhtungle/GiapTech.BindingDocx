using GiapTech.BindingDocx.Application.Admin.Users.DTOs;
using GiapTech.BindingDocx.Application.Common.Models;
using MediatR;

namespace GiapTech.BindingDocx.Application.Admin.Users.Queries.GetAllUsers;

public record GetAllUsersQuery(string? Search, int Page, int PageSize) : IRequest<PagedResult<UserDto>>;
