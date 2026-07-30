using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using SportsGurukul.Platform.FinancialReporting.Dashboard;
using SportsGurukul.Platform.FinancialReporting.Interfaces;
using SportsGurukul.Platform.FinancialReporting.Queries;
using SportsGurukul.Platform.FinancialReporting.Reports;

namespace SportsGurukul.Platform.FinancialReporting.Tests;

public class QueryHandlerTests
{
    private readonly IDashboardService _dashboardService;
    private readonly IReportService _reportService;
    private readonly Interfaces.IReconciliationService _reconciliationService;

    public QueryHandlerTests()
    {
        _dashboardService = new DashboardService(NullLogger<DashboardService>.Instance);
        _reportService = new ReportService(NullLogger<ReportService>.Instance);
        _reconciliationService = new Reconciliation.ReconciliationService(NullLogger<Reconciliation.ReconciliationService>.Instance);
    }

    [Fact]
    public async Task FinancialDashboardQuery_ReturnsDashboard()
    {
        var handler = new FinancialDashboardQueryHandler(_dashboardService);
        var query = new FinancialDashboardQuery();
        var result = await handler.Handle(query, CancellationToken.None);
        Assert.NotNull(result);
        Assert.NotNull(result.Revenue);
    }

    [Fact]
    public async Task RevenueSummaryQuery_ReturnsReport()
    {
        var handler = new RevenueSummaryQueryHandler(_reportService);
        var query = new RevenueSummaryQuery(
            DateTime.UtcNow.AddMonths(-1), DateTime.UtcNow);
        var result = await handler.Handle(query, CancellationToken.None);
        Assert.NotNull(result);
        Assert.True(result.TotalRevenue > 0);
    }

    [Fact]
    public async Task OutstandingSummaryQuery_ReturnsReport()
    {
        var handler = new OutstandingSummaryQueryHandler(_reportService);
        var query = new OutstandingSummaryQuery();
        var result = await handler.Handle(query, CancellationToken.None);
        Assert.NotNull(result);
        Assert.True(result.TotalInvoices > 0);
    }

    [Fact]
    public async Task SettlementSummaryQuery_ReturnsReport()
    {
        var handler = new SettlementSummaryQueryHandler(_reportService);
        var query = new SettlementSummaryQuery(
            DateTime.UtcNow.AddMonths(-1), DateTime.UtcNow);
        var result = await handler.Handle(query, CancellationToken.None);
        Assert.NotNull(result);
        Assert.True(result.TotalSettlements > 0);
    }

    [Fact]
    public async Task TaxSummaryQuery_ReturnsReport()
    {
        var handler = new TaxSummaryQueryHandler(_reportService);
        var query = new TaxSummaryQuery(
            DateTime.UtcNow.AddMonths(-1), DateTime.UtcNow);
        var result = await handler.Handle(query, CancellationToken.None);
        Assert.NotNull(result);
        Assert.True(result.TotalTaxCollected > 0);
    }

    [Fact]
    public async Task LedgerQuery_ReturnsReport()
    {
        var handler = new LedgerQueryHandler(_reportService);
        var query = new LedgerQuery(
            DateTime.UtcNow.AddMonths(-1), DateTime.UtcNow);
        var result = await handler.Handle(query, CancellationToken.None);
        Assert.NotNull(result);
        Assert.NotEmpty(result.Entries);
    }

    [Fact]
    public async Task ReconciliationQuery_ReturnsResult()
    {
        var handler = new ReconciliationQueryHandler(_reconciliationService);
        var query = new ReconciliationQuery(
            Models.ReconciliationType.Gateway,
            DateTime.UtcNow.AddMonths(-1), DateTime.UtcNow);
        var result = await handler.Handle(query, CancellationToken.None);
        Assert.NotNull(result);
        Assert.NotEmpty(result.Differences);
    }
}
