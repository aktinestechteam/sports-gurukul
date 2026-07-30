using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Notification;

public record UpdateNotificationCommand(
    Guid Id,
    string? Subject,
    string? Body,
    NotificationPriority? Priority,
    Guid? ProviderId,
    DateTime? ScheduledAt,
    string? Metadata
) : IRequest<Result<NotificationDto>>;
