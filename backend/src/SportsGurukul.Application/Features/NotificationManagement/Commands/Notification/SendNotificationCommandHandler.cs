using MediatR;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Notification;

public class SendNotificationCommandHandler
    : IRequestHandler<SendNotificationCommand, Result<bool>>
{
    private readonly INotificationService _notificationService;

    public SendNotificationCommandHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task<Result<bool>> Handle(
        SendNotificationCommand request,
        CancellationToken cancellationToken)
    {
        return await _notificationService.SendAsync(request.Id, cancellationToken);
    }
}
