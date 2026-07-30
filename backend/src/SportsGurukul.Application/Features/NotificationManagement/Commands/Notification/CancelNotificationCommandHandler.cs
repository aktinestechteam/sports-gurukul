using MediatR;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Notification;

public class CancelNotificationCommandHandler
    : IRequestHandler<CancelNotificationCommand, Result<bool>>
{
    private readonly INotificationService _notificationService;

    public CancelNotificationCommandHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task<Result<bool>> Handle(
        CancelNotificationCommand request,
        CancellationToken cancellationToken)
    {
        return await _notificationService.CancelAsync(request.Id, cancellationToken);
    }
}
