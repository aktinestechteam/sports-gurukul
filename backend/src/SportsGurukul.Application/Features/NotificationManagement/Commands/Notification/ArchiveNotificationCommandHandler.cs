using MediatR;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Notification;

public class ArchiveNotificationCommandHandler
    : IRequestHandler<ArchiveNotificationCommand, Result<bool>>
{
    private readonly INotificationService _notificationService;

    public ArchiveNotificationCommandHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task<Result<bool>> Handle(
        ArchiveNotificationCommand request,
        CancellationToken cancellationToken)
    {
        return await _notificationService.ArchiveAsync(request.Id, cancellationToken);
    }
}
