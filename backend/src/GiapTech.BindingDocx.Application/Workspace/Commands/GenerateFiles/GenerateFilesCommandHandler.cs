using GiapTech.BindingDocx.Application.Workspace.Interfaces;
using GiapTech.BindingDocx.Domain.Interfaces;
using MediatR;

namespace GiapTech.BindingDocx.Application.Workspace.Commands.GenerateFiles;

public class GenerateFilesCommandHandler(
    ITemplateRenderer renderer,
    ITemplateKeyExtractor extractor,
    ITokenRepository tokenRepository,
    IWorkspaceSettings settings)
    : IRequestHandler<GenerateFilesCommand, byte[]>
{
    public async Task<byte[]> Handle(GenerateFilesCommand request, CancellationToken cancellationToken)
    {
        var groupPath = Path.Combine(settings.TemplatePath, request.GroupId);
        if (!Directory.Exists(groupPath))
            throw new InvalidOperationException($"Nhóm '{request.GroupId}' không tồn tại.");

        // Re-extract keys to get TableFileInfoDto list (for sheet mapping)
        var groupKeys = extractor.ExtractKeys(groupPath);

        // Build tableKey → sheetName map from extractor output
        var tableKeyToSheet = groupKeys.TableFiles
            .ToDictionary(t => t.TableKey, t => t.SheetName);

        // Convert TableData (tableKey → rows) to per-file tableDataBySheet (sheetName → rows)
        // grouped by actual xlsx FileName
        var tableDataPerFile = new Dictionary<string, Dictionary<string, List<Dictionary<string, string>>>>();
        foreach (var (tableKey, rows) in request.TableData)
        {
            if (!tableKeyToSheet.TryGetValue(tableKey, out var sheetName)) continue;
            var tableInfo = groupKeys.TableFiles.First(t => t.TableKey == tableKey);
            var fileName = tableInfo.FileName;
            if (!tableDataPerFile.ContainsKey(fileName))
                tableDataPerFile[fileName] = [];
            tableDataPerFile[fileName][sheetName] = rows;
        }

        // Calculate token cost
        int totalRows = request.TableData.Values.Sum(rows => rows.Count);
        int tokenCost = Math.Max(1, totalRows);

        var balance = await tokenRepository.GetCurrentBalanceAsync(request.UserId, cancellationToken);
        if (balance < tokenCost)
            throw new InvalidOperationException(
                $"Không đủ token. Hiện có: {balance} token, cần: {tokenCost} token.");

        var files = Directory.GetFiles(groupPath)
            .Where(f => f.EndsWith(".docx", StringComparison.OrdinalIgnoreCase)
                     || f.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
            .ToList();

        var rendered = new Dictionary<string, byte[]>();
        foreach (var file in files)
        {
            var fileName = Path.GetFileName(file);

            // Resolve per-file single fields
            Dictionary<string, string> singleFields;
            if (request.SyncMode)
            {
                singleFields = request.SingleFields;
            }
            else
            {
                singleFields = request.SingleFieldsByFile.TryGetValue(fileName, out var perFile)
                    ? perFile
                    : request.SingleFields;
            }

            byte[] content;
            if (file.EndsWith(".docx", StringComparison.OrdinalIgnoreCase))
            {
                content = renderer.RenderDocx(file, singleFields);
            }
            else
            {
                tableDataPerFile.TryGetValue(fileName, out var tableDataBySheet);
                content = renderer.RenderXlsx(file, singleFields, tableDataBySheet);
            }

            rendered[fileName] = content;
        }

        var zipBytes = renderer.CreateZip(rendered);

        await tokenRepository.DeductTokensAsync(
            request.UserId,
            tokenCost,
            $"Xuất {request.GroupId}: {totalRows} dòng dữ liệu",
            cancellationToken);

        return zipBytes;
    }
}
