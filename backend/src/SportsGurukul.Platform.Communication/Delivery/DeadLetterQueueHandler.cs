using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Domain.Enums.Notification;
using SportsGurukul.Platform.Communication.Configuration;

namespace SportsGurukul.Platform.Communication.Delivery;

public class DeadLetterQueueHandler
{
    private readonly IQueueRepository _queueRepository;
    private readonly DeliveryOptions _options;
    private readonly ILogger<DeadLetterQueueHandler> _logger;

    public DeadLetterQueueHandler(
        IQueueRepository queueRepository,
        IOptions<CommunicationOptions> options,
        ILogger<DeadLetterQueueHandler> logger)
    {
        _queueRepository = queueRepository;
        _options = options.Value.Delivery;
        _logger = logger;
    }

    public async Task ProcessDeadLetterQueueAsync(CancellationToken cancellationToken)
    {
        if (!_options.DeadLetterEnabled)
            return;

        var staleItems = await _queueRepository.GetStaleLocksAsync(
            DateTime.UtcNow.AddMinutes(-30), cancellationToken);

        foreach (var item in staleItems)
        {
            _logger.LogWarning("Moving stale queue item {QueueId} (notification {NotificationId}) to dead letter status",
                item.Id, item.NotificationId);

            item.Status = NotificationStatus.Failed;
            _queueRepository.Update(item);
        }
    }
}
