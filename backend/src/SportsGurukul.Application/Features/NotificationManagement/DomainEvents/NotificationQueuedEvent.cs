using MediatR;

namespace SportsGurukul.Application.Features.NotificationManagement.DomainEvents;

public record NotificationQueuedEvent(
    Guid NotificationId,
    Guid ChannelId,
    string ChannelType,
    string Priority,
    int RecipientCount,
    DateTime QueuedAt
) : INotification;
