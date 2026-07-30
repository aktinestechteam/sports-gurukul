using MediatR;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Notification;

public class ExpireNotificationCommandHandler
    : IRequestHandler<ExpireNotificationCommand, Result<bool>>
{
    private readonly INotificationService _notificationService;

    public ExpireNotificationCommandHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task<Result<bool>> Handle(
        ExpireNotificationCommand request,
        CancellationToken cancellationToken)
    {
        return await _notificationService.ExpireAsync(request.Id, cancellationToken);
    }
}
