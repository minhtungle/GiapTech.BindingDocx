using GiapTech.BindingDocx.Application.Workspace.Interfaces;
using GiapTech.BindingDocx.Domain.Interfaces;
using MediatR;

namespace GiapTech.BindingDocx.Application.Workspace.Commands.GenerateFiles;

public class GenerateFilesCommandHandler(
    ITemplateRenderer renderer,
    ITokenRepository tokenRepository,
    IWorkspaceSettings settings)
    : IRequestHandler<GenerateFilesCommand, byte[]>
{
    public async Task<byte[]> Handle(GenerateFilesCommand request, CancellationToken cancellationToken)
    {
        var basePath = settings.TemplatePath;
        var groupPath = Path.Combine(basePath, request.GroupId);

        if (!Directory.Exists(groupPath))
            throw new InvalidOperationException($"Nhóm '{request.GroupId}' không tồn tại.");

        // Calculate token cost: sum of all table rows, minimum 1
        int totalRows = request.TableData.Values.Sum(rows => rows.Count);
        int tokenCost = Math.Max(1, totalRows);

        // Check balance
        var balance = await tokenRepository.GetCurrentBalanceAsync(request.UserId, cancellationToken);
        if (balance < tokenCost)
            throw new InvalidOperationException(
                $"Không đủ token. Hiện có: {balance} token, cần: {tokenCost} token.");

        // Render all template files
        var files = Directory.GetFiles(groupPath)
            .Where(f => f.EndsWith(".docx", StringComparison.OrdinalIgnoreCase)
                     || f.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
            .ToList();

        var rendered = new Dictionary<string, byte[]>();
        foreach (var file in files)
        {
            var fileName = Path.GetFileName(file);
            byte[] content;

            if (file.EndsWith(".docx", StringComparison.OrdinalIgnoreCase))
            {
                content = renderer.RenderDocx(file, request.SingleFields);
            }
            else
            {
                var tableRows = request.TableData.TryGetValue(fileName, out var rows) ? rows : null;
                content = renderer.RenderXlsx(file, request.SingleFields, tableRows);
            }

            rendered[fileName] = content;
        }

        // Create ZIP
        var zipBytes = renderer.CreateZip(rendered);

        // Deduct tokens
        await tokenRepository.DeductTokensAsync(
            request.UserId,
            tokenCost,
            $"Xuất {request.GroupId}: {totalRows} dòng dữ liệu",
            cancellationToken);

        return zipBytes;
    }
}
