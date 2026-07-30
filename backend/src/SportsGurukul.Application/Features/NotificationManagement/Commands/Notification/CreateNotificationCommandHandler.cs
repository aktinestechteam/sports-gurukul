using MediatR;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Notification;

public class CreateNotificationCommandHandler
    : IRequestHandler<CreateNotificationCommand, Result<NotificationDto>>
{
    private readonly INotificationService _notificationService;

    public CreateNotificationCommandHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task<Result<NotificationDto>> Handle(
        CreateNotificationCommand request,
        CancellationToken cancellationToken)
    {
        var createRequest = new CreateNotificationRequest(
            request.TemplateId,
            request.ChannelId,
            request.ProviderId,
            request.Priority,
            request.Subject,
            request.Body,
            request.SenderId,
            request.ScheduledAt,
            request.BatchId,
            request.CampaignId,
            request.ExternalId,
            request.Metadata,
            request.Recipients,
            request.Attachments
        );

        return await _notificationService.CreateAsync(createRequest, cancellationToken);
    }
}
