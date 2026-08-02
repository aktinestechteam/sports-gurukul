using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Infrastructure.Persistence.Repositories.AI;

public class AIRoutingPolicyRepository : Repository<Domain.Entities.AI.AIRoutingPolicy>, IAIRoutingPolicyRepository
{
    public AIRoutingPolicyRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Domain.Entities.AI.AIRoutingPolicy?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.AIRoutingPolicy>()
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.AI.AIRoutingPolicy>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.AIRoutingPolicy>()
            .AsNoTracking()
            .Where(p => p.Status == RoutingStatus.Active)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.AI.AIRoutingPolicy>> GetByStrategyAsync(string strategy, CancellationToken cancellationToken = default)
    {
        var routingStrategy = Enum.Parse<RoutingStrategy>(strategy);
        return await Context.Set<Domain.Entities.AI.AIRoutingPolicy>()
            .AsNoTracking()
            .Where(p => p.Strategy == routingStrategy)
            .ToListAsync(cancellationToken);
    }
}
