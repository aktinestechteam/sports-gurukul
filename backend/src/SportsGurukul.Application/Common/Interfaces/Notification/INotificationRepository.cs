using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Application.Common.Interfaces.Notification;

public interface INotificationRepository : IRepository<Domain.Entities.Notification.Notification>
{
    Task<Domain.Entities.Notification.Notification?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Domain.Entities.Notification.Notification>> GetByStatusAsync(NotificationStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Domain.Entities.Notification.Notification>> GetByPriorityAsync(NotificationPriority priority, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Domain.Entities.Notification.Notification>> GetByBatchIdAsync(Guid batchId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Domain.Entities.Notification.Notification>> GetByCampaignIdAsync(Guid campaignId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Domain.Entities.Notification.Notification>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Domain.Entities.Notification.Notification>> GetPendingAsync(int take, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Domain.Entities.Notification.Notification>> GetScheduledDueAsync(CancellationToken cancellationToken = default);
}
