using SportsGurukul.Domain.Entities.AI;

namespace SportsGurukul.Application.Common.Interfaces.AI;

public interface IAIAuditLogRepository : IRepository<AIAuditLog>
{
    Task<AIAuditLog?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AIAuditLog>> GetByEntityIdAsync(Guid entityId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AIAuditLog>> GetByEventTypeAsync(string eventType, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AIAuditLog>> GetRecentBySeverityAsync(string severity, int count, CancellationToken cancellationToken = default);
}
