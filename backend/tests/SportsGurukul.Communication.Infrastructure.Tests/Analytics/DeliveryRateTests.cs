using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.Communication.Analytics.Abstractions;
using SportsGurukul.Platform.Communication.Analytics.DTOs;
using SportsGurukul.Platform.Communication.Analytics.Services;

namespace SportsGurukul.Communication.Infrastructure.Tests.Analytics;

public class DeliveryRateTests
{
    private readonly Mock<ILogger<AnalyticsService>> _loggerMock = new();
    private readonly Mock<ICacheService> _cacheMock = new();

    private AnalyticsService CreateService() => new(_loggerMock.Object, _cacheMock.Object);

    [Fact]
    public async Task GetDeliveryRateAsync_ReturnsDtoWithRate()
    {
        var service = CreateService();

        var result = await service.GetDeliveryRateAsync(null);

        result.Should().NotBeNull();
        result.Total.Should().Be(result.Sent + result.Failed);
        result.DeliveryRate.Should().BeInRange(0, 100);
        result.FailureRate.Should().BeInRange(0, 100);
    }

    [Fact]
    public async Task GetDeliveryRateAsync_DeliveredEqualsSent_Returns100Percent()
    {
        var service = CreateService();
        var filter = new AnalyticsFilterDto(
            DateTime.UtcNow.AddDays(-1), DateTime.UtcNow,
            Guid.Parse("00000000-0000-0000-0000-000000000001"), null, null, null, null);

        var result = await service.GetDeliveryRateAsync(filter);

        result.Delivered.Should().BeLessThanOrEqualTo(result.Sent);
        result.DeliveryRate.Should().BeInRange(0, 100);
    }

    [Fact]
    public async Task GetDeliveryRateAsync_RespectsDateFilter()
    {
        var startDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var endDate = new DateTime(2026, 1, 31, 23, 59, 59, DateTimeKind.Utc);
        var filter = new AnalyticsFilterDto(startDate, endDate, null, null, null, null, null);

        var service = CreateService();

        var result = await service.GetDeliveryRateAsync(filter);

        result.PeriodStart.Should().Be(startDate);
        result.PeriodEnd.Should().Be(endDate);
    }

    [Fact]
    public async Task GetDeliveryRateAsync_RespectsCampaignFilter()
    {
        var campaignId = Guid.NewGuid();
        var filter = new AnalyticsFilterDto(null, null, campaignId, null, null, null, null);

        var service = CreateService();

        var result = await service.GetDeliveryRateAsync(filter);

        result.CampaignId.Should().Be(campaignId);
    }

    [Fact]
    public async Task GetDeliveryRateAsync_WithNullFilter_DefaultsTo30DayPeriod()
    {
        var service = CreateService();

        var result = await service.GetDeliveryRateAsync(null);

        result.PeriodStart.Should().BeCloseTo(DateTime.UtcNow.AddDays(-30), TimeSpan.FromMinutes(1));
        result.PeriodEnd.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
    }

    [Fact]
    public async Task GetDeliveryRateAsync_AverageDeliveryTime_IsPositive()
    {
        var service = CreateService();

        var result = await service.GetDeliveryRateAsync(null);

        result.AverageDeliveryTime.Should().BeGreaterThan(TimeSpan.Zero);
    }

    [Fact]
    public async Task GetDeliveryRateAsync_SentAndDeliveredCounts_AreConsistent()
    {
        var service = CreateService();
        var campaignId = Guid.NewGuid();
        var filter = new AnalyticsFilterDto(null, null, campaignId, null, null, null, null);

        var result = await service.GetDeliveryRateAsync(filter);

        result.Sent.Should().BeGreaterThan(0);
        result.Delivered.Should().BeGreaterThanOrEqualTo(0);
        result.Failed.Should().BeGreaterThanOrEqualTo(0);
        (result.Delivered + result.Failed).Should().BeLessThanOrEqualTo(result.Total);
    }
}
