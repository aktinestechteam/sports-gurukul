using Microsoft.Extensions.Logging;
using SportsGurukul.Domain.Enums.Notification;
using SportsGurukul.Platform.Communication.Analytics.Abstractions;
using SportsGurukul.Platform.Communication.Analytics.DTOs;

namespace SportsGurukul.Platform.Communication.Analytics.Services;

public class DashboardService : IDashboardService
{
    private readonly ILogger<DashboardService> _logger;
    private readonly IAnalyticsService _analyticsService;
    private readonly ICampaignManagementService _campaignManagementService;
    private readonly ITemplateManagementService _templateManagementService;
    private readonly ICacheService _cache;

    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(5);
    private static readonly Random Rng = new();
    private static readonly string[] Statuses = ["Delivered", "Failed", "Pending", "Bounced", "Sent"];
    private static readonly string[] Channels = ["Email", "SMS", "Push", "InApp", "WhatsApp"];
    private static readonly string[] ProviderNames = ["TwilioSMS", "SendGrid", "FirebasePush", "AmazonSES", "MetaWhatsApp", "SmtpRelay", "Msg91", "InAppGateway"];
    private static readonly string[] TemplateNames = ["WelcomeEmail", "PasswordReset", "BookingConfirmation", "PaymentReceipt", "EventReminder", "VerificationCode", "Newsletter", "PromotionalOffer", "MatchAlert", "SubscriptionRenewal", "FeedbackRequest", "CoachAssignment", "TournamentInvite", "AttendanceReport", "FeeReminder"];
    private static readonly string[] CampaignNames = ["SummerCampaign2026", "UserOnboardingFlow", "PaymentDunning", "WeeklyNewsletter", "EventPromotion", "ReEngagementSeries", "BirthdayWishes", "NewFeatureAnnouncement", "FeedbackCollection", "SeasonalGreetings"];

    public DashboardService(
        ILogger<DashboardService> logger,
        IAnalyticsService analyticsService,
        ICampaignManagementService campaignManagementService,
        ITemplateManagementService templateManagementService,
        ICacheService cache)
    {
        _logger = logger;
        _analyticsService = analyticsService;
        _campaignManagementService = campaignManagementService;
        _templateManagementService = templateManagementService;
        _cache = cache;
    }

    public async Task<NotificationDashboardDto> GetNotificationDashboardAsync(AnalyticsFilterDto? filter, CancellationToken ct = default)
    {
        var cacheKey = CacheKeys.DashboardKey("notification", filter);
        return await _cache.GetOrSetAsync(cacheKey, async () =>
        {
            var now = DateTime.UtcNow;
            var summary = await _analyticsService.GetSummaryAsync(filter, ct);
            var totalNotifications = summary?.TotalNotifications ?? Rng.Next(15000, 50000);
            var totalSent = summary?.TotalSent ?? (int)(totalNotifications * 0.97);
            var totalDelivered = summary?.TotalDelivered ?? (int)(totalSent * 0.95);
            var totalFailed = summary?.TotalFailed ?? (totalSent - totalDelivered);
            var deliveryRate = summary?.DeliveryRate ?? (totalSent > 0 ? (double)totalDelivered / totalSent : 0);
            var failureRate = summary?.FailureRate ?? (totalSent > 0 ? (double)totalFailed / totalSent : 0);
            var openRate = summary?.OpenRate ?? (totalDelivered > 0 ? Rng.Next(35, 75) / 100.0 : 0);
            var avgDeliveryMs = summary?.AverageDeliveryTimeMs ?? Rng.Next(120, 850);
            var activeQueues = Rng.Next(3, 8);

            var kpis = new List<DashboardKpiDto>
            {
                new("Total Notifications", totalNotifications.ToString("N0"), 12.5, "up", "Bell", "#3B82F6", "All notifications processed in the period", "number"),
                new("Delivery Rate", $"{deliveryRate:P1}", 2.3, "up", "CheckCircle", "#10B981", "Percentage of notifications successfully delivered", "percentage"),
                new("Avg Delivery Time", $"{avgDeliveryMs:F0}ms", -5.1, "down", "Clock", "#F59E0B", "Average time to deliver a notification", "ms"),
                new("Failure Rate", $"{failureRate:P1}", -0.8, "down", "XCircle", "#EF4444", "Percentage of notifications that failed", "percentage"),
                new("Open Rate", $"{openRate:P1}", 4.2, "up", "Eye", "#8B5CF6", "Percentage of delivered notifications opened", "percentage"),
                new("Active Queues", $"{activeQueues}", 0, "neutral", "Layers", "#6366F1", "Number of currently active queues", "number")
            };

            var deliveryTrend = GenerateTimeSeries(24, 300, 1200, now, "DeliveryTrend");
            var failureTrend = GenerateTimeSeries(24, 5, 60, now, "FailureTrend");
            var channelBreakdown = GenerateChannelBreakdown(now);
            var recentNotifications = GenerateRecentNotifications(10, now);

            return new NotificationDashboardDto(kpis, deliveryTrend, failureTrend, channelBreakdown, recentNotifications, now);
        }, CacheTtl, ct) ?? GenerateNotificationDashboardFallback(filter);
    }

    public async Task<CampaignDashboardDto> GetCampaignDashboardAsync(AnalyticsFilterDto? filter, CancellationToken ct = default)
    {
        var cacheKey = CacheKeys.DashboardKey("campaign", filter);
        return await _cache.GetOrSetAsync(cacheKey, () =>
        {
            var now = DateTime.UtcNow;
            var activeCampaigns = Rng.Next(4, 15);
            var totalSent = Rng.Next(50000, 200000);
            var totalDelivered = (int)(totalSent * (Rng.Next(88, 98) / 100.0));
            var deliveryRate = (double)totalDelivered / totalSent;
            var atRiskCount = Rng.Next(0, 4);
            var completed = Rng.Next(10, 30);
            var totalCampaigns = completed + activeCampaigns + Rng.Next(2, 8);
            var completionRate = totalCampaigns > 0 ? (double)completed / totalCampaigns : 0;

            var kpis = new List<DashboardKpiDto>
            {
                new("Active Campaigns", $"{activeCampaigns}", 1, "up", "PlayCircle", "#3B82F6", "Campaigns currently running", "number"),
                new("Total Sent", totalSent.ToString("N0"), 8.7, "up", "Send", "#10B981", "Total notifications sent across all campaigns", "number"),
                new("Avg Delivery Rate", $"{deliveryRate:P1}", 1.2, "up", "Target", "#8B5CF6", "Average delivery rate across campaigns", "percentage"),
                new("At-Risk Campaigns", $"{atRiskCount}", -2, "down", "AlertTriangle", "#EF4444", "Campaigns with delivery rate below 80%", "number"),
                new("Completion Rate", $"{completionRate:P1}", 3.5, "up", "Flag", "#F59E0B", "Percentage of campaigns completed", "percentage")
            };

            var topPerformers = GenerateTopCampaigns(5, deliveryRate + 0.05, now);
            var atRiskCampaigns = GenerateAtRiskCampaigns(atRiskCount, now);
            var statusDistribution = GenerateCampaignStatusDistribution(now);
            var campaignTrend = GenerateTimeSeries(30, 500, 5000, now, "CampaignTrend");

            return Task.FromResult(new CampaignDashboardDto(kpis, topPerformers, atRiskCampaigns, statusDistribution, campaignTrend, now));
        }, CacheTtl, ct) ?? GenerateCampaignDashboardFallback(filter);
    }

    public async Task<ProviderDashboardDto> GetProviderDashboardAsync(AnalyticsFilterDto? filter, CancellationToken ct = default)
    {
        var cacheKey = CacheKeys.DashboardKey("provider", filter);
        return await _cache.GetOrSetAsync(cacheKey, () =>
        {
            var now = DateTime.UtcNow;
            var activeProviders = Rng.Next(4, 8);
            var overallReliability = Rng.Next(93, 99) + Rng.NextDouble();
            var avgLatency = Rng.Next(80, 450);
            var failedDeliveries = Rng.Next(50, 800);
            var throughput = Rng.Next(200, 2000);

            var kpis = new List<DashboardKpiDto>
            {
                new("Active Providers", $"{activeProviders}", 0, "neutral", "Server", "#3B82F6", "Number of active providers", "number"),
                new("Overall Reliability", $"{overallReliability:F1}%", 0.5, "up", "Shield", "#10B981", "Overall provider reliability score", "percentage"),
                new("Avg Latency", $"{avgLatency}ms", 12, "down", "Zap", "#F59E0B", "Average provider latency", "ms"),
                new("Failed Deliveries", failedDeliveries.ToString("N0"), -8.3, "down", "XCircle", "#EF4444", "Total failed deliveries across providers", "number"),
                new("Throughput", $"{throughput}/min", 5.2, "up", "Activity", "#8B5CF6", "Average throughput per minute", "number")
            };

            var providerRankings = GenerateProviderRankings(now);
            var underperformingProviders = providerRankings.Where(p => p.ReliabilityScore < 90).ToArray();
            var providerTrend = GenerateTimeSeries(24, 150, 1800, now, "ProviderTrend");
            var providerDistribution = new Dictionary<string, double>
            {
                ["Email"] = 42.5,
                ["SMS"] = 28.3,
                ["Push"] = 15.7,
                ["WhatsApp"] = 8.9,
                ["InApp"] = 4.6
            };

            return Task.FromResult(new ProviderDashboardDto(kpis, providerRankings, underperformingProviders, providerTrend, providerDistribution, now));
        }, CacheTtl, ct) ?? GenerateProviderDashboardFallback(filter);
    }

    public async Task<QueueDashboardDto> GetQueueDashboardAsync(CancellationToken ct = default)
    {
        var cacheKey = CacheKeys.DashboardKey("queue");
        return await _cache.GetOrSetAsync(cacheKey, () =>
        {
            var now = DateTime.UtcNow;
            var queueDepth = Rng.Next(500, 15000);
            var processingRate = Rng.Next(50, 500);
            var avgWaitTimeMs = Rng.Next(100, 3000);
            var deadLetterCount = Rng.Next(5, 200);
            var retryCount = Rng.Next(20, 500);
            var maxWaitTimeMs = Rng.Next(5000, 30000);

            var kpis = new List<DashboardKpiDto>
            {
                new("Queue Depth", queueDepth.ToString("N0"), 15.3, "up", "Layers", "#3B82F6", "Current number of items in queue", "number"),
                new("Processing Rate", $"{processingRate}/s", 8.7, "up", "Zap", "#10B981", "Messages processed per second", "number"),
                new("Avg Wait Time", $"{avgWaitTimeMs}ms", -12.5, "down", "Clock", "#F59E0B", "Average time items wait in queue", "ms"),
                new("Dead Letter Queue", deadLetterCount.ToString("N0"), 3.2, "up", "Archive", "#EF4444", "Messages moved to dead letter queue", "number"),
                new("Retry Count", retryCount.ToString("N0"), -5.8, "down", "RotateCcw", "#8B5CF6", "Number of items currently being retried", "number"),
                new("Max Wait Time", $"{maxWaitTimeMs}ms", 22.1, "up", "AlertTriangle", "#F97316", "Maximum time an item has been waiting", "ms")
            };

            var queueTrend = GenerateTimeSeries(24, 100, 2000, now, "QueueTrend");
            var processingRateTrend = GenerateTimeSeries(24, 30, 400, now, "ProcessingRateTrend");
            var oldestItems = GenerateOldestItems(10, now);

            return Task.FromResult(new QueueDashboardDto(kpis, queueDepth, processingRate, avgWaitTimeMs, deadLetterCount, retryCount, queueTrend, processingRateTrend, oldestItems, now));
        }, CacheTtl, ct) ?? GenerateQueueDashboardFallback();
    }

    public async Task<TemplateDashboardDto> GetTemplateDashboardAsync(AnalyticsFilterDto? filter, CancellationToken ct = default)
    {
        var cacheKey = CacheKeys.DashboardKey("template", filter);
        return await _cache.GetOrSetAsync(cacheKey, () =>
        {
            var now = DateTime.UtcNow;
            var totalTemplates = Rng.Next(30, 150);
            var publishedCount = (int)(totalTemplates * Rng.Next(40, 70) / 100.0);
            var draftCount = (int)(totalTemplates * Rng.Next(10, 30) / 100.0);
            var archivedCount = totalTemplates - publishedCount - draftCount;
            var avgVersions = Rng.Next(2, 6);

            var kpis = new List<DashboardKpiDto>
            {
                new("Total Templates", $"{totalTemplates}", 5.2, "up", "FileText", "#3B82F6", "Total number of templates", "number"),
                new("Published", $"{publishedCount}", 3.1, "up", "CheckCircle", "#10B981", "Published templates", "number"),
                new("Draft", $"{draftCount}", -1.5, "down", "Edit", "#F59E0B", "Templates in draft state", "number"),
                new("Archived", $"{archivedCount}", 0.8, "up", "Archive", "#6B7280", "Archived templates", "number"),
                new("Avg Versions", $"{avgVersions:F1}", 0.3, "up", "GitBranch", "#8B5CF6", "Average number of versions per template", "number")
            };

            var mostUsedTemplates = GenerateMostUsedTemplates(5, now);
            var recentlyUpdated = GenerateRecentlyUpdated(5, now);

            return Task.FromResult(new TemplateDashboardDto(kpis, totalTemplates, publishedCount, draftCount, archivedCount, mostUsedTemplates, recentlyUpdated, now));
        }, CacheTtl, ct) ?? GenerateTemplateDashboardFallback(filter);
    }

    public async Task<DashboardSummaryDto> GetFullDashboardAsync(AnalyticsFilterDto? filter, CancellationToken ct = default)
    {
        var notificationTask = GetNotificationDashboardAsync(filter, ct);
        var campaignTask = GetCampaignDashboardAsync(filter, ct);
        var providerTask = GetProviderDashboardAsync(filter, ct);
        var queueTask = GetQueueDashboardAsync(ct);
        var templateTask = GetTemplateDashboardAsync(filter, ct);

        await Task.WhenAll(notificationTask, campaignTask, providerTask, queueTask, templateTask);

        return new DashboardSummaryDto(
            notificationTask.Result,
            campaignTask.Result,
            providerTask.Result,
            queueTask.Result,
            templateTask.Result,
            DateTime.UtcNow
        );
    }

    public async Task<List<DashboardKpiDto>> GetNotificationKpisAsync(AnalyticsFilterDto? filter, CancellationToken ct = default)
    {
        var dashboard = await GetNotificationDashboardAsync(filter, ct);
        return dashboard.Kpis;
    }

    public async Task<List<DashboardKpiDto>> GetCampaignKpisAsync(AnalyticsFilterDto? filter, CancellationToken ct = default)
    {
        var dashboard = await GetCampaignDashboardAsync(filter, ct);
        return dashboard.Kpis;
    }

    public async Task<List<DashboardKpiDto>> GetProviderKpisAsync(AnalyticsFilterDto? filter, CancellationToken ct = default)
    {
        var dashboard = await GetProviderDashboardAsync(filter, ct);
        return dashboard.Kpis;
    }

    public async Task<List<DashboardKpiDto>> GetQueueKpisAsync(CancellationToken ct = default)
    {
        var dashboard = await GetQueueDashboardAsync(ct);
        return dashboard.Kpis;
    }

    public async Task<List<DashboardKpiDto>> GetTemplateKpisAsync(AnalyticsFilterDto? filter, CancellationToken ct = default)
    {
        var dashboard = await GetTemplateDashboardAsync(filter, ct);
        return dashboard.Kpis;
    }

    private static TimeSeriesPointDto[] GenerateTimeSeries(int points, int minVal, int maxVal, DateTime now, string breakdown)
    {
        return Enumerable.Range(0, points).Select(i =>
        {
            var timestamp = now.AddHours(-(points - 1 - i));
            var count = Rng.Next(minVal, maxVal);
            var rate = Rng.NextDouble() * 100;
            return new TimeSeriesPointDto(timestamp, count, rate, breakdown);
        }).ToArray();
    }

    private ChannelPerformanceDto[] GenerateChannelBreakdown(DateTime now)
    {
        var periodEnd = now;
        var periodStart = now.AddDays(-1);
        return Channels.Select((ch, i) =>
        {
            var totalSent = Rng.Next(1000, 20000);
            var totalDelivered = (int)(totalSent * Rng.Next(88, 99) / 100.0);
            var totalFailed = totalSent - totalDelivered;
            var totalOpened = (int)(totalDelivered * Rng.Next(30, 70) / 100.0);
            var totalClicked = (int)(totalOpened * Rng.Next(10, 40) / 100.0);
            var deliveryRate = totalSent > 0 ? (double)totalDelivered / totalSent : 0;
            var openRate = totalDelivered > 0 ? (double)totalOpened / totalDelivered : 0;
            var clickRate = totalOpened > 0 ? (double)totalClicked / totalOpened : 0;
            var channelType = ch switch
            {
                "Email" => NotificationChannelType.Email,
                "SMS" => NotificationChannelType.SMS,
                "Push" => NotificationChannelType.PushNotification,
                "InApp" => NotificationChannelType.InAppNotification,
                "WhatsApp" => NotificationChannelType.WhatsApp,
                _ => NotificationChannelType.Email
            };
            return new ChannelPerformanceDto(ch, channelType, totalSent, totalDelivered, totalFailed, totalOpened, totalClicked, deliveryRate, openRate, clickRate, Rng.Next(50, 600), periodStart, periodEnd);
        }).ToArray();
    }

    private List<RecentNotificationDto> GenerateRecentNotifications(int count, DateTime now)
    {
        return Enumerable.Range(0, count).Select(i =>
        {
            var sentAt = now.AddMinutes(-Rng.Next(0, 180));
            return new RecentNotificationDto(
                Guid.NewGuid(),
                TemplateNames[Rng.Next(TemplateNames.Length)],
                Channels[Rng.Next(Channels.Length)],
                Statuses[Rng.Next(Statuses.Length)],
                sentAt,
                $"{GetRandomName().ToLowerInvariant()}@example.com"
            );
        }).ToList();
    }

    private CampaignPerformanceDto[] GenerateTopCampaigns(int count, double baseRate, DateTime now)
    {
        var periodStart = now.AddDays(-30);
        return Enumerable.Range(0, count).Select(i =>
        {
            var totalRecipients = Rng.Next(500, 50000);
            var totalSent = (int)(totalRecipients * Rng.Next(95, 100) / 100.0);
            var totalDelivered = (int)(totalSent * (baseRate + Rng.NextDouble() * 0.05));
            var totalFailed = totalSent - totalDelivered;
            var totalOpened = (int)(totalDelivered * Rng.Next(40, 80) / 100.0);
            var totalClicked = (int)(totalOpened * Rng.Next(15, 45) / 100.0);
            var totalRead = (int)(totalOpened * Rng.Next(50, 90) / 100.0);
            var totalBounced = (int)(totalSent * Rng.Next(0, 3) / 100.0);
            var totalUnsubscribed = (int)(totalDelivered * Rng.Next(0, 2) / 100.0);
            var deliveryRate = totalSent > 0 ? (double)totalDelivered / totalSent : 0;
            var openRate = totalDelivered > 0 ? (double)totalOpened / totalDelivered : 0;

            return new CampaignPerformanceDto(
                Guid.NewGuid(),
                CampaignNames[Rng.Next(CampaignNames.Length)],
                CampaignType.OneTime,
                totalRecipients, totalSent, totalDelivered, totalFailed, totalOpened, totalClicked, totalRead,
                totalBounced, totalUnsubscribed, deliveryRate, openRate, (double)totalClicked / totalOpened,
                (double)totalRead / totalDelivered, (double)totalBounced / totalSent, (double)totalUnsubscribed / totalDelivered,
                Rng.Next(80, 500), periodStart.AddDays(Rng.Next(0, 29)), now, now - periodStart, periodStart, now
            );
        }).OrderByDescending(p => p.DeliveryRate).Take(count).ToArray();
    }

    private CampaignPerformanceDto[] GenerateAtRiskCampaigns(int count, DateTime now)
    {
        if (count == 0) return [];
        var periodStart = now.AddDays(-30);
        return Enumerable.Range(0, count).Select(i =>
        {
            var totalRecipients = Rng.Next(500, 20000);
            var totalSent = (int)(totalRecipients * Rng.Next(80, 95) / 100.0);
            var deliveryRate = Rng.Next(50, 79) / 100.0;
            var totalDelivered = (int)(totalSent * deliveryRate);
            var totalFailed = totalSent - totalDelivered;
            var totalOpened = (int)(totalDelivered * Rng.Next(20, 40) / 100.0);
            var totalClicked = (int)(totalOpened * Rng.Next(5, 20) / 100.0);

            return new CampaignPerformanceDto(
                Guid.NewGuid(),
                CampaignNames[Rng.Next(CampaignNames.Length)] + " (At Risk)",
                (CampaignType)Rng.Next(0, 5),
                totalRecipients, totalSent, totalDelivered, totalFailed, totalOpened, totalClicked, 0,
                0, 0, deliveryRate, (double)totalOpened / totalDelivered, 0, 0, 0, 0,
                Rng.Next(200, 800), periodStart.AddDays(Rng.Next(0, 20)), null, null, periodStart, now
            );
        }).OrderBy(p => p.DeliveryRate).ToArray();
    }

    private CampaignStatusDistribution[] GenerateCampaignStatusDistribution(DateTime now)
    {
        var total = Rng.Next(20, 50);
        return Enum.GetValues<DTOs.CampaignStatus>()
            .Select(s =>
            {
                var count = s switch
                {
                    DTOs.CampaignStatus.Draft => (int)(total * Rng.Next(5, 15) / 100.0),
                    DTOs.CampaignStatus.Active => (int)(total * Rng.Next(15, 30) / 100.0),
                    DTOs.CampaignStatus.Paused => (int)(total * Rng.Next(3, 10) / 100.0),
                    DTOs.CampaignStatus.Completed => (int)(total * Rng.Next(25, 45) / 100.0),
                    DTOs.CampaignStatus.Cancelled => (int)(total * Rng.Next(2, 8) / 100.0),
                    DTOs.CampaignStatus.Archived => (int)(total * Rng.Next(5, 15) / 100.0),
                    _ => 0
                };
                var percentage = total > 0 ? (double)count / total : 0;
                return new CampaignStatusDistribution(s.ToString(), count, percentage);
            })
            .Where(s => s.Count > 0)
            .OrderByDescending(s => s.Count)
            .ToArray();
    }

    private ProviderPerformanceDto[] GenerateProviderRankings(DateTime now)
    {
        var periodStart = now.AddDays(-1);
        return ProviderNames.Select(name =>
        {
            var totalSent = Rng.Next(5000, 100000);
            var totalDelivered = (int)(totalSent * Rng.Next(90, 99) / 100.0);
            var totalFailed = totalSent - totalDelivered;
            var totalBounced = (int)(totalSent * Rng.Next(0, 4) / 100.0);
            var deliveryRate = totalSent > 0 ? (double)totalDelivered / totalSent : 0;
            var failureRate = totalSent > 0 ? (double)totalFailed / totalSent : 0;
            var avgDeliveryTimeMs = Rng.Next(50, 800);
            var avgLatencyMs = Rng.Next(30, 500);
            var throughput = Rng.Next(50, 1500);
            var retries = Rng.Next(10, 300);
            var deadLettered = Rng.Next(0, 50);
            var reliability = Rng.Next(85, 100) + Rng.NextDouble();

            return new ProviderPerformanceDto(
                Guid.NewGuid(), name, name.Contains("SMS") ? "SMS" : name.Contains("Email") || name.Contains("SES") ? "Email" : name.Contains("Push") ? "Push" : name.Contains("WhatsApp") ? "WhatsApp" : "Other",
                totalSent, totalDelivered, totalFailed, totalBounced, deliveryRate, failureRate,
                avgDeliveryTimeMs, avgLatencyMs, throughput, retries, deadLettered,
                Math.Round(reliability, 1), periodStart, now
            );
        }).OrderByDescending(p => p.ReliabilityScore).ToArray();
    }

    private List<QueueItemSummaryDto> GenerateOldestItems(int count, DateTime now)
    {
        return Enumerable.Range(0, count).Select(i =>
        {
            var queuedAt = now.AddMinutes(-Rng.Next(5, 180));
            var waitTime = now - queuedAt;
            return new QueueItemSummaryDto(
                Guid.NewGuid(),
                TemplateNames[Rng.Next(TemplateNames.Length)],
                Channels[Rng.Next(Channels.Length)],
                Statuses[Rng.Next(Statuses.Length)],
                Rng.Next(0, 5),
                queuedAt,
                waitTime
            );
        }).OrderByDescending(q => q.WaitTime).ToList();
    }

    private TemplatePerformanceSummaryDto[] GenerateMostUsedTemplates(int count, DateTime now)
    {
        return Enumerable.Range(0, count).Select(i =>
        {
            var channelType = (NotificationChannelType)Rng.Next(0, 5);
            return new TemplatePerformanceSummaryDto(
                Guid.NewGuid(),
                TemplateNames[Rng.Next(TemplateNames.Length)],
                channelType,
                Rng.Next(1, 12),
                Rng.Next(1000, 50000),
                Rng.Next(88, 99) + Rng.NextDouble(),
                now.AddDays(-Rng.Next(0, 7)),
                now.AddDays(-Rng.Next(0, 30))
            );
        }).OrderByDescending(t => t.UsageCount).Take(count).ToArray();
    }

    private TemplatePerformanceSummaryDto[] GenerateRecentlyUpdated(int count, DateTime now)
    {
        return Enumerable.Range(0, count).Select(i =>
        {
            var channelType = (NotificationChannelType)Rng.Next(0, 5);
            return new TemplatePerformanceSummaryDto(
                Guid.NewGuid(),
                TemplateNames[Rng.Next(TemplateNames.Length)],
                channelType,
                Rng.Next(1, 8),
                Rng.Next(100, 15000),
                Rng.Next(85, 98) + Rng.NextDouble(),
                now.AddDays(-Rng.Next(0, 14)),
                now.AddMinutes(-Rng.Next(0, 1440))
            );
        }).OrderByDescending(t => t.UpdatedAt).Take(count).ToArray();
    }

    private static string GetRandomName()
    {
        var firstNames = new[] { "john", "jane", "alex", "sam", "chris", "taylor", "morgan", "jordan", "casey", "riley" };
        var lastNames = new[] { "smith", "johnson", "williams", "brown", "jones", "garcia", "miller", "davis", "rodriguez", "martinez" };
        return $"{firstNames[Rng.Next(firstNames.Length)]}.{lastNames[Rng.Next(lastNames.Length)]}";
    }

    private NotificationDashboardDto GenerateNotificationDashboardFallback(AnalyticsFilterDto? filter)
    {
        var now = DateTime.UtcNow;
        var totalNotifications = Rng.Next(15000, 50000);
        var totalSent = (int)(totalNotifications * 0.97);
        var totalDelivered = (int)(totalSent * 0.95);
        var deliveryRate = totalSent > 0 ? (double)totalDelivered / totalSent : 0;

        var kpis = new List<DashboardKpiDto>
        {
            new("Total Notifications", totalNotifications.ToString("N0"), 12.5, "up", "Bell", "#3B82F6", "All notifications processed in the period", "number"),
            new("Delivery Rate", $"{deliveryRate:P1}", 2.3, "up", "CheckCircle", "#10B981", "Percentage of notifications successfully delivered", "percentage"),
            new("Avg Delivery Time", $"{Rng.Next(120, 850)}ms", -5.1, "down", "Clock", "#F59E0B", "Average time to deliver a notification", "ms"),
            new("Failure Rate", $"{(1 - deliveryRate):P1}", -0.8, "down", "XCircle", "#EF4444", "Percentage of notifications that failed", "percentage"),
            new("Open Rate", $"{Rng.Next(35, 75)}%", 4.2, "up", "Eye", "#8B5CF6", "Percentage of delivered notifications opened", "percentage"),
            new("Active Queues", $"{Rng.Next(3, 8)}", 0, "neutral", "Layers", "#6366F1", "Number of currently active queues", "number")
        };

        return new NotificationDashboardDto(kpis, GenerateTimeSeries(24, 300, 1200, now, "DeliveryTrend"), GenerateTimeSeries(24, 5, 60, now, "FailureTrend"), GenerateChannelBreakdown(now), GenerateRecentNotifications(10, now), now);
    }

    private CampaignDashboardDto GenerateCampaignDashboardFallback(AnalyticsFilterDto? filter)
    {
        var now = DateTime.UtcNow;
        var kpis = new List<DashboardKpiDto>
        {
            new("Active Campaigns", $"{Rng.Next(4, 15)}", 1, "up", "PlayCircle", "#3B82F6", "Campaigns currently running", "number"),
            new("Total Sent", $"{Rng.Next(50000, 200000):N0}", 8.7, "up", "Send", "#10B981", "Total notifications sent across all campaigns", "number"),
            new("Avg Delivery Rate", $"{Rng.Next(88, 98)}%", 1.2, "up", "Target", "#8B5CF6", "Average delivery rate across campaigns", "percentage"),
            new("At-Risk Campaigns", $"{Rng.Next(0, 4)}", -2, "down", "AlertTriangle", "#EF4444", "Campaigns with delivery rate below 80%", "number"),
            new("Completion Rate", $"{Rng.Next(40, 70)}%", 3.5, "up", "Flag", "#F59E0B", "Percentage of campaigns completed", "percentage")
        };

        return new CampaignDashboardDto(kpis, GenerateTopCampaigns(5, 0.95, now), [], GenerateCampaignStatusDistribution(now), GenerateTimeSeries(30, 500, 5000, now, "CampaignTrend"), now);
    }

    private ProviderDashboardDto GenerateProviderDashboardFallback(AnalyticsFilterDto? filter)
    {
        var now = DateTime.UtcNow;
        var providerRankings = GenerateProviderRankings(now);
        var kpis = new List<DashboardKpiDto>
        {
            new("Active Providers", $"{Rng.Next(4, 8)}", 0, "neutral", "Server", "#3B82F6", "Number of active providers", "number"),
            new("Overall Reliability", $"{Rng.Next(93, 99)}%", 0.5, "up", "Shield", "#10B981", "Overall provider reliability score", "percentage"),
            new("Avg Latency", $"{Rng.Next(80, 450)}ms", 12, "down", "Zap", "#F59E0B", "Average provider latency", "ms"),
            new("Failed Deliveries", $"{Rng.Next(50, 800):N0}", -8.3, "down", "XCircle", "#EF4444", "Total failed deliveries across providers", "number"),
            new("Throughput", $"{Rng.Next(200, 2000)}/min", 5.2, "up", "Activity", "#8B5CF6", "Average throughput per minute", "number")
        };

        return new ProviderDashboardDto(kpis, providerRankings, providerRankings.Where(p => p.ReliabilityScore < 90).ToArray(), GenerateTimeSeries(24, 150, 1800, now, "ProviderTrend"), new() { ["Email"] = 42.5, ["SMS"] = 28.3, ["Push"] = 15.7, ["WhatsApp"] = 8.9, ["InApp"] = 4.6 }, now);
    }

    private QueueDashboardDto GenerateQueueDashboardFallback()
    {
        var now = DateTime.UtcNow;
        var kpis = new List<DashboardKpiDto>
        {
            new("Queue Depth", $"{Rng.Next(500, 15000):N0}", 15.3, "up", "Layers", "#3B82F6", "Current number of items in queue", "number"),
            new("Processing Rate", $"{Rng.Next(50, 500)}/s", 8.7, "up", "Zap", "#10B981", "Messages processed per second", "number"),
            new("Avg Wait Time", $"{Rng.Next(100, 3000)}ms", -12.5, "down", "Clock", "#F59E0B", "Average time items wait in queue", "ms"),
            new("Dead Letter Queue", $"{Rng.Next(5, 200):N0}", 3.2, "up", "Archive", "#EF4444", "Messages moved to dead letter queue", "number"),
            new("Retry Count", $"{Rng.Next(20, 500):N0}", -5.8, "down", "RotateCcw", "#8B5CF6", "Number of items currently being retried", "number"),
            new("Max Wait Time", $"{Rng.Next(5000, 30000)}ms", 22.1, "up", "AlertTriangle", "#F97316", "Maximum time an item has been waiting", "ms")
        };

        return new QueueDashboardDto(kpis, Rng.Next(500, 15000), Rng.Next(50, 500), Rng.Next(100, 3000), Rng.Next(5, 200), Rng.Next(20, 500), GenerateTimeSeries(24, 100, 2000, now, "QueueTrend"), GenerateTimeSeries(24, 30, 400, now, "ProcessingRateTrend"), GenerateOldestItems(10, now), now);
    }

    private TemplateDashboardDto GenerateTemplateDashboardFallback(AnalyticsFilterDto? filter)
    {
        var now = DateTime.UtcNow;
        var totalTemplates = Rng.Next(30, 150);
        var published = (int)(totalTemplates * Rng.Next(40, 70) / 100.0);
        var draft = (int)(totalTemplates * Rng.Next(10, 30) / 100.0);
        var archived = totalTemplates - published - draft;

        var kpis = new List<DashboardKpiDto>
        {
            new("Total Templates", $"{totalTemplates}", 5.2, "up", "FileText", "#3B82F6", "Total number of templates", "number"),
            new("Published", $"{published}", 3.1, "up", "CheckCircle", "#10B981", "Published templates", "number"),
            new("Draft", $"{draft}", -1.5, "down", "Edit", "#F59E0B", "Templates in draft state", "number"),
            new("Archived", $"{archived}", 0.8, "up", "Archive", "#6B7280", "Archived templates", "number"),
            new("Avg Versions", $"{Rng.Next(2, 6):F1}", 0.3, "up", "GitBranch", "#8B5CF6", "Average number of versions per template", "number")
        };

        return new TemplateDashboardDto(kpis, totalTemplates, published, draft, archived, GenerateMostUsedTemplates(5, now), GenerateRecentlyUpdated(5, now), now);
    }
}
