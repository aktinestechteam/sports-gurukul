using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Infrastructure.Persistence.Repositories.Notification;

public class QueueRepository : Repository<NotificationQueue>, IQueueRepository
{
    public QueueRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<NotificationQueue>> GetPendingItemsAsync(int take, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await Context.Set<NotificationQueue>()
            .AsNoTracking()
            .Where(q => q.Status == NotificationStatus.Queued
                && (q.NextAttemptAt == null || q.NextAttemptAt <= now)
                && (q.LockExpiresAt == null || q.LockExpiresAt <= now))
            .OrderBy(q => q.Priority)
            .ThenBy(q => q.QueuedAt)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<NotificationQueue>> GetByStatusAsync(NotificationStatus status, CancellationToken cancellationToken = default)
    {
        return await Context.Set<NotificationQueue>()
            .AsNoTracking()
            .Where(q => q.Status == status)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<NotificationQueue>> GetByPriorityAsync(NotificationPriority priority, CancellationToken cancellationToken = default)
    {
        return await Context.Set<NotificationQueue>()
            .AsNoTracking()
            .Where(q => q.Priority == priority)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<NotificationQueue>> GetStaleLocksAsync(DateTime threshold, CancellationToken cancellationToken = default)
    {
        return await Context.Set<NotificationQueue>()
            .AsNoTracking()
            .Where(q => q.LockExpiresAt != null && q.LockExpiresAt < threshold)
            .ToListAsync(cancellationToken);
    }

    public async Task<NotificationQueue?> GetByNotificationIdAsync(Guid notificationId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<NotificationQueue>()
            .AsNoTracking()
            .FirstOrDefaultAsync(q => q.NotificationId == notificationId, cancellationToken);
    }
}
