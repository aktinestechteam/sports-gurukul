using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.Communication.Analytics.Abstractions;
using SportsGurukul.Platform.Communication.Analytics.DTOs;
using SportsGurukul.Platform.Communication.Analytics.Services;

namespace SportsGurukul.Communication.Infrastructure.Tests.Analytics;

public class ReadRateTests
{
    private readonly Mock<ILogger<AnalyticsService>> _loggerMock = new();
    private readonly Mock<ICacheService> _cacheMock = new();

    private AnalyticsService CreateService()
    {
        _cacheMock
            .Setup(c => c.GetOrSetAsync(It.IsAny<string>(), It.IsAny<Func<Task<AnalyticsSummaryDto?>>>(), It.IsAny<TimeSpan?>(), It.IsAny<CancellationToken>()))
            .Returns((string _, Func<Task<AnalyticsSummaryDto?>> f, TimeSpan? _, CancellationToken _) => f());
        return new AnalyticsService(_loggerMock.Object, _cacheMock.Object);
    }

    [Fact]
    public async Task GetEngagementRateAsync_ReturnsReadRate()
    {
        var service = CreateService();

        var result = await service.GetEngagementRateAsync(null);

        result.ReadRate.Should().BeInRange(0, 100);
        result.TotalReads.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task GetEngagementRateAsync_ReadRateComputedCorrectly()
    {
        var service = CreateService();

        var result = await service.GetEngagementRateAsync(null);

        var expectedRate = result.UniqueOpens > 0
            ? Math.Round((double)result.TotalReads / result.UniqueOpens * 100, 2)
            : 0;
        result.ReadRate.Should().Be(expectedRate);
    }

    [Fact]
    public async Task GetEngagementRateAsync_TotalReads_DoesNotExceedDelivered()
    {
        var service = CreateService();

        var result = await service.GetEngagementRateAsync(null);

        result.TotalReads.Should().BeLessThanOrEqualTo(result.TotalDelivered);
    }

    [Fact]
    public async Task GetEngagementRateAsync_ReadRateRelationship()
    {
        var service = CreateService();

        var result = await service.GetEngagementRateAsync(null);

        result.TotalReads.Should().BeGreaterThanOrEqualTo(result.UniqueClicks);
    }

    [Fact]
    public async Task GetSummaryAsync_ReadRateConsistent()
    {
        var service = CreateService();

        var result = await service.GetSummaryAsync(null);

        result.ReadRate.Should().BeInRange(0, 100);
        result.TotalRead.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task GetSummaryAsync_ReadRateCalculation()
    {
        var service = CreateService();

        var result = await service.GetSummaryAsync(null);

        var expected = result.TotalOpened > 0
            ? Math.Round((double)result.TotalRead / result.TotalOpened * 100, 2)
            : 0;
        result.ReadRate.Should().Be(expected);
    }

    [Fact]
    public async Task GetSummaryAsync_ReturnsBounceRate()
    {
        var service = CreateService();

        var result = await service.GetSummaryAsync(null);

        result.BounceRate.Should().BeInRange(0, 100);
        result.TotalBounced.Should().BeGreaterThanOrEqualTo(0);
    }
}
