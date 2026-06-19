using GiapTech.BindingDocx.Domain.Common;

namespace GiapTech.BindingDocx.Domain.Entities;

public class TokenTransaction : BaseEntity
{
    public Guid UserId { get; set; }
    public string Type { get; set; } = string.Empty;
    public int Amount { get; set; }
    public string? Description { get; set; }
    public string? ReferenceId { get; set; }
    public User? User { get; set; }
}
