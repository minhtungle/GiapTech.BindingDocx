using GiapTech.BindingDocx.Domain.Common;

namespace GiapTech.BindingDocx.Domain.Entities;

public class TemplateFile : BaseEntity
{
    public Guid GroupId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public bool IsActive { get; set; } = true;
    public ProfileGroup? Group { get; set; }
}
