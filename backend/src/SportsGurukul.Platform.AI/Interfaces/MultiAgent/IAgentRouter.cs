using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Interfaces.MultiAgent;

public interface IAgentRouter
{
    Task<AgentRoutingDecision> RouteAsync(DelegatedTask task, IReadOnlyList<IWorkerAgent> workers, CancellationToken cancellationToken = default);
}
