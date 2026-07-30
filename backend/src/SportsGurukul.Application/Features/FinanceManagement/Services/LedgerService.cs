using SportsGurukul.Application.Common.Interfaces.Finance;
using SportsGurukul.Application.Common.Interfaces.Finance.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Entities.Finance;
using SportsGurukul.Domain.Enums.Finance;

namespace SportsGurukul.Application.Features.FinanceManagement.Services;

public class LedgerService : ILedgerService
{
    private readonly ILedgerRepository _ledgerRepository;

    public LedgerService(ILedgerRepository ledgerRepository)
    {
        _ledgerRepository = ledgerRepository;
    }

    public async Task<Result<LedgerEntry>> CreateJournalEntryAsync(string accountCode, decimal debitAmount, decimal creditAmount, string? description, string? reference, CancellationToken cancellationToken)
    {
        var ledger = await _ledgerRepository.GetByCodeAsync(accountCode, cancellationToken);
        if (ledger is null)
            return Result<LedgerEntry>.Failure($"Ledger not found for code: {accountCode}");

        var entry = new LedgerEntry
        {
            DebitAmount = debitAmount,
            CreditAmount = creditAmount,
            Description = description,
            Reference = reference,
            EntryDate = DateTime.UtcNow,
        };

        return Result<LedgerEntry>.Success(entry);
    }

    public async Task<Result<bool>> PostJournalAsync(Journal journal, CancellationToken cancellationToken)
    {
        foreach (var entry in journal.Entries)
        {
            var ledger = await _ledgerRepository.GetByCodeAsync(entry.AccountCode, cancellationToken);
            if (ledger is null)
                return Result<bool>.Failure($"Ledger not found for account code: {entry.AccountCode}");
        }

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> PostLedgerEntryAsync(Ledger ledger, LedgerEntry entry, CancellationToken cancellationToken)
    {
        var existing = await _ledgerRepository.GetByIdWithEntriesAsync(ledger.Id, cancellationToken);
        if (existing is null)
            return Result<bool>.Failure("Ledger not found");

        entry.LedgerId = ledger.Id;
        entry.EntryDate = DateTime.UtcNow;
        existing.Entries.Add(entry);
        _ledgerRepository.Update(existing);

        return Result<bool>.Success(true);
    }

    public async Task<Result<Ledger>> GetOrCreateLedgerAsync(string code, string name, LedgerType type, string? description, CancellationToken cancellationToken)
    {
        var existing = await _ledgerRepository.GetByCodeAsync(code, cancellationToken);
        if (existing is not null)
            return Result<Ledger>.Success(existing);

        var ledger = new Ledger
        {
            Code = code,
            Name = name,
            Type = type,
            Description = description,
            IsActive = true,
        };

        var created = await _ledgerRepository.AddAsync(ledger, cancellationToken);
        return Result<Ledger>.Success(created);
    }
}
