using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Preference;

public record UnmuteChannelCommand(Guid UserId, NotificationChannelType ChannelType) : IRequest<Result<bool>>;
