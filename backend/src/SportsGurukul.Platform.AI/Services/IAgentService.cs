using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Services;

public interface IAgentService
{
    Task<AgentDefinition> RegisterAsync(AgentDefinition definition, CancellationToken cancellationToken = default);

    Task<AgentDefinition?> GetAsync(string name, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AgentDefinition>> ListAsync(CancellationToken cancellationToken = default);

    Task<AgentRunResult> RunAsync(AgentRunRequest request, CancellationToken cancellationToken = default);

    Task<AgentRunResult?> GetRunAsync(Guid runId, CancellationToken cancellationToken = default);

    Task<bool> CancelAsync(Guid runId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AgentSession>> GetSessionsAsync(CancellationToken cancellationToken = default);
}
