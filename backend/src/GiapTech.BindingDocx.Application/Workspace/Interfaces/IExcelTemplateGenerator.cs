using GiapTech.BindingDocx.Application.Workspace.DTOs;

namespace GiapTech.BindingDocx.Application.Workspace.Interfaces;

public interface IExcelTemplateGenerator
{
    byte[] GenerateTemplate(GroupKeysDto keys);
}
