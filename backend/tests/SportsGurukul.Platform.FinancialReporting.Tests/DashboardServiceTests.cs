using Microsoft.Extensions.Logging.Abstractions;
using SportsGurukul.Platform.FinancialReporting.Dashboard;
using SportsGurukul.Platform.FinancialReporting.Models;

namespace SportsGurukul.Platform.FinancialReporting.Tests;

public class DashboardServiceTests
{
    private readonly DashboardService _service;

    public DashboardServiceTests()
    {
        _service = new DashboardService(NullLogger<DashboardService>.Instance);
    }

    [Fact]
    public async Task GetDashboardAsync_ReturnsAllKpis()
    {
        var result = await _service.GetDashboardAsync();
        Assert.NotNull(result);
        Assert.NotNull(result.Revenue);
        Assert.NotNull(result.Payments);
        Assert.NotNull(result.Refunds);
        Assert.NotNull(result.Outstanding);
        Assert.NotNull(result.Settlements);
        Assert.NotNull(result.Wallet);
        Assert.NotNull(result.Scholarships);
        Assert.NotNull(result.Coupons);
    }

    [Fact]
    public async Task GetDashboardAsync_RevenueKpi_HasValues()
    {
        var result = await _service.GetRevenueKpiAsync();
        Assert.True(result.TotalRevenue > 0);
        Assert.True(result.MonthlyRevenue > 0);
        Assert.NotEmpty(result.RevenueBySource);
    }

    [Fact]
    public async Task GetDashboardAsync_PaymentKpi_HasValues()
    {
        var result = await _service.GetPaymentKpiAsync();
        Assert.True(result.TotalTransactions > 0);
        Assert.True(result.SuccessRate > 0);
        Assert.NotEmpty(result.TransactionsByGateway);
    }

    [Fact]
    public async Task GetDashboardAsync_RefundKpi_HasValues()
    {
        var result = await _service.GetRefundKpiAsync();
        Assert.True(result.TotalRefunds >= 0);
        Assert.True(result.RefundRate >= 0);
    }

    [Fact]
    public async Task GetDashboardAsync_OutstandingKpi_HasValues()
    {
        var result = await _service.GetOutstandingKpiAsync();
        Assert.True(result.TotalOutstandingAmount >= 0);
        Assert.NotEmpty(result.AgingBreakdown);
    }

    [Fact]
    public async Task GetDashboardAsync_SettlementKpi_HasValues()
    {
        var result = await _service.GetSettlementKpiAsync();
        Assert.True(result.CompletedSettlementAmount > 0);
        Assert.NotEmpty(result.SettlementByGateway);
    }

    [Fact]
    public async Task GetDashboardAsync_WalletKpi_HasValues()
    {
        var result = await _service.GetWalletKpiAsync();
        Assert.True(result.TotalWalletBalance >= 0);
        Assert.True(result.ActiveWallets > 0);
    }

    [Fact]
    public async Task GetDashboardAsync_ScholarshipKpi_HasValues()
    {
        var result = await _service.GetScholarshipKpiAsync();
        Assert.True(result.TotalScholarshipAmount > 0);
        Assert.NotEmpty(result.ScholarshipByType);
    }

    [Fact]
    public async Task GetDashboardAsync_CouponKpi_HasValues()
    {
        var result = await _service.GetCouponKpiAsync();
        Assert.True(result.TotalCouponsUsed > 0);
        Assert.NotEmpty(result.MostUsedCoupon);
    }

    [Fact]
    public async Task GetDashboardAsync_GeneratedAt_IsRecent()
    {
        var result = await _service.GetDashboardAsync();
        Assert.True((DateTime.UtcNow - result.GeneratedAt).TotalSeconds < 5);
    }

    [Fact]
    public async Task GetDashboardAsync_WithFilter_ReturnsData()
    {
        var filter = new ReportFilter { AcademyId = "academy_1" };
        var result = await _service.GetDashboardAsync(filter);
        Assert.NotNull(result);
    }
}
