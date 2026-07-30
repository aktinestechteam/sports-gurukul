namespace SportsGurukul.Platform.FinancialReporting.Models;

public class ReportRequest
{
    public ReportType Type { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public ReportFilter? Filter { get; set; }
    public ReportFormat Format { get; set; } = ReportFormat.Excel;
}

public class ReportFilter
{
    public string? AcademyId { get; set; }
    public string? SportType { get; set; }
    public string? CoachId { get; set; }
    public string? AthleteId { get; set; }
    public string? PaymentMethod { get; set; }
    public string? Gateway { get; set; }
    public InvoiceStatus? InvoiceStatus { get; set; }
    public PaymentStatus? PaymentStatus { get; set; }
    public string? RefundStatus { get; set; }
}

public class ReportResult
{
    public string ReportId { get; set; } = Guid.NewGuid().ToString("N");
    public ReportType Type { get; set; }
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public int TotalRecords { get; set; }
    public byte[]? Data { get; set; }
    public string? FileName { get; set; }
    public ReportFormat Format { get; set; }
    public Dictionary<string, object> Summary { get; set; } = new();
}

public class RevenueReport
{
    public decimal TotalRevenue { get; set; }
    public decimal GrossRevenue { get; set; }
    public decimal NetRevenue { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal RefundAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public int TransactionCount { get; set; }
    public Dictionary<string, decimal> RevenueByCategory { get; set; } = new();
    public List<RevenueLineItem> LineItems { get; set; } = new();
}

public class RevenueLineItem
{
    public DateTime Date { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Category { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public class DailyCollectionReport
{
    public DateTime Date { get; set; }
    public decimal TotalCollected { get; set; }
    public int TransactionCount { get; set; }
    public decimal CashAmount { get; set; }
    public decimal OnlineAmount { get; set; }
    public decimal WalletAmount { get; set; }
    public Dictionary<string, decimal> CollectionByAcademy { get; set; } = new();
}

public class MonthlyCollectionReport
{
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal TotalCollection { get; set; }
    public decimal TargetAmount { get; set; }
    public decimal AchievementPercent { get; set; }
    public int TransactionCount { get; set; }
    public Dictionary<string, decimal> CollectionByWeek { get; set; } = new();
}

public class YearlyRevenueReport
{
    public int Year { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal Q1Revenue { get; set; }
    public decimal Q2Revenue { get; set; }
    public decimal Q3Revenue { get; set; }
    public decimal Q4Revenue { get; set; }
    public decimal GrowthRate { get; set; }
    public Dictionary<string, decimal> RevenueByMonth { get; set; } = new();
}

public class OutstandingInvoicesReport
{
    public int TotalInvoices { get; set; }
    public decimal TotalOutstanding { get; set; }
    public int OverdueCount { get; set; }
    public decimal OverdueAmount { get; set; }
    public List<OutstandingInvoiceItem> Items { get; set; } = new();
}

public class OutstandingInvoiceItem
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal DueAmount { get; set; }
    public DateTime DueDate { get; set; }
    public int DaysOverdue { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class PaymentSuccessReport
{
    public int TotalSuccessful { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal AverageAmount { get; set; }
    public Dictionary<string, int> SuccessByGateway { get; set; } = new();
    public Dictionary<string, int> SuccessByMethod { get; set; } = new();
    public List<PaymentTransactionItem> Transactions { get; set; } = new();
}

public class FailedPaymentsReport
{
    public int TotalFailed { get; set; }
    public decimal TotalFailedAmount { get; set; }
    public Dictionary<string, int> FailureByReason { get; set; } = new();
    public Dictionary<string, int> FailureByGateway { get; set; } = new();
    public List<FailedTransactionItem> FailedTransactions { get; set; } = new();
}

public class PaymentTransactionItem
{
    public string TransactionId { get; set; } = string.Empty;
    public string OrderId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "INR";
    public string PaymentMethod { get; set; } = string.Empty;
    public string Gateway { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class FailedTransactionItem
{
    public string TransactionId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Gateway { get; set; } = string.Empty;
    public string FailureReason { get; set; } = string.Empty;
    public string FailureCode { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}

public class RefundReport
{
    public int TotalRefunds { get; set; }
    public decimal TotalRefundAmount { get; set; }
    public decimal AverageRefundAmount { get; set; }
    public Dictionary<string, int> RefundByReason { get; set; } = new();
    public List<RefundTransactionItem> Refunds { get; set; } = new();
}

public class RefundTransactionItem
{
    public string RefundId { get; set; } = string.Empty;
    public string OriginalTransactionId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime RequestedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public class SettlementReport
{
    public int TotalSettlements { get; set; }
    public decimal TotalSettlementAmount { get; set; }
    public decimal TotalFees { get; set; }
    public decimal NetAmount { get; set; }
    public Dictionary<string, decimal> SettlementByGateway { get; set; } = new();
    public List<SettlementItem> Settlements { get; set; } = new();
}

public class SettlementItem
{
    public string SettlementId { get; set; } = string.Empty;
    public string Gateway { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal Fee { get; set; }
    public decimal NetAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime InitiatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public class LedgerReport
{
    public decimal OpeningBalance { get; set; }
    public decimal ClosingBalance { get; set; }
    public decimal TotalDebits { get; set; }
    public decimal TotalCredits { get; set; }
    public List<LedgerEntry> Entries { get; set; } = new();
}

public class LedgerEntry
{
    public DateTime Date { get; set; }
    public string Reference { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
    public decimal Balance { get; set; }
    public string Category { get; set; } = string.Empty;
}

public class JournalReport
{
    public List<JournalEntry> Entries { get; set; } = new();
}

public class JournalEntry
{
    public string JournalId { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Description { get; set; } = string.Empty;
    public List<JournalLine> Lines { get; set; } = new();
}

public class JournalLine
{
    public string AccountCode { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
}

public class WalletTransactionsReport
{
    public int TotalTransactions { get; set; }
    public decimal TotalCredits { get; set; }
    public decimal TotalDebits { get; set; }
    public List<WalletTransactionItem> Transactions { get; set; } = new();
}

public class WalletTransactionItem
{
    public string WalletId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public TransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public decimal Balance { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}

public class CouponUsageReport
{
    public int TotalCouponsUsed { get; set; }
    public decimal TotalDiscountAmount { get; set; }
    public decimal AverageDiscountPercent { get; set; }
    public Dictionary<string, int> UsageByCoupon { get; set; } = new();
    public List<CouponUsageItem> Usage { get; set; } = new();
}

public class CouponUsageItem
{
    public string CouponCode { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int UsageCount { get; set; }
    public decimal TotalDiscount { get; set; }
    public decimal RevenueImpact { get; set; }
}

public class ScholarshipReport
{
    public int TotalScholarships { get; set; }
    public decimal TotalAmount { get; set; }
    public int ActiveCount { get; set; }
    public Dictionary<string, decimal> ScholarshipByType { get; set; } = new();
    public List<ScholarshipItem> Items { get; set; } = new();
}

public class ScholarshipItem
{
    public string ScholarshipId { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public string ScholarshipType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal UsedAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime AwardedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

public class TaxReport
{
    public decimal TotalTaxableAmount { get; set; }
    public decimal TotalTaxCollected { get; set; }
    public Dictionary<string, decimal> TaxByRate { get; set; } = new();
    public List<TaxLineItem> LineItems { get; set; } = new();
}

public class GstReport
{
    public decimal TotalTaxableValue { get; set; }
    public decimal TotalCgst { get; set; }
    public decimal TotalSgst { get; set; }
    public decimal TotalIgst { get; set; }
    public decimal TotalGst { get; set; }
    public decimal TotalCess { get; set; }
    public Dictionary<string, decimal> GstByHsn { get; set; } = new();
    public List<GstLineItem> LineItems { get; set; } = new();
}

public class TaxLineItem
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public decimal TaxableAmount { get; set; }
    public string HsnCode { get; set; } = string.Empty;
    public decimal TaxRate { get; set; }
    public decimal TaxAmount { get; set; }
    public string TaxType { get; set; } = string.Empty;
}

public class GstLineItem
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public decimal TaxableValue { get; set; }
    public string HsnCode { get; set; } = string.Empty;
    public decimal CgstAmount { get; set; }
    public decimal SgstAmount { get; set; }
    public decimal IgstAmount { get; set; }
    public decimal CessAmount { get; set; }
    public decimal TotalGst { get; set; }
    public string SupplyType { get; set; } = string.Empty;
}

public class AcademyRevenueReport
{
    public string AcademyId { get; set; } = string.Empty;
    public string AcademyName { get; set; } = string.Empty;
    public decimal TotalRevenue { get; set; }
    public decimal CommissionPaid { get; set; }
    public decimal NetRevenue { get; set; }
    public int StudentCount { get; set; }
    public int SessionCount { get; set; }
    public Dictionary<string, decimal> RevenueBySport { get; set; } = new();
}

public class CoachRevenueReport
{
    public string CoachId { get; set; } = string.Empty;
    public string CoachName { get; set; } = string.Empty;
    public decimal TotalEarnings { get; set; }
    public decimal CommissionDeducted { get; set; }
    public decimal NetPayout { get; set; }
    public int SessionCount { get; set; }
    public int StudentCount { get; set; }
    public Dictionary<string, decimal> EarningsByMonth { get; set; } = new();
}

public class AthletePaymentReport
{
    public string AthleteId { get; set; } = string.Empty;
    public string AthleteName { get; set; } = string.Empty;
    public decimal TotalPaid { get; set; }
    public decimal TotalRefunded { get; set; }
    public decimal NetSpend { get; set; }
    public int TransactionCount { get; set; }
    public List<AthletePaymentItem> Payments { get; set; } = new();
}

public class AthletePaymentItem
{
    public DateTime Date { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
}
