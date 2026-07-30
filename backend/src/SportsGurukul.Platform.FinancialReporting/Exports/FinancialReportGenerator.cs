using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.FinancialReporting.Interfaces;
using SportsGurukul.Platform.FinancialReporting.Models;

namespace SportsGurukul.Platform.FinancialReporting.Exports;

public class FinancialReportGenerator : IFinancialReportGenerator
{
    private readonly IReportService _reportService;
    private readonly IExportService _exportService;
    private readonly ILogger<FinancialReportGenerator> _logger;

    public FinancialReportGenerator(
        IReportService reportService,
        IExportService exportService,
        ILogger<FinancialReportGenerator> logger)
    {
        _reportService = reportService;
        _exportService = exportService;
        _logger = logger;
    }

    public async Task<ReportResult> GenerateReportAsync(ReportRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Generating report {Type} from {From} to {To}", request.Type, request.FromDate, request.ToDate);
        var filter = request.Filter;
        var summary = new Dictionary<string, object>();

        object reportData = request.Type switch
        {
            ReportType.Revenue => await _reportService.GenerateRevenueReportAsync(filter, cancellationToken),
            ReportType.DailyCollection => await _reportService.GenerateDailyCollectionReportAsync(request.FromDate, filter, cancellationToken),
            ReportType.MonthlyCollection => await _reportService.GenerateMonthlyCollectionReportAsync(request.FromDate.Year, request.FromDate.Month, filter, cancellationToken),
            ReportType.YearlyRevenue => await _reportService.GenerateYearlyRevenueReportAsync(request.FromDate.Year, filter, cancellationToken),
            ReportType.OutstandingInvoices => await _reportService.GenerateOutstandingInvoicesReportAsync(filter, cancellationToken),
            ReportType.PaymentSuccess => await _reportService.GeneratePaymentSuccessReportAsync(filter, cancellationToken),
            ReportType.FailedPayments => await _reportService.GenerateFailedPaymentsReportAsync(filter, cancellationToken),
            ReportType.Refund => await _reportService.GenerateRefundReportAsync(filter, cancellationToken),
            ReportType.Settlement => await _reportService.GenerateSettlementReportAsync(filter, cancellationToken),
            ReportType.Ledger => await _reportService.GenerateLedgerReportAsync(filter, cancellationToken),
            ReportType.Journal => await _reportService.GenerateJournalReportAsync(filter, cancellationToken),
            ReportType.WalletTransactions => await _reportService.GenerateWalletTransactionsReportAsync(filter, cancellationToken),
            ReportType.CouponUsage => await _reportService.GenerateCouponUsageReportAsync(filter, cancellationToken),
            ReportType.Scholarship => await _reportService.GenerateScholarshipReportAsync(filter, cancellationToken),
            ReportType.Tax => await _reportService.GenerateTaxReportAsync(filter, cancellationToken),
            ReportType.Gst => await _reportService.GenerateGstReportAsync(filter, cancellationToken),
            ReportType.AcademyRevenue => await _reportService.GenerateAcademyRevenueReportAsync(filter?.AcademyId ?? "", filter, cancellationToken),
            ReportType.CoachRevenue => await _reportService.GenerateCoachRevenueReportAsync(filter?.CoachId ?? "", filter, cancellationToken),
            ReportType.AthletePayment => await _reportService.GenerateAthletePaymentReportAsync(filter?.AthleteId ?? "", filter, cancellationToken),
            _ => await _reportService.GenerateRevenueReportAsync(filter, cancellationToken)
        };

        return new ReportResult
        {
            Type = request.Type,
            Summary = summary,
            TotalRecords = 1
        };
    }

    public async Task<ReportResult> GenerateAndExportAsync(ReportRequest request, CancellationToken cancellationToken = default)
    {
        var report = await GenerateReportAsync(request, cancellationToken);
        var fileName = $"{request.Type}_{request.FromDate:yyyyMMdd}_{request.ToDate:yyyyMMdd}";

        var exportResult = request.Format switch
        {
            ReportFormat.Excel => await _exportService.ExportToExcelAsync(report, fileName, cancellationToken),
            ReportFormat.Csv => await _exportService.ExportToCsvAsync(report, fileName, cancellationToken),
            ReportFormat.Pdf => await _exportService.ExportToPdfAsync(report, fileName, cancellationToken),
            _ => await _exportService.ExportToExcelAsync(report, fileName, cancellationToken)
        };

        report.Data = exportResult.FileContent;
        report.FileName = exportResult.FileName;
        report.Format = request.Format;

        return report;
    }
}
