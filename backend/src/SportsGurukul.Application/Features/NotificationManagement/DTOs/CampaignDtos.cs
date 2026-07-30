using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Application.Features.NotificationManagement.DTOs;

public record CampaignDto(
    Guid Id,
    string Name,
    string? Description,
    Guid? TemplateId,
    NotificationChannelType ChannelType,
    NotificationStatus Status,
    DateTime? ScheduledAt,
    DateTime? StartedAt,
    DateTime? CompletedAt,
    string? TargetCriteria,
    int TotalCount,
    int SuccessCount,
    int FailureCount,
    string? Metadata,
    DateTime CreatedAt
);

public record CreateCampaignRequest(
    string Name,
    string? Description,
    Guid? TemplateId,
    NotificationChannelType ChannelType,
    DateTime? ScheduledAt,
    string? TargetCriteria,
    string? Metadata
);

public record ScheduleCampaignRequest(
    Guid CampaignId,
    DateTime ScheduledAt
);
