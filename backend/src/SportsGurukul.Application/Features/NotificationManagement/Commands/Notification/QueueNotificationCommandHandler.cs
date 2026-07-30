using MediatR;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Notification;

public class QueueNotificationCommandHandler
    : IRequestHandler<QueueNotificationCommand, Result<bool>>
{
    private readonly INotificationService _notificationService;

    public QueueNotificationCommandHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task<Result<bool>> Handle(
        QueueNotificationCommand request,
        CancellationToken cancellationToken)
    {
        return await _notificationService.QueueAsync(request.Id, cancellationToken);
    }
}
