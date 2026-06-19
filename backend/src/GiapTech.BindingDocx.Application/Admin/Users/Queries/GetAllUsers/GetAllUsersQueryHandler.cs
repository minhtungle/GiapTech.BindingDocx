using GiapTech.BindingDocx.Application.Admin.Users.DTOs;
using GiapTech.BindingDocx.Application.Common.Models;
using GiapTech.BindingDocx.Domain.Interfaces;
using MediatR;

namespace GiapTech.BindingDocx.Application.Admin.Users.Queries.GetAllUsers;

public class GetAllUsersQueryHandler(IUserRepository userRepository, ITokenRepository tokenRepository)
    : IRequestHandler<GetAllUsersQuery, PagedResult<UserDto>>
{
    public async Task<PagedResult<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var skip = (page - 1) * pageSize;

        var (users, total) = await userRepository.GetPagedAsync(request.Search, skip, pageSize, cancellationToken);
        var userList = users.ToList();

        var tokens = await tokenRepository.GetTokensForUsersAsync(userList.Select(u => u.Id), cancellationToken);

        var dtos = userList.Select(u => new UserDto
        {
            Id = u.Id,
            Username = u.Username,
            Email = u.Email,
            Role = u.Role,
            IsActive = u.IsActive,
            CurrentToken = tokens.TryGetValue(u.Id, out var t) ? t : 0,
            CreatedAt = u.CreatedAt,
            UpdatedAt = u.UpdatedAt,
        }).ToList();

        return PagedResult<UserDto>.Create(dtos, total, page, pageSize);
    }
}
