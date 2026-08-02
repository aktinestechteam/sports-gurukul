using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.AI.Interfaces.Tools;
using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Services;

public class ToolService : IToolService
{
    private readonly IToolRegistry _registry;
    private readonly IToolExecutor _executor;
    private readonly ILogger<ToolService> _logger;

    public ToolService(
        IToolRegistry registry,
        IToolExecutor executor,
        ILogger<ToolService>? logger = null)
    {
        _registry = registry;
        _executor = executor;
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<ToolService>.Instance;
    }

    public Task<ITool> RegisterAsync(ITool tool, CancellationToken cancellationToken = default) =>
        _registry.RegisterAsync(tool, cancellationToken);

    public Task<bool> UnregisterAsync(string name, CancellationToken cancellationToken = default) =>
        _registry.UnregisterAsync(name, cancellationToken);

    public async Task<IReadOnlyList<ToolDescriptor>> ListAsync(CancellationToken cancellationToken = default)
    {
        var tools = await _registry.GetAllAsync(cancellationToken);
        return tools.Select(ToolDescriptor.From).ToList();
    }

    public async Task<ToolDescriptor?> DescribeAsync(string name, CancellationToken cancellationToken = default)
    {
        var tool = await _registry.GetAsync(name, cancellationToken);
        return tool is null ? null : ToolDescriptor.From(tool);
    }

    public Task<ToolResult> ExecuteAsync(string toolName, IDictionary<string, object?> arguments, ToolExecutionContext context, CancellationToken cancellationToken = default) =>
        _executor.ExecuteAsync(toolName, arguments, context, cancellationToken);
}
