using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.PaymentGateway.Models;

namespace SportsGurukul.Platform.PaymentGateway.Accounting;

public class AccountingService : IAccountingService
{
    private readonly ILogger<AccountingService> _logger;

    public AccountingService(ILogger<AccountingService> logger)
    {
        _logger = logger;
    }

    public Task UpdateLedgerAsync(
        LedgerEntryRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Ledger updated: {LedgerCode} | Debit: {Debit} | Credit: {Credit} | Ref: {Reference}",
            request.LedgerCode, request.DebitAmount, request.CreditAmount, request.Reference);

        return Task.CompletedTask;
    }

    public Task CreateJournalEntryAsync(
        JournalEntryRequest request,
        CancellationToken cancellationToken = default)
    {
        var totalDebits = request.Lines.Sum(l => l.DebitAmount);
        var totalCredits = request.Lines.Sum(l => l.CreditAmount);

        if (Math.Abs(totalDebits - totalCredits) > 0.01m)
        {
            _logger.LogWarning(
                "Unbalanced journal entry {JournalNumber}: Debits={TotalDebits}, Credits={TotalCredits}",
                request.JournalNumber, totalDebits, totalCredits);
        }

        _logger.LogInformation(
            "Journal entry created: {JournalNumber} | Lines: {LineCount} | Total: {Total}",
            request.JournalNumber, request.Lines.Count, totalDebits);

        return Task.CompletedTask;
    }

    public Task EnqueueSettlementAsync(
        SettlementEntryRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Settlement enqueued: Batch {BatchNumber} | Provider: {Provider} | Amount: {Amount} | Transactions: {Count}",
            request.BatchNumber, request.GatewayProvider, request.TotalAmount, request.TransactionCount);

        return Task.CompletedTask;
    }

    public Task RecognizeRevenueAsync(
        RevenueRecognitionRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Revenue recognized: Invoice {InvoiceNumber} | Amount: {Amount} | Date: {RecognitionDate}",
            request.InvoiceNumber, request.Amount, request.RecognitionDate);

        var journalRequest = new JournalEntryRequest
        {
            JournalNumber = $"REV-{request.InvoiceNumber}",
            JournalDate = request.RecognitionDate,
            Description = $"Revenue recognition for invoice {request.InvoiceNumber}",
            Period = $"{request.RecognitionDate:yyyy-MM}",
            ReferenceType = "Invoice",
            ReferenceId = request.InvoiceNumber,
            Lines =
            [
                new JournalLine
                {
                    AccountCode = request.RevenueAccountCode,
                    Description = $"Revenue - {request.InvoiceNumber}",
                    DebitAmount = 0,
                    CreditAmount = request.Amount
                },
                new JournalLine
                {
                    AccountCode = request.DeferredRevenueAccountCode,
                    Description = $"Release from deferred revenue - {request.InvoiceNumber}",
                    DebitAmount = request.Amount,
                    CreditAmount = 0
                }
            ]
        };

        _ = CreateJournalEntryAsync(journalRequest, cancellationToken);

        return Task.CompletedTask;
    }

    public Task<LedgerBalanceResult> GetLedgerBalanceAsync(
        string ledgerCode,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new LedgerBalanceResult
        {
            LedgerCode = ledgerCode,
            LedgerName = ledgerCode,
            TotalDebits = 0,
            TotalCredits = 0,
            Balance = 0,
            AsOfDate = DateTime.UtcNow
        });
    }
}
