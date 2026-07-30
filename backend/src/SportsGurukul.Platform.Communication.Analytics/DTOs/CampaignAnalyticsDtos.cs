using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Platform.Communication.Analytics.DTOs;

public enum CampaignType
{
    OneTime,
    Recurring,
    Scheduled,
    Triggered,
    Bulk,
    Segment
}

public enum CampaignStatus
{
    Draft,
    Active,
    Paused,
    Completed,
    Cancelled,
    Archived
}

public enum RecurrencePattern
{
    Daily,
    Weekly,
    Monthly,
    Yearly,
    Custom
}

public record CampaignDetailDto(
    Guid Id,
    string Name,
    string? Description,
    CampaignType CampaignType,
    CampaignStatus Status,
    Guid? TemplateId,
    string? TemplateName,
    NotificationChannelType ChannelType,
    ScheduleDefinitionDto? Schedule,
    AudienceDefinitionDto? Audience,
    int TotalRecipients,
    int SentCount,
    int DeliveredCount,
    int FailedCount,
    int OpenCount,
    int ClickCount,
    int ReadCount,
    int UnsubscribeCount,
    double DeliveryRate,
    double OpenRate,
    double ClickRate,
    double FailureRate,
    DateTime? ScheduledAt,
    DateTime? StartedAt,
    DateTime? CompletedAt,
    DateTime? LastProcessedAt,
    string? CreatedBy,
    string? Metadata,
    DateTime CreatedAt,
    List<CampaignBatchDto> Batches
);

public record CampaignBatchDto(
    Guid Id,
    int BatchNumber,
    int TotalCount,
    int SentCount,
    int DeliveredCount,
    int FailedCount,
    DateTime? StartedAt,
    DateTime? CompletedAt,
    string? Error
);

public record ScheduleDefinitionDto(
    RecurrencePattern? Pattern,
    string? CronExpression,
    DateTime? StartDate,
    DateTime? EndDate,
    TimeSpan? TimeOfDay,
    int? IntervalMinutes,
    string? TimeZone,
    List<DayOfWeek>? DaysOfWeek,
    int? DayOfMonth,
    int? RepeatCount,
    int? MaxRetries,
    int? RetryDelayMinutes
);

public record AudienceDefinitionDto(
    List<string>? SegmentIds,
    List<string>? UserIds,
    List<string>? RoleNames,
    List<string>? TagFilters,
    string? CustomQuery,
    bool IncludeAllUsers,
    Dictionary<string, object>? DynamicFilters
);

public record CreateCampaignFullRequest(
    string Name,
    string? Description,
    CampaignType CampaignType,
    Guid? TemplateId,
    NotificationChannelType ChannelType,
    ScheduleDefinitionDto? Schedule,
    AudienceDefinitionDto? Audience,
    string? Metadata
);

public record UpdateCampaignRequest(
    string? Name,
    string? Description,
    CampaignType? CampaignType,
    Guid? TemplateId,
    NotificationChannelType? ChannelType,
    ScheduleDefinitionDto? Schedule,
    AudienceDefinitionDto? Audience,
    string? Metadata
);

public record CampaignCloneRequest(
    string NewName,
    string? NewDescription,
    bool IncludeSchedule,
    bool IncludeAudience,
    bool IncludeTemplate
);

public record PauseCampaignResult(
    Guid CampaignId,
    CampaignStatus PreviousStatus,
    CampaignStatus NewStatus,
    DateTime PausedAt
);

public record ResumeCampaignResult(
    Guid CampaignId,
    CampaignStatus PreviousStatus,
    CampaignStatus NewStatus,
    DateTime ResumedAt
);

public record CampaignSearchCriteria(
    string? Query,
    CampaignType? CampaignType,
    CampaignStatus? Status,
    NotificationChannelType? ChannelType,
    DateTime? CreatedAfter,
    DateTime? CreatedBefore,
    string? CreatedBy,
    bool? HasSchedule,
    bool? HasAudience,
    int PageNumber = 1,
    int PageSize = 20
);

public record CampaignSearchResult(
    List<CampaignDetailDto> Items,
    int TotalCount,
    int PageNumber,
    int PageSize,
    bool HasNextPage
);

public record CampaignTriggerResult(
    Guid CampaignId,
    int RecipientsQueued,
    string Status,
    DateTime TriggeredAt
);

public record CampaignBulkCreateRequest(
    List<CreateCampaignFullRequest> Campaigns,
    bool ValidateOnly = false
);

public record CampaignBulkCreateResult(
    int TotalRequested,
    int Created,
    int Failed,
    List<string> Errors
);
