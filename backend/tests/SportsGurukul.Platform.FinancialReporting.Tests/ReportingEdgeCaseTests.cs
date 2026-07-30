using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using SportsGurukul.Platform.FinancialReporting.Dashboard;
using SportsGurukul.Platform.FinancialReporting.Exports;
using SportsGurukul.Platform.FinancialReporting.Interfaces;
using SportsGurukul.Platform.FinancialReporting.Models;
using SportsGurukul.Platform.FinancialReporting.Reconciliation;
using SportsGurukul.Platform.FinancialReporting.Reports;

namespace SportsGurukul.Platform.FinancialReporting.Tests;

public class ReportingEdgeCaseTests
{
    private readonly ReportService _reportService;
    private readonly DashboardService _dashboardService;
    private readonly ReconciliationService _reconciliationService;
    private readonly IExportService _exportService;

    public ReportingEdgeCaseTests()
    {
        _reportService = new ReportService(NullLogger<ReportService>.Instance);
        _dashboardService = new DashboardService(NullLogger<DashboardService>.Instance);
        _reconciliationService = new ReconciliationService(NullLogger<ReconciliationService>.Instance);

        var excelService = new StubExcelExportService();
        var csvService = new StubCsvExportService();
        var pdfService = new StubPdfExportService();
        _exportService = new ExportService(
            NullLogger<ExportService>.Instance, excelService, csvService, pdfService);
    }

    [Fact]
    public async Task RevenueReport_EmptyFilter_ReturnsDefaultData()
    {
        var result = await _reportService.GenerateRevenueReportAsync(new ReportFilter());

        result.Should().NotBeNull();
        result.TotalRevenue.Should().Be(1250000m);
        result.TransactionCount.Should().Be(14580);
        result.RevenueByCategory.Should().NotBeEmpty();
        result.LineItems.Should().NotBeEmpty();
    }

    [Fact]
    public async Task RevenueReport_RefundMetrics_AreNonNegative()
    {
        var result = await _reportService.GenerateRevenueReportAsync();

        result.RefundAmount.Should().Be(28500m);
        result.DiscountAmount.Should().Be(89000m);
        result.NetRevenue.Should().Be(result.TotalRevenue);
    }

    [Fact]
    public async Task RevenueReport_NetRevenue_EqualsTotalRevenue()
    {
        var result = await _reportService.GenerateRevenueReportAsync();

        result.NetRevenue.Should().Be(result.TotalRevenue);
    }

    [Fact]
    public async Task DashboardKpi_AllMetrics_HavePositiveValues()
    {
        var dashboard = await _dashboardService.GetDashboardAsync();

        dashboard.Revenue.TotalRevenue.Should().BePositive();
        dashboard.Revenue.MonthlyRevenue.Should().BePositive();
        dashboard.Revenue.RevenueGrowth.Should().BePositive();

        dashboard.Payments.TotalTransactions.Should().BePositive();
        dashboard.Payments.SuccessfulTransactions.Should().BePositive();
        dashboard.Payments.SuccessRate.Should().BePositive();

        dashboard.Refunds.TotalRefunds.Should().BePositive();
        dashboard.Refunds.TotalRefundAmount.Should().BePositive();
        dashboard.Refunds.RefundRate.Should().BePositive();

        dashboard.Outstanding.TotalOutstandingInvoices.Should().BePositive();
        dashboard.Outstanding.TotalOutstandingAmount.Should().BePositive();

        dashboard.Settlements.PendingSettlements.Should().BePositive();
        dashboard.Settlements.CompletedSettlementAmount.Should().BePositive();

        dashboard.Wallet.TotalWalletBalance.Should().BePositive();
        dashboard.Wallet.ActiveWallets.Should().BePositive();

        dashboard.Scholarships.TotalScholarships.Should().BePositive();
        dashboard.Scholarships.TotalScholarshipAmount.Should().BePositive();

        dashboard.Coupons.TotalCouponsUsed.Should().BePositive();
        dashboard.Coupons.TotalDiscountAmount.Should().BePositive();
    }

    [Fact]
    public async Task DashboardKpi_SuccessRate_IsWithinValidRange()
    {
        var dashboard = await _dashboardService.GetDashboardAsync();

        dashboard.Payments.SuccessRate.Should().BeInRange(0, 100);
    }

    [Fact]
    public async Task DashboardKpi_RefundRate_IsWithinValidRange()
    {
        var dashboard = await _dashboardService.GetDashboardAsync();

        dashboard.Refunds.RefundRate.Should().BeInRange(0, 100);
    }

    [Fact]
    public async Task DashboardKpi_TransactionsByGateway_HasExpectedGateways()
    {
        var dashboard = await _dashboardService.GetDashboardAsync();

        dashboard.Payments.TransactionsByGateway.Should().ContainKeys("Razorpay", "Stripe", "Cashfree", "PayU");
        var totalFromGateways = dashboard.Payments.TransactionsByGateway.Values.Sum();
        totalFromGateways.Should().BeLessOrEqualTo(dashboard.Payments.TotalTransactions);
    }

    [Fact]
    public async Task Reconciliation_WithMismatchedRecords_DetectsDifferences()
    {
        var result = await _reconciliationService.DetectDifferencesAsync(ReconciliationType.Gateway);

        result.Should().NotBeNull();
        result.TotalExceptions.Should().BePositive();
        result.Exceptions.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Reconciliation_AllTypes_ReturnExceptions()
    {
        foreach (ReconciliationType type in Enum.GetValues<ReconciliationType>())
        {
            var result = await _reconciliationService.DetectDifferencesAsync(type);
            result.Should().NotBeNull();
            result.Exceptions.Should().NotBeEmpty($"ReconciliationType {type} should have exceptions");
        }
    }

    [Fact]
    public async Task Reconciliation_Gateway_ReturnsMatchingGatewayName()
    {
        var result = await _reconciliationService.ReconcileGatewayAsync(
            "Razorpay", DateTime.UtcNow.AddMonths(-1), DateTime.UtcNow);

        result.GatewayName.Should().Be("Razorpay");
        result.TotalGatewayTransactions.Should().BePositive();
        result.Differences.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Reconciliation_Bank_ReturnsCompleteResult()
    {
        var result = await _reconciliationService.ReconcileBankAsync("stmt_001");

        result.BankName.Should().NotBeNullOrEmpty();
        result.MatchedTransactions.Should().NotBeEmpty();
        result.UnmatchedBankTransactions.Should().BeEmpty();
        result.UnmatchedSystemTransactions.Should().BeEmpty();
    }

    [Fact]
    public async Task Reconciliation_Invoice_ReturnsMatchingData()
    {
        var result = await _reconciliationService.ReconcileInvoicesAsync();

        result.TotalInvoices.Should().BePositive();
        result.Items.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Reconciliation_Settlement_ReturnsPositiveValues()
    {
        var result = await _reconciliationService.ReconcileSettlementsAsync();

        result.TotalSettlements.Should().BePositive();
        result.TotalSettlementAmount.Should().BePositive();
    }

    [Fact]
    public async Task Export_WithNullData_ShouldNotThrow()
    {
        var act = async () => await _exportService.ExportToExcelAsync<RevenueReport>(null!, "null_test");

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Export_WithEmptyList_ReturnsContent()
    {
        var data = new List<RevenueLineItem>();

        var result = await _exportService.ExportToCsvAsync(data, "empty_list");

        result.Success.Should().BeTrue();
        result.FileContent.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Export_WithSingleItem_ReturnsContent()
    {
        var data = new List<RevenueLineItem>
        {
            new() { Date = DateTime.UtcNow, TransactionId = "TXN001", Amount = 100 }
        };

        var result = await _exportService.ExportToCsvAsync(data, "single_item");

        result.Success.Should().BeTrue();
        result.FileContent.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Export_AllFormats_WithEmptyDashboard()
    {
        var data = new FinancialDashboard();

        var excelResult = await _exportService.ExportAsync(data, ReportFormat.Excel, "empty_dash");
        var csvResult = await _exportService.ExportAsync(data, ReportFormat.Csv, "empty_dash");
        var pdfResult = await _exportService.ExportAsync(data, ReportFormat.Pdf, "empty_dash");

        excelResult.Success.Should().BeTrue();
        excelResult.FileName.Should().EndWith(".xlsx");

        csvResult.Success.Should().BeTrue();
        csvResult.FileName.Should().EndWith(".csv");

        pdfResult.Success.Should().BeTrue();
        pdfResult.FileName.Should().EndWith(".pdf");
    }

    [Fact]
    public async Task Export_FileContent_IsNotEmpty()
    {
        var data = await _reportService.GenerateRevenueReportAsync();

        var excel = await _exportService.ExportToExcelAsync(data, "revenue");
        var csv = await _exportService.ExportToCsvAsync(data, "revenue");
        var pdf = await _exportService.ExportToPdfAsync(data, "revenue");

        excel.FileContent.Should().NotBeEmpty();
        excel.ContentType.Should().Be("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        excel.FileSize.Should().Be(excel.FileContent.Length);

        csv.FileContent.Should().NotBeEmpty();
        csv.ContentType.Should().Be("text/csv");

        pdf.FileContent.Should().NotBeEmpty();
        pdf.ContentType.Should().Be("application/pdf");
    }

    [Fact]
    public async Task Export_WithRevenueReport_HasCorrectMetadata()
    {
        var data = await _reportService.GenerateRevenueReportAsync();

        var result = await _exportService.ExportToExcelAsync(data, "monthly_revenue");

        result.FileName.Should().Be("monthly_revenue.xlsx");
        result.Success.Should().BeTrue();
    }

    [Fact]
    public async Task SettlementReport_FeesAndNetAmount_AreConsistent()
    {
        var result = await _reportService.GenerateSettlementReportAsync();

        result.NetAmount.Should().Be(result.TotalSettlementAmount - result.TotalFees);
    }

    [Fact]
    public async Task ScholarshipReport_Amounts_AreConsistent()
    {
        var result = await _reportService.GenerateScholarshipReportAsync();

        result.TotalScholarships.Should().BePositive();
        result.ActiveCount.Should().BeLessOrEqualTo(result.TotalScholarships);
        result.ScholarshipByType.Values.Sum().Should().Be(result.TotalAmount);
    }

    [Fact]
    public async Task CouponUsageReport_UsageCounts_AreConsistent()
    {
        var result = await _reportService.GenerateCouponUsageReportAsync();

        result.TotalCouponsUsed.Should().BePositive();
        var usageSum = result.Usage.Sum(u => u.UsageCount);
        usageSum.Should().Be(result.TotalCouponsUsed);
    }
}
