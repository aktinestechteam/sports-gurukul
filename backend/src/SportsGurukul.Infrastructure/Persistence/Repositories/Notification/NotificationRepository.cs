using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Infrastructure.Persistence.Repositories.Notification;

public class NotificationRepository : Repository<Domain.Entities.Notification.Notification>, INotificationRepository
{
    public NotificationRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Domain.Entities.Notification.Notification?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.Notification.Notification>()
            .AsNoTracking()
            .Include(n => n.Channel)
            .Include(n => n.Provider)
            .Include(n => n.Template)
            .Include(n => n.Batch)
            .Include(n => n.Campaign)
            .Include(n => n.Recipients)
            .Include(n => n.Deliveries)
            .Include(n => n.Attachments)
            .Include(n => n.Schedule)
            .AsSplitQuery()
            .FirstOrDefaultAsync(n => n.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.Notification.Notification>> GetByStatusAsync(NotificationStatus status, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.Notification.Notification>()
            .AsNoTracking()
            .Where(n => n.Status == status)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.Notification.Notification>> GetByPriorityAsync(NotificationPriority priority, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.Notification.Notification>()
            .AsNoTracking()
            .Where(n => n.Priority == priority)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.Notification.Notification>> GetByBatchIdAsync(Guid batchId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.Notification.Notification>()
            .AsNoTracking()
            .Where(n => n.BatchId == batchId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.Notification.Notification>> GetByCampaignIdAsync(Guid campaignId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.Notification.Notification>()
            .AsNoTracking()
            .Where(n => n.CampaignId == campaignId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.Notification.Notification>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.Notification.Notification>()
            .AsNoTracking()
            .Where(n => n.Recipients.Any(r => r.UserId == userId))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.Notification.Notification>> GetPendingAsync(int take, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.Notification.Notification>()
            .AsNoTracking()
            .Where(n => n.Status == NotificationStatus.Queued || n.Status == NotificationStatus.Scheduled)
            .OrderBy(n => n.CreatedAt)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.Notification.Notification>> GetScheduledDueAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await Context.Set<Domain.Entities.Notification.Notification>()
            .AsNoTracking()
            .Where(n => n.Status == NotificationStatus.Scheduled && n.ScheduledAt <= now)
            .ToListAsync(cancellationToken);
    }
}
