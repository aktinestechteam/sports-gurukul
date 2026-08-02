using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Interfaces.Security;

public interface IAuditLogger
{
    Task AuditAsync(
        string action,
        string? entityType,
        string? entityId,
        string? actor,
        string? tenantId,
        string? severity,
        string? details,
        string? correlationId = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AuditRecord>> GetAsync(AuditQuery query, CancellationToken cancellationToken = default);
}
