using SportsGurukul.Platform.FinancialReporting.Models;

namespace SportsGurukul.Platform.FinancialReporting.Interfaces;

public interface IReportService
{
    Task<RevenueReport> GenerateRevenueReportAsync(ReportFilter? filter = null, CancellationToken cancellationToken = default);
    Task<DailyCollectionReport> GenerateDailyCollectionReportAsync(DateTime date, ReportFilter? filter = null, CancellationToken cancellationToken = default);
    Task<MonthlyCollectionReport> GenerateMonthlyCollectionReportAsync(int year, int month, ReportFilter? filter = null, CancellationToken cancellationToken = default);
    Task<YearlyRevenueReport> GenerateYearlyRevenueReportAsync(int year, ReportFilter? filter = null, CancellationToken cancellationToken = default);
    Task<OutstandingInvoicesReport> GenerateOutstandingInvoicesReportAsync(ReportFilter? filter = null, CancellationToken cancellationToken = default);
    Task<PaymentSuccessReport> GeneratePaymentSuccessReportAsync(ReportFilter? filter = null, CancellationToken cancellationToken = default);
    Task<FailedPaymentsReport> GenerateFailedPaymentsReportAsync(ReportFilter? filter = null, CancellationToken cancellationToken = default);
    Task<RefundReport> GenerateRefundReportAsync(ReportFilter? filter = null, CancellationToken cancellationToken = default);
    Task<SettlementReport> GenerateSettlementReportAsync(ReportFilter? filter = null, CancellationToken cancellationToken = default);
    Task<LedgerReport> GenerateLedgerReportAsync(ReportFilter? filter = null, CancellationToken cancellationToken = default);
    Task<JournalReport> GenerateJournalReportAsync(ReportFilter? filter = null, CancellationToken cancellationToken = default);
    Task<WalletTransactionsReport> GenerateWalletTransactionsReportAsync(ReportFilter? filter = null, CancellationToken cancellationToken = default);
    Task<CouponUsageReport> GenerateCouponUsageReportAsync(ReportFilter? filter = null, CancellationToken cancellationToken = default);
    Task<ScholarshipReport> GenerateScholarshipReportAsync(ReportFilter? filter = null, CancellationToken cancellationToken = default);
    Task<TaxReport> GenerateTaxReportAsync(ReportFilter? filter = null, CancellationToken cancellationToken = default);
    Task<GstReport> GenerateGstReportAsync(ReportFilter? filter = null, CancellationToken cancellationToken = default);
    Task<AcademyRevenueReport> GenerateAcademyRevenueReportAsync(string academyId, ReportFilter? filter = null, CancellationToken cancellationToken = default);
    Task<CoachRevenueReport> GenerateCoachRevenueReportAsync(string coachId, ReportFilter? filter = null, CancellationToken cancellationToken = default);
    Task<AthletePaymentReport> GenerateAthletePaymentReportAsync(string athleteId, ReportFilter? filter = null, CancellationToken cancellationToken = default);
}
