using Asp.Versioning;
using GiapTech.BindingDocx.Application.Common.Models;
using GiapTech.BindingDocx.Application.ProfileGroups.DTOs;
using GiapTech.BindingDocx.Application.ProfileGroups.Queries.GetAll;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GiapTech.BindingDocx.Api.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class ProfileGroupsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProfileGroupDto>>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] bool activeOnly = true, CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetAllProfileGroupsQuery(activeOnly), ct);
        return Ok(ApiResponse<IEnumerable<ProfileGroupDto>>.Ok(result));
    }
}
