using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Infrastructure.Persistence.Repositories.Notification;

public class DeliveryRepository : Repository<NotificationDelivery>, IDeliveryRepository
{
    public DeliveryRepository(ApplicationDbContext context) : base(context) { }

    public async Task<NotificationDelivery?> GetByProviderMessageIdAsync(string providerMessageId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<NotificationDelivery>()
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.ProviderMessageId == providerMessageId, cancellationToken);
    }

    public async Task<IReadOnlyList<NotificationDelivery>> GetByNotificationIdAsync(Guid notificationId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<NotificationDelivery>()
            .AsNoTracking()
            .Where(d => d.NotificationId == notificationId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<NotificationDelivery>> GetByStatusAsync(NotificationStatus status, CancellationToken cancellationToken = default)
    {
        return await Context.Set<NotificationDelivery>()
            .AsNoTracking()
            .Where(d => d.Status == status)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<NotificationDelivery>> GetFailedDeliveriesAsync(int maxRetries, CancellationToken cancellationToken = default)
    {
        return await Context.Set<NotificationDelivery>()
            .AsNoTracking()
            .Where(d => d.Status == NotificationStatus.Failed && d.AttemptCount < maxRetries)
            .ToListAsync(cancellationToken);
    }
}
