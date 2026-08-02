using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Interfaces.Runtime;

public interface IAgentLifecycleService
{
    Task<AgentSession> StartAsync(AgentRunRequest request, CancellationToken cancellationToken = default);

    Task CompleteAsync(Guid runId, AgentRunResult result, CancellationToken cancellationToken = default);

    Task FailAsync(Guid runId, string reason, CancellationToken cancellationToken = default);

    Task CancelAsync(Guid runId, string? reason = null, CancellationToken cancellationToken = default);

    Task PauseAsync(Guid runId, CancellationToken cancellationToken = default);

    Task<AgentSession?> GetSessionAsync(Guid runId, CancellationToken cancellationToken = default);

    Task<AgentRunResult?> GetResultAsync(Guid runId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AgentSession>> GetActiveSessionsAsync(CancellationToken cancellationToken = default);
}
