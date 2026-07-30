using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.FinancialReporting.Analytics;
using SportsGurukul.Platform.FinancialReporting.Caching;
using SportsGurukul.Platform.FinancialReporting.Dashboard;
using SportsGurukul.Platform.FinancialReporting.Exports;
using SportsGurukul.Platform.FinancialReporting.Interfaces;
using SportsGurukul.Platform.FinancialReporting.Reconciliation;
using SportsGurukul.Platform.FinancialReporting.Reports;
using SportsGurukul.Platform.FinancialReporting.Security;

namespace SportsGurukul.Platform.FinancialReporting;

public static class DependencyInjection
{
    public static IServiceCollection AddFinancialReportingPlatform(
        this IServiceCollection services,
        Action<FinancialReportingOptions>? configureOptions = null)
    {
        var options = new FinancialReportingOptions();
        configureOptions?.Invoke(options);

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        services.AddSingleton<IDashboardService, DashboardService>();
        services.AddSingleton<IReportService, ReportService>();
        services.AddSingleton<IAnalyticsService, AnalyticsService>();
        services.AddSingleton<IReconciliationService, ReconciliationService>();
        services.AddSingleton<IFinancialAuditService, FinancialAuditService>();
        services.AddSingleton<IFinancialCacheService, FinancialCacheService>();
        services.AddSingleton<IFinancialReportGenerator, FinancialReportGenerator>();

        services.AddSingleton<IExcelExportService, StubExcelExportService>();
        services.AddSingleton<ICsvExportService, StubCsvExportService>();
        services.AddSingleton<IPdfExportService, StubPdfExportService>();
        services.AddSingleton<IExportService, ExportService>();

        return services;
    }
}

public class FinancialReportingOptions
{
    public bool EnableCaching { get; set; } = true;
    public int DashboardCacheDurationMinutes { get; set; } = 5;
    public int ReportCacheDurationMinutes { get; set; } = 10;
    public bool EnableAuditLogging { get; set; } = true;
    public bool EnableSensitiveDataMasking { get; set; } = true;
}
