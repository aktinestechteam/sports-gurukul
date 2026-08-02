using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.AI.Interfaces.Security;
using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Security;

public class DefaultTenantIsolation : ITenantIsolation
{
    private readonly ITenantContextAccessor _accessor;
    private readonly ILogger<DefaultTenantIsolation> _logger;

    public DefaultTenantIsolation(ITenantContextAccessor accessor, ILogger<DefaultTenantIsolation>? logger = null)
    {
        _accessor = accessor;
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<DefaultTenantIsolation>.Instance;
    }

    public void VerifyAccess(string? tenantId)
    {
        var current = _accessor.Current;
        if (current is null)
        {
            _logger.LogWarning("Tenant access verified without an ambient tenant context.");
            return;
        }

        if (!string.IsNullOrEmpty(tenantId) && !string.IsNullOrEmpty(current.TenantId)
            && !tenantId.Equals(current.TenantId, StringComparison.OrdinalIgnoreCase))
        {
            throw new AgentPlatformException(
                $"Tenant isolation violation: requested tenant '{tenantId}' does not match ambient tenant '{current.TenantId}'.",
                "TENANT_MISMATCH");
        }
    }

    public void VerifyAccess(string? tenantId, string? scope)
    {
        VerifyAccess(tenantId);

        if (scope is not null && scope.Contains("tenant", StringComparison.OrdinalIgnoreCase))
        {
            var current = _accessor.Current;
            if (current is null || string.IsNullOrEmpty(current.TenantId))
            {
                throw new AgentPlatformException(
                    "Tenant isolation violation: operation requires an ambient tenant context.",
                    "TENANT_REQUIRED");
            }
        }
    }
}
