using System.Diagnostics;
using Microsoft.Extensions.Logging.Abstractions;
using SportsGurukul.Platform.FinancialReporting.Analytics;
using SportsGurukul.Platform.FinancialReporting.Dashboard;
using SportsGurukul.Platform.FinancialReporting.Exports;
using SportsGurukul.Platform.FinancialReporting.Interfaces;
using SportsGurukul.Platform.FinancialReporting.Models;
using SportsGurukul.Platform.FinancialReporting.Reports;

namespace SportsGurukul.Platform.FinancialReporting.Tests;

public class PerformanceTests
{
    private readonly DashboardService _dashboardService;
    private readonly ReportService _reportService;
    private readonly AnalyticsService _analyticsService;
    private readonly IExportService _exportService;

    public PerformanceTests()
    {
        _dashboardService = new DashboardService(NullLogger<DashboardService>.Instance);
        _reportService = new ReportService(NullLogger<ReportService>.Instance);
        _analyticsService = new AnalyticsService(NullLogger<AnalyticsService>.Instance);
        var excelService = new StubExcelExportService();
        var csvService = new StubCsvExportService();
        var pdfService = new StubPdfExportService();
        _exportService = new ExportService(
            NullLogger<ExportService>.Instance, excelService, csvService, pdfService);
    }

    [Fact]
    public async Task Dashboard_Under300ms()
    {
        var sw = Stopwatch.StartNew();
        await _dashboardService.GetDashboardAsync();
        sw.Stop();
        Assert.True(sw.ElapsedMilliseconds < 300,
            $"Dashboard took {sw.ElapsedMilliseconds}ms (target <300ms)");
    }

    [Fact]
    public async Task Report_Under1Second()
    {
        var sw = Stopwatch.StartNew();
        await _reportService.GenerateRevenueReportAsync();
        sw.Stop();
        Assert.True(sw.ElapsedMilliseconds < 1000,
            $"Report took {sw.ElapsedMilliseconds}ms (target <1s)");
    }

    [Fact]
    public async Task Export_Under5Seconds()
    {
        var data = await _reportService.GenerateRevenueReportAsync();
        var sw = Stopwatch.StartNew();
        await _exportService.ExportToExcelAsync(data, "perf_test");
        sw.Stop();
        Assert.True(sw.ElapsedMilliseconds < 5000,
            $"Export took {sw.ElapsedMilliseconds}ms (target <5s)");
    }

    [Fact]
    public async Task Analytics_Under1Second()
    {
        var sw = Stopwatch.StartNew();
        await _analyticsService.GetRevenueTrendsAsync(
            DateTime.UtcNow.AddMonths(-3), DateTime.UtcNow);
        sw.Stop();
        Assert.True(sw.ElapsedMilliseconds < 1000,
            $"Analytics took {sw.ElapsedMilliseconds}ms (target <1s)");
    }

    [Fact]
    public async Task AllReports_Under5SecondsCombined()
    {
        var sw = Stopwatch.StartNew();
        await _reportService.GenerateRevenueReportAsync();
        await _reportService.GenerateDailyCollectionReportAsync(DateTime.UtcNow);
        await _reportService.GenerateMonthlyCollectionReportAsync(2026, 7);
        await _reportService.GenerateYearlyRevenueReportAsync(2026);
        await _reportService.GenerateOutstandingInvoicesReportAsync();
        sw.Stop();
        Assert.True(sw.ElapsedMilliseconds < 5000,
            $"5 reports combined took {sw.ElapsedMilliseconds}ms (target <5s)");
    }
}
