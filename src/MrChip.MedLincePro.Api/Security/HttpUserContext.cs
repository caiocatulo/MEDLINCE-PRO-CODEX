using System.Security.Claims;
using MrChip.MedLincePro.Business.Interfaces;

namespace MrChip.MedLincePro.Api.Security;

public sealed class HttpUserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpUserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UsuarioId => GetGuid(ClaimTypes.NameIdentifier, "UsuarioId");
    public Guid EmpresaId => GetGuid("EmpresaId");
    public Guid BureauId => GetGuid("BureauId");
    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true;

    private Guid GetGuid(params string[] claimTypes)
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user is null) return Guid.Empty;

        foreach (var claimType in claimTypes)
        {
            var value = user.FindFirstValue(claimType);
            if (Guid.TryParse(value, out var id)) return id;
        }

        return Guid.Empty;
    }
}
