using GiapTech.BindingDocx.Domain.Common;

namespace GiapTech.BindingDocx.Domain.Entities;

public class ImportRecord : BaseEntity
{
    public Guid BatchId { get; set; }
    public string JsonData { get; set; } = string.Empty;
    public string Status { get; set; } = "pending";
    public ImportBatch? Batch { get; set; }
}
