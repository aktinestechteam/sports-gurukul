using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Preference;

public record SubscribeCommand(
    Guid UserId,
    string EntityType,
    Guid EntityId,
    NotificationChannelType ChannelType,
    string EventType
) : IRequest<Result<bool>>;
