using MediatR;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Notification;

public class ScheduleNotificationCommandHandler
    : IRequestHandler<ScheduleNotificationCommand, Result<bool>>
{
    private readonly INotificationService _notificationService;

    public ScheduleNotificationCommandHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task<Result<bool>> Handle(
        ScheduleNotificationCommand request,
        CancellationToken cancellationToken)
    {
        return await _notificationService.ScheduleAsync(request.Id, request.ScheduledAt, cancellationToken);
    }
}
