using SportsGurukul.Platform.Communication.Analytics.DTOs;

namespace SportsGurukul.Platform.Communication.Analytics.Abstractions;

public interface IAnalyticsService
{
    Task<AnalyticsSummaryDto> GetSummaryAsync(AnalyticsFilterDto? filter, CancellationToken ct = default);
    Task<DeliveryRateDto> GetDeliveryRateAsync(AnalyticsFilterDto? filter, CancellationToken ct = default);
    Task<EngagementRateDto> GetEngagementRateAsync(AnalyticsFilterDto? filter, CancellationToken ct = default);
    Task<List<ProviderPerformanceDto>> GetProviderPerformanceAsync(AnalyticsFilterDto? filter, CancellationToken ct = default);
    Task<List<ChannelPerformanceDto>> GetChannelPerformanceAsync(AnalyticsFilterDto? filter, CancellationToken ct = default);
    Task<List<CampaignPerformanceDto>> GetCampaignPerformanceAsync(AnalyticsFilterDto? filter, CancellationToken ct = default);
    Task<List<TimeSeriesPointDto>> GetTimeSeriesAsync(string metric, AnalyticsFilterDto filter, CancellationToken ct = default);
    Task<TrendAnalysisDto> GetTrendAsync(string metric, AnalyticsFilterDto filter, CancellationToken ct = default);
    Task<AnalyticsReportDto> GenerateReportAsync(AnalyticsFilterDto? filter, CancellationToken ct = default);
    Task<Dictionary<string, double>> GetChannelDistributionAsync(AnalyticsFilterDto? filter, CancellationToken ct = default);
    Task<Dictionary<string, double>> GetProviderDistributionAsync(AnalyticsFilterDto? filter, CancellationToken ct = default);
    Task<BenchmarkMetricsDto> GetBenchmarkMetricsAsync(CancellationToken ct = default);
    Task<List<TimeSeriesPointDto>> GetDeliveryTrendAsync(AnalyticsFilterDto filter, CancellationToken ct = default);
    Task<List<TimeSeriesPointDto>> GetFailureTrendAsync(AnalyticsFilterDto filter, CancellationToken ct = default);
    Task<List<TimeSeriesPointDto>> GetOpenTrendAsync(AnalyticsFilterDto filter, CancellationToken ct = default);
    Task<List<TimeSeriesPointDto>> GetClickTrendAsync(AnalyticsFilterDto filter, CancellationToken ct = default);
    Task<double> GetAverageDeliveryTimeAsync(AnalyticsFilterDto? filter, CancellationToken ct = default);
    Task<Dictionary<string, double>> GetFailureReasonBreakdownAsync(AnalyticsFilterDto? filter, CancellationToken ct = default);
    Task<List<CampaignPerformanceDto>> GetTopCampaignsAsync(int count = 10, AnalyticsFilterDto? filter = null, CancellationToken ct = default);
    Task<List<CampaignPerformanceDto>> GetAtRiskCampaignsAsync(double threshold = 0.1, AnalyticsFilterDto? filter = null, CancellationToken ct = default);
}
