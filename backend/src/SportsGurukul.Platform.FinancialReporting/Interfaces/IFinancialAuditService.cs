using SportsGurukul.Platform.FinancialReporting.Models;

namespace SportsGurukul.Platform.FinancialReporting.Interfaces;

public interface IFinancialAuditService
{
    Task LogAsync(FinancialAuditLogEntry entry, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FinancialAuditLogEntry>> GetAuditLogsAsync(DateTime from, DateTime to, string? userId = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FinancialAuditLogEntry>> GetAuditLogsByResourceAsync(string resourceType, string resourceId, CancellationToken cancellationToken = default);
    Task<bool> HasPermissionAsync(string userId, string permission, CancellationToken cancellationToken = default);
    string? MaskSensitiveData(string? data, int visibleChars = 4);
}
