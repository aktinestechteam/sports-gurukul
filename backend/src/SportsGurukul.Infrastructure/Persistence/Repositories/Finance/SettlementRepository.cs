using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces.Finance;
using SportsGurukul.Domain.Entities.Finance;

namespace SportsGurukul.Infrastructure.Persistence.Repositories.Finance;

public class SettlementRepository : Repository<SettlementBatch>, ISettlementRepository
{
    public SettlementRepository(ApplicationDbContext context) : base(context) { }

    public async Task<SettlementBatch?> GetByBatchNumberAsync(string batchNumber, CancellationToken cancellationToken = default)
    {
        return await Context.Set<SettlementBatch>()
            .AsNoTracking()
            .FirstOrDefaultAsync(sb => sb.BatchNumber == batchNumber, cancellationToken);
    }

    public async Task<SettlementBatch?> GetByIdWithSettlementsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<SettlementBatch>()
            .AsNoTracking()
            .Include(sb => sb.Settlements)
            .FirstOrDefaultAsync(sb => sb.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Settlement>> GetSettlementsByBatchIdAsync(Guid batchId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Settlement>()
            .AsNoTracking()
            .Where(s => s.SettlementBatchId == batchId)
            .ToListAsync(cancellationToken);
    }
}
