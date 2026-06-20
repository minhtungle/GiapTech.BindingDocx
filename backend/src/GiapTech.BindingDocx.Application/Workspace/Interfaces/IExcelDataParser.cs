using GiapTech.BindingDocx.Application.Workspace.DTOs;

namespace GiapTech.BindingDocx.Application.Workspace.Interfaces;

public interface IExcelDataParser
{
    ImportDataResultDto Parse(Stream fileStream);
}
