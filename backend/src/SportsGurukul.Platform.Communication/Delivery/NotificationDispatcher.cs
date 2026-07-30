using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;
using SportsGurukul.Platform.Communication.Abstractions;
using SportsGurukul.Platform.Communication.Configuration;
using SportsGurukul.Platform.Communication.Observability;
using SportsGurukul.Platform.Communication.Security;
using SportsGurukul.Platform.Communication.Webhook;

namespace SportsGurukul.Platform.Communication.Delivery;

public class NotificationDispatcher : INotificationDispatcher
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly IPreferenceRepository _preferenceRepository;
    private readonly IRecipientResolver _recipientResolver;
    private readonly ITemplateRenderer _templateRenderer;
    private readonly INotificationProviderFactory _providerFactory;
    private readonly RetryEngine _retryEngine;
    private readonly DeliveryTracker _deliveryTracker;
    private readonly DeliveryAuditLogger _auditLogger;
    private readonly DataMasker _dataMasker;
    private readonly DeliveryMetricsCollector _metrics;
    private readonly IOptions<CommunicationOptions> _options;
    private readonly ILogger<NotificationDispatcher> _logger;

    public NotificationDispatcher(
        INotificationRepository notificationRepository,
        IDeliveryRepository deliveryRepository,
        IPreferenceRepository preferenceRepository,
        IRecipientResolver recipientResolver,
        ITemplateRenderer templateRenderer,
        INotificationProviderFactory providerFactory,
        RetryEngine retryEngine,
        DeliveryTracker deliveryTracker,
        DeliveryAuditLogger auditLogger,
        DataMasker dataMasker,
        DeliveryMetricsCollector metrics,
        IOptions<CommunicationOptions> options,
        ILogger<NotificationDispatcher> logger)
    {
        _notificationRepository = notificationRepository;
        _deliveryRepository = deliveryRepository;
        _preferenceRepository = preferenceRepository;
        _recipientResolver = recipientResolver;
        _templateRenderer = templateRenderer;
        _providerFactory = providerFactory;
        _retryEngine = retryEngine;
        _deliveryTracker = deliveryTracker;
        _auditLogger = auditLogger;
        _dataMasker = dataMasker;
        _metrics = metrics;
        _options = options;
        _logger = logger;
    }

    public async Task<Result<bool>> DispatchAsync(Guid notificationId, CancellationToken cancellationToken = default)
    {
        var notification = await _notificationRepository.GetByIdWithDetailsAsync(notificationId, cancellationToken);
        if (notification is null)
            return Result<bool>.Failure($"Notification {notificationId} not found");

        _logger.LogInformation("Dispatching notification {Id} with {Count} recipients via {Channel}",
            notificationId, notification.Recipients.Count, notification.Channel?.ChannelType);

        var channelType = notification.Channel?.ChannelType ?? NotificationChannelType.Email;
        var successCount = 0;
        var failureCount = 0;

        foreach (var recipient in notification.Recipients)
        {
            var result = await DispatchToRecipient(notification, recipient, channelType, cancellationToken);
            if (result.IsSuccess)
                successCount++;
            else
                failureCount++;
        }

        UpdateNotificationStatus(notification, successCount, failureCount);

        _logger.LogInformation("Dispatch completed for {Id}: {Success} succeeded, {Failure} failed",
            notificationId, successCount, failureCount);

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> DispatchToRecipientAsync(Guid notificationId, Guid recipientId,
        CancellationToken cancellationToken = default)
    {
        var notification = await _notificationRepository.GetByIdWithDetailsAsync(notificationId, cancellationToken);
        if (notification is null)
            return Result<bool>.Failure($"Notification {notificationId} not found");

        var recipient = notification.Recipients.FirstOrDefault(r => r.Id == recipientId);
        if (recipient is null)
            return Result<bool>.Failure($"Recipient {recipientId} not found on notification {notificationId}");

        var channelType = notification.Channel?.ChannelType ?? NotificationChannelType.Email;
        var result = await DispatchToRecipient(notification, recipient, channelType, cancellationToken);

        return Result<bool>.Success(result.IsSuccess);
    }

    private async Task<ProviderSendResult> DispatchToRecipient(
        Domain.Entities.Notification.Notification notification,
        NotificationRecipient recipient,
        NotificationChannelType channelType,
        CancellationToken cancellationToken)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            if (!await CheckUserPreferences(recipient, channelType, cancellationToken))
            {
                _logger.LogInformation("Skipping recipient {RecipientId} due to preferences", recipient.Id);
                return new ProviderSendResult { IsSuccess = true, DurationMs = 0 };
            }

            var renderedContent = await RenderContent(notification, recipient, cancellationToken);

            var delivery = await CreateDeliveryRecord(notification, recipient, channelType, cancellationToken);

            var result = await SendWithRetry(notification, recipient, delivery, renderedContent, channelType, cancellationToken);

            sw.Stop();
            await _deliveryTracker.RecordDeliveryAttempt(delivery.Id, result, sw.ElapsedMilliseconds, cancellationToken);

            _metrics.RecordDelivery(channelType.ToString(), result.IsSuccess, sw.ElapsedMilliseconds);

            if (_options.Value.Security.AuditLoggingEnabled)
            {
                await _auditLogger.LogDispatch(notification, recipient, result, cancellationToken);
            }

            return result;
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogError(ex, "Failed to dispatch to recipient {RecipientId}", recipient.Id);
            _metrics.RecordDelivery(channelType.ToString(), false, sw.ElapsedMilliseconds);

            return new ProviderSendResult
            {
                IsSuccess = false,
                ErrorMessage = ex.Message,
                ErrorCode = "DISPATCH_ERROR",
                DurationMs = sw.ElapsedMilliseconds
            };
        }
    }

    private async Task<bool> CheckUserPreferences(NotificationRecipient recipient, NotificationChannelType channelType,
        CancellationToken cancellationToken)
    {
        if (recipient.UserId is null)
            return true;

        var isEnabled = await _preferenceRepository.IsChannelEnabledAsync(
            recipient.UserId.Value, channelType, cancellationToken);

        if (!isEnabled)
        {
            _logger.LogInformation("Channel {Channel} disabled for user {UserId}",
                channelType, recipient.UserId.Value);
            return false;
        }

        var preferences = await _preferenceRepository.GetByUserIdAsync(recipient.UserId.Value, cancellationToken);
        var channelPref = preferences.FirstOrDefault(p => p.ChannelType == channelType);

        if (channelPref?.QuietHoursStart is not null && channelPref?.QuietHoursEnd is not null)
        {
            var now = TimeOnly.FromDateTime(DateTime.UtcNow);
            if (IsInQuietHours(now, channelPref.QuietHoursStart.Value, channelPref.QuietHoursEnd.Value))
            {
                _logger.LogInformation("Quiet hours active for user {UserId} on channel {Channel}",
                    recipient.UserId.Value, channelType);
                return false;
            }
        }

        return true;
    }

    private static bool IsInQuietHours(TimeOnly now, TimeOnly start, TimeOnly end)
    {
        if (start <= end)
            return now >= start && now <= end;

        return now >= start || now <= end;
    }

    private async Task<(string Subject, string Body)> RenderContent(
        Domain.Entities.Notification.Notification notification,
        NotificationRecipient recipient,
        CancellationToken cancellationToken)
    {
        if (notification.Template is null)
            return (notification.Subject, notification.Body);

        var variables = new Dictionary<string, string>
        {
            ["recipientName"] = recipient.RecipientName ?? "User",
            ["recipientEmail"] = _dataMasker.MaskEmail(recipient.DestinationAddress),
            ["notificationId"] = notification.Id.ToString()
        };

        var renderResult = await _templateRenderer.RenderAsync(
            notification.Template.SubjectTemplate,
            notification.Template.BodyTemplate,
            variables,
            cancellationToken);

        return renderResult.IsSuccess
            ? renderResult.Value!
            : (notification.Subject, notification.Body);
    }

    private async Task<Domain.Entities.Notification.NotificationDelivery> CreateDeliveryRecord(
        Domain.Entities.Notification.Notification notification,
        NotificationRecipient recipient,
        NotificationChannelType channelType,
        CancellationToken cancellationToken)
    {
        var delivery = new Domain.Entities.Notification.NotificationDelivery
        {
            Id = Guid.NewGuid(),
            NotificationId = notification.Id,
            RecipientId = recipient.Id,
            ChannelType = channelType,
            Status = NotificationStatus.Sending,
            AttemptCount = 0
        };

        await _deliveryRepository.AddAsync(delivery, cancellationToken);
        return delivery;
    }

    private async Task<ProviderSendResult> SendWithRetry(
        Domain.Entities.Notification.Notification notification,
        NotificationRecipient recipient,
        Domain.Entities.Notification.NotificationDelivery delivery,
        (string Subject, string Body) renderedContent,
        NotificationChannelType channelType,
        CancellationToken cancellationToken)
    {
        var providers = _options.Value.Delivery.FailoverEnabled
            ? _providerFactory.GetProvidersForChannel(channelType)
            : new[] { _providerFactory.GetProvider(channelType) };

        if (providers.Count == 0)
        {
            return new ProviderSendResult
            {
                IsSuccess = false,
                ErrorMessage = $"No available providers for channel {channelType}"
            };
        }

        foreach (var provider in providers)
        {
            var message = new ProviderMessage
            {
                To = recipient.DestinationAddress,
                Subject = renderedContent.Subject,
                Body = renderedContent.Body,
                IsHtml = channelType == NotificationChannelType.Email,
                RecipientName = recipient.RecipientName
            };

            var result = await _retryEngine.ExecuteWithRetryAsync(
                () => provider.SendAsync(message, cancellationToken),
                delivery.Id,
                cancellationToken);

            if (result.IsSuccess)
                return result;
        }

        return new ProviderSendResult
        {
            IsSuccess = false,
            ErrorMessage = $"All providers failed for channel {channelType}",
            ErrorCode = "ALL_PROVIDERS_FAILED"
        };
    }

    private void UpdateNotificationStatus(
        Domain.Entities.Notification.Notification notification,
        int successCount,
        int failureCount)
    {
        if (failureCount == 0 && successCount > 0)
        {
            notification.Status = NotificationStatus.Sent;
            notification.SentAt = DateTime.UtcNow;
        }
        else if (successCount == 0 && failureCount > 0)
        {
            notification.Status = NotificationStatus.Failed;
            notification.FailedAt = DateTime.UtcNow;
            notification.FailureReason = "All delivery attempts failed";
        }
        else
        {
            notification.Status = NotificationStatus.Sent;
            notification.SentAt = DateTime.UtcNow;
        }

        _notificationRepository.Update(notification);
    }
}
