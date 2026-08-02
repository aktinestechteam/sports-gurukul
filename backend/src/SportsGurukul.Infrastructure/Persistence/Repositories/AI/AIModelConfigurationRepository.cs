using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces.AI;

namespace SportsGurukul.Infrastructure.Persistence.Repositories.AI;

public class AIModelConfigurationRepository : Repository<Domain.Entities.AI.AIModelConfiguration>, IAIModelConfigurationRepository
{
    public AIModelConfigurationRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Domain.Entities.AI.AIModelConfiguration?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.AIModelConfiguration>()
            .AsNoTracking()
            .Include(c => c.Model)
            .AsSplitQuery()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.AI.AIModelConfiguration>> GetByModelIdAsync(Guid modelId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.AIModelConfiguration>()
            .AsNoTracking()
            .Where(c => c.ModelId == modelId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Domain.Entities.AI.AIModelConfiguration?> GetDefaultForModelAsync(Guid modelId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.AIModelConfiguration>()
            .AsNoTracking()
            .Where(c => c.ModelId == modelId && c.IsDefault)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
