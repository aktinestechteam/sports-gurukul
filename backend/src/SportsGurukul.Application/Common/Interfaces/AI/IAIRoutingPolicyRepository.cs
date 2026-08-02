using SportsGurukul.Domain.Entities.AI;

namespace SportsGurukul.Application.Common.Interfaces.AI;

public interface IAIRoutingPolicyRepository : IRepository<AIRoutingPolicy>
{
    Task<AIRoutingPolicy?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AIRoutingPolicy>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AIRoutingPolicy>> GetByStrategyAsync(string strategy, CancellationToken cancellationToken = default);
}
