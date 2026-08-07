using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Infrastructure.Persistence.Repositories.AI;

public class AuditRepository : Repository<AIAuditLog>, IAuditRepository
{
    public AuditRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<AIAuditLog>> GetByEntityAsync(string entityType, Guid entityId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<AIAuditLog>()
            .AsNoTracking()
            .Where(a => a.EntityType == entityType && a.EntityId == entityId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AIAuditLog>> GetByActionAsync(AIAuditAction action, CancellationToken cancellationToken = default)
    {
        return await Context.Set<AIAuditLog>()
            .AsNoTracking()
            .Where(a => a.Action == action)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AIAuditLog>> GetByActorAsync(Guid actorUserId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<AIAuditLog>()
            .AsNoTracking()
            .Where(a => a.ActorUserId == actorUserId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AIAuditLog>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        return await Context.Set<AIAuditLog>()
            .AsNoTracking()
            .Where(a => a.CreatedAt >= from && a.CreatedAt < to)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AIAuditLog>> GetBySeverityAsync(AIAuditSeverity severity, CancellationToken cancellationToken = default)
    {
        return await Context.Set<AIAuditLog>()
            .AsNoTracking()
            .Where(a => a.Severity == severity)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
