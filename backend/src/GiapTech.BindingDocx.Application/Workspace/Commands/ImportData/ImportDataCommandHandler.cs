using GiapTech.BindingDocx.Application.Workspace.DTOs;
using GiapTech.BindingDocx.Application.Workspace.Interfaces;
using MediatR;

namespace GiapTech.BindingDocx.Application.Workspace.Commands.ImportData;

public class ImportDataCommandHandler(IExcelDataParser parser)
    : IRequestHandler<ImportDataCommand, ImportDataResultDto>
{
    public Task<ImportDataResultDto> Handle(ImportDataCommand request, CancellationToken cancellationToken)
    {
        var result = parser.Parse(request.FileStream);
        return Task.FromResult(result);
    }
}
