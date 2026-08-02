using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.AI.Interfaces.Runtime;
using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Runtime;

public class AgentRuntime : IAgentRuntime
{
    private readonly IAgentExecutor _executor;
    private readonly IAgentLifecycleService _lifecycle;
    private readonly ILogger<AgentRuntime> _logger;

    public AgentRuntime(
        IAgentExecutor executor,
        IAgentLifecycleService lifecycle,
        ILogger<AgentRuntime>? logger = null)
    {
        _executor = executor;
        _lifecycle = lifecycle;
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<AgentRuntime>.Instance;
    }

    public Task<AgentRunResult> RunAsync(string agentId, string goal, AgentRunOptions? options = null, CancellationToken cancellationToken = default) =>
        RunAsync(new AgentRunRequest { AgentId = agentId, Goal = goal, Options = options }, cancellationToken);

    public async Task<AgentRunResult> RunAsync(AgentRunRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Runtime dispatching run for agent '{Agent}'", request.AgentId);
        return await _executor.ExecuteAsync(request, cancellationToken);
    }

    public Task<AgentRunResult?> GetRunAsync(Guid runId, CancellationToken cancellationToken = default) =>
        _lifecycle.GetResultAsync(runId, cancellationToken);

    public async Task<bool> CancelAsync(Guid runId, CancellationToken cancellationToken = default)
    {
        var session = await _lifecycle.GetSessionAsync(runId, cancellationToken);
        if (session is null)
        {
            return false;
        }

        await _lifecycle.CancelAsync(runId, "Cancelled by user", cancellationToken);
        return true;
    }
}
