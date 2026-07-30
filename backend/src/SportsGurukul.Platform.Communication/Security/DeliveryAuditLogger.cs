using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Platform.Communication.Abstractions;

namespace SportsGurukul.Platform.Communication.Security;

public class DeliveryAuditLogger
{
    private readonly IAuditRepository _auditRepository;
    private readonly DataMasker _dataMasker;
    private readonly ILogger<DeliveryAuditLogger> _logger;

    public DeliveryAuditLogger(
        IAuditRepository auditRepository,
        DataMasker dataMasker,
        ILogger<DeliveryAuditLogger> logger)
    {
        _auditRepository = auditRepository;
        _dataMasker = dataMasker;
        _logger = logger;
    }

    public async Task LogDispatch(
        Domain.Entities.Notification.Notification notification,
        NotificationRecipient recipient,
        ProviderSendResult result,
        CancellationToken cancellationToken)
    {
        try
        {
            var audit = new NotificationAudit
            {
                Id = Guid.NewGuid(),
                EntityType = "NotificationDelivery",
                EntityId = notification.Id,
                Action = result.IsSuccess ? "DispatchSuccess" : "DispatchFailed",
                OldValue = null,
                NewValue = System.Text.Json.JsonSerializer.Serialize(new
                {
                    notification.Id,
                    RecipientId = recipient.Id,
                    RecipientAddress = _dataMasker.MaskEmail(recipient.DestinationAddress),
                    result.ProviderMessageId,
                    result.ErrorMessage,
                    result.ErrorCode,
                    result.DurationMs,
                    Timestamp = DateTime.UtcNow
                }),
                ChangedAt = DateTime.UtcNow
            };

            await _auditRepository.AddAsync(audit, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to write audit log for notification {NotificationId}", notification.Id);
        }
    }

    public async Task LogQueueAction(
        Guid notificationId,
        string action,
        string? details,
        CancellationToken cancellationToken)
    {
        try
        {
            var audit = new NotificationAudit
            {
                Id = Guid.NewGuid(),
                EntityType = "NotificationQueue",
                EntityId = notificationId,
                Action = action,
                NewValue = details,
                ChangedAt = DateTime.UtcNow
            };

            await _auditRepository.AddAsync(audit, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to write queue audit log for notification {NotificationId}", notificationId);
        }
    }
}
