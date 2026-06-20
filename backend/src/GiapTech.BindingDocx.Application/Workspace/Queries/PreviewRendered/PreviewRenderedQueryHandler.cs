using GiapTech.BindingDocx.Application.Workspace.Interfaces;
using MediatR;

namespace GiapTech.BindingDocx.Application.Workspace.Queries.PreviewRendered;

public class PreviewRenderedQueryHandler(
    ITemplateRenderer renderer,
    ITemplateKeyExtractor extractor,
    IWorkspaceSettings settings)
    : IRequestHandler<PreviewRenderedQuery, (byte[] Bytes, string ContentType, string FileName)>
{
    public Task<(byte[] Bytes, string ContentType, string FileName)> Handle(
        PreviewRenderedQuery request, CancellationToken cancellationToken)
    {
        var safeFileName = Path.GetFileName(request.FileName);
        var filePath = Path.Combine(settings.TemplatePath, request.GroupId, safeFileName);

        if (!File.Exists(filePath))
            throw new FileNotFoundException($"File '{safeFileName}' không tồn tại.");

        var ext = Path.GetExtension(safeFileName).ToLowerInvariant();
        byte[] bytes;
        string contentType;

        if (ext == ".docx")
        {
            bytes = renderer.RenderDocx(filePath, request.SingleFields);
            contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
        }
        else if (ext == ".xlsx")
        {
            // Build tableDataBySheet from TableData using extractor
            var groupPath = Path.Combine(settings.TemplatePath, request.GroupId);
            var groupKeys = extractor.ExtractKeys(groupPath);

            var tableKeyToSheet = groupKeys.TableFiles
                .Where(t => t.FileName == safeFileName)
                .ToDictionary(t => t.TableKey, t => t.SheetName);

            var tableDataBySheet = new Dictionary<string, List<Dictionary<string, string>>>();
            foreach (var (tableKey, rows) in request.TableData)
            {
                if (tableKeyToSheet.TryGetValue(tableKey, out var sheetName))
                    tableDataBySheet[sheetName] = rows;
            }

            bytes = renderer.RenderXlsx(filePath, request.SingleFields, tableDataBySheet);
            contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        }
        else
        {
            throw new InvalidOperationException("Định dạng file không được hỗ trợ.");
        }

        return Task.FromResult((bytes, contentType, safeFileName));
    }
}
