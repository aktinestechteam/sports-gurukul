using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Common.Interfaces.Notification.Services;

public interface INotificationDispatcher
{
    Task<Result<bool>> DispatchAsync(Guid notificationId, CancellationToken cancellationToken = default);
    Task<Result<bool>> DispatchToRecipientAsync(Guid notificationId, Guid recipientId, CancellationToken cancellationToken = default);
}
