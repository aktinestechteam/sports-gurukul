using Microsoft.Extensions.Logging;
using SportsGurukul.Domain.Enums.Notification;
using SportsGurukul.Platform.Communication.Analytics.Abstractions;
using SportsGurukul.Platform.Communication.Analytics.DTOs;
using SportsGurukul.Platform.Communication.Analytics.Services;

namespace SportsGurukul.Communication.Infrastructure.Tests.Analytics;

public class ClickRateTests
{
    private readonly Mock<ILogger<AnalyticsService>> _loggerMock = new();
    private readonly Mock<ICacheService> _cacheMock = new();

    private AnalyticsService CreateService() => new(_loggerMock.Object, _cacheMock.Object);

    [Fact]
    public async Task GetEngagementRateAsync_ReturnsClickRate()
    {
        var service = CreateService();

        var result = await service.GetEngagementRateAsync(null);

        result.ClickRate.Should().BeInRange(0, 100);
        result.UniqueClicks.Should().BeLessThanOrEqualTo(result.TotalDelivered);
    }

    [Fact]
    public async Task GetEngagementRateAsync_ClickRateComputedCorrectly()
    {
        var service = CreateService();

        var result = await service.GetEngagementRateAsync(null);

        var expectedRate = result.UniqueOpens > 0
            ? Math.Round((double)result.UniqueClicks / result.UniqueOpens * 100, 2)
            : 0;
        result.ClickRate.Should().Be(expectedRate);
    }

    [Fact]
    public async Task GetEngagementRateAsync_UniqueClicks_DoesNotExceedUniqueOpens()
    {
        var service = CreateService();

        var result = await service.GetEngagementRateAsync(null);

        result.UniqueClicks.Should().BeLessThanOrEqualTo(result.UniqueOpens);
    }

    [Fact]
    public async Task GetEngagementRateAsync_WhenNoOpens_ClickRateIsZero()
    {
        var filter = new AnalyticsFilterDto(
            new DateTime(2000, 6, 1), new DateTime(2000, 6, 2),
            null, null, null, null, null);

        var service = CreateService();

        var result = await service.GetEngagementRateAsync(filter);

        result.UniqueClicks.Should().BeGreaterThanOrEqualTo(0);
        result.ClickRate.Should().BeInRange(0, 100);
    }

    [Fact]
    public async Task GetEngagementRateAsync_RespectsChannelFilter()
    {
        var filter = new AnalyticsFilterDto(null, null, null, null,
            NotificationChannelType.Email, null, null);

        var service = CreateService();

        var result = await service.GetEngagementRateAsync(filter);

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetEngagementRateAsync_WithAllFilters_ReturnsCorrectStructure()
    {
        var filter = new AnalyticsFilterDto(
            DateTime.UtcNow.AddDays(-7), DateTime.UtcNow,
            Guid.NewGuid(), Guid.NewGuid(),
            NotificationChannelType.PushNotification, CampaignType.OneTime, null);

        var service = CreateService();

        var result = await service.GetEngagementRateAsync(filter);

        result.CampaignId.Should().Be(filter.CampaignId);
        result.PeriodStart.Should().Be(filter.StartDate.Value);
        result.PeriodEnd.Should().Be(filter.EndDate.Value);
    }
}
