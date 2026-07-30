using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Notification;

public record ArchiveNotificationCommand(Guid Id) : IRequest<Result<bool>>;
