using GiapTech.BindingDocx.Domain.Common;

namespace GiapTech.BindingDocx.Domain.Entities;

public class UserToken : BaseEntity
{
    public Guid UserId { get; set; }
    public int CurrentToken { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public User? User { get; set; }
}
