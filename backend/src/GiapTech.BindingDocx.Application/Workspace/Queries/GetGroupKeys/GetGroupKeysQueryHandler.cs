using GiapTech.BindingDocx.Application.Workspace.DTOs;
using GiapTech.BindingDocx.Application.Workspace.Interfaces;
using MediatR;

namespace GiapTech.BindingDocx.Application.Workspace.Queries.GetGroupKeys;

public class GetGroupKeysQueryHandler(ITemplateKeyExtractor extractor, IWorkspaceSettings settings)
    : IRequestHandler<GetGroupKeysQuery, GroupKeysDto>
{
    public Task<GroupKeysDto> Handle(GetGroupKeysQuery request, CancellationToken cancellationToken)
    {
        var basePath = settings.TemplatePath;
        var groupPath = Path.Combine(basePath, request.GroupId);

        if (!Directory.Exists(groupPath))
            return Task.FromResult(new GroupKeysDto());

        var result = extractor.ExtractKeys(groupPath);
        return Task.FromResult(result);
    }
}
