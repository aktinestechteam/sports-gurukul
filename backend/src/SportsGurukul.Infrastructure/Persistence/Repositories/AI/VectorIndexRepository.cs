using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Infrastructure.Persistence.Repositories.AI;

public class VectorIndexRepository : Repository<Domain.Entities.AI.VectorIndex>, IVectorIndexRepository
{
    public VectorIndexRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Domain.Entities.AI.VectorIndex?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.VectorIndex>()
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.AI.VectorIndex>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.VectorIndex>()
            .AsNoTracking()
            .Where(i => i.Status == VectorIndexStatus.Active)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.AI.VectorIndex>> GetByStatusAsync(string status, CancellationToken cancellationToken = default)
    {
        var indexStatus = Enum.Parse<VectorIndexStatus>(status);
        return await Context.Set<Domain.Entities.AI.VectorIndex>()
            .AsNoTracking()
            .Where(i => i.Status == indexStatus)
            .ToListAsync(cancellationToken);
    }
}
