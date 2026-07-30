using SportsGurukul.Platform.FinancialReporting.Models;

namespace SportsGurukul.Platform.FinancialReporting.Interfaces;

public interface IAnalyticsService
{
    Task<RevenueTrendsResult> GetRevenueTrendsAsync(DateTime from, DateTime to, ReportFilter? filter = null, CancellationToken cancellationToken = default);
    Task<PaymentTrendsResult> GetPaymentTrendsAsync(DateTime from, DateTime to, ReportFilter? filter = null, CancellationToken cancellationToken = default);
    Task<RefundTrendsResult> GetRefundTrendsAsync(DateTime from, DateTime to, ReportFilter? filter = null, CancellationToken cancellationToken = default);
    Task<CollectionEfficiencyResult> GetCollectionEfficiencyAsync(DateTime from, DateTime to, ReportFilter? filter = null, CancellationToken cancellationToken = default);
    Task<OutstandingAgingResult> GetOutstandingAgingAsync(ReportFilter? filter = null, CancellationToken cancellationToken = default);
    Task<PaymentMethodDistributionResult> GetPaymentMethodDistributionAsync(DateTime from, DateTime to, ReportFilter? filter = null, CancellationToken cancellationToken = default);
    Task<GatewaySuccessRateResult> GetGatewaySuccessRateAsync(DateTime from, DateTime to, ReportFilter? filter = null, CancellationToken cancellationToken = default);
    Task<SettlementPerformanceResult> GetSettlementPerformanceAsync(DateTime from, DateTime to, ReportFilter? filter = null, CancellationToken cancellationToken = default);
    Task<ScholarshipImpactResult> GetScholarshipImpactAsync(ReportFilter? filter = null, CancellationToken cancellationToken = default);
    Task<CouponEffectivenessResult> GetCouponEffectivenessAsync(DateTime from, DateTime to, ReportFilter? filter = null, CancellationToken cancellationToken = default);
}
