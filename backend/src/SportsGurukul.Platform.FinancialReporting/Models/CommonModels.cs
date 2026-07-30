namespace SportsGurukul.Platform.FinancialReporting.Models;

public enum FinancialEntityType
{
    Academy, Coach, Athlete, Parent, Sponsor, Tournament, Platform
}

public enum TransactionType
{
    Payment, Refund, Settlement, Fee, Commission, Discount, Scholarship, WalletCredit, WalletDebit
}

public enum InvoiceStatus
{
    Draft, Sent, Paid, Overdue, Cancelled, Refunded
}

public enum PaymentStatus
{
    Created, Authorized, Captured, Failed, Refunded, PartiallyRefunded, Disputed
}

public enum SettlementStatus
{
    Pending, Initiated, Completed, Failed, Disputed
}

public enum ReconciliationStatus
{
    Matched, Unmatched, Discrepancy, Pending, Exception
}

public enum ReportFormat
{
    Excel, Csv, Pdf
}

public enum ReportType
{
    Revenue, DailyCollection, MonthlyCollection, YearlyRevenue,
    OutstandingInvoices, PaymentSuccess, FailedPayments, Refund,
    Settlement, Ledger, Journal, WalletTransactions,
    CouponUsage, Scholarship, Tax, Gst,
    AcademyRevenue, CoachRevenue, AthletePayment
}
