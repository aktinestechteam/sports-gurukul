using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Application.Common.Interfaces.Notification;

public interface IQueueRepository : IRepository<NotificationQueue>
{
    Task<IReadOnlyList<NotificationQueue>> GetPendingItemsAsync(int take, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<NotificationQueue>> GetByStatusAsync(NotificationStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<NotificationQueue>> GetByPriorityAsync(NotificationPriority priority, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<NotificationQueue>> GetStaleLocksAsync(DateTime threshold, CancellationToken cancellationToken = default);
    Task<NotificationQueue?> GetByNotificationIdAsync(Guid notificationId, CancellationToken cancellationToken = default);
}
