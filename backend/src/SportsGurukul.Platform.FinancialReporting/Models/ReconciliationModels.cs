namespace SportsGurukul.Platform.FinancialReporting.Models;

public class ReconciliationRequest
{
    public string ReferenceId { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public ReconciliationType Type { get; set; }
}

public enum ReconciliationType
{
    Bank, Gateway, Invoice, Settlement, Ledger
}

public class ReconciliationResult
{
    public string ReconciliationId { get; set; } = Guid.NewGuid().ToString("N");
    public ReconciliationType Type { get; set; }
    public ReconciliationStatus Status { get; set; }
    public DateTime PerformedAt { get; set; } = DateTime.UtcNow;
    public int TotalRecords { get; set; }
    public int MatchedRecords { get; set; }
    public int UnmatchedRecords { get; set; }
    public int DiscrepancyCount { get; set; }
    public decimal TotalDifference { get; set; }
    public List<ReconciliationDifference> Differences { get; set; } = new();
    public string? ReportUrl { get; set; }
}

public class ReconciliationDifference
{
    public string RecordId { get; set; } = string.Empty;
    public string SourceRecord { get; set; } = string.Empty;
    public string TargetRecord { get; set; } = string.Empty;
    public decimal ExpectedAmount { get; set; }
    public decimal ActualAmount { get; set; }
    public decimal Difference { get; set; }
    public string Description { get; set; } = string.Empty;
    public ReconciliationStatus Status { get; set; }
}

public class BankReconciliationResult
{
    public string BankStatementId { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
    public DateTime StatementPeriod { get; set; }
    public decimal OpeningBalance { get; set; }
    public decimal ClosingBalance { get; set; }
    public decimal SystemBalance { get; set; }
    public decimal Difference { get; set; }
    public List<BankTransactionItem> MatchedTransactions { get; set; } = new();
    public List<BankTransactionItem> UnmatchedBankTransactions { get; set; } = new();
    public List<BankTransactionItem> UnmatchedSystemTransactions { get; set; } = new();
}

public class BankTransactionItem
{
    public DateTime Date { get; set; }
    public string Reference { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
    public decimal Balance { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class GatewayReconciliationResult
{
    public string GatewayName { get; set; } = string.Empty;
    public int TotalGatewayTransactions { get; set; }
    public int TotalSystemTransactions { get; set; }
    public int MatchedTransactions { get; set; }
    public int UnmatchedTransactions { get; set; }
    public decimal TotalGatewayAmount { get; set; }
    public decimal TotalSystemAmount { get; set; }
    public decimal AmountDifference { get; set; }
    public List<GatewayTransactionDifference> Differences { get; set; } = new();
}

public class GatewayTransactionDifference
{
    public string GatewayTransactionId { get; set; } = string.Empty;
    public string SystemTransactionId { get; set; } = string.Empty;
    public decimal GatewayAmount { get; set; }
    public decimal SystemAmount { get; set; }
    public decimal Difference { get; set; }
    public string GatewayStatus { get; set; } = string.Empty;
    public string SystemStatus { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class InvoiceReconciliationResult
{
    public int TotalInvoices { get; set; }
    public int MatchedInvoices { get; set; }
    public int UnmatchedInvoices { get; set; }
    public decimal TotalInvoiceAmount { get; set; }
    public decimal TotalPaymentAmount { get; set; }
    public decimal Difference { get; set; }
    public List<InvoiceReconciliationItem> Items { get; set; } = new();
}

public class InvoiceReconciliationItem
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public decimal InvoiceAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal Difference { get; set; }
    public string PaymentReference { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public class LedgerReconciliationResult
{
    public decimal SystemBalance { get; set; }
    public decimal LedgerBalance { get; set; }
    public decimal Difference { get; set; }
    public List<LedgerDiscrepancy> Discrepancies { get; set; } = new();
}

public class LedgerDiscrepancy
{
    public string EntryId { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal SystemAmount { get; set; }
    public decimal LedgerAmount { get; set; }
    public decimal Difference { get; set; }
    public string Category { get; set; } = string.Empty;
}

public class ExceptionReport
{
    public string ReportId { get; set; } = Guid.NewGuid().ToString("N");
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public int TotalExceptions { get; set; }
    public List<ReconciliationException> Exceptions { get; set; } = new();
}

public class ReconciliationException
{
    public string ExceptionId { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Reference { get; set; } = string.Empty;
    public DateTime DetectedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Resolution { get; set; }
}
