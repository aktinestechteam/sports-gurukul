using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Platform.Communication.Delivery;

public class PriorityQueueProcessor
{
    private readonly IQueueRepository _queueRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly INotificationDispatcher _dispatcher;
    private readonly ILogger<PriorityQueueProcessor> _logger;

    public PriorityQueueProcessor(
        IQueueRepository queueRepository,
        INotificationRepository notificationRepository,
        INotificationDispatcher dispatcher,
        ILogger<PriorityQueueProcessor> logger)
    {
        _queueRepository = queueRepository;
        _notificationRepository = notificationRepository;
        _dispatcher = dispatcher;
        _logger = logger;
    }

    public async Task ProcessQueueItemsAsync(CancellationToken cancellationToken)
    {
        var pendingItems = await _queueRepository.GetPendingItemsAsync(50, cancellationToken);

        var ordered = pendingItems
            .OrderByDescending(q => q.Priority)
            .ThenBy(q => q.QueuedAt)
            .ToList();

        foreach (var queueItem in ordered)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            await ProcessQueueItem(queueItem, cancellationToken);
        }
    }

    private async Task ProcessQueueItem(NotificationQueue queueItem, CancellationToken cancellationToken)
    {
        try
        {
            queueItem.Status = NotificationStatus.Sending;
            queueItem.ProcessStartedAt = DateTime.UtcNow;
            queueItem.LockToken = Guid.NewGuid().ToString("N");
            queueItem.LockExpiresAt = DateTime.UtcNow.AddMinutes(5);
            _queueRepository.Update(queueItem);

            var result = await _dispatcher.DispatchAsync(queueItem.NotificationId, cancellationToken);

            if (result.IsSuccess)
            {
                queueItem.Status = NotificationStatus.Sent;
                queueItem.ProcessCompletedAt = DateTime.UtcNow;
                queueItem.LockToken = null;
                queueItem.LockExpiresAt = null;
            }
            else
            {
                queueItem.AttemptCount++;
                queueItem.Status = queueItem.AttemptCount >= queueItem.MaxAttempts
                    ? NotificationStatus.Failed
                    : NotificationStatus.Queued;

                queueItem.NextAttemptAt = DateTime.UtcNow.AddMinutes(Math.Pow(2, queueItem.AttemptCount));
                queueItem.LockToken = null;
                queueItem.LockExpiresAt = null;
            }

            _queueRepository.Update(queueItem);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing queue item {QueueId} for notification {NotificationId}",
                queueItem.Id, queueItem.NotificationId);

            queueItem.AttemptCount++;
            queueItem.Status = NotificationStatus.Failed;
            queueItem.LockToken = null;
            queueItem.LockExpiresAt = null;

            try { _queueRepository.Update(queueItem); }
            catch { }
        }
    }
}
