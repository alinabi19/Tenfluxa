using System.Security.Claims;

public class TenantProvider : ITenantProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TenantProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid GetTenantId()
    {
        var tenantClaim = _httpContextAccessor.HttpContext?
            .User?
            .FindFirst("tenantId")?.Value;

        if (string.IsNullOrEmpty(tenantClaim))
            throw new UnauthorizedAccessException("TenantId not found in token");

        return Guid.Parse(tenantClaim);
    }
}