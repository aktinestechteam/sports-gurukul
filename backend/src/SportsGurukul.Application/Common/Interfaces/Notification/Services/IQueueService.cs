using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Common.Interfaces.Notification.Services;

public interface IQueueService
{
    Task<Result<bool>> EnqueueAsync(Guid notificationId, CancellationToken cancellationToken = default);
    Task<Result<bool>> DequeueAsync(Guid notificationId, CancellationToken cancellationToken = default);
    Task<Result<bool>> MarkProcessingAsync(Guid queueId, string lockToken, CancellationToken cancellationToken = default);
    Task<Result<bool>> MarkCompletedAsync(Guid queueId, CancellationToken cancellationToken = default);
    Task<Result<bool>> MarkFailedAsync(Guid queueId, CancellationToken cancellationToken = default);
}
