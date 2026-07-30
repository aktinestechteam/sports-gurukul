using MediatR;

namespace SportsGurukul.Application.Features.NotificationManagement.DomainEvents;

public record CampaignCompletedEvent(
    Guid CampaignId,
    string Name,
    int TotalSent,
    int TotalSuccess,
    int TotalFailed,
    DateTime CompletedAt
) : INotification;
