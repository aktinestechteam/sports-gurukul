using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Notification;

public record ScheduleNotificationCommand(Guid Id, DateTime ScheduledAt) : IRequest<Result<bool>>;
