using System.Security.Claims;

namespace GiapTech.BindingDocx.Api.Extensions;

public interface ICurrentUserService
{
    Guid UserId { get; }
    string Username { get; }
    bool IsAuthenticated { get; }
}

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public Guid UserId
    {
        get
        {
            var claim = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)
                ?? httpContextAccessor.HttpContext?.User?.FindFirst("sub");
            return claim != null && Guid.TryParse(claim.Value, out var id) ? id : Guid.Empty;
        }
    }

    public string Username =>
        httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value
        ?? httpContextAccessor.HttpContext?.User?.FindFirst("unique_name")?.Value
        ?? string.Empty;

    public bool IsAuthenticated =>
        httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
}
