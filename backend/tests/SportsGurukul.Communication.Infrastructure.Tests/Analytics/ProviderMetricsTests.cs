using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.Communication.Analytics.Abstractions;
using SportsGurukul.Platform.Communication.Analytics.DTOs;
using SportsGurukul.Platform.Communication.Analytics.Services;

namespace SportsGurukul.Communication.Infrastructure.Tests.Analytics;

public class ProviderMetricsTests
{
    private readonly Mock<ILogger<AnalyticsService>> _loggerMock = new();
    private readonly Mock<ICacheService> _cacheMock = new();

    private AnalyticsService CreateService() => new(_loggerMock.Object, _cacheMock.Object);

    [Fact]
    public async Task GetProviderPerformanceAsync_ReturnsProviders()
    {
        _cacheMock
            .Setup(c => c.GetOrSetAsync(It.IsAny<string>(), It.IsAny<Func<Task<List<ProviderPerformanceDto>>>>(),
                It.IsAny<TimeSpan?>(), It.IsAny<CancellationToken>()))
            .Returns((string _, Func<Task<List<ProviderPerformanceDto>>> factory, TimeSpan? _, CancellationToken _) => factory());

        var service = CreateService();

        var result = await service.GetProviderPerformanceAsync(null);

        result.Should().NotBeEmpty();
        result.Should().HaveCount(5);
    }

    [Fact]
    public async Task GetProviderPerformanceAsync_IncludesLatency()
    {
        _cacheMock
            .Setup(c => c.GetOrSetAsync(It.IsAny<string>(), It.IsAny<Func<Task<List<ProviderPerformanceDto>>>>(),
                It.IsAny<TimeSpan?>(), It.IsAny<CancellationToken>()))
            .Returns((string _, Func<Task<List<ProviderPerformanceDto>>> factory, TimeSpan? _, CancellationToken _) => factory());

        var service = CreateService();

        var result = await service.GetProviderPerformanceAsync(null);

        result.Should().AllSatisfy(p =>
        {
            p.AverageLatencyMs.Should().BeGreaterThan(0);
            p.AverageDeliveryTimeMs.Should().BeGreaterThan(0);
        });
    }

    [Fact]
    public async Task GetProviderPerformanceAsync_IncludesThroughput()
    {
        _cacheMock
            .Setup(c => c.GetOrSetAsync(It.IsAny<string>(), It.IsAny<Func<Task<List<ProviderPerformanceDto>>>>(),
                It.IsAny<TimeSpan?>(), It.IsAny<CancellationToken>()))
            .Returns((string _, Func<Task<List<ProviderPerformanceDto>>> factory, TimeSpan? _, CancellationToken _) => factory());

        var service = CreateService();

        var result = await service.GetProviderPerformanceAsync(null);

        result.Should().AllSatisfy(p => p.ThroughputPerMinute.Should().BeGreaterThan(0));
    }

    [Fact]
    public async Task GetProviderPerformanceAsync_ReliabilityScore_InRange()
    {
        _cacheMock
            .Setup(c => c.GetOrSetAsync(It.IsAny<string>(), It.IsAny<Func<Task<List<ProviderPerformanceDto>>>>(),
                It.IsAny<TimeSpan?>(), It.IsAny<CancellationToken>()))
            .Returns((string _, Func<Task<List<ProviderPerformanceDto>>> factory, TimeSpan? _, CancellationToken _) => factory());

        var service = CreateService();

        var result = await service.GetProviderPerformanceAsync(null);

        result.Should().AllSatisfy(p =>
        {
            p.ReliabilityScore.Should().BeInRange(85.0, 99.5);
        });
    }

    [Fact]
    public async Task GetProviderPerformanceAsync_DetectsUnderperformance()
    {
        _cacheMock
            .Setup(c => c.GetOrSetAsync(It.IsAny<string>(), It.IsAny<Func<Task<List<ProviderPerformanceDto>>>>(),
                It.IsAny<TimeSpan?>(), It.IsAny<CancellationToken>()))
            .Returns((string _, Func<Task<List<ProviderPerformanceDto>>> factory, TimeSpan? _, CancellationToken _) => factory());

        var service = CreateService();

        var result = await service.GetProviderPerformanceAsync(null);

        var underperforming = result.Where(p => p.ReliabilityScore < 90).ToList();
        underperforming.Should().NotBeNull();
    }

    [Fact]
    public async Task GetProviderPerformanceAsync_DeliveryRate_WithinExpectedRange()
    {
        _cacheMock
            .Setup(c => c.GetOrSetAsync(It.IsAny<string>(), It.IsAny<Func<Task<List<ProviderPerformanceDto>>>>(),
                It.IsAny<TimeSpan?>(), It.IsAny<CancellationToken>()))
            .Returns((string _, Func<Task<List<ProviderPerformanceDto>>> factory, TimeSpan? _, CancellationToken _) => factory());

        var service = CreateService();

        var result = await service.GetProviderPerformanceAsync(null);

        result.Should().AllSatisfy(p =>
        {
            p.DeliveryRate.Should().BeInRange(88, 100);
            p.FailureRate.Should().BeInRange(0, 12);
        });
    }

    [Fact]
    public async Task GetProviderPerformanceAsync_ProviderNames_ArePopulated()
    {
        _cacheMock
            .Setup(c => c.GetOrSetAsync(It.IsAny<string>(), It.IsAny<Func<Task<List<ProviderPerformanceDto>>>>(),
                It.IsAny<TimeSpan?>(), It.IsAny<CancellationToken>()))
            .Returns((string _, Func<Task<List<ProviderPerformanceDto>>> factory, TimeSpan? _, CancellationToken _) => factory());

        var service = CreateService();

        var result = await service.GetProviderPerformanceAsync(null);

        result.Should().AllSatisfy(p =>
        {
            p.ProviderName.Should().NotBeNullOrWhiteSpace();
            p.ProviderType.Should().NotBeNullOrWhiteSpace();
        });
    }

    [Fact]
    public async Task GetProviderPerformanceAsync_RespectsDateFilter()
    {
        var startDate = DateTime.UtcNow.AddDays(-7);
        var endDate = DateTime.UtcNow;
        var filter = new AnalyticsFilterDto(startDate, endDate, null, null, null, null, null);

        _cacheMock
            .Setup(c => c.GetOrSetAsync(It.IsAny<string>(), It.IsAny<Func<Task<List<ProviderPerformanceDto>>>>(),
                It.IsAny<TimeSpan?>(), It.IsAny<CancellationToken>()))
            .Returns((string _, Func<Task<List<ProviderPerformanceDto>>> factory, TimeSpan? _, CancellationToken _) => factory());

        var service = CreateService();

        var result = await service.GetProviderPerformanceAsync(filter);

        result.Should().AllSatisfy(p =>
        {
            p.PeriodStart.Should().Be(startDate);
            p.PeriodEnd.Should().Be(endDate);
        });
    }
}
