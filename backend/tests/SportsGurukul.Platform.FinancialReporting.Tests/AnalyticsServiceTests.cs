using Microsoft.Extensions.Logging.Abstractions;
using SportsGurukul.Platform.FinancialReporting.Analytics;
using SportsGurukul.Platform.FinancialReporting.Models;

namespace SportsGurukul.Platform.FinancialReporting.Tests;

public class AnalyticsServiceTests
{
    private readonly AnalyticsService _service;

    public AnalyticsServiceTests()
    {
        _service = new AnalyticsService(NullLogger<AnalyticsService>.Instance);
    }

    [Fact]
    public async Task GetRevenueTrends_ReturnsData()
    {
        var result = await _service.GetRevenueTrendsAsync(
            DateTime.UtcNow.AddMonths(-3), DateTime.UtcNow);
        Assert.NotNull(result);
        Assert.NotEmpty(result.DailyTrend);
        Assert.NotEmpty(result.WeeklyTrend);
        Assert.NotEmpty(result.MonthlyTrend);
        Assert.True(result.GrowthRate != 0);
    }

    [Fact]
    public async Task GetPaymentTrends_ReturnsData()
    {
        var result = await _service.GetPaymentTrendsAsync(
            DateTime.UtcNow.AddMonths(-3), DateTime.UtcNow);
        Assert.NotNull(result);
        Assert.NotEmpty(result.VolumeTrend);
        Assert.NotEmpty(result.ValueTrend);
        Assert.True(result.SuccessRateTrend > 0);
    }

    [Fact]
    public async Task GetRefundTrends_ReturnsData()
    {
        var result = await _service.GetRefundTrendsAsync(
            DateTime.UtcNow.AddMonths(-3), DateTime.UtcNow);
        Assert.NotNull(result);
        Assert.NotEmpty(result.RefundRateTrend);
        Assert.NotEmpty(result.RefundAmountTrend);
    }

    [Fact]
    public async Task GetCollectionEfficiency_ReturnsData()
    {
        var result = await _service.GetCollectionEfficiencyAsync(
            DateTime.UtcNow.AddMonths(-3), DateTime.UtcNow);
        Assert.NotNull(result);
        Assert.True(result.OverallEfficiency > 0);
        Assert.NotEmpty(result.EfficiencyByAcademy);
    }

    [Fact]
    public async Task GetOutstandingAging_ReturnsData()
    {
        var result = await _service.GetOutstandingAgingAsync();
        Assert.NotNull(result);
        Assert.NotEmpty(result.AgingBuckets);
        Assert.True(result.TotalOutstanding > 0);
    }

    [Fact]
    public async Task GetPaymentMethodDistribution_ReturnsData()
    {
        var result = await _service.GetPaymentMethodDistributionAsync(
            DateTime.UtcNow.AddMonths(-3), DateTime.UtcNow);
        Assert.NotNull(result);
        Assert.NotEmpty(result.TransactionCount);
        Assert.NotEmpty(result.VolumeByMethod);
    }

    [Fact]
    public async Task GetGatewaySuccessRate_ReturnsData()
    {
        var result = await _service.GetGatewaySuccessRateAsync(
            DateTime.UtcNow.AddMonths(-3), DateTime.UtcNow);
        Assert.NotNull(result);
        Assert.NotEmpty(result.OverallSuccessRate);
        Assert.NotEmpty(result.AverageResponseTime);
    }

    [Fact]
    public async Task GetSettlementPerformance_ReturnsData()
    {
        var result = await _service.GetSettlementPerformanceAsync(
            DateTime.UtcNow.AddMonths(-3), DateTime.UtcNow);
        Assert.NotNull(result);
        Assert.True(result.AverageSettlementTime > 0);
        Assert.NotEmpty(result.SettlementTimeByGateway);
    }

    [Fact]
    public async Task GetScholarshipImpact_ReturnsData()
    {
        var result = await _service.GetScholarshipImpactAsync();
        Assert.NotNull(result);
        Assert.True(result.TotalScholarshipAmount > 0);
        Assert.True(result.StudentsBenefited > 0);
    }

    [Fact]
    public async Task GetCouponEffectiveness_ReturnsData()
    {
        var result = await _service.GetCouponEffectivenessAsync(
            DateTime.UtcNow.AddMonths(-3), DateTime.UtcNow);
        Assert.NotNull(result);
        Assert.True(result.TotalDiscountGiven > 0);
        Assert.NotEmpty(result.TopCoupons);
    }

    [Fact]
    public async Task TrendDataPoints_HaveRequiredFields()
    {
        var result = await _service.GetRevenueTrendsAsync(
            DateTime.UtcNow.AddMonths(-1), DateTime.UtcNow);
        foreach (var point in result.DailyTrend)
        {
            Assert.NotEmpty(point.Label);
            Assert.True(point.Value != 0);
        }
    }
}
