using SportsGurukul.Domain.Entities.Finance;

namespace SportsGurukul.Application.Common.Interfaces.Finance;

public interface ILedgerRepository : IRepository<Ledger>
{
    Task<Ledger?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<Ledger?> GetByIdWithEntriesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Ledger>> GetActiveLedgersAsync(CancellationToken cancellationToken = default);
}
