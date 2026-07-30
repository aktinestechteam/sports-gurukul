using MediatR;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Notification;

public class UpdateNotificationCommandHandler
    : IRequestHandler<UpdateNotificationCommand, Result<NotificationDto>>
{
    private readonly INotificationService _notificationService;

    public UpdateNotificationCommandHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task<Result<NotificationDto>> Handle(
        UpdateNotificationCommand request,
        CancellationToken cancellationToken)
    {
        var updateRequest = new UpdateNotificationRequest(
            request.Id,
            request.Subject,
            request.Body,
            request.Priority,
            request.ProviderId,
            request.ScheduledAt,
            request.Metadata
        );

        return await _notificationService.UpdateAsync(updateRequest, cancellationToken);
    }
}
