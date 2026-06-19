using Asp.Versioning;
using GiapTech.BindingDocx.Application.Admin.Users.Commands.AdjustUserTokens;
using GiapTech.BindingDocx.Application.Admin.Users.Commands.CreateUser;
using GiapTech.BindingDocx.Application.Admin.Users.Commands.DeleteUser;
using GiapTech.BindingDocx.Application.Admin.Users.Commands.ToggleUserActive;
using GiapTech.BindingDocx.Application.Admin.Users.Commands.UpdateUser;
using GiapTech.BindingDocx.Application.Admin.Users.DTOs;
using GiapTech.BindingDocx.Application.Admin.Users.Queries.GetAllUsers;
using GiapTech.BindingDocx.Application.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GiapTech.BindingDocx.Api.Controllers.v1.Admin;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin/users")]
[Authorize(Roles = "admin")]
public class AdminUsersController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<UserDto>>), 200)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetAllUsersQuery(search, page, pageSize), ct);
        return Ok(ApiResponse<PagedResult<UserDto>>.Ok(result));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 201)]
    public async Task<IActionResult> Create([FromBody] CreateUserCommand command, CancellationToken ct)
    {
        var id = await mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetAll), new { }, ApiResponse<Guid>.Ok(id, "User created."));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequest request, CancellationToken ct)
    {
        await mediator.Send(new UpdateUserCommand(id, request.Username, request.Email, request.Password, request.Role, request.IsActive), ct);
        return Ok(ApiResponse.Ok("User updated."));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await mediator.Send(new DeleteUserCommand(id), ct);
        return Ok(ApiResponse.Ok("User deleted."));
    }

    [HttpPatch("{id:guid}/toggle-active")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    public async Task<IActionResult> ToggleActive(Guid id, CancellationToken ct)
    {
        var isActive = await mediator.Send(new ToggleUserActiveCommand(id), ct);
        return Ok(ApiResponse<bool>.Ok(isActive, isActive ? "User activated." : "User deactivated."));
    }

    [HttpPost("{id:guid}/adjust-tokens")]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    public async Task<IActionResult> AdjustTokens(Guid id, [FromBody] AdjustTokensRequest request, CancellationToken ct)
    {
        await mediator.Send(new AdjustUserTokensCommand(id, request.Amount, request.Description), ct);
        return Ok(ApiResponse.Ok("Tokens adjusted."));
    }
}

public record UpdateUserRequest(string Username, string Email, string? Password, string Role, bool IsActive);
public record AdjustTokensRequest(int Amount, string Description);
