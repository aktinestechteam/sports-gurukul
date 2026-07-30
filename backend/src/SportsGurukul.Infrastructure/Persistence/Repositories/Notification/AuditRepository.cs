using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Domain.Entities.Notification;

namespace SportsGurukul.Infrastructure.Persistence.Repositories.Notification;

public class AuditRepository : Repository<NotificationAudit>, IAuditRepository
{
    public AuditRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<NotificationAudit>> GetByEntityAsync(string entityType, Guid entityId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<NotificationAudit>()
            .AsNoTracking()
            .Where(a => a.EntityType == entityType && a.EntityId == entityId)
            .OrderByDescending(a => a.ChangedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<NotificationAudit>> GetByActionAsync(string action, CancellationToken cancellationToken = default)
    {
        return await Context.Set<NotificationAudit>()
            .AsNoTracking()
            .Where(a => a.Action == action)
            .OrderByDescending(a => a.ChangedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<NotificationAudit>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        return await Context.Set<NotificationAudit>()
            .AsNoTracking()
            .Where(a => a.ChangedAt >= from && a.ChangedAt <= to)
            .OrderByDescending(a => a.ChangedAt)
            .ToListAsync(cancellationToken);
    }
}
