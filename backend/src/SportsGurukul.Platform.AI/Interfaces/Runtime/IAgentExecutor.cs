using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Interfaces.Runtime;

public interface IAgentExecutor
{
    Task<AgentRunResult> ExecuteAsync(AgentRunRequest request, CancellationToken cancellationToken = default);
}
