using MediatR;

namespace SportsGurukul.Application.Features.NotificationManagement.DomainEvents;

public record NotificationReadEvent(
    Guid NotificationId,
    Guid? UserId,
    DateTime ReadAt
) : INotification;
