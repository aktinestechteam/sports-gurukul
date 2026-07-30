using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Platform.Communication.Queue;

public class QueueService : IQueueService
{
    private readonly IQueueRepository _queueRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly ILogger<QueueService> _logger;

    public QueueService(
        IQueueRepository queueRepository,
        INotificationRepository notificationRepository,
        IDeliveryRepository deliveryRepository,
        ILogger<QueueService> logger)
    {
        _queueRepository = queueRepository;
        _notificationRepository = notificationRepository;
        _deliveryRepository = deliveryRepository;
        _logger = logger;
    }

    public async Task<Result<bool>> EnqueueAsync(Guid notificationId, CancellationToken cancellationToken = default)
    {
        var notification = await _notificationRepository.GetByIdAsync(notificationId, cancellationToken);
        if (notification is null)
            return Result<bool>.Failure($"Notification {notificationId} not found");

        var existing = await _queueRepository.GetByNotificationIdAsync(notificationId, cancellationToken);
        if (existing is not null)
            return Result<bool>.Failure($"Notification {notificationId} is already queued");

        var queueItem = new NotificationQueue
        {
            Id = Guid.NewGuid(),
            NotificationId = notificationId,
            ChannelType = notification.Channel?.ChannelType ?? NotificationChannelType.Email,
            Status = NotificationStatus.Queued,
            Priority = notification.Priority,
            QueuedAt = DateTime.UtcNow,
            MaxAttempts = 3
        };

        await _queueRepository.AddAsync(queueItem, cancellationToken);
        notification.Status = NotificationStatus.Queued;
        _notificationRepository.Update(notification);

        _logger.LogInformation("Enqueued notification {NotificationId} as queue item {QueueId} with priority {Priority}",
            notificationId, queueItem.Id, queueItem.Priority);

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> DequeueAsync(Guid notificationId, CancellationToken cancellationToken = default)
    {
        var queueItem = await _queueRepository.GetByNotificationIdAsync(notificationId, cancellationToken);
        if (queueItem is null)
            return Result<bool>.Failure($"No queue entry found for notification {notificationId}");

        queueItem.Status = NotificationStatus.Cancelled;
        queueItem.ProcessCompletedAt = DateTime.UtcNow;
        _queueRepository.Update(queueItem);

        _logger.LogInformation("Dequeued notification {NotificationId}", notificationId);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> MarkProcessingAsync(Guid queueId, string lockToken,
        CancellationToken cancellationToken = default)
    {
        var queueItem = await _queueRepository.GetByIdAsync(queueId, cancellationToken);
        if (queueItem is null)
            return Result<bool>.Failure($"Queue item {queueId} not found");

        queueItem.Status = NotificationStatus.Sending;
        queueItem.ProcessStartedAt = DateTime.UtcNow;
        queueItem.LockToken = lockToken;
        queueItem.LockExpiresAt = DateTime.UtcNow.AddMinutes(5);
        _queueRepository.Update(queueItem);

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> MarkCompletedAsync(Guid queueId, CancellationToken cancellationToken = default)
    {
        var queueItem = await _queueRepository.GetByIdAsync(queueId, cancellationToken);
        if (queueItem is null)
            return Result<bool>.Failure($"Queue item {queueId} not found");

        queueItem.Status = NotificationStatus.Sent;
        queueItem.ProcessCompletedAt = DateTime.UtcNow;
        queueItem.LockToken = null;
        queueItem.LockExpiresAt = null;
        _queueRepository.Update(queueItem);

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> MarkFailedAsync(Guid queueId, CancellationToken cancellationToken = default)
    {
        var queueItem = await _queueRepository.GetByIdAsync(queueId, cancellationToken);
        if (queueItem is null)
            return Result<bool>.Failure($"Queue item {queueId} not found");

        queueItem.AttemptCount++;
        queueItem.Status = queueItem.AttemptCount >= queueItem.MaxAttempts
            ? NotificationStatus.Failed
            : NotificationStatus.Queued;
        queueItem.LockToken = null;
        queueItem.LockExpiresAt = null;

        if (queueItem.Status == NotificationStatus.Queued)
        {
            queueItem.NextAttemptAt = DateTime.UtcNow.AddMinutes(Math.Pow(2, queueItem.AttemptCount));
        }

        _queueRepository.Update(queueItem);

        return Result<bool>.Success(true);
    }
}
