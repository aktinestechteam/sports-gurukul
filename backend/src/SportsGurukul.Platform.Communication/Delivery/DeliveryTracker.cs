using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;
using SportsGurukul.Platform.Communication.Abstractions;

namespace SportsGurukul.Platform.Communication.Delivery;

public class DeliveryTracker
{
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly ILogger<DeliveryTracker> _logger;

    public DeliveryTracker(
        IDeliveryRepository deliveryRepository,
        ILogger<DeliveryTracker> logger)
    {
        _deliveryRepository = deliveryRepository;
        _logger = logger;
    }

    public async Task RecordDeliveryAttempt(
        Guid deliveryId,
        ProviderSendResult result,
        long durationMs,
        CancellationToken cancellationToken)
    {
        var delivery = await _deliveryRepository.GetByIdAsync(deliveryId, cancellationToken);
        if (delivery is null)
        {
            _logger.LogWarning("Delivery record {DeliveryId} not found for tracking", deliveryId);
            return;
        }

        delivery.DurationMs = durationMs;

        if (result.IsSuccess)
        {
            delivery.Status = NotificationStatus.Sent;
            delivery.SentAt = DateTime.UtcNow;
            delivery.ProviderMessageId = result.ProviderMessageId;
            delivery.ProviderResponse = System.Text.Json.JsonSerializer.Serialize(result.ProviderResponse);
            _logger.LogInformation("Delivery {DeliveryId} succeeded (msg: {MessageId})",
                deliveryId, result.ProviderMessageId);
        }
        else
        {
            delivery.Status = NotificationStatus.Failed;
            delivery.FailedAt = DateTime.UtcNow;
            delivery.FailureReason = result.ErrorMessage;
            _logger.LogWarning("Delivery {DeliveryId} failed: {Error}", deliveryId, result.ErrorMessage);
        }

        _deliveryRepository.Update(delivery);
    }
}
