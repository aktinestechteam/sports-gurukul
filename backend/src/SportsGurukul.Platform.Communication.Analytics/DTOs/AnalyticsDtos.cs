using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Platform.Communication.Analytics.DTOs;

public record AnalyticsSummaryDto(
    int TotalNotifications,
    int TotalSent,
    int TotalDelivered,
    int TotalFailed,
    int TotalOpened,
    int TotalClicked,
    int TotalRead,
    int TotalBounced,
    int TotalUnsubscribed,
    double DeliveryRate,
    double OpenRate,
    double ClickRate,
    double ReadRate,
    double BounceRate,
    double FailureRate,
    double UnsubscribeRate,
    double AverageDeliveryTimeMs,
    DateTime CalculatedAt
);

public record DeliveryRateDto(
    Guid? CampaignId,
    Guid? ProviderId,
    int Total,
    int Sent,
    int Delivered,
    int Failed,
    double DeliveryRate,
    double FailureRate,
    TimeSpan AverageDeliveryTime,
    DateTime PeriodStart,
    DateTime PeriodEnd
);

public record EngagementRateDto(
    Guid? CampaignId,
    int TotalDelivered,
    int UniqueOpens,
    int UniqueClicks,
    int TotalReads,
    int TotalBounces,
    int TotalUnsubscribes,
    double OpenRate,
    double ClickRate,
    double ReadRate,
    double BounceRate,
    double UnsubscribeRate,
    DateTime PeriodStart,
    DateTime PeriodEnd
);

public record ProviderPerformanceDto(
    Guid ProviderId,
    string ProviderName,
    string ProviderType,
    int TotalSent,
    int TotalDelivered,
    int TotalFailed,
    int TotalBounced,
    double DeliveryRate,
    double FailureRate,
    double AverageDeliveryTimeMs,
    double AverageLatencyMs,
    double ThroughputPerMinute,
    int TotalRetries,
    int TotalDeadLettered,
    double ReliabilityScore,
    DateTime PeriodStart,
    DateTime PeriodEnd
);

public record ChannelPerformanceDto(
    string ChannelName,
    NotificationChannelType ChannelType,
    int TotalSent,
    int TotalDelivered,
    int TotalFailed,
    int TotalOpened,
    int TotalClicked,
    double DeliveryRate,
    double OpenRate,
    double ClickRate,
    double AverageDeliveryTimeMs,
    DateTime PeriodStart,
    DateTime PeriodEnd
);

public record CampaignPerformanceDto(
    Guid CampaignId,
    string CampaignName,
    CampaignType CampaignType,
    int TotalRecipients,
    int TotalSent,
    int TotalDelivered,
    int TotalFailed,
    int TotalOpened,
    int TotalClicked,
    int TotalRead,
    int TotalBounced,
    int TotalUnsubscribed,
    double DeliveryRate,
    double OpenRate,
    double ClickRate,
    double ReadRate,
    double BounceRate,
    double UnsubscribeRate,
    double AverageDeliveryTimeMs,
    DateTime? StartedAt,
    DateTime? CompletedAt,
    TimeSpan? Duration,
    DateTime PeriodStart,
    DateTime PeriodEnd
);

public record TimeSeriesPointDto(
    DateTime Timestamp,
    int Count,
    double? Rate,
    string? Breakdown
);

public record AnalyticsFilterDto(
    DateTime? StartDate,
    DateTime? EndDate,
    Guid? CampaignId,
    Guid? ProviderId,
    NotificationChannelType? ChannelType,
    CampaignType? CampaignType,
    string? Granularity,
    int PageNumber = 1,
    int PageSize = 20
);

public record Granularity
{
    public const string Hourly = "hourly";
    public const string Daily = "daily";
    public const string Weekly = "weekly";
    public const string Monthly = "monthly";
    public const string Quarterly = "quarterly";
    public const string Yearly = "yearly";
}

public record TrendAnalysisDto(
    List<TimeSeriesPointDto> DataPoints,
    double TrendDirection,
    double AverageValue,
    double MinValue,
    double MaxValue,
    double StandardDeviation,
    double PercentageChange,
    string Insight
);

public record AnalyticsReportDto(
    AnalyticsSummaryDto Summary,
    List<CampaignPerformanceDto> TopCampaigns,
    List<ProviderPerformanceDto> ProviderPerformance,
    List<ChannelPerformanceDto> ChannelPerformance,
    List<TimeSeriesPointDto> TrendData,
    DateTime GeneratedAt
);

public record BenchmarkMetricsDto(
    double AverageRenderTimeMs,
    double P95RenderTimeMs,
    double P99RenderTimeMs,
    double AverageScheduleTimeMs,
    double AverageDashboardLoadMs,
    double P95DashboardLoadMs,
    double ThroughputPerSecond,
    int TotalTemplatesRendered,
    int TotalCampaignsScheduled,
    int TotalDashboardLoads,
    double CacheHitRate,
    DateTime MeasuredAt
);
