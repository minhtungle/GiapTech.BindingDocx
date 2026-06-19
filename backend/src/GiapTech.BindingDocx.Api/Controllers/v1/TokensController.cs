using Asp.Versioning;
using GiapTech.BindingDocx.Application.Common.Models;
using GiapTech.BindingDocx.Application.Tokens.DTOs;
using GiapTech.BindingDocx.Application.Tokens.Queries.GetBalance;
using GiapTech.BindingDocx.Application.Tokens.Queries.GetHistory;
using GiapTech.BindingDocx.Application.Tokens.Queries.GetPackages;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GiapTech.BindingDocx.Api.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class TokensController(IMediator mediator) : ControllerBase
{
    [HttpGet("balance")]
    [ProducesResponseType(typeof(ApiResponse<TokenBalanceDto>), 200)]
    public async Task<IActionResult> GetBalance(CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        var result = await mediator.Send(new GetTokenBalanceQuery(userId), ct);
        return Ok(ApiResponse<TokenBalanceDto>.Ok(result));
    }

    [HttpGet("packages")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<TokenPackageDto>>), 200)]
    public async Task<IActionResult> GetPackages(CancellationToken ct)
    {
        var result = await mediator.Send(new GetTokenPackagesQuery(), ct);
        return Ok(ApiResponse<IEnumerable<TokenPackageDto>>.Ok(result));
    }

    [HttpGet("history")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<TokenTransactionDto>>), 200)]
    public async Task<IActionResult> GetHistory([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var userId = GetCurrentUserId();
        var result = await mediator.Send(new GetTokenHistoryQuery(userId, page, pageSize), ct);
        return Ok(ApiResponse<PagedResult<TokenTransactionDto>>.Ok(result));
    }

    private Guid GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
        return claim != null && Guid.TryParse(claim.Value, out var id) ? id : Guid.Empty;
    }
}
