using GiapTech.BindingDocx.Application.Workspace.DTOs;
using GiapTech.BindingDocx.Application.Workspace.Interfaces;
using MediatR;

namespace GiapTech.BindingDocx.Application.Workspace.Queries.GetGroups;

public class GetGroupsQueryHandler(IWorkspaceSettings settings)
    : IRequestHandler<GetGroupsQuery, IEnumerable<WorkspaceGroupDto>>
{
    private static readonly HashSet<string> EnabledGroups = ["group_1"];

    public Task<IEnumerable<WorkspaceGroupDto>> Handle(GetGroupsQuery request, CancellationToken cancellationToken)
    {
        var basePath = settings.TemplatePath;
        if (!Directory.Exists(basePath))
            return Task.FromResult<IEnumerable<WorkspaceGroupDto>>([]);

        var groups = Directory.GetDirectories(basePath)
            .OrderBy(d => d)
            .Select(dir =>
            {
                var id = Path.GetFileName(dir);
                var isAvailable = EnabledGroups.Contains(id);
                var groupNumber = id.Replace("group_", "");
                var name = isAvailable
                    ? $"Nhóm {groupNumber}"
                    : $"Nhóm {groupNumber} (sắp ra mắt)";
                var fileCount = isAvailable
                    ? Directory.GetFiles(dir, "*.docx").Length + Directory.GetFiles(dir, "*.xlsx").Length
                    : 0;

                return new WorkspaceGroupDto(id, name, isAvailable, fileCount);
            })
            .ToList();

        return Task.FromResult<IEnumerable<WorkspaceGroupDto>>(groups);
    }
}
