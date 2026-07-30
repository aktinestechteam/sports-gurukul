using MediatR;
using Microsoft.Extensions.Logging;

namespace SportsGurukul.Platform.Communication.Analytics.Events;

public record CampaignStartedEvent(
    Guid CampaignId,
    string Name,
    DateTime StartedAt
) : INotification;

public record CampaignPausedEvent(
    Guid CampaignId,
    DateTime PausedAt
) : INotification;

public record CampaignResumedEvent(
    Guid CampaignId,
    DateTime ResumedAt
) : INotification;

public record CampaignCompletedEvent(
    Guid CampaignId,
    int TotalSent,
    int TotalDelivered,
    int TotalFailed,
    DateTime CompletedAt
) : INotification;

public record CampaignCancelledEvent(
    Guid CampaignId,
    DateTime CancelledAt
) : INotification;

public class CampaignCompletedEventHandler(ILogger<CampaignCompletedEventHandler> logger)
    : INotificationHandler<CampaignCompletedEvent>
{
    public async Task Handle(CampaignCompletedEvent notification, CancellationToken ct)
    {
        logger.LogInformation(
            "Campaign {CampaignId} completed at {CompletedAt}. Sent: {TotalSent}, Delivered: {TotalDelivered}, Failed: {TotalFailed}",
            notification.CampaignId,
            notification.CompletedAt,
            notification.TotalSent,
            notification.TotalDelivered,
            notification.TotalFailed);

        await Task.CompletedTask;
    }
}
