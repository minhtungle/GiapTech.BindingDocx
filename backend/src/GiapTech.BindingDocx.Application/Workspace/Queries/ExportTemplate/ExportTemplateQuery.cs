using MediatR;

namespace GiapTech.BindingDocx.Application.Workspace.Queries.ExportTemplate;

public record ExportTemplateQuery(string GroupId) : IRequest<byte[]>;
