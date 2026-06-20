using MediatR;

namespace GiapTech.BindingDocx.Application.Workspace.Commands.GenerateFiles;

public record GenerateFilesCommand(
    string GroupId,
    Guid UserId,
    Dictionary<string, string> SingleFields,
    Dictionary<string, List<Dictionary<string, string>>> TableData
) : IRequest<byte[]>;
