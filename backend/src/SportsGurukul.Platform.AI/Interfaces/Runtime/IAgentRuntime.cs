using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Interfaces.Runtime;

public interface IAgentRuntime
{
    Task<AgentRunResult> RunAsync(string agentId, string goal, AgentRunOptions? options = null, CancellationToken cancellationToken = default);

    Task<AgentRunResult> RunAsync(AgentRunRequest request, CancellationToken cancellationToken = default);

    Task<AgentRunResult?> GetRunAsync(Guid runId, CancellationToken cancellationToken = default);

    Task<bool> CancelAsync(Guid runId, CancellationToken cancellationToken = default);
}
