using System.Diagnostics;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using SportsGurukul.Platform.FinancialReporting.Caching;
using SportsGurukul.Platform.FinancialReporting.Dashboard;
using SportsGurukul.Platform.FinancialReporting.Exports;
using SportsGurukul.Platform.FinancialReporting.Interfaces;
using SportsGurukul.Platform.FinancialReporting.Models;
using SportsGurukul.Platform.FinancialReporting.Reports;

namespace SportsGurukul.Platform.FinancialReporting.Tests;

public class PerformanceReportingTests
{
    private readonly FinancialCacheService _cache;
    private readonly DashboardService _dashboardService;
    private readonly ReportService _reportService;

    public PerformanceReportingTests()
    {
        _cache = new FinancialCacheService(NullLogger<FinancialCacheService>.Instance);
        _dashboardService = new DashboardService(NullLogger<DashboardService>.Instance);
        _reportService = new ReportService(NullLogger<ReportService>.Instance);
    }

    [Fact]
    public async Task Cache_SetAndGet_ReturnsCachedValue()
    {
        var key = "perf_cache_key";
        var value = new RevenueKpi { TotalRevenue = 5000 };

        await _cache.SetAsync(key, value);
        var result = await _cache.GetAsync<RevenueKpi>(key);

        result.Should().NotBeNull();
        result!.TotalRevenue.Should().Be(5000);
    }

    [Fact]
    public async Task Cache_Miss_ReturnsNull()
    {
        var result = await _cache.GetAsync<RevenueKpi>("non_existent_key");

        result.Should().BeNull();
    }

    [Fact]
    public async Task Cache_Expiration_RemovesEntry()
    {
        var key = "expire_test_key";
        var value = new RevenueKpi { TotalRevenue = 100 };

        var options = new CacheOptions { AbsoluteExpiration = TimeSpan.FromMilliseconds(1) };
        await _cache.SetAsync(key, value, options);

        await Task.Delay(50);

        var result = await _cache.GetAsync<RevenueKpi>(key);
        result.Should().BeNull();
    }

    [Fact]
    public async Task Cache_Exists_AfterExpiration_ReturnsFalse()
    {
        var key = "expire_exists_test";
        await _cache.SetAsync(key, new RevenueKpi(), new CacheOptions { AbsoluteExpiration = TimeSpan.FromMilliseconds(1) });

        await Task.Delay(50);

        var exists = await _cache.ExistsAsync(key);
        exists.Should().BeFalse();
    }

    [Fact]
    public async Task Cache_Remove_RemovesEntry()
    {
        var key = "remove_perf_test";
        await _cache.SetAsync(key, new RevenueKpi { TotalRevenue = 200 });

        await _cache.RemoveAsync(key);

        var exists = await _cache.ExistsAsync(key);
        exists.Should().BeFalse();
    }

    [Fact]
    public async Task Cache_Overwrite_UpdatesValue()
    {
        var key = "overwrite_perf_test";

        await _cache.SetAsync(key, new RevenueKpi { TotalRevenue = 100 });
        await _cache.SetAsync(key, new RevenueKpi { TotalRevenue = 200 });

        var result = await _cache.GetAsync<RevenueKpi>(key);
        result!.TotalRevenue.Should().Be(200);
    }

    [Fact]
    public async Task Cache_MultipleKeys_WorkIndependently()
    {
        await _cache.SetAsync("perf_key_a", new RevenueKpi { TotalRevenue = 1000 });
        await _cache.SetAsync("perf_key_b", new RevenueKpi { TotalRevenue = 2000 });

        await _cache.RemoveAsync("perf_key_a");

        var valA = await _cache.GetAsync<RevenueKpi>("perf_key_a");
        var valB = await _cache.GetAsync<RevenueKpi>("perf_key_b");

        valA.Should().BeNull();
        valB.Should().NotBeNull();
        valB!.TotalRevenue.Should().Be(2000);
    }

    [Fact]
    public void BuildKey_ReturnsCorrectFormat()
    {
        var key = _cache.BuildKey(CacheRegion.Dashboard, "monthly");

        key.Should().Be("fin:Dashboard:monthly");
    }

    [Fact]
    public void BuildKey_WithAnalyticsRegion()
    {
        var key = _cache.BuildKey(CacheRegion.Analytics, "revenue_trends");

        key.Should().Be("fin:Analytics:revenue_trends");
    }

    [Fact]
    public void BuildKey_AllRegions_ProduceUniqueKeys()
    {
        var keys = new HashSet<string>();
        foreach (CacheRegion region in Enum.GetValues<CacheRegion>())
        {
            var key = _cache.BuildKey(region, "test");
            keys.Add(key).Should().BeTrue($"Key for region {region} should be unique");
        }
    }

    [Fact]
    public async Task Dashboard_Under300ms()
    {
        var sw = Stopwatch.StartNew();
        await _dashboardService.GetDashboardAsync();
        sw.Stop();

        sw.ElapsedMilliseconds.Should().BeLessThan(300);
    }

    [Fact]
    public async Task Dashboard_IndividualKpis_Under200ms()
    {
        var sw = Stopwatch.StartNew();

        await _dashboardService.GetRevenueKpiAsync();
        await _dashboardService.GetPaymentKpiAsync();
        await _dashboardService.GetRefundKpiAsync();
        await _dashboardService.GetOutstandingKpiAsync();
        await _dashboardService.GetSettlementKpiAsync();
        await _dashboardService.GetWalletKpiAsync();
        await _dashboardService.GetScholarshipKpiAsync();
        await _dashboardService.GetCouponKpiAsync();

        sw.Stop();
        sw.ElapsedMilliseconds.Should().BeLessThan(200);
    }

    [Fact]
    public async Task Dashboard_GetRevenueKpi_ReturnsDataMatchingDashboard()
    {
        var dashboard = await _dashboardService.GetDashboardAsync();
        var revenueKpi = await _dashboardService.GetRevenueKpiAsync();

        revenueKpi.TotalRevenue.Should().Be(dashboard.Revenue.TotalRevenue);
        revenueKpi.MonthlyRevenue.Should().Be(dashboard.Revenue.MonthlyRevenue);
        revenueKpi.RevenueGrowth.Should().Be(dashboard.Revenue.RevenueGrowth);
    }

    [Fact]
    public async Task Dashboard_GetPaymentKpi_ReturnsDataMatchingDashboard()
    {
        var dashboard = await _dashboardService.GetDashboardAsync();
        var paymentKpi = await _dashboardService.GetPaymentKpiAsync();

        paymentKpi.TotalTransactions.Should().Be(dashboard.Payments.TotalTransactions);
        paymentKpi.SuccessRate.Should().Be(dashboard.Payments.SuccessRate);
    }

    [Fact]
    public async Task Report_Under1Second()
    {
        var sw = Stopwatch.StartNew();
        await _reportService.GenerateRevenueReportAsync();
        sw.Stop();

        sw.ElapsedMilliseconds.Should().BeLessThan(1000);
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
        await _reportService.GeneratePaymentSuccessReportAsync();
        await _reportService.GenerateFailedPaymentsReportAsync();
        await _reportService.GenerateRefundReportAsync();
        await _reportService.GenerateSettlementReportAsync();
        await _reportService.GenerateLedgerReportAsync();
        await _reportService.GenerateJournalReportAsync();
        await _reportService.GenerateWalletTransactionsReportAsync();

        sw.Stop();
        sw.ElapsedMilliseconds.Should().BeLessThan(5000);
    }

    [Fact]
    public async Task Export_MultipleFormats_Under5Seconds()
    {
        var data = await _reportService.GenerateRevenueReportAsync();

        var sw = Stopwatch.StartNew();
        await new ExportService(
            NullLogger<ExportService>.Instance,
            new StubExcelExportService(),
            new StubCsvExportService(),
            new StubPdfExportService())
            .ExportToExcelAsync(data, "perf_excel");
        sw.Stop();

        sw.ElapsedMilliseconds.Should().BeLessThan(5000);
    }

    [Fact]
    public async Task Cache_AbsoluteExpiration_Respected()
    {
        var key = "abs_expire";
        await _cache.SetAsync(key, new RevenueKpi { TotalRevenue = 999 },
            new CacheOptions { AbsoluteExpiration = TimeSpan.FromSeconds(30) });

        var immediately = await _cache.GetAsync<RevenueKpi>(key);
        immediately.Should().NotBeNull();
    }

    [Fact]
    public async Task Cache_MultipleSetAndRemove_DoesNotThrow()
    {
        var act = async () =>
        {
            for (int i = 0; i < 20; i++)
            {
                var key = $"stress_key_{i}";
                await _cache.SetAsync(key, new RevenueKpi { TotalRevenue = i * 100 });
            }

            for (int i = 0; i < 20; i++)
            {
                var key = $"stress_key_{i}";
                await _cache.RemoveAsync(key);
            }
        };

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Dashboard_GeneratedAt_IsRecent()
    {
        var result = await _dashboardService.GetDashboardAsync();

        (DateTime.UtcNow - result.GeneratedAt).TotalSeconds.Should().BeLessThan(5);
    }
}
