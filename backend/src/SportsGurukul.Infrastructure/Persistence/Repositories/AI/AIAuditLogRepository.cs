using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Infrastructure.Persistence.Repositories.AI;

public class AIAuditLogRepository : Repository<Domain.Entities.AI.AIAuditLog>, IAIAuditLogRepository
{
    public AIAuditLogRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Domain.Entities.AI.AIAuditLog?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.AIAuditLog>()
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.AI.AIAuditLog>> GetByEntityIdAsync(Guid entityId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.AIAuditLog>()
            .AsNoTracking()
            .Where(l => l.EntityId == entityId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.AI.AIAuditLog>> GetByEventTypeAsync(string eventType, CancellationToken cancellationToken = default)
    {
        var auditEventType = Enum.Parse<AuditEventType>(eventType);
        return await Context.Set<Domain.Entities.AI.AIAuditLog>()
            .AsNoTracking()
            .Where(l => l.EventType == auditEventType)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.AI.AIAuditLog>> GetRecentBySeverityAsync(string severity, int count, CancellationToken cancellationToken = default)
    {
        var auditSeverity = Enum.Parse<AuditSeverity>(severity);
        return await Context.Set<Domain.Entities.AI.AIAuditLog>()
            .AsNoTracking()
            .Where(l => l.Severity == auditSeverity)
            .OrderByDescending(l => l.CreatedAt)
            .Take(count)
            .ToListAsync(cancellationToken);
    }
}
