using GiapTech.BindingDocx.Application.Workspace.DTOs;
using MediatR;

namespace GiapTech.BindingDocx.Application.Workspace.Commands.ImportData;

public record ImportDataCommand(string GroupId, Stream FileStream) : IRequest<ImportDataResultDto>;
