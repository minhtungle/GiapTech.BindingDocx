namespace GiapTech.BindingDocx.Application.Workspace.DTOs;

public class TableFileInfoDto
{
    public string FileName { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string SheetName { get; set; } = "";
    public string TableKey { get; set; } = "";
    public List<string> Columns { get; set; } = [];
}
