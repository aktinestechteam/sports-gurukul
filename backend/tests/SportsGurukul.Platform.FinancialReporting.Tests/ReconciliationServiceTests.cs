using Microsoft.Extensions.Logging.Abstractions;
using SportsGurukul.Platform.FinancialReporting.Models;
using SportsGurukul.Platform.FinancialReporting.Reconciliation;

namespace SportsGurukul.Platform.FinancialReporting.Tests;

public class ReconciliationServiceTests
{
    private readonly ReconciliationService _service;

    public ReconciliationServiceTests()
    {
        _service = new ReconciliationService(NullLogger<ReconciliationService>.Instance);
    }

    [Fact]
    public async Task ReconcileBank_ReturnsResult()
    {
        var result = await _service.ReconcileBankAsync("stmt_001");
        Assert.NotNull(result);
        Assert.NotEmpty(result.BankName);
        Assert.NotEmpty(result.MatchedTransactions);
    }

    [Fact]
    public async Task ReconcileGateway_ReturnsResult()
    {
        var result = await _service.ReconcileGatewayAsync(
            "Razorpay", DateTime.UtcNow.AddMonths(-1), DateTime.UtcNow);
        Assert.NotNull(result);
        Assert.Equal("Razorpay", result.GatewayName);
        Assert.True(result.TotalGatewayTransactions > 0);
    }

    [Fact]
    public async Task ReconcileInvoices_ReturnsResult()
    {
        var result = await _service.ReconcileInvoicesAsync();
        Assert.NotNull(result);
        Assert.True(result.TotalInvoices > 0);
        Assert.NotEmpty(result.Items);
    }

    [Fact]
    public async Task ReconcileSettlements_ReturnsResult()
    {
        var result = await _service.ReconcileSettlementsAsync();
        Assert.NotNull(result);
        Assert.True(result.TotalSettlements > 0);
    }

    [Fact]
    public async Task ReconcileLedger_ReturnsResult()
    {
        var result = await _service.ReconcileLedgerAsync();
        Assert.NotNull(result);
        Assert.NotEmpty(result.Discrepancies);
    }

    [Fact]
    public async Task DetectDifferences_ReturnsExceptions()
    {
        var result = await _service.DetectDifferencesAsync(ReconciliationType.Gateway);
        Assert.NotNull(result);
        Assert.True(result.TotalExceptions > 0);
        Assert.NotEmpty(result.Exceptions);
    }

    [Fact]
    public async Task GenerateExceptionReport_ReturnsData()
    {
        var result = await _service.GenerateExceptionReportAsync(ReconciliationType.Bank);
        Assert.NotNull(result);
        Assert.NotEmpty(result.Exceptions);
    }

    [Fact]
    public async Task ReconciliationTypes_AllWork()
    {
        foreach (ReconciliationType type in Enum.GetValues<ReconciliationType>())
        {
            var result = await _service.DetectDifferencesAsync(type);
            Assert.NotNull(result);
            Assert.NotEmpty(result.Exceptions);
        }
    }
}
