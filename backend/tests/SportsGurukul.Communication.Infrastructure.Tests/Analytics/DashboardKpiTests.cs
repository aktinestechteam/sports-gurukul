using Microsoft.Extensions.Logging;
using SportsGurukul.Domain.Enums.Notification;
using SportsGurukul.Platform.Communication.Analytics.Abstractions;
using SportsGurukul.Platform.Communication.Analytics.DTOs;
using SportsGurukul.Platform.Communication.Analytics.Services;

namespace SportsGurukul.Communication.Infrastructure.Tests.Analytics;

public class DashboardKpiTests
{
    private readonly Mock<ILogger<DashboardService>> _loggerMock = new();
    private readonly Mock<IAnalyticsService> _analyticsServiceMock = new();
    private readonly Mock<ICampaignManagementService> _campaignMgmtMock = new();
    private readonly Mock<ITemplateManagementService> _templateMgmtMock = new();
    private readonly Mock<ICacheService> _cacheMock = new();

    private DashboardService CreateService() => new(
        _loggerMock.Object,
        _analyticsServiceMock.Object,
        _campaignMgmtMock.Object,
        _templateMgmtMock.Object,
        _cacheMock.Object);

    [Fact]
    public async Task GetNotificationDashboardAsync_ReturnsKpis()
    {
        _analyticsServiceMock
            .Setup(a => a.GetSummaryAsync(It.IsAny<AnalyticsFilterDto?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AnalyticsSummaryDto(50000, 48500, 46075, 2500, 18430, 5529, 9215, 750, 230,
                95.0, 40.0, 30.0, 50.0, 1.5, 5.0, 0.5, 250.0, DateTime.UtcNow));

        _cacheMock
            .Setup(c => c.GetOrSetAsync(It.IsAny<string>(), It.IsAny<Func<Task<NotificationDashboardDto>>>(),
                It.IsAny<TimeSpan?>(), It.IsAny<CancellationToken>()))
            .Returns((string _, Func<Task<NotificationDashboardDto>> factory, TimeSpan? _, CancellationToken _) => factory());

        var service = CreateService();

        var result = await service.GetNotificationDashboardAsync(null);

        result.Kpis.Should().NotBeEmpty();
        result.Kpis.Should().Contain(k => k.Label == "Total Notifications");
        result.Kpis.Should().Contain(k => k.Label == "Delivery Rate");
        result.Kpis.Should().Contain(k => k.Label == "Avg Delivery Time");
        result.Kpis.Should().Contain(k => k.Label == "Failure Rate");
        result.Kpis.Should().Contain(k => k.Label == "Open Rate");
        result.Kpis.Should().Contain(k => k.Label == "Active Queues");
    }

    [Fact]
    public async Task GetNotificationDashboardAsync_KpisHaveChangePercentage()
    {
        _analyticsServiceMock
            .Setup(a => a.GetSummaryAsync(It.IsAny<AnalyticsFilterDto?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AnalyticsSummaryDto(50000, 48500, 46075, 2500, 18430, 5529, 9215, 750, 230,
                95.0, 40.0, 30.0, 50.0, 1.5, 5.0, 0.5, 250.0, DateTime.UtcNow));

        _cacheMock
            .Setup(c => c.GetOrSetAsync(It.IsAny<string>(), It.IsAny<Func<Task<NotificationDashboardDto>>>(),
                It.IsAny<TimeSpan?>(), It.IsAny<CancellationToken>()))
            .Returns((string _, Func<Task<NotificationDashboardDto>> factory, TimeSpan? _, CancellationToken _) => factory());

        var service = CreateService();

        var result = await service.GetNotificationDashboardAsync(null);

        result.Kpis.Should().AllSatisfy(k => k.ChangePercentage.Should().NotBeNull());
    }

    [Fact]
    public async Task GetCampaignDashboardAsync_ReturnsCampaignMetrics()
    {
        _cacheMock
            .Setup(c => c.GetOrSetAsync(It.IsAny<string>(), It.IsAny<Func<Task<CampaignDashboardDto>>>(),
                It.IsAny<TimeSpan?>(), It.IsAny<CancellationToken>()))
            .Returns((string _, Func<Task<CampaignDashboardDto>> factory, TimeSpan? _, CancellationToken _) => factory());

        var service = CreateService();

        var result = await service.GetCampaignDashboardAsync(null);

        result.Kpis.Should().NotBeEmpty();
        result.Kpis.Should().Contain(k => k.Label == "Active Campaigns");
        result.Kpis.Should().Contain(k => k.Label == "Total Sent");
        result.Kpis.Should().Contain(k => k.Label == "Avg Delivery Rate");
        result.Kpis.Should().Contain(k => k.Label == "At-Risk Campaigns");
        result.Kpis.Should().Contain(k => k.Label == "Completion Rate");
    }

    [Fact]
    public async Task GetProviderDashboardAsync_ReturnsProviderStats()
    {
        _cacheMock
            .Setup(c => c.GetOrSetAsync(It.IsAny<string>(), It.IsAny<Func<Task<ProviderDashboardDto>>>(),
                It.IsAny<TimeSpan?>(), It.IsAny<CancellationToken>()))
            .Returns((string _, Func<Task<ProviderDashboardDto>> factory, TimeSpan? _, CancellationToken _) => factory());

        var service = CreateService();

        var result = await service.GetProviderDashboardAsync(null);

        result.Kpis.Should().NotBeEmpty();
        result.Kpis.Should().Contain(k => k.Label == "Active Providers");
        result.Kpis.Should().Contain(k => k.Label == "Overall Reliability");
    }

    [Fact]
    public async Task GetQueueDashboardAsync_ReturnsQueueDepth()
    {
        _cacheMock
            .Setup(c => c.GetOrSetAsync(It.IsAny<string>(), It.IsAny<Func<Task<QueueDashboardDto>>>(),
                It.IsAny<TimeSpan?>(), It.IsAny<CancellationToken>()))
            .Returns((string _, Func<Task<QueueDashboardDto>> factory, TimeSpan? _, CancellationToken _) => factory());

        var service = CreateService();

        var result = await service.GetQueueDashboardAsync();

        result.Kpis.Should().NotBeEmpty();
        result.Kpis.Should().Contain(k => k.Label == "Queue Depth");
        result.Kpis.Should().Contain(k => k.Label == "Processing Rate");
        result.QueueDepth.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetTemplateDashboardAsync_ReturnsTemplateStats()
    {
        _cacheMock
            .Setup(c => c.GetOrSetAsync(It.IsAny<string>(), It.IsAny<Func<Task<TemplateDashboardDto>>>(),
                It.IsAny<TimeSpan?>(), It.IsAny<CancellationToken>()))
            .Returns((string _, Func<Task<TemplateDashboardDto>> factory, TimeSpan? _, CancellationToken _) => factory());

        var service = CreateService();

        var result = await service.GetTemplateDashboardAsync(null);

        result.Kpis.Should().NotBeEmpty();
        result.Kpis.Should().Contain(k => k.Label == "Total Templates");
        result.Kpis.Should().Contain(k => k.Label == "Published");
        result.TotalTemplates.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetFullDashboardAsync_CombinesAllDashboards()
    {
        _analyticsServiceMock
            .Setup(a => a.GetSummaryAsync(It.IsAny<AnalyticsFilterDto?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AnalyticsSummaryDto(50000, 48500, 46075, 2500, 18430, 5529, 9215, 750, 230,
                95.0, 40.0, 30.0, 50.0, 1.5, 5.0, 0.5, 250.0, DateTime.UtcNow));

        _cacheMock
            .Setup(c => c.GetOrSetAsync(It.IsAny<string>(), It.IsAny<Func<Task<NotificationDashboardDto>>>(),
                It.IsAny<TimeSpan?>(), It.IsAny<CancellationToken>()))
            .Returns((string _, Func<Task<NotificationDashboardDto>> factory, TimeSpan? _, CancellationToken _) => factory());
        _cacheMock
            .Setup(c => c.GetOrSetAsync(It.IsAny<string>(), It.IsAny<Func<Task<CampaignDashboardDto>>>(),
                It.IsAny<TimeSpan?>(), It.IsAny<CancellationToken>()))
            .Returns((string _, Func<Task<CampaignDashboardDto>> factory, TimeSpan? _, CancellationToken _) => factory());
        _cacheMock
            .Setup(c => c.GetOrSetAsync(It.IsAny<string>(), It.IsAny<Func<Task<ProviderDashboardDto>>>(),
                It.IsAny<TimeSpan?>(), It.IsAny<CancellationToken>()))
            .Returns((string _, Func<Task<ProviderDashboardDto>> factory, TimeSpan? _, CancellationToken _) => factory());
        _cacheMock
            .Setup(c => c.GetOrSetAsync(It.IsAny<string>(), It.IsAny<Func<Task<QueueDashboardDto>>>(),
                It.IsAny<TimeSpan?>(), It.IsAny<CancellationToken>()))
            .Returns((string _, Func<Task<QueueDashboardDto>> factory, TimeSpan? _, CancellationToken _) => factory());
        _cacheMock
            .Setup(c => c.GetOrSetAsync(It.IsAny<string>(), It.IsAny<Func<Task<TemplateDashboardDto>>>(),
                It.IsAny<TimeSpan?>(), It.IsAny<CancellationToken>()))
            .Returns((string _, Func<Task<TemplateDashboardDto>> factory, TimeSpan? _, CancellationToken _) => factory());

        var service = CreateService();

        var result = await service.GetFullDashboardAsync(null);

        result.NotificationDashboard.Should().NotBeNull();
        result.CampaignDashboard.Should().NotBeNull();
        result.ProviderDashboard.Should().NotBeNull();
        result.QueueDashboard.Should().NotBeNull();
        result.TemplateDashboard.Should().NotBeNull();
    }

    [Fact]
    public async Task GetNotificationKpisAsync_ReturnsKpis()
    {
        _analyticsServiceMock
            .Setup(a => a.GetSummaryAsync(It.IsAny<AnalyticsFilterDto?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AnalyticsSummaryDto(50000, 48500, 46075, 2500, 18430, 5529, 9215, 750, 230,
                95.0, 40.0, 30.0, 50.0, 1.5, 5.0, 0.5, 250.0, DateTime.UtcNow));

        _cacheMock
            .Setup(c => c.GetOrSetAsync(It.IsAny<string>(), It.IsAny<Func<Task<NotificationDashboardDto>>>(),
                It.IsAny<TimeSpan?>(), It.IsAny<CancellationToken>()))
            .Returns((string _, Func<Task<NotificationDashboardDto>> factory, TimeSpan? _, CancellationToken _) => factory());

        var service = CreateService();

        var kpis = await service.GetNotificationKpisAsync(null);

        kpis.Should().HaveCount(6);
    }

    [Fact]
    public async Task Dashboard_RespectsCache()
    {
        _analyticsServiceMock
            .Setup(a => a.GetSummaryAsync(It.IsAny<AnalyticsFilterDto?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AnalyticsSummaryDto(50000, 48500, 46075, 2500, 18430, 5529, 9215, 750, 230,
                95.0, 40.0, 30.0, 50.0, 1.5, 5.0, 0.5, 250.0, DateTime.UtcNow));

        var factoryCallCount = 0;
        _cacheMock
            .Setup(c => c.GetOrSetAsync(It.IsAny<string>(), It.IsAny<Func<Task<NotificationDashboardDto>>>(),
                It.IsAny<TimeSpan?>(), It.IsAny<CancellationToken>()))
            .Returns((string _, Func<Task<NotificationDashboardDto>> factory, TimeSpan? _, CancellationToken _) =>
                {
                    factoryCallCount++;
                    return factory();
                });

        var service = CreateService();

        await service.GetNotificationDashboardAsync(null);
        await service.GetNotificationDashboardAsync(null);

        factoryCallCount.Should().Be(2);
    }
}
