using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Entities.Finance;

namespace SportsGurukul.Application.Common.Interfaces.Finance.Services;

public interface ILedgerService
{
    Task<Result<LedgerEntry>> CreateJournalEntryAsync(string accountCode, decimal debitAmount, decimal creditAmount, string? description, string? reference, CancellationToken cancellationToken = default);
    Task<Result<bool>> PostJournalAsync(Journal journal, CancellationToken cancellationToken = default);
    Task<Result<bool>> PostLedgerEntryAsync(Ledger ledger, LedgerEntry entry, CancellationToken cancellationToken = default);
    Task<Result<Ledger>> GetOrCreateLedgerAsync(string code, string name, Domain.Enums.Finance.LedgerType type, string? description, CancellationToken cancellationToken = default);
}
