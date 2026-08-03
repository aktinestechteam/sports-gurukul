using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Infrastructure.Persistence.Repositories.AI;

public class VectorIndexRepository : Repository<VectorIndex>, IVectorIndexRepository
{
    public VectorIndexRepository(ApplicationDbContext context) : base(context) { }

    public async Task<VectorIndex?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await Context.Set<VectorIndex>()
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Name == name, cancellationToken);
    }

    public async Task<IReadOnlyList<VectorIndex>> GetByProviderAsync(AIVectorIndexProvider provider, CancellationToken cancellationToken = default)
    {
        return await Context.Set<VectorIndex>()
            .AsNoTracking()
            .Where(v => v.Provider == provider)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<VectorIndex>> GetByStatusAsync(AIVectorIndexStatus status, CancellationToken cancellationToken = default)
    {
        return await Context.Set<VectorIndex>()
            .AsNoTracking()
            .Where(v => v.Status == status)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<VectorIndex>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Set<VectorIndex>()
            .AsNoTracking()
            .Where(v => v.IsActive)
            .OrderBy(v => v.Name)
            .ToListAsync(cancellationToken);
    }
}
