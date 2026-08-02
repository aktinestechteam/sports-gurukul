using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.AI.Interfaces.Runtime;
using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Services;

public class AgentService : IAgentService
{
    private readonly IAgentRegistry _registry;
    private readonly IAgentRuntime _runtime;
    private readonly IAgentLifecycleService _lifecycle;
    private readonly ILogger<AgentService> _logger;

    public AgentService(
        IAgentRegistry registry,
        IAgentRuntime runtime,
        IAgentLifecycleService lifecycle,
        ILogger<AgentService>? logger = null)
    {
        _registry = registry;
        _runtime = runtime;
        _lifecycle = lifecycle;
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<AgentService>.Instance;
    }

    public Task<AgentDefinition> RegisterAsync(AgentDefinition definition, CancellationToken cancellationToken = default) =>
        _registry.RegisterAsync(definition, cancellationToken);

    public Task<AgentDefinition?> GetAsync(string name, CancellationToken cancellationToken = default) =>
        _registry.GetAsync(name, cancellationToken);

    public Task<IReadOnlyList<AgentDefinition>> ListAsync(CancellationToken cancellationToken = default) =>
        _registry.GetAllAsync(cancellationToken);

    public Task<AgentRunResult> RunAsync(AgentRunRequest request, CancellationToken cancellationToken = default) =>
        _runtime.RunAsync(request, cancellationToken);

    public Task<AgentRunResult?> GetRunAsync(Guid runId, CancellationToken cancellationToken = default) =>
        _runtime.GetRunAsync(runId, cancellationToken);

    public Task<bool> CancelAsync(Guid runId, CancellationToken cancellationToken = default) =>
        _runtime.CancelAsync(runId, cancellationToken);

    public Task<IReadOnlyList<AgentSession>> GetSessionsAsync(CancellationToken cancellationToken = default) =>
        _lifecycle.GetActiveSessionsAsync(cancellationToken);
}
