using Asp.Versioning;
using GiapTech.BindingDocx.Api.Extensions;
using GiapTech.BindingDocx.Application.Common.Models;
using GiapTech.BindingDocx.Application.Workspace.Commands.GenerateFiles;
using GiapTech.BindingDocx.Application.Workspace.Commands.ImportData;
using GiapTech.BindingDocx.Application.Workspace.DTOs;
using GiapTech.BindingDocx.Application.Workspace.Interfaces;
using GiapTech.BindingDocx.Application.Workspace.Queries.ExportTemplate;
using GiapTech.BindingDocx.Application.Workspace.Queries.GetGroupKeys;
using GiapTech.BindingDocx.Application.Workspace.Queries.GetGroups;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GiapTech.BindingDocx.Api.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class WorkspaceController(IMediator mediator, ICurrentUserService currentUser, IWorkspaceSettings workspaceSettings) : ControllerBase
{
    [HttpGet("groups")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<WorkspaceGroupDto>>), 200)]
    public async Task<IActionResult> GetGroups(CancellationToken ct)
    {
        var result = await mediator.Send(new GetGroupsQuery(), ct);
        return Ok(ApiResponse<IEnumerable<WorkspaceGroupDto>>.Ok(result));
    }

    [HttpGet("groups/{groupId}/keys")]
    [ProducesResponseType(typeof(ApiResponse<GroupKeysDto>), 200)]
    public async Task<IActionResult> GetGroupKeys(string groupId, CancellationToken ct)
    {
        var result = await mediator.Send(new GetGroupKeysQuery(groupId), ct);
        return Ok(ApiResponse<GroupKeysDto>.Ok(result));
    }

    [HttpGet("groups/{groupId}/export-template")]
    public async Task<IActionResult> ExportTemplate(string groupId, CancellationToken ct)
    {
        var bytes = await mediator.Send(new ExportTemplateQuery(groupId), ct);
        if (bytes.Length == 0) return NotFound();
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"template_{groupId}.xlsx");
    }

    [HttpPost("groups/{groupId}/import-data")]
    [ProducesResponseType(typeof(ApiResponse<ImportDataResultDto>), 200)]
    public async Task<IActionResult> ImportData(string groupId, IFormFile file, CancellationToken ct)
    {
        if (file == null || file.Length == 0)
            return BadRequest(ApiResponse.Fail("File không hợp lệ."));

        using var stream = file.OpenReadStream();
        var result = await mediator.Send(new ImportDataCommand(groupId, stream), ct);
        return Ok(ApiResponse<ImportDataResultDto>.Ok(result));
    }

    [HttpGet("groups/{groupId}/files/{fileName}")]
    public IActionResult GetFilePreview(string groupId, string fileName)
    {
        if (Path.GetFileName(fileName) != fileName || groupId.Contains('/') || groupId.Contains('\\'))
            return BadRequest();

        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        if (ext != ".docx" && ext != ".xlsx")
            return BadRequest();

        var filePath = Path.Combine(workspaceSettings.TemplatePath, groupId, fileName);
        if (!System.IO.File.Exists(filePath)) return NotFound();

        var contentType = ext switch
        {
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            _ => "application/octet-stream"
        };

        var bytes = System.IO.File.ReadAllBytes(filePath);
        return File(bytes, contentType, fileName);
    }

    [HttpPost("groups/{groupId}/generate")]
    public async Task<IActionResult> GenerateFiles(string groupId, [FromBody] GenerateFilesRequestDto request, CancellationToken ct)
    {
        var userId = currentUser.UserId;
        try
        {
            var zipBytes = await mediator.Send(
                new GenerateFilesCommand(groupId, userId, request.SingleFields, request.TableData), ct);
            return File(zipBytes, "application/zip", $"{groupId}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.zip");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse.Fail(ex.Message));
        }
    }
}

public class GenerateFilesRequestDto
{
    public Dictionary<string, string> SingleFields { get; set; } = [];
    public Dictionary<string, List<Dictionary<string, string>>> TableData { get; set; } = [];
}
