using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Interfaces.Security;

public interface ITenantContextAccessor
{
    TenantContext? Current { get; }

    IDisposable Push(TenantContext context);
}
