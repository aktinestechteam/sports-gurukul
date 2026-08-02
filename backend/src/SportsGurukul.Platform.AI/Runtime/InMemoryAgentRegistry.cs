using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.AI.Interfaces.Runtime;
using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Runtime;

public class InMemoryAgentRegistry : IAgentRegistry
{
    private readonly ConcurrentDictionary<string, AgentDefinition> _agents = new(StringComparer.OrdinalIgnoreCase);
    private readonly ILogger<InMemoryAgentRegistry> _logger;

    public InMemoryAgentRegistry(ILogger<InMemoryAgentRegistry>? logger = null)
    {
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<InMemoryAgentRegistry>.Instance;
    }

    public Task<AgentDefinition> RegisterAsync(AgentDefinition definition, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(definition.Name);
        cancellationToken.ThrowIfCancellationRequested();

        _agents[definition.Name] = definition;
        _logger.LogInformation("Registered agent '{Agent}'", definition.Name);
        return Task.FromResult(definition);
    }

    public Task<AgentDefinition?> GetAsync(string name, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_agents.TryGetValue(name, out var definition) ? definition : null);
    }

    public Task<IReadOnlyList<AgentDefinition>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult<IReadOnlyList<AgentDefinition>>(_agents.Values.ToList());
    }

    public Task<bool> UnregisterAsync(string name, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var removed = _agents.TryRemove(name, out _);
        if (removed)
        {
            _logger.LogInformation("Unregistered agent '{Agent}'", name);
        }

        return Task.FromResult(removed);
    }
}
