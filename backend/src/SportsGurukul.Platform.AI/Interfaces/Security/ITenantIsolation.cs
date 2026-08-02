namespace SportsGurukul.Platform.AI.Interfaces.Security;

public interface ITenantIsolation
{
    void VerifyAccess(string? tenantId);

    void VerifyAccess(string? tenantId, string? scope);
}
