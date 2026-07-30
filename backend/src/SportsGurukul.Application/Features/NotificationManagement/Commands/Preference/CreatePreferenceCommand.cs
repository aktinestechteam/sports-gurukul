using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Preference;

public record CreatePreferenceCommand(
    Guid UserId,
    NotificationChannelType ChannelType,
    bool IsEnabled,
    TimeOnly? QuietHoursStart,
    TimeOnly? QuietHoursEnd,
    int? MaxPerDay
) : IRequest<Result<PreferenceDto>>;
