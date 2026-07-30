namespace SportsGurukul.Platform.FinancialReporting.Models;

public class FinancialDashboard
{
    public RevenueKpi Revenue { get; set; } = new();
    public PaymentKpi Payments { get; set; } = new();
    public RefundKpi Refunds { get; set; } = new();
    public OutstandingKpi Outstanding { get; set; } = new();
    public SettlementKpi Settlements { get; set; } = new();
    public WalletKpi Wallet { get; set; } = new();
    public ScholarshipKpi Scholarships { get; set; } = new();
    public CouponKpi Coupons { get; set; } = new();
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}

public class RevenueKpi
{
    public decimal TotalRevenue { get; set; }
    public decimal MonthlyRevenue { get; set; }
    public decimal DailyRevenue { get; set; }
    public decimal RevenueGrowth { get; set; }
    public decimal ProjectedRevenue { get; set; }
    public Dictionary<string, decimal> RevenueBySource { get; set; } = new();
}

public class PaymentKpi
{
    public int TotalTransactions { get; set; }
    public int SuccessfulTransactions { get; set; }
    public int FailedTransactions { get; set; }
    public decimal SuccessRate { get; set; }
    public decimal AverageTransactionValue { get; set; }
    public decimal TotalVolume { get; set; }
    public Dictionary<string, int> TransactionsByGateway { get; set; } = new();
}

public class RefundKpi
{
    public int TotalRefunds { get; set; }
    public decimal TotalRefundAmount { get; set; }
    public decimal RefundRate { get; set; }
    public int PendingRefunds { get; set; }
    public decimal AverageRefundTime { get; set; }
}

public class OutstandingKpi
{
    public int TotalOutstandingInvoices { get; set; }
    public decimal TotalOutstandingAmount { get; set; }
    public decimal OverdueAmount { get; set; }
    public int OverdueInvoices { get; set; }
    public Dictionary<string, decimal> AgingBreakdown { get; set; } = new();
}

public class SettlementKpi
{
    public int PendingSettlements { get; set; }
    public decimal PendingSettlementAmount { get; set; }
    public decimal CompletedSettlementAmount { get; set; }
    public decimal AverageSettlementTime { get; set; }
    public Dictionary<string, decimal> SettlementByGateway { get; set; } = new();
}

public class WalletKpi
{
    public decimal TotalWalletBalance { get; set; }
    public int ActiveWallets { get; set; }
    public decimal TotalCredits { get; set; }
    public decimal TotalDebits { get; set; }
    public int TransactionsToday { get; set; }
}

public class ScholarshipKpi
{
    public int TotalScholarships { get; set; }
    public decimal TotalScholarshipAmount { get; set; }
    public int ActiveScholarships { get; set; }
    public decimal AverageScholarshipValue { get; set; }
    public Dictionary<string, decimal> ScholarshipByType { get; set; } = new();
}

public class CouponKpi
{
    public int TotalCouponsUsed { get; set; }
    public decimal TotalDiscountAmount { get; set; }
    public decimal AverageDiscountValue { get; set; }
    public int ActiveCoupons { get; set; }
    public string MostUsedCoupon { get; set; } = string.Empty;
}
