using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.FinancialReporting.Interfaces;
using SportsGurukul.Platform.FinancialReporting.Models;

namespace SportsGurukul.Platform.FinancialReporting.Security;

public class FinancialAuditService : IFinancialAuditService
{
    private readonly ConcurrentBag<FinancialAuditLogEntry> _auditLog = new();
    private readonly ILogger<FinancialAuditService> _logger;

    public FinancialAuditService(ILogger<FinancialAuditService> logger)
    {
        _logger = logger;
    }

    public Task LogAsync(FinancialAuditLogEntry entry, CancellationToken cancellationToken = default)
    {
        _auditLog.Add(entry);
        _logger.LogInformation(
            "Audit: {Action} on {ResourceType}:{ResourceId} by {UserId} at {Timestamp}",
            entry.Action, entry.ResourceType, entry.ResourceId, entry.UserId, entry.Timestamp);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<FinancialAuditLogEntry>> GetAuditLogsAsync(
        DateTime from, DateTime to, string? userId = null, CancellationToken cancellationToken = default)
    {
        var query = _auditLog.Where(e => e.Timestamp >= from && e.Timestamp <= to);
        if (!string.IsNullOrEmpty(userId))
            query = query.Where(e => e.UserId == userId);

        return Task.FromResult<IReadOnlyList<FinancialAuditLogEntry>>(query.OrderByDescending(e => e.Timestamp).ToList());
    }

    public Task<IReadOnlyList<FinancialAuditLogEntry>> GetAuditLogsByResourceAsync(
        string resourceType, string resourceId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<FinancialAuditLogEntry>>(
            _auditLog.Where(e => e.ResourceType == resourceType && e.ResourceId == resourceId)
                     .OrderByDescending(e => e.Timestamp).ToList());
    }

    public Task<bool> HasPermissionAsync(string userId, string permission, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(true);
    }

    public string? MaskSensitiveData(string? data, int visibleChars = 4)
    {
        if (string.IsNullOrEmpty(data)) return data;
        if (data.Length <= visibleChars) return data;
        var masked = new string('*', data.Length - visibleChars);
        return masked + data[^visibleChars..];
    }
}
