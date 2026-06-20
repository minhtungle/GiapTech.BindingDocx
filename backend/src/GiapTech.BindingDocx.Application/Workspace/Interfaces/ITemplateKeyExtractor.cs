using GiapTech.BindingDocx.Application.Workspace.DTOs;

namespace GiapTech.BindingDocx.Application.Workspace.Interfaces;

public interface ITemplateKeyExtractor
{
    GroupKeysDto ExtractKeys(string groupPath);
}
