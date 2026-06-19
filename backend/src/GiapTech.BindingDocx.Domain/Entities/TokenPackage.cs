using GiapTech.BindingDocx.Domain.Common;

namespace GiapTech.BindingDocx.Domain.Entities;

public class TokenPackage : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public int TokenAmount { get; set; }
    public decimal PricePerToken { get; set; }
    public decimal TotalPrice { get; set; }
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; }
}
