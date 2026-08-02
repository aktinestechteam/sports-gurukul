using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Common.Interfaces.AI.ModelRouting;

public interface IRoutingPolicyService
{
    Task<Result<AIRoutingPolicy>> GetActivePolicyAsync(CancellationToken cancellationToken = default);
    Task<Result<AIRoutingPolicy>> GetPolicyByStrategyAsync(RoutingStrategy strategy, CancellationToken cancellationToken = default);
}
