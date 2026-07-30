using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Domain.Enums.Notification;
using SportsGurukul.Platform.Communication.Configuration;

namespace SportsGurukul.Platform.Communication.Queue;

public class BulkDeliveryService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IQueueService _queueService;
    private readonly DeliveryOptions _options;
    private readonly ILogger<BulkDeliveryService> _logger;

    public BulkDeliveryService(
        INotificationRepository notificationRepository,
        IQueueService queueService,
        IOptions<CommunicationOptions> options,
        ILogger<BulkDeliveryService> logger)
    {
        _notificationRepository = notificationRepository;
        _queueService = queueService;
        _options = options.Value.Delivery;
        _logger = logger;
    }

    public async Task<BulkDeliveryResult> ProcessBulkAsync(Guid batchId, CancellationToken cancellationToken)
    {
        var notifications = await _notificationRepository.GetByBatchIdAsync(batchId, cancellationToken);
        var total = notifications.Count;
        var successCount = 0;
        var failureCount = 0;
        var batch = new List<Domain.Entities.Notification.Notification>();

        _logger.LogInformation("Processing bulk delivery for batch {BatchId} with {Count} notifications",
            batchId, total);

        foreach (var notification in notifications.OrderByDescending(n => n.Priority))
        {
            batch.Add(notification);

            if (batch.Count >= _options.BulkBatchSize)
            {
                await ProcessBatchChunk(batch, cancellationToken);
                successCount += batch.Count;
                batch.Clear();
                await Task.Delay(_options.ThrottleDelayMs, cancellationToken);
            }
        }

        if (batch.Count > 0)
        {
            await ProcessBatchChunk(batch, cancellationToken);
            successCount += batch.Count;
        }

        _logger.LogInformation("Bulk delivery for batch {BatchId} completed: {Success}/{Total}",
            batchId, successCount, total);

        return new BulkDeliveryResult
        {
            BatchId = batchId,
            TotalCount = total,
            SuccessCount = successCount,
            FailureCount = failureCount,
            CompletedAt = DateTime.UtcNow
        };
    }

    private async Task ProcessBatchChunk(
        List<Domain.Entities.Notification.Notification> batch,
        CancellationToken cancellationToken)
    {
        var tasks = batch.Select(n => _queueService.EnqueueAsync(n.Id, cancellationToken));
        await Task.WhenAll(tasks);
    }
}

public class BulkDeliveryResult
{
    public Guid BatchId { get; set; }
    public int TotalCount { get; set; }
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
    public DateTime CompletedAt { get; set; }
}
