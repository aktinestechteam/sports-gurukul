using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Platform.Communication.Analytics.DTOs;

public record DashboardKpiDto(
    string Label,
    string Value,
    double? ChangePercentage,
    string? Trend,
    string? Icon,
    string? Color,
    string? Tooltip,
    string? Format
);

public record NotificationDashboardDto(
    List<DashboardKpiDto> Kpis,
    TimeSeriesPointDto[] DeliveryTrend,
    TimeSeriesPointDto[] FailureTrend,
    ChannelPerformanceDto[] ChannelBreakdown,
    List<RecentNotificationDto> RecentNotifications,
    DateTime RefreshedAt
);

public record RecentNotificationDto(
    Guid Id,
    string? TemplateName,
    string Channel,
    string Status,
    DateTime SentAt,
    string? Recipient
);

public record CampaignDashboardDto(
    List<DashboardKpiDto> Kpis,
    CampaignPerformanceDto[] TopPerformers,
    CampaignPerformanceDto[] AtRiskCampaigns,
    CampaignStatusDistribution[] CampaignStatusDistribution,
    TimeSeriesPointDto[] CampaignTrend,
    DateTime RefreshedAt
);

public record CampaignStatusDistribution(
    string Status,
    int Count,
    double Percentage
);

public record ProviderDashboardDto(
    List<DashboardKpiDto> Kpis,
    ProviderPerformanceDto[] ProviderRankings,
    ProviderPerformanceDto[] UnderperformingProviders,
    TimeSeriesPointDto[] ProviderTrend,
    Dictionary<string, double> ProviderDistribution,
    DateTime RefreshedAt
);

public record QueueDashboardDto(
    List<DashboardKpiDto> Kpis,
    int QueueDepth,
    int ProcessingRate,
    int AverageWaitTimeMs,
    int DeadLetterCount,
    int RetryCount,
    TimeSeriesPointDto[] QueueTrend,
    TimeSeriesPointDto[] ProcessingRateTrend,
    List<QueueItemSummaryDto> OldestItems,
    DateTime RefreshedAt
);

public record QueueItemSummaryDto(
    Guid Id,
    string? TemplateName,
    string Channel,
    string Status,
    int Attempts,
    DateTime QueuedAt,
    TimeSpan WaitTime
);

public record TemplateDashboardDto(
    List<DashboardKpiDto> Kpis,
    int TotalTemplates,
    int PublishedCount,
    int DraftCount,
    int ArchivedCount,
    TemplatePerformanceSummaryDto[] MostUsedTemplates,
    TemplatePerformanceSummaryDto[] RecentlyUpdated,
    DateTime RefreshedAt
);

public record TemplatePerformanceSummaryDto(
    Guid Id,
    string Name,
    NotificationChannelType ChannelType,
    int VersionCount,
    int UsageCount,
    double AverageDeliveryRate,
    DateTime? LastUsedAt,
    DateTime UpdatedAt
);

public record DashboardSummaryDto(
    NotificationDashboardDto NotificationDashboard,
    CampaignDashboardDto CampaignDashboard,
    ProviderDashboardDto ProviderDashboard,
    QueueDashboardDto QueueDashboard,
    TemplateDashboardDto TemplateDashboard,
    DateTime GeneratedAt
);
