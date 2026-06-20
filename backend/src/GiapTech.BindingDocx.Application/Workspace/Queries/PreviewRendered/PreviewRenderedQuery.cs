using MediatR;

namespace GiapTech.BindingDocx.Application.Workspace.Queries.PreviewRendered;

public record PreviewRenderedQuery(
    string GroupId,
    string FileName,
    Dictionary<string, string> SingleFields,
    Dictionary<string, List<Dictionary<string, string>>> TableData
) : IRequest<(byte[] Bytes, string ContentType, string FileName)>;
