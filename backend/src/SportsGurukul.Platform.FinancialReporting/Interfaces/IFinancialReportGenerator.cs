using SportsGurukul.Platform.FinancialReporting.Models;

namespace SportsGurukul.Platform.FinancialReporting.Interfaces;

public interface IFinancialReportGenerator
{
    Task<ReportResult> GenerateReportAsync(ReportRequest request, CancellationToken cancellationToken = default);
    Task<ReportResult> GenerateAndExportAsync(ReportRequest request, CancellationToken cancellationToken = default);
}
