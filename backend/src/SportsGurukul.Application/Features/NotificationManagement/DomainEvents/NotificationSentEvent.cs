using MediatR;

namespace SportsGurukul.Application.Features.NotificationManagement.DomainEvents;

public record NotificationSentEvent(
    Guid NotificationId,
    Guid ChannelId,
    string ChannelType,
    DateTime SentAt
) : INotification;
