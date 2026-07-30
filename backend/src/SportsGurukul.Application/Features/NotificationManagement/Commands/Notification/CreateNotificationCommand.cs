using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Notification;

public record CreateNotificationCommand(
    Guid? TemplateId,
    Guid ChannelId,
    Guid? ProviderId,
    NotificationPriority Priority,
    string Subject,
    string Body,
    string? SenderId,
    DateTime? ScheduledAt,
    Guid? BatchId,
    Guid? CampaignId,
    string? ExternalId,
    string? Metadata,
    List<CreateRecipientRequest> Recipients,
    List<CreateAttachmentRequest>? Attachments
) : IRequest<Result<NotificationDto>>;
