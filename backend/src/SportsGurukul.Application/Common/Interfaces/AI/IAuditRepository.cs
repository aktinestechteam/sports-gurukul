using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Common.Interfaces.AI;

public interface IAuditRepository : IRepository<AIAuditLog>
{
    Task<IReadOnlyList<AIAuditLog>> GetByEntityAsync(string entityType, Guid entityId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AIAuditLog>> GetByActionAsync(AIAuditAction action, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AIAuditLog>> GetByActorAsync(Guid actorUserId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AIAuditLog>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AIAuditLog>> GetBySeverityAsync(AIAuditSeverity severity, CancellationToken cancellationToken = default);
}
