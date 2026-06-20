using MediatR;

namespace GiapTech.BindingDocx.Application.Workspace.Commands.GenerateFiles;

public record GenerateFilesCommand(
    string GroupId,
    Guid UserId,
    bool SyncMode,
    Dictionary<string, string> SingleFields,
    Dictionary<string, Dictionary<string, string>> SingleFieldsByFile,
    Dictionary<string, List<Dictionary<string, string>>> TableData
) : IRequest<byte[]>;
