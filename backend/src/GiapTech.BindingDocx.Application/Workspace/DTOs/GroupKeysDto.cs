namespace GiapTech.BindingDocx.Application.Workspace.DTOs;

public class GroupKeysDto
{
    public List<string> SingleFields { get; set; } = [];
    public List<TableFileInfoDto> TableFiles { get; set; } = [];
    public List<TemplateFileDto> Files { get; set; } = [];
}
