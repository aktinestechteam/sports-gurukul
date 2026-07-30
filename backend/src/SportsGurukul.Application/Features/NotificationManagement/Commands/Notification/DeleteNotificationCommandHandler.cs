using MediatR;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Notification;

public class DeleteNotificationCommandHandler
    : IRequestHandler<DeleteNotificationCommand, Result<bool>>
{
    private readonly INotificationService _notificationService;

    public DeleteNotificationCommandHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task<Result<bool>> Handle(
        DeleteNotificationCommand request,
        CancellationToken cancellationToken)
    {
        return await _notificationService.DeleteAsync(request.Id, cancellationToken);
    }
}
