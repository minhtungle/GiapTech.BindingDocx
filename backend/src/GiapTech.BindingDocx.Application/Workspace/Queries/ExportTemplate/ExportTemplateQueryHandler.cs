using GiapTech.BindingDocx.Application.Workspace.Interfaces;
using MediatR;

namespace GiapTech.BindingDocx.Application.Workspace.Queries.ExportTemplate;

public class ExportTemplateQueryHandler(
    ITemplateKeyExtractor extractor,
    IExcelTemplateGenerator generator,
    IWorkspaceSettings settings)
    : IRequestHandler<ExportTemplateQuery, byte[]>
{
    public Task<byte[]> Handle(ExportTemplateQuery request, CancellationToken cancellationToken)
    {
        var basePath = settings.TemplatePath;
        var groupPath = Path.Combine(basePath, request.GroupId);

        if (!Directory.Exists(groupPath))
            return Task.FromResult(Array.Empty<byte>());

        var keys = extractor.ExtractKeys(groupPath);
        var excelBytes = generator.GenerateTemplate(keys);
        return Task.FromResult(excelBytes);
    }
}
