using SportsGurukul.Platform.Knowledge.Configuration;
using SportsGurukul.Platform.Knowledge.Models;

using SportsGurukul.Platform.Knowledge.Abstractions;

namespace SportsGurukul.Platform.Knowledge.Security;

internal sealed class TenantIsolationService : ITenantIsolationService
{
    private readonly SecurityOptions _options;

    public TenantIsolationService(KnowledgePlatformOptions options)
    {
        _options = options.Security;
    }

    public VectorFilter ScopeFilter(VectorFilter filter, KnowledgePrincipal principal)
    {
        if (!_options.EnforceTenantIsolation)
        {
            return filter;
        }

        if (string.IsNullOrEmpty(principal.TenantId))
        {
            return filter with { TenantId = filter.TenantId };
        }

        var isPrivileged = principal.Roles.Contains("admin", StringComparer.OrdinalIgnoreCase)
                          || principal.Roles.Contains("system", StringComparer.OrdinalIgnoreCase);

        if (string.IsNullOrEmpty(filter.TenantId))
        {
            return filter with { TenantId = principal.TenantId };
        }

        if (isPrivileged)
        {
            return filter;
        }

        return filter with { TenantId = principal.TenantId };
    }
}
