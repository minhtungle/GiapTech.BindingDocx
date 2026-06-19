using GiapTech.BindingDocx.Domain.Common;

namespace GiapTech.BindingDocx.Domain.Entities;

public class ImportBatch : BaseEntity
{
    public Guid GroupId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int TotalRecords { get; set; }
    public string Status { get; set; } = "pending";
    public ProfileGroup? Group { get; set; }
}
