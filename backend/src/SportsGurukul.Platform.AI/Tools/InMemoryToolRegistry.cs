using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.AI.Interfaces.Tools;
using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Tools;

public class InMemoryToolRegistry : IToolRegistry
{
    private readonly ConcurrentDictionary<string, ITool> _tools = new(StringComparer.OrdinalIgnoreCase);
    private readonly ILogger<InMemoryToolRegistry> _logger;

    public InMemoryToolRegistry(IEnumerable<ITool>? builtInTools = null, ILogger<InMemoryToolRegistry>? logger = null)
    {
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<InMemoryToolRegistry>.Instance;

        if (builtInTools is not null)
        {
            foreach (var tool in builtInTools)
            {
                if (tool is not null)
                {
                    _tools[tool.Name] = tool;
                }
            }
        }
    }

    public Task<ITool> RegisterAsync(ITool tool, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(tool);
        cancellationToken.ThrowIfCancellationRequested();

        _tools[tool.Name] = tool;
        _logger.LogInformation("Registered tool '{Tool}' (type {Type})", tool.Name, tool.Type);
        return Task.FromResult(tool);
    }

    public Task<ITool?> GetAsync(string name, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_tools.TryGetValue(name, out var tool) ? tool : null);
    }

    public Task<IReadOnlyList<ITool>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult<IReadOnlyList<ITool>>(_tools.Values.ToList());
    }

    public Task<IReadOnlyList<ITool>> GetByTypeAsync(ToolType type, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult<IReadOnlyList<ITool>>(_tools.Values.Where(t => t.Type == type).ToList());
    }

    public Task<bool> UnregisterAsync(string name, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var removed = _tools.TryRemove(name, out _);
        if (removed)
        {
            _logger.LogInformation("Unregistered tool '{Tool}'", name);
        }

        return Task.FromResult(removed);
    }
}
