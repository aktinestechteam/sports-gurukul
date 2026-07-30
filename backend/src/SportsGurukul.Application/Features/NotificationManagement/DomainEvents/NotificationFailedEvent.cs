using MediatR;

namespace SportsGurukul.Application.Features.NotificationManagement.DomainEvents;

public record NotificationFailedEvent(
    Guid NotificationId,
    Guid DeliveryId,
    string ChannelType,
    string FailureReason,
    int AttemptCount,
    bool IsFinal,
    DateTime FailedAt
) : INotification;
