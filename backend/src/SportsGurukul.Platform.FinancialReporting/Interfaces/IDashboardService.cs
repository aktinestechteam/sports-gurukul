using SportsGurukul.Platform.FinancialReporting.Models;

namespace SportsGurukul.Platform.FinancialReporting.Interfaces;

public interface IDashboardService
{
    Task<FinancialDashboard> GetDashboardAsync(ReportFilter? filter = null, CancellationToken cancellationToken = default);
    Task<RevenueKpi> GetRevenueKpiAsync(ReportFilter? filter = null, CancellationToken cancellationToken = default);
    Task<PaymentKpi> GetPaymentKpiAsync(ReportFilter? filter = null, CancellationToken cancellationToken = default);
    Task<RefundKpi> GetRefundKpiAsync(ReportFilter? filter = null, CancellationToken cancellationToken = default);
    Task<OutstandingKpi> GetOutstandingKpiAsync(ReportFilter? filter = null, CancellationToken cancellationToken = default);
    Task<SettlementKpi> GetSettlementKpiAsync(ReportFilter? filter = null, CancellationToken cancellationToken = default);
    Task<WalletKpi> GetWalletKpiAsync(ReportFilter? filter = null, CancellationToken cancellationToken = default);
    Task<ScholarshipKpi> GetScholarshipKpiAsync(ReportFilter? filter = null, CancellationToken cancellationToken = default);
    Task<CouponKpi> GetCouponKpiAsync(ReportFilter? filter = null, CancellationToken cancellationToken = default);
}
