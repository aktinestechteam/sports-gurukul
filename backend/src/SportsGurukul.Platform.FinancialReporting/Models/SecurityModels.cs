namespace SportsGurukul.Platform.FinancialReporting.Models;

public class FinancialAuditLogEntry
{
    public string AuditId { get; set; } = Guid.NewGuid().ToString("N");
    public string UserId { get; set; } = string.Empty;
    public string UserRole { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string ResourceType { get; set; } = string.Empty;
    public string ResourceId { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public bool IsSensitive { get; set; }
}

public class FinancialPermission
{
    public const string ViewDashboard = "financial.dashboard.view";
    public const string ViewReports = "financial.reports.view";
    public const string ExportReports = "financial.reports.export";
    public const string ViewReconciliation = "financial.reconciliation.view";
    public const string RunReconciliation = "financial.reconciliation.run";
    public const string ViewAnalytics = "financial.analytics.view";
    public const string ViewAuditLogs = "financial.audit.view";
    public const string ManageSettings = "financial.settings.manage";
}

public class FinancialRole
{
    public const string FinanceTeam = "FinanceTeam";
    public const string AcademyAdmin = "AcademyAdmin";
    public const string Management = "Management";
    public const string Auditor = "Auditor";
}
