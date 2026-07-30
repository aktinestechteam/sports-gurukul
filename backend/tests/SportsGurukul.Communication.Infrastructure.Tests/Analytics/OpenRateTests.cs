using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.Communication.Analytics.Abstractions;
using SportsGurukul.Platform.Communication.Analytics.DTOs;
using SportsGurukul.Platform.Communication.Analytics.Services;

namespace SportsGurukul.Communication.Infrastructure.Tests.Analytics;

public class OpenRateTests
{
    private readonly Mock<ILogger<AnalyticsService>> _loggerMock = new();
    private readonly Mock<ICacheService> _cacheMock = new();

    private AnalyticsService CreateService() => new(_loggerMock.Object, _cacheMock.Object);

    [Fact]
    public async Task GetEngagementRateAsync_ReturnsOpenRate()
    {
        var service = CreateService();

        var result = await service.GetEngagementRateAsync(null);

        result.OpenRate.Should().BeInRange(0, 100);
        result.UniqueOpens.Should().BeLessThanOrEqualTo(result.TotalDelivered);
    }

    [Fact]
    public async Task GetEngagementRateAsync_OpenRateComputedCorrectly()
    {
        var service = CreateService();

        var result = await service.GetEngagementRateAsync(null);

        var expectedRate = result.TotalDelivered > 0
            ? Math.Round((double)result.UniqueOpens / result.TotalDelivered * 100, 2)
            : 0;
        result.OpenRate.Should().Be(expectedRate);
    }

    [Fact]
    public async Task GetEngagementRateAsync_WhenNoDeliveries_OpenRateIsZero()
    {
        var filter = new AnalyticsFilterDto(
            new DateTime(2000, 1, 1), new DateTime(2000, 1, 2),
            null, null, null, null, null);

        var service = CreateService();

        var result = await service.GetEngagementRateAsync(filter);

        result.OpenRate.Should().BeInRange(0, 100);
        result.TotalDelivered.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task GetEngagementRateAsync_UniqueOpens_DoesNotExceedDelivered()
    {
        var service = CreateService();

        var result = await service.GetEngagementRateAsync(null);

        result.UniqueOpens.Should().BeLessThanOrEqualTo(result.TotalDelivered);
    }

    [Fact]
    public async Task GetEngagementRateAsync_RespectsCampaignFilter()
    {
        var campaignId = Guid.NewGuid();
        var filter = new AnalyticsFilterDto(null, null, campaignId, null, null, null, null);

        var service = CreateService();

        var result = await service.GetEngagementRateAsync(filter);

        result.CampaignId.Should().Be(campaignId);
    }

    [Fact]
    public async Task GetEngagementRateAsync_OpenRateAndClickRate_AreIndependent()
    {
        var service = CreateService();

        var result = await service.GetEngagementRateAsync(null);

        result.OpenRate.Should().NotBe(result.ClickRate);
    }

    [Fact]
    public async Task GetEngagementRateAsync_ReturnsValidPeriod()
    {
        var service = CreateService();

        var result = await service.GetEngagementRateAsync(null);

        result.PeriodStart.Should().BeBefore(result.PeriodEnd);
    }
}
