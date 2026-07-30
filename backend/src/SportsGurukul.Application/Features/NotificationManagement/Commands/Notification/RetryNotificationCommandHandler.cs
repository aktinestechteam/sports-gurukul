using MediatR;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Notification;

public class RetryNotificationCommandHandler
    : IRequestHandler<RetryNotificationCommand, Result<bool>>
{
    private readonly INotificationService _notificationService;

    public RetryNotificationCommandHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task<Result<bool>> Handle(
        RetryNotificationCommand request,
        CancellationToken cancellationToken)
    {
        return await _notificationService.RetryAsync(request.Id, cancellationToken);
    }
}
