using Microsoft.Extensions.Logging.Abstractions;
using SportsGurukul.Platform.FinancialReporting.Models;
using SportsGurukul.Platform.FinancialReporting.Security;

namespace SportsGurukul.Platform.FinancialReporting.Tests;

public class FinancialAuditServiceTests
{
    private readonly FinancialAuditService _service;

    public FinancialAuditServiceTests()
    {
        _service = new FinancialAuditService(NullLogger<FinancialAuditService>.Instance);
    }

    [Fact]
    public async Task Log_CreatesAuditEntry()
    {
        var entry = new FinancialAuditLogEntry
        {
            UserId = "user_1", Action = "VIEW_REPORT",
            ResourceType = "RevenueReport", ResourceId = "rep_001"
        };
        await _service.LogAsync(entry);
        Assert.NotEmpty(entry.AuditId);
    }

    [Fact]
    public async Task GetAuditLogs_ReturnsLoggedEntries()
    {
        await _service.LogAsync(new FinancialAuditLogEntry
        {
            UserId = "user_2", Action = "EXPORT", ResourceType = "Report", ResourceId = "rep_002"
        });
        var logs = await _service.GetAuditLogsAsync(
            DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1));
        Assert.NotEmpty(logs);
    }

    [Fact]
    public async Task GetAuditLogs_FiltersByUser()
    {
        await _service.LogAsync(new FinancialAuditLogEntry
        {
            UserId = "user_filter", Action = "VIEW", ResourceType = "Dashboard", ResourceId = "dash_001"
        });
        var logs = await _service.GetAuditLogsAsync(
            DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1), userId: "user_filter");
        Assert.All(logs, l => Assert.Equal("user_filter", l.UserId));
    }

    [Fact]
    public async Task GetAuditLogsByResource_ReturnsFiltered()
    {
        await _service.LogAsync(new FinancialAuditLogEntry
        {
            UserId = "user_3", Action = "VIEW", ResourceType = "Invoice", ResourceId = "inv_001"
        });
        var logs = await _service.GetAuditLogsByResourceAsync("Invoice", "inv_001");
        Assert.NotEmpty(logs);
    }

    [Fact]
    public async Task HasPermission_ReturnsTrue()
    {
        var permitted = await _service.HasPermissionAsync("user_1", FinancialPermission.ViewDashboard);
        Assert.True(permitted);
    }

    [Fact]
    public void MaskSensitiveData_MasksCorrectly()
    {
        var masked = _service.MaskSensitiveData("1234567890", 4);
        Assert.Equal("******7890", masked);
    }

    [Fact]
    public void MaskSensitiveData_NullInput_ReturnsNull()
    {
        Assert.Null(_service.MaskSensitiveData(null));
    }

    [Fact]
    public void MaskSensitiveData_ShortString_ReturnsUnchanged()
    {
        var result = _service.MaskSensitiveData("abcd", 4);
        Assert.Equal("abcd", result);
    }
}
