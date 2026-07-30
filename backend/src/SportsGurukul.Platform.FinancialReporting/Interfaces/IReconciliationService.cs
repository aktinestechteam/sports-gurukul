using SportsGurukul.Platform.FinancialReporting.Models;

namespace SportsGurukul.Platform.FinancialReporting.Interfaces;

public interface IReconciliationService
{
    Task<BankReconciliationResult> ReconcileBankAsync(string bankStatementId, ReportFilter? filter = null, CancellationToken cancellationToken = default);
    Task<GatewayReconciliationResult> ReconcileGatewayAsync(string gatewayName, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<InvoiceReconciliationResult> ReconcileInvoicesAsync(ReportFilter? filter = null, CancellationToken cancellationToken = default);
    Task<SettlementReport> ReconcileSettlementsAsync(ReportFilter? filter = null, CancellationToken cancellationToken = default);
    Task<LedgerReconciliationResult> ReconcileLedgerAsync(ReportFilter? filter = null, CancellationToken cancellationToken = default);
    Task<ExceptionReport> DetectDifferencesAsync(ReconciliationType type, ReportFilter? filter = null, CancellationToken cancellationToken = default);
    Task<ExceptionReport> GenerateExceptionReportAsync(ReconciliationType type, CancellationToken cancellationToken = default);
}
