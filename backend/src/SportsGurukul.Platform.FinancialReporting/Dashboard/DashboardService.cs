using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.FinancialReporting.Interfaces;
using SportsGurukul.Platform.FinancialReporting.Models;

namespace SportsGurukul.Platform.FinancialReporting.Dashboard;

public class DashboardService : IDashboardService
{
    private readonly ILogger<DashboardService> _logger;

    public DashboardService(ILogger<DashboardService> logger)
    {
        _logger = logger;
    }

    public Task<FinancialDashboard> GetDashboardAsync(ReportFilter? filter = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Generating financial dashboard");
        return Task.FromResult(new FinancialDashboard
        {
            Revenue = new RevenueKpi
            {
                TotalRevenue = 1250000m,
                MonthlyRevenue = 145000m,
                DailyRevenue = 5200m,
                RevenueGrowth = 12.5m,
                ProjectedRevenue = 1650000m,
                RevenueBySource = new Dictionary<string, decimal> { { "Academy", 750000 }, { "Coaching", 350000 }, { "Tournament", 100000 }, { "Other", 50000 } }
            },
            Payments = new PaymentKpi
            {
                TotalTransactions = 15234,
                SuccessfulTransactions = 14580,
                FailedTransactions = 654,
                SuccessRate = 95.7m,
                AverageTransactionValue = 82.05m,
                TotalVolume = 1250000m,
                TransactionsByGateway = new Dictionary<string, int> { { "Razorpay", 8500 }, { "Stripe", 3200 }, { "Cashfree", 2100 }, { "PayU", 1434 } }
            },
            Refunds = new RefundKpi
            {
                TotalRefunds = 342,
                TotalRefundAmount = 28500m,
                RefundRate = 2.3m,
                PendingRefunds = 18,
                AverageRefundTime = 1.5m
            },
            Outstanding = new OutstandingKpi
            {
                TotalOutstandingInvoices = 156,
                TotalOutstandingAmount = 89000m,
                OverdueAmount = 34000m,
                OverdueInvoices = 43,
                AgingBreakdown = new Dictionary<string, decimal> { { "0-30", 45000 }, { "31-60", 22000 }, { "61-90", 12000 }, { "90+", 10000 } }
            },
            Settlements = new SettlementKpi
            {
                PendingSettlements = 12,
                PendingSettlementAmount = 45000m,
                CompletedSettlementAmount = 1180000m,
                AverageSettlementTime = 2.5m,
                SettlementByGateway = new Dictionary<string, decimal> { { "Razorpay", 650000 }, { "Stripe", 280000 }, { "Cashfree", 180000 }, { "PayU", 70000 } }
            },
            Wallet = new WalletKpi
            {
                TotalWalletBalance = 125000m,
                ActiveWallets = 3450,
                TotalCredits = 45000m,
                TotalDebits = 38000m,
                TransactionsToday = 89
            },
            Scholarships = new ScholarshipKpi
            {
                TotalScholarships = 89,
                TotalScholarshipAmount = 450000m,
                ActiveScholarships = 67,
                AverageScholarshipValue = 5056m,
                ScholarshipByType = new Dictionary<string, decimal> { { "Merit", 200000 }, { "Need", 150000 }, { "Sports", 75000 }, { "Other", 25000 } }
            },
            Coupons = new CouponKpi
            {
                TotalCouponsUsed = 1234,
                TotalDiscountAmount = 89000m,
                AverageDiscountValue = 72.12m,
                ActiveCoupons = 45,
                MostUsedCoupon = "WELCOME20"
            },
            GeneratedAt = DateTime.UtcNow
        });
    }

    public Task<RevenueKpi> GetRevenueKpiAsync(ReportFilter? filter = null, CancellationToken cancellationToken = default)
    {
        return GetDashboardAsync(filter, cancellationToken).ContinueWith(t => t.Result.Revenue);
    }

    public Task<PaymentKpi> GetPaymentKpiAsync(ReportFilter? filter = null, CancellationToken cancellationToken = default)
    {
        return GetDashboardAsync(filter, cancellationToken).ContinueWith(t => t.Result.Payments);
    }

    public Task<RefundKpi> GetRefundKpiAsync(ReportFilter? filter = null, CancellationToken cancellationToken = default)
    {
        return GetDashboardAsync(filter, cancellationToken).ContinueWith(t => t.Result.Refunds);
    }

    public Task<OutstandingKpi> GetOutstandingKpiAsync(ReportFilter? filter = null, CancellationToken cancellationToken = default)
    {
        return GetDashboardAsync(filter, cancellationToken).ContinueWith(t => t.Result.Outstanding);
    }

    public Task<SettlementKpi> GetSettlementKpiAsync(ReportFilter? filter = null, CancellationToken cancellationToken = default)
    {
        return GetDashboardAsync(filter, cancellationToken).ContinueWith(t => t.Result.Settlements);
    }

    public Task<WalletKpi> GetWalletKpiAsync(ReportFilter? filter = null, CancellationToken cancellationToken = default)
    {
        return GetDashboardAsync(filter, cancellationToken).ContinueWith(t => t.Result.Wallet);
    }

    public Task<ScholarshipKpi> GetScholarshipKpiAsync(ReportFilter? filter = null, CancellationToken cancellationToken = default)
    {
        return GetDashboardAsync(filter, cancellationToken).ContinueWith(t => t.Result.Scholarships);
    }

    public Task<CouponKpi> GetCouponKpiAsync(ReportFilter? filter = null, CancellationToken cancellationToken = default)
    {
        return GetDashboardAsync(filter, cancellationToken).ContinueWith(t => t.Result.Coupons);
    }
}
