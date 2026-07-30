using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces.Finance;
using SportsGurukul.Domain.Entities.Finance;

namespace SportsGurukul.Infrastructure.Persistence.Repositories.Finance;

public class LedgerRepository : Repository<Ledger>, ILedgerRepository
{
    public LedgerRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Ledger?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Ledger>()
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Code == code, cancellationToken);
    }

    public async Task<Ledger?> GetByIdWithEntriesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Ledger>()
            .AsNoTracking()
            .Include(l => l.Entries)
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Ledger>> GetActiveLedgersAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Set<Ledger>()
            .AsNoTracking()
            .Where(l => l.IsActive)
            .ToListAsync(cancellationToken);
    }
}
