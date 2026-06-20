namespace GiapTech.BindingDocx.Application.Workspace.Interfaces;

public interface ITemplateRenderer
{
    byte[] RenderDocx(string templatePath, Dictionary<string, string> data);
    byte[] RenderXlsx(string templatePath, Dictionary<string, string> singleFields,
        List<Dictionary<string, string>>? tableData = null);
    byte[] CreateZip(Dictionary<string, byte[]> files);
}
