using SportsGurukul.Platform.PaymentGateway.Models;

namespace SportsGurukul.Platform.PaymentGateway.Accounting;

public interface IAccountingService
{
    Task UpdateLedgerAsync(
        LedgerEntryRequest request,
        CancellationToken cancellationToken = default);

    Task CreateJournalEntryAsync(
        JournalEntryRequest request,
        CancellationToken cancellationToken = default);

    Task EnqueueSettlementAsync(
        SettlementEntryRequest request,
        CancellationToken cancellationToken = default);

    Task RecognizeRevenueAsync(
        RevenueRecognitionRequest request,
        CancellationToken cancellationToken = default);

    Task<LedgerBalanceResult> GetLedgerBalanceAsync(
        string ledgerCode,
        CancellationToken cancellationToken = default);
}

public class LedgerEntryRequest
{
    public string LedgerCode { get; set; } = string.Empty;
    public string LedgerName { get; set; } = string.Empty;
    public string EntryType { get; set; } = string.Empty;
    public decimal DebitAmount { get; set; }
    public decimal CreditAmount { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
    public string? ReferenceType { get; set; }
    public DateTime EntryDate { get; set; } = DateTime.UtcNow;
    public Dictionary<string, string>? Metadata { get; set; }
}

public class JournalEntryRequest
{
    public string JournalNumber { get; set; } = string.Empty;
    public DateTime JournalDate { get; set; } = DateTime.UtcNow;
    public string Description { get; set; } = string.Empty;
    public string Period { get; set; } = string.Empty;
    public List<JournalLine> Lines { get; set; } = [];
    public string? ReferenceType { get; set; }
    public string? ReferenceId { get; set; }
}

public class JournalLine
{
    public string AccountCode { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal DebitAmount { get; set; }
    public decimal CreditAmount { get; set; }
}

public class SettlementEntryRequest
{
    public string BatchNumber { get; set; } = string.Empty;
    public string GatewayProvider { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public int TransactionCount { get; set; }
    public DateTime SettlementDate { get; set; } = DateTime.UtcNow;
    public List<string> PaymentReferences { get; set; } = [];
    public Dictionary<string, string>? Metadata { get; set; }
}

public class RevenueRecognitionRequest
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime RecognitionDate { get; set; } = DateTime.UtcNow;
    public string RevenueAccountCode { get; set; } = string.Empty;
    public string DeferredRevenueAccountCode { get; set; } = string.Empty;
    public string? ContractId { get; set; }
    public string? PerformanceObligation { get; set; }
}

public class LedgerBalanceResult
{
    public string LedgerCode { get; set; } = string.Empty;
    public string LedgerName { get; set; } = string.Empty;
    public decimal TotalDebits { get; set; }
    public decimal TotalCredits { get; set; }
    public decimal Balance { get; set; }
    public DateTime AsOfDate { get; set; } = DateTime.UtcNow;
}
