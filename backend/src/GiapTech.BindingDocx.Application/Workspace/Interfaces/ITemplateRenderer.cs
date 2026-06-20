namespace GiapTech.BindingDocx.Application.Workspace.Interfaces;

public interface ITemplateRenderer
{
    byte[] RenderDocx(string templatePath, Dictionary<string, string> data);
    byte[] RenderXlsx(string templatePath, Dictionary<string, string> singleFields,
        Dictionary<string, List<Dictionary<string, string>>>? tableDataBySheet = null);
    byte[] CreateZip(Dictionary<string, byte[]> files);
}
