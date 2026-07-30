using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Notification;

public record MarkNotificationReadCommand(Guid Id, Guid? UserId) : IRequest<Result<bool>>;
