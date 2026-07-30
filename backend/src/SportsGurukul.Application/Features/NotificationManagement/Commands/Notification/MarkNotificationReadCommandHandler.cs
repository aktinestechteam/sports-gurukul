using MediatR;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Notification;

public class MarkNotificationReadCommandHandler
    : IRequestHandler<MarkNotificationReadCommand, Result<bool>>
{
    private readonly INotificationService _notificationService;

    public MarkNotificationReadCommandHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task<Result<bool>> Handle(
        MarkNotificationReadCommand request,
        CancellationToken cancellationToken)
    {
        return await _notificationService.MarkReadAsync(request.Id, request.UserId, cancellationToken);
    }
}
