using MediatR;

namespace SportsGurukul.Application.Features.NotificationManagement.DomainEvents;

public record CampaignStartedEvent(
    Guid CampaignId,
    string Name,
    int TargetCount,
    DateTime StartedAt
) : INotification;
