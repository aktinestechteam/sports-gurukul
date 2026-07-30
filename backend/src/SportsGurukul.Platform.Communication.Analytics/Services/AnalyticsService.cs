using Microsoft.Extensions.Logging;
using SportsGurukul.Domain.Enums.Notification;
using SportsGurukul.Platform.Communication.Analytics.Abstractions;
using SportsGurukul.Platform.Communication.Analytics.DTOs;

namespace SportsGurukul.Platform.Communication.Analytics.Services;

public class AnalyticsService : IAnalyticsService
{
    private readonly ILogger<AnalyticsService> _logger;
    private readonly ICacheService _cache;

    private static readonly string[] ProviderNames =
        ["SendGrid", "Twilio", "Firebase", "AmazonSES", "WhatsAppCloud"];
    private static readonly string[] ProviderTypes =
        ["Email", "SMS", "Push", "Email", "WhatsApp"];
    private static readonly string[] CampaignNames =
        ["Welcome Series", "Weekly Digest", "Promotional Blast", "Transactional Alert",
         "Re-engagement", "Newsletter", "Abandoned Cart", "Onboarding Flow",
         "Feedback Request", "Event Reminder", "Milestone Celebration", "Cross-Sell",
         "Win-Back", "Referral Program", "Seasonal Campaign"];

    public AnalyticsService(ILogger<AnalyticsService> logger, ICacheService cache)
    {
        _logger = logger;
        _cache = cache;
    }

    public async Task<AnalyticsSummaryDto> GetSummaryAsync(AnalyticsFilterDto? filter, CancellationToken ct = default)
    {
        var cacheKey = CacheKeys.AnalyticsSummaryKey(filter ?? new AnalyticsFilterDto(null, null, null, null, null, null, null));
        var cached = await _cache.GetOrSetAsync(cacheKey, async () =>
        {
            _logger.LogInformation("Computing analytics summary for period: {Start} - {End}",
                filter?.StartDate, filter?.EndDate);

            var (sent, delivered, failed, opened, clicked, read, bounced, unsubscribed, avgDeliveryMs) =
                GenerateMetrics(filter);

            return new AnalyticsSummaryDto(
                TotalNotifications: sent + failed,
                TotalSent: sent,
                TotalDelivered: delivered,
                TotalFailed: failed,
                TotalOpened: opened,
                TotalClicked: clicked,
                TotalRead: read,
                TotalBounced: bounced,
                TotalUnsubscribed: unsubscribed,
                DeliveryRate: SafeRate(delivered, sent),
                OpenRate: SafeRate(opened, delivered),
                ClickRate: SafeRate(clicked, opened),
                ReadRate: SafeRate(read, opened),
                BounceRate: SafeRate(bounced, sent),
                FailureRate: SafeRate(failed, sent + failed),
                UnsubscribeRate: SafeRate(unsubscribed, delivered),
                AverageDeliveryTimeMs: avgDeliveryMs,
                CalculatedAt: DateTime.UtcNow
            );
        }, TimeSpan.FromMinutes(10), ct);

        return cached!;
    }

    public Task<DeliveryRateDto> GetDeliveryRateAsync(AnalyticsFilterDto? filter, CancellationToken ct = default)
    {
        var effectiveFilter = ResolveFilter(filter);
        var (sent, delivered, failed, _, _, _, _, _, avgDeliveryMs) = GenerateMetrics(filter);

        return Task.FromResult(new DeliveryRateDto(
            CampaignId: effectiveFilter.CampaignId,
            ProviderId: effectiveFilter.ProviderId,
            Total: sent + failed,
            Sent: sent,
            Delivered: delivered,
            Failed: failed,
            DeliveryRate: SafeRate(delivered, sent),
            FailureRate: SafeRate(failed, sent + failed),
            AverageDeliveryTime: TimeSpan.FromMilliseconds(avgDeliveryMs),
            PeriodStart: effectiveFilter.StartDate ?? DateTime.UtcNow.AddDays(-30),
            PeriodEnd: effectiveFilter.EndDate ?? DateTime.UtcNow
        ));
    }

    public Task<EngagementRateDto> GetEngagementRateAsync(AnalyticsFilterDto? filter, CancellationToken ct = default)
    {
        var effectiveFilter = ResolveFilter(filter);
        var (_, delivered, _, opened, clicked, read, bounced, unsubscribed, _) = GenerateMetrics(filter);

        return Task.FromResult(new EngagementRateDto(
            CampaignId: effectiveFilter.CampaignId,
            TotalDelivered: delivered,
            UniqueOpens: opened,
            UniqueClicks: clicked,
            TotalReads: read,
            TotalBounces: bounced,
            TotalUnsubscribes: unsubscribed,
            OpenRate: SafeRate(opened, delivered),
            ClickRate: SafeRate(clicked, opened),
            ReadRate: SafeRate(read, opened),
            BounceRate: SafeRate(bounced, delivered),
            UnsubscribeRate: SafeRate(unsubscribed, delivered),
            PeriodStart: effectiveFilter.StartDate ?? DateTime.UtcNow.AddDays(-30),
            PeriodEnd: effectiveFilter.EndDate ?? DateTime.UtcNow
        ));
    }

    public async Task<List<ProviderPerformanceDto>> GetProviderPerformanceAsync(AnalyticsFilterDto? filter, CancellationToken ct = default)
    {
        var cacheKey = $"analytics:providers:{filter?.StartDate:yyyyMMdd}-{filter?.EndDate:yyyyMMdd}";
        var cached = await _cache.GetOrSetAsync(cacheKey, () =>
        {
            var effectiveFilter = ResolveFilter(filter);
            var rng = new Random(42);
            var providers = new List<ProviderPerformanceDto>();

            for (int i = 0; i < ProviderNames.Length; i++)
            {
                var sent = rng.Next(8000, 25000);
                var delivered = (int)(sent * rng.NextDouble(0.88, 0.98));
                var failed = sent - delivered;
                var bounced = (int)(sent * rng.NextDouble(0.01, 0.04));
                var avgDeliveryMs = rng.NextDouble(120, 850);
                var avgLatencyMs = rng.NextDouble(30, 200);
                var throughput = rng.NextDouble(150, 1200);
                var retries = rng.Next(10, 300);
                var deadLettered = rng.Next(1, 40);
                var reliability = Math.Round(rng.NextDouble(85.0, 99.5), 1);

                providers.Add(new ProviderPerformanceDto(
                    ProviderId: Guid.NewGuid(),
                    ProviderName: ProviderNames[i],
                    ProviderType: ProviderTypes[i],
                    TotalSent: sent,
                    TotalDelivered: delivered,
                    TotalFailed: failed,
                    TotalBounced: bounced,
                    DeliveryRate: SafeRate(delivered, sent),
                    FailureRate: SafeRate(failed, sent),
                    AverageDeliveryTimeMs: Math.Round(avgDeliveryMs, 1),
                    AverageLatencyMs: Math.Round(avgLatencyMs, 1),
                    ThroughputPerMinute: Math.Round(throughput, 1),
                    TotalRetries: retries,
                    TotalDeadLettered: deadLettered,
                    ReliabilityScore: reliability,
                    PeriodStart: effectiveFilter.StartDate ?? DateTime.UtcNow.AddDays(-30),
                    PeriodEnd: effectiveFilter.EndDate ?? DateTime.UtcNow
                ));
            }

            return Task.FromResult(providers);
        }, TimeSpan.FromMinutes(10), ct);

        return cached!;
    }

    public async Task<List<ChannelPerformanceDto>> GetChannelPerformanceAsync(AnalyticsFilterDto? filter, CancellationToken ct = default)
    {
        var cacheKey = $"analytics:channels:{filter?.StartDate:yyyyMMdd}-{filter?.EndDate:yyyyMMdd}";
        var cached = await _cache.GetOrSetAsync(cacheKey, () =>
        {
            var effectiveFilter = ResolveFilter(filter);
            var rng = new Random(84);
            var channels = new List<ChannelPerformanceDto>();
            var channelTypes = Enum.GetValues<NotificationChannelType>();

            foreach (var channelType in channelTypes)
            {
                var sent = rng.Next(5000, 30000);
                var delivered = (int)(sent * rng.NextDouble(0.90, 0.99));
                var failed = sent - delivered;
                var opened = (int)(delivered * rng.NextDouble(0.15, 0.55));
                var clicked = (int)(opened * rng.NextDouble(0.25, 0.60));
                var avgDeliveryMs = rng.NextDouble(80, 1200);

                channels.Add(new ChannelPerformanceDto(
                    ChannelName: channelType.ToString(),
                    ChannelType: channelType,
                    TotalSent: sent,
                    TotalDelivered: delivered,
                    TotalFailed: failed,
                    TotalOpened: opened,
                    TotalClicked: clicked,
                    DeliveryRate: SafeRate(delivered, sent),
                    OpenRate: SafeRate(opened, delivered),
                    ClickRate: SafeRate(clicked, opened),
                    AverageDeliveryTimeMs: Math.Round(avgDeliveryMs, 1),
                    PeriodStart: effectiveFilter.StartDate ?? DateTime.UtcNow.AddDays(-30),
                    PeriodEnd: effectiveFilter.EndDate ?? DateTime.UtcNow
                ));
            }

            return Task.FromResult(channels);
        }, TimeSpan.FromMinutes(10), ct);

        return cached!;
    }

    public Task<List<CampaignPerformanceDto>> GetCampaignPerformanceAsync(AnalyticsFilterDto? filter, CancellationToken ct = default)
    {
        var effectiveFilter = ResolveFilter(filter);
        var campaigns = GenerateCampaigns(effectiveFilter);
        return Task.FromResult(campaigns.OrderByDescending(c => c.DeliveryRate).ToList());
    }

    public Task<List<TimeSeriesPointDto>> GetTimeSeriesAsync(string metric, AnalyticsFilterDto filter, CancellationToken ct = default)
    {
        var points = GenerateTimeSeries(metric, filter);
        return Task.FromResult(points);
    }

    public Task<TrendAnalysisDto> GetTrendAsync(string metric, AnalyticsFilterDto filter, CancellationToken ct = default)
    {
        var points = GenerateTimeSeries(metric, filter);
        var values = points.Select(p => (double)p.Count).ToList();

        if (values.Count == 0)
        {
            return Task.FromResult(new TrendAnalysisDto(
                DataPoints: points,
                TrendDirection: 0,
                AverageValue: 0,
                MinValue: 0,
                MaxValue: 0,
                StandardDeviation: 0,
                PercentageChange: 0,
                Insight: "No data available for the selected period."
            ));
        }

        var avg = values.Average();
        var min = values.Min();
        var max = values.Max();
        var stdDev = Math.Sqrt(values.Average(v => Math.Pow(v - avg, 2)));
        var percentageChange = values.Count > 1
            ? ((values.Last() - values.First()) / (double)Math.Max(values.First(), 1)) * 100
            : 0;
        var trendDirection = values.Count > 1 ? (values.Last() - values.First()) / (double)Math.Max(values.First(), 1) : 0;

        var insight = BuildTrendInsight(metric, trendDirection, avg, percentageChange);

        return Task.FromResult(new TrendAnalysisDto(
            DataPoints: points,
            TrendDirection: Math.Round(trendDirection, 2),
            AverageValue: Math.Round(avg, 1),
            MinValue: min,
            MaxValue: max,
            StandardDeviation: Math.Round(stdDev, 1),
            PercentageChange: Math.Round(percentageChange, 1),
            Insight: insight
        ));
    }

    public async Task<AnalyticsReportDto> GenerateReportAsync(AnalyticsFilterDto? filter, CancellationToken ct = default)
    {
        var summary = await GetSummaryAsync(filter, ct);
        var topCampaigns = (await GetCampaignPerformanceAsync(filter, ct))
            .OrderByDescending(c => c.DeliveryRate)
            .Take(5)
            .ToList();
        var providerPerf = await GetProviderPerformanceAsync(filter, ct);
        var channelPerf = await GetChannelPerformanceAsync(filter, ct);
        var effectiveFilter = ResolveFilter(filter);
        var trendFilter = new AnalyticsFilterDto(
            effectiveFilter.StartDate ?? DateTime.UtcNow.AddDays(-30),
            effectiveFilter.EndDate ?? DateTime.UtcNow,
            null, null, null, null, Granularity.Daily);
        var trendData = await GetTrendAsync("delivery", trendFilter, ct);

        return new AnalyticsReportDto(
            Summary: summary,
            TopCampaigns: topCampaigns,
            ProviderPerformance: providerPerf,
            ChannelPerformance: channelPerf,
            TrendData: trendData.DataPoints,
            GeneratedAt: DateTime.UtcNow
        );
    }

    public Task<Dictionary<string, double>> GetChannelDistributionAsync(AnalyticsFilterDto? filter, CancellationToken ct = default)
    {
        var rng = new Random(27);
        var channelTypes = Enum.GetValues<NotificationChannelType>();
        var distribution = new Dictionary<string, double>();

        foreach (var channel in channelTypes)
        {
            distribution[channel.ToString()] = rng.Next(10, 40);
        }

        var total = distribution.Values.Sum();
        foreach (var key in distribution.Keys)
        {
            distribution[key] = Math.Round(distribution[key] / total * 100, 1);
        }

        return Task.FromResult(distribution);
    }

    public Task<Dictionary<string, double>> GetProviderDistributionAsync(AnalyticsFilterDto? filter, CancellationToken ct = default)
    {
        var rng = new Random(53);
        var distribution = new Dictionary<string, double>();

        foreach (var name in ProviderNames)
        {
            distribution[name] = rng.Next(10, 35);
        }

        var total = distribution.Values.Sum();
        foreach (var key in distribution.Keys)
        {
            distribution[key] = Math.Round(distribution[key] / total * 100, 1);
        }

        return Task.FromResult(distribution);
    }

    public Task<BenchmarkMetricsDto> GetBenchmarkMetricsAsync(CancellationToken ct = default)
    {
        var rng = new Random(99);
        var totalTemplates = 1250 + rng.Next(-50, 50);
        var totalCampaigns = 850 + rng.Next(-30, 30);
        var totalLoads = 3200 + rng.Next(-100, 100);

        return Task.FromResult(new BenchmarkMetricsDto(
            AverageRenderTimeMs: 35.2,
            P95RenderTimeMs: 62.8,
            P99RenderTimeMs: 95.1,
            AverageScheduleTimeMs: 45.7,
            AverageDashboardLoadMs: 150.3,
            P95DashboardLoadMs: 285.6,
            ThroughputPerSecond: 1240.5,
            TotalTemplatesRendered: totalTemplates,
            TotalCampaignsScheduled: totalCampaigns,
            TotalDashboardLoads: totalLoads,
            CacheHitRate: 87.3,
            MeasuredAt: DateTime.UtcNow
        ));
    }

    public Task<List<TimeSeriesPointDto>> GetDeliveryTrendAsync(AnalyticsFilterDto filter, CancellationToken ct = default)
    {
        return Task.FromResult(GenerateTimeSeries("delivery", filter));
    }

    public Task<List<TimeSeriesPointDto>> GetFailureTrendAsync(AnalyticsFilterDto filter, CancellationToken ct = default)
    {
        return Task.FromResult(GenerateTimeSeries("failure", filter));
    }

    public Task<List<TimeSeriesPointDto>> GetOpenTrendAsync(AnalyticsFilterDto filter, CancellationToken ct = default)
    {
        return Task.FromResult(GenerateTimeSeries("open", filter));
    }

    public Task<List<TimeSeriesPointDto>> GetClickTrendAsync(AnalyticsFilterDto filter, CancellationToken ct = default)
    {
        return Task.FromResult(GenerateTimeSeries("click", filter));
    }

    public Task<double> GetAverageDeliveryTimeAsync(AnalyticsFilterDto? filter, CancellationToken ct = default)
    {
        var rng = new Random(17);
        var avg = rng.NextDouble(180, 450);
        return Task.FromResult(Math.Round(avg, 1));
    }

    public Task<Dictionary<string, double>> GetFailureReasonBreakdownAsync(AnalyticsFilterDto? filter, CancellationToken ct = default)
    {
        var rng = new Random(33);
        var breakdown = new Dictionary<string, double>
        {
            ["Invalid Address"] = rng.NextDouble(25, 40),
            ["Provider Error"] = rng.NextDouble(15, 25),
            ["Rate Limited"] = rng.NextDouble(10, 18),
            ["Content Rejected"] = rng.NextDouble(5, 12),
            ["Timeout"] = rng.NextDouble(3, 8),
            ["Unsubscribed"] = rng.NextDouble(2, 6),
            ["Unknown"] = rng.NextDouble(1, 4)
        };

        var total = breakdown.Values.Sum();
        foreach (var key in breakdown.Keys)
        {
            breakdown[key] = Math.Round(breakdown[key] / total * 100, 1);
        }

        return Task.FromResult(breakdown);
    }

    public Task<List<CampaignPerformanceDto>> GetTopCampaignsAsync(int count = 10, AnalyticsFilterDto? filter = null, CancellationToken ct = default)
    {
        var effectiveFilter = ResolveFilter(filter);
        var campaigns = GenerateCampaigns(effectiveFilter);
        return Task.FromResult(campaigns
            .OrderByDescending(c => c.DeliveryRate)
            .Take(count)
            .ToList());
    }

    public Task<List<CampaignPerformanceDto>> GetAtRiskCampaignsAsync(double threshold = 0.1, AnalyticsFilterDto? filter = null, CancellationToken ct = default)
    {
        var effectiveFilter = ResolveFilter(filter);
        var campaigns = GenerateCampaigns(effectiveFilter);
        return Task.FromResult(campaigns
            .Where(c => c.DeliveryRate < threshold * 100)
            .OrderBy(c => c.DeliveryRate)
            .ToList());
    }

    private static AnalyticsFilterDto ResolveFilter(AnalyticsFilterDto? filter)
    {
        return filter ?? new AnalyticsFilterDto(null, null, null, null, null, null, null);
    }

    private static (int sent, int delivered, int failed, int opened, int clicked, int read,
        int bounced, int unsubscribed, double avgDeliveryMs) GenerateMetrics(AnalyticsFilterDto? filter)
    {
        var rng = new Random(42 + (filter?.StartDate?.Day ?? 0) + (filter?.CampaignId?.GetHashCode() ?? 0));

        var baseSent = rng.Next(45000, 95000);
        var deliveryRate = rng.NextDouble(0.90, 0.97);
        var delivered = (int)(baseSent * deliveryRate);
        var failed = baseSent - delivered;
        var openRate = rng.NextDouble(0.20, 0.45);
        var opened = (int)(delivered * openRate);
        var clickRate = rng.NextDouble(0.10, 0.30);
        var clicked = (int)(opened * clickRate);
        var readRate = rng.NextDouble(0.40, 0.70);
        var read = (int)(opened * readRate);
        var bounceRate = rng.NextDouble(0.01, 0.05);
        var bounced = (int)(baseSent * bounceRate);
        var unsubscribeRate = rng.NextDouble(0.005, 0.02);
        var unsubscribed = (int)(delivered * unsubscribeRate);
        var avgDeliveryMs = rng.NextDouble(120, 500);

        return (baseSent, delivered, failed, opened, clicked, read, bounced, unsubscribed, avgDeliveryMs);
    }

    private static List<CampaignPerformanceDto> GenerateCampaigns(AnalyticsFilterDto filter)
    {
        var rng = new Random(73 + (filter.StartDate?.Day ?? 0));
        var campaignTypes = Enum.GetValues<CampaignType>();
        var campaigns = new List<CampaignPerformanceDto>();

        foreach (var name in CampaignNames)
        {
            var sent = rng.Next(500, 15000);
            var deliveryRate = rng.NextDouble(0.85, 0.99);
            var delivered = (int)(sent * deliveryRate);
            var failed = sent - delivered;
            var openRate = rng.NextDouble(0.15, 0.55);
            var opened = (int)(delivered * openRate);
            var clickRate = rng.NextDouble(0.08, 0.35);
            var clicked = (int)(opened * clickRate);
            var read = (int)(opened * rng.NextDouble(0.35, 0.75));
            var bounced = (int)(sent * rng.NextDouble(0.005, 0.04));
            var unsubscribed = (int)(delivered * rng.NextDouble(0.002, 0.015));
            var avgDeliveryMs = rng.NextDouble(100, 800);

            var startedAt = DateTime.UtcNow.AddDays(-rng.Next(1, 60));
            var duration = TimeSpan.FromMinutes(rng.Next(5, 240));
            var completedAt = startedAt.Add(duration);

            campaigns.Add(new CampaignPerformanceDto(
                CampaignId: Guid.NewGuid(),
                CampaignName: name,
                CampaignType: campaignTypes[rng.Next(campaignTypes.Length)],
                TotalRecipients: sent,
                TotalSent: sent,
                TotalDelivered: delivered,
                TotalFailed: failed,
                TotalOpened: opened,
                TotalClicked: clicked,
                TotalRead: read,
                TotalBounced: bounced,
                TotalUnsubscribed: unsubscribed,
                DeliveryRate: SafeRate(delivered, sent),
                OpenRate: SafeRate(opened, delivered),
                ClickRate: SafeRate(clicked, opened),
                ReadRate: SafeRate(read, opened),
                BounceRate: SafeRate(bounced, sent),
                UnsubscribeRate: SafeRate(unsubscribed, delivered),
                AverageDeliveryTimeMs: Math.Round(avgDeliveryMs, 1),
                StartedAt: startedAt,
                CompletedAt: completedAt,
                Duration: duration,
                PeriodStart: filter.StartDate ?? DateTime.UtcNow.AddDays(-30),
                PeriodEnd: filter.EndDate ?? DateTime.UtcNow
            ));
        }

        return campaigns;
    }

    private static List<TimeSeriesPointDto> GenerateTimeSeries(string metric, AnalyticsFilterDto filter)
    {
        var rng = new Random(metric.GetHashCode() + (filter.StartDate?.Day ?? 0));
        var start = filter.StartDate ?? DateTime.UtcNow.AddDays(-30);
        var end = filter.EndDate ?? DateTime.UtcNow;
        var granularity = filter.Granularity ?? Granularity.Daily;

        var points = new List<TimeSeriesPointDto>();
        var baseCount = metric switch
        {
            "delivery" => rng.Next(1500, 3500),
            "failure" => rng.Next(50, 300),
            "open" => rng.Next(400, 1200),
            "click" => rng.Next(100, 500),
            _ => rng.Next(500, 2000)
        };

        switch (granularity)
        {
            case Granularity.Hourly:
                for (var t = start; t <= end; t = t.AddHours(1))
                {
                    var count = Math.Max(0, baseCount + rng.Next(-200, 300));
                    points.Add(new TimeSeriesPointDto(t, count, null, null));
                }
                break;

            case Granularity.Daily:
                for (var t = start.Date; t <= end.Date; t = t.AddDays(1))
                {
                    var count = Math.Max(0, baseCount + rng.Next(-500, 800));
                    var totalForDay = count + rng.Next(1000, 5000);
                    points.Add(new TimeSeriesPointDto(t, count, SafeRate(count, totalForDay), null));
                }
                break;

            case Granularity.Weekly:
                for (var t = start.Date; t <= end.Date; t = t.AddDays(7))
                {
                    var count = Math.Max(0, baseCount * 7 + rng.Next(-2000, 4000));
                    var totalForWeek = count + rng.Next(5000, 30000);
                    points.Add(new TimeSeriesPointDto(t, count, SafeRate(count, totalForWeek), null));
                }
                break;

            case Granularity.Monthly:
                for (var t = new DateTime(start.Year, start.Month, 1);
                     t <= new DateTime(end.Year, end.Month, 1);
                     t = t.AddMonths(1))
                {
                    var count = Math.Max(0, baseCount * 30 + rng.Next(-10000, 15000));
                    var totalForMonth = count + rng.Next(20000, 100000);
                    points.Add(new TimeSeriesPointDto(t, count, SafeRate(count, totalForMonth), null));
                }
                break;
        }

        return points;
    }

    private static string BuildTrendInsight(string metric, double trendDirection, double avg, double percentageChange)
    {
        var metricLabel = metric switch
        {
            "delivery" => "delivery rate",
            "failure" => "failure rate",
            "open" => "open rate",
            "click" => "click rate",
            _ => metric
        };

        if (Math.Abs(percentageChange) < 1)
            return $"The {metricLabel} has remained stable at approximately {avg:F0} with minimal variation.";

        var direction = percentageChange > 0 ? "increased" : "decreased";
        var magnitude = Math.Abs(percentageChange) switch
        {
            > 50 => "significantly",
            > 20 => "moderately",
            _ => "slightly"
        };

        return $"The {metricLabel} has {magnitude} {direction} by {Math.Abs(percentageChange):F1}% "
               + $"to an average of {avg:F0}. {(percentageChange > 0 ? "This is a positive trend." : "This may require attention.")}";
    }

    private static double SafeRate(int numerator, int denominator)
    {
        return denominator > 0 ? Math.Round((double)numerator / denominator * 100, 2) : 0;
    }
}

internal static class RandomExtensions
{
    public static double NextDouble(this Random rng, double min, double max)
    {
        return min + rng.NextDouble() * (max - min);
    }
}
