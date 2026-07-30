using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Application.Features.NotificationManagement.DTOs;

public record PreferenceDto(
    Guid Id,
    Guid UserId,
    NotificationChannelType ChannelType,
    bool IsEnabled,
    TimeOnly? QuietHoursStart,
    TimeOnly? QuietHoursEnd,
    int? MaxPerDay
);

public record CreatePreferenceRequest(
    Guid UserId,
    NotificationChannelType ChannelType,
    bool IsEnabled,
    TimeOnly? QuietHoursStart,
    TimeOnly? QuietHoursEnd,
    int? MaxPerDay
);

public record UpdatePreferenceRequest(
    Guid UserId,
    NotificationChannelType ChannelType,
    bool? IsEnabled,
    TimeOnly? QuietHoursStart,
    TimeOnly? QuietHoursEnd,
    int? MaxPerDay
);

public record SubscribeRequest(
    Guid UserId,
    string EntityType,
    Guid EntityId,
    NotificationChannelType ChannelType,
    string EventType
);

public record UnsubscribeRequest(
    Guid UserId,
    string EntityType,
    Guid EntityId,
    NotificationChannelType ChannelType,
    string EventType
);

public record MuteChannelRequest(
    Guid UserId,
    NotificationChannelType ChannelType
);

public record UnmuteChannelRequest(
    Guid UserId,
    NotificationChannelType ChannelType
);
