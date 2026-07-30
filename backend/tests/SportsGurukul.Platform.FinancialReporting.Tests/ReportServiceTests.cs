using Microsoft.Extensions.Logging.Abstractions;
using SportsGurukul.Platform.FinancialReporting.Models;
using SportsGurukul.Platform.FinancialReporting.Reports;

namespace SportsGurukul.Platform.FinancialReporting.Tests;

public class ReportServiceTests
{
    private readonly ReportService _service;

    public ReportServiceTests()
    {
        _service = new ReportService(NullLogger<ReportService>.Instance);
    }

    [Fact]
    public async Task GenerateRevenueReport_ReturnsData()
    {
        var result = await _service.GenerateRevenueReportAsync();
        Assert.NotNull(result);
        Assert.True(result.TotalRevenue > 0);
        Assert.NotEmpty(result.RevenueByCategory);
        Assert.NotEmpty(result.LineItems);
    }

    [Fact]
    public async Task GenerateDailyCollectionReport_ReturnsData()
    {
        var result = await _service.GenerateDailyCollectionReportAsync(DateTime.UtcNow);
        Assert.NotNull(result);
        Assert.True(result.TotalCollected > 0);
        Assert.NotEmpty(result.CollectionByAcademy);
    }

    [Fact]
    public async Task GenerateMonthlyCollectionReport_ReturnsData()
    {
        var result = await _service.GenerateMonthlyCollectionReportAsync(2026, 7);
        Assert.NotNull(result);
        Assert.True(result.TotalCollection > 0);
        Assert.True(result.AchievementPercent > 0);
    }

    [Fact]
    public async Task GenerateYearlyRevenueReport_ReturnsData()
    {
        var result = await _service.GenerateYearlyRevenueReportAsync(2026);
        Assert.NotNull(result);
        Assert.True(result.TotalRevenue > 0);
        Assert.NotEmpty(result.RevenueByMonth);
    }

    [Fact]
    public async Task GenerateOutstandingInvoicesReport_ReturnsData()
    {
        var result = await _service.GenerateOutstandingInvoicesReportAsync();
        Assert.NotNull(result);
        Assert.True(result.TotalInvoices > 0);
        Assert.NotEmpty(result.Items);
    }

    [Fact]
    public async Task GeneratePaymentSuccessReport_ReturnsData()
    {
        var result = await _service.GeneratePaymentSuccessReportAsync();
        Assert.NotNull(result);
        Assert.True(result.TotalSuccessful > 0);
        Assert.NotEmpty(result.SuccessByGateway);
        Assert.NotEmpty(result.SuccessByMethod);
    }

    [Fact]
    public async Task GenerateFailedPaymentsReport_ReturnsData()
    {
        var result = await _service.GenerateFailedPaymentsReportAsync();
        Assert.NotNull(result);
        Assert.True(result.TotalFailed > 0);
        Assert.NotEmpty(result.FailureByReason);
    }

    [Fact]
    public async Task GenerateRefundReport_ReturnsData()
    {
        var result = await _service.GenerateRefundReportAsync();
        Assert.NotNull(result);
        Assert.True(result.TotalRefunds > 0);
        Assert.NotEmpty(result.Refunds);
    }

    [Fact]
    public async Task GenerateSettlementReport_ReturnsData()
    {
        var result = await _service.GenerateSettlementReportAsync();
        Assert.NotNull(result);
        Assert.True(result.TotalSettlements > 0);
        Assert.NotEmpty(result.Settlements);
    }

    [Fact]
    public async Task GenerateLedgerReport_ReturnsData()
    {
        var result = await _service.GenerateLedgerReportAsync();
        Assert.NotNull(result);
        Assert.NotEmpty(result.Entries);
    }

    [Fact]
    public async Task GenerateJournalReport_ReturnsData()
    {
        var result = await _service.GenerateJournalReportAsync();
        Assert.NotNull(result);
        Assert.NotEmpty(result.Entries);
    }

    [Fact]
    public async Task GenerateWalletTransactionsReport_ReturnsData()
    {
        var result = await _service.GenerateWalletTransactionsReportAsync();
        Assert.NotNull(result);
        Assert.True(result.TotalTransactions > 0);
        Assert.NotEmpty(result.Transactions);
    }

    [Fact]
    public async Task GenerateCouponUsageReport_ReturnsData()
    {
        var result = await _service.GenerateCouponUsageReportAsync();
        Assert.NotNull(result);
        Assert.True(result.TotalCouponsUsed > 0);
        Assert.NotEmpty(result.Usage);
    }

    [Fact]
    public async Task GenerateScholarshipReport_ReturnsData()
    {
        var result = await _service.GenerateScholarshipReportAsync();
        Assert.NotNull(result);
        Assert.True(result.TotalScholarships > 0);
        Assert.NotEmpty(result.Items);
    }

    [Fact]
    public async Task GenerateTaxReport_ReturnsData()
    {
        var result = await _service.GenerateTaxReportAsync();
        Assert.NotNull(result);
        Assert.True(result.TotalTaxCollected > 0);
        Assert.NotEmpty(result.LineItems);
    }

    [Fact]
    public async Task GenerateGstReport_ReturnsData()
    {
        var result = await _service.GenerateGstReportAsync();
        Assert.NotNull(result);
        Assert.True(result.TotalGst > 0);
        Assert.NotEmpty(result.LineItems);
    }

    [Fact]
    public async Task GenerateAcademyRevenueReport_ReturnsData()
    {
        var result = await _service.GenerateAcademyRevenueReportAsync("academy_1");
        Assert.NotNull(result);
        Assert.Equal("academy_1", result.AcademyId);
        Assert.True(result.TotalRevenue > 0);
    }

    [Fact]
    public async Task GenerateCoachRevenueReport_ReturnsData()
    {
        var result = await _service.GenerateCoachRevenueReportAsync("coach_1");
        Assert.NotNull(result);
        Assert.Equal("coach_1", result.CoachId);
        Assert.True(result.TotalEarnings > 0);
    }

    [Fact]
    public async Task GenerateAthletePaymentReport_ReturnsData()
    {
        var result = await _service.GenerateAthletePaymentReportAsync("athlete_1");
        Assert.NotNull(result);
        Assert.Equal("athlete_1", result.AthleteId);
        Assert.True(result.TotalPaid > 0);
    }

    [Fact]
    public async Task AllReports_WithFilter_ReturnData()
    {
        var filter = new ReportFilter { AcademyId = "academy_1", SportType = "cricket" };
        var revenue = await _service.GenerateRevenueReportAsync(filter);
        var outstanding = await _service.GenerateOutstandingInvoicesReportAsync(filter);
        Assert.NotNull(revenue);
        Assert.NotNull(outstanding);
    }
}
