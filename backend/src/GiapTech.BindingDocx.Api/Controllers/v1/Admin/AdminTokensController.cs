using Asp.Versioning;
using GiapTech.BindingDocx.Application.Admin.Tokens.Commands.CreateTokenPackage;
using GiapTech.BindingDocx.Application.Admin.Tokens.Commands.DeleteTokenPackage;
using GiapTech.BindingDocx.Application.Admin.Tokens.Commands.UpdateTokenPackage;
using GiapTech.BindingDocx.Application.Admin.Tokens.DTOs;
using GiapTech.BindingDocx.Application.Admin.Tokens.Queries.GetAllTransactions;
using GiapTech.BindingDocx.Application.Common.Models;
using GiapTech.BindingDocx.Application.Tokens.DTOs;
using GiapTech.BindingDocx.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GiapTech.BindingDocx.Api.Controllers.v1.Admin;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin")]
[Authorize(Roles = "admin")]
public class AdminTokensController(IMediator mediator, ITokenRepository tokenRepository) : ControllerBase
{
    [HttpGet("transactions")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<AdminTokenTransactionDto>>), 200)]
    public async Task<IActionResult> GetAllTransactions(
        [FromQuery] Guid? userId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetAllTransactionsQuery(userId, page, pageSize), ct);
        return Ok(ApiResponse<PagedResult<AdminTokenTransactionDto>>.Ok(result));
    }

    [HttpGet("token-packages")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<TokenPackageDto>>), 200)]
    public async Task<IActionResult> GetAllPackages(CancellationToken ct)
    {
        var packages = await tokenRepository.GetAllPackagesAsync(ct);
        var dtos = packages.Select(p => new TokenPackageDto(p.Id, p.Name, p.TokenAmount, p.PricePerToken, p.TotalPrice, p.IsActive, p.SortOrder));
        return Ok(ApiResponse<IEnumerable<TokenPackageDto>>.Ok(dtos));
    }

    [HttpPost("token-packages")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 201)]
    public async Task<IActionResult> CreatePackage([FromBody] CreateTokenPackageCommand command, CancellationToken ct)
    {
        var id = await mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetAllPackages), new { }, ApiResponse<Guid>.Ok(id, "Package created."));
    }

    [HttpPut("token-packages/{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    public async Task<IActionResult> UpdatePackage(Guid id, [FromBody] UpdateTokenPackageRequest request, CancellationToken ct)
    {
        await mediator.Send(new UpdateTokenPackageCommand(id, request.Name, request.TokenAmount, request.PricePerToken, request.TotalPrice, request.IsActive, request.SortOrder), ct);
        return Ok(ApiResponse.Ok("Package updated."));
    }

    [HttpDelete("token-packages/{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    public async Task<IActionResult> DeletePackage(Guid id, CancellationToken ct)
    {
        await mediator.Send(new DeleteTokenPackageCommand(id), ct);
        return Ok(ApiResponse.Ok("Package deleted."));
    }

    [HttpPatch("token-packages/{id:guid}/toggle-active")]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    public async Task<IActionResult> TogglePackageActive(Guid id, CancellationToken ct)
    {
        await tokenRepository.TogglePackageActiveAsync(id, ct);
        return Ok(ApiResponse.Ok("Package status toggled."));
    }
}

public record UpdateTokenPackageRequest(string Name, int TokenAmount, decimal PricePerToken, decimal TotalPrice, bool IsActive, int SortOrder);
