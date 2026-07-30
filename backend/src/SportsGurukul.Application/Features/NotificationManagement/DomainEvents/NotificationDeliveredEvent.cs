using MediatR;

namespace SportsGurukul.Application.Features.NotificationManagement.DomainEvents;

public record NotificationDeliveredEvent(
    Guid NotificationId,
    Guid DeliveryId,
    string ChannelType,
    string? ProviderMessageId,
    DateTime DeliveredAt
) : INotification;
