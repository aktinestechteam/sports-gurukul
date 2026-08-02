using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Infrastructure.Persistence.Repositories.AI;

public class AIModelRepository : Repository<Domain.Entities.AI.AIModel>, IAIModelRepository
{
    public AIModelRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Domain.Entities.AI.AIModel?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.AIModel>()
            .AsNoTracking()
            .Include(m => m.Provider)
            .Include(m => m.ModelConfigurations)
            .AsSplitQuery()
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.AI.AIModel>> GetByProviderAsync(Guid providerId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.AIModel>()
            .AsNoTracking()
            .Where(m => m.ProviderId == providerId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.AI.AIModel>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.AIModel>()
            .AsNoTracking()
            .Where(m => m.Status == AIModelStatus.Active)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.AI.AIModel>> GetByCapabilityAsync(string capability, CancellationToken cancellationToken = default)
    {
        var cap = Enum.Parse<AIModelCapability>(capability);
        return await Context.Set<Domain.Entities.AI.AIModel>()
            .AsNoTracking()
            .Where(m => m.Capabilities.HasFlag(cap))
            .ToListAsync(cancellationToken);
    }
}
