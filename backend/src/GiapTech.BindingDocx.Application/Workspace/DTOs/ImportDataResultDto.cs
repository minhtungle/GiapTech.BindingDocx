namespace GiapTech.BindingDocx.Application.Workspace.DTOs;

public class ImportDataResultDto
{
    public Dictionary<string, string> SingleFields { get; set; } = [];
    public Dictionary<string, List<Dictionary<string, string>>> TableData { get; set; } = [];
    public int TotalRows { get; set; }
}
