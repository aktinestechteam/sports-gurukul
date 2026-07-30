using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Application.Common.Interfaces.Notification;

public interface IDeliveryRepository : IRepository<NotificationDelivery>
{
    Task<NotificationDelivery?> GetByProviderMessageIdAsync(string providerMessageId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<NotificationDelivery>> GetByNotificationIdAsync(Guid notificationId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<NotificationDelivery>> GetByStatusAsync(NotificationStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<NotificationDelivery>> GetFailedDeliveriesAsync(int maxRetries, CancellationToken cancellationToken = default);
}
