using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.AI.Interfaces.Workflow;
using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Services;

public class WorkflowService : IWorkflowService
{
    private readonly ConcurrentDictionary<string, WorkflowDefinition> _definitions = new(StringComparer.OrdinalIgnoreCase);
    private readonly IWorkflowEngine _engine;
    private readonly ILogger<WorkflowService> _logger;

    public WorkflowService(IWorkflowEngine engine, ILogger<WorkflowService>? logger = null)
    {
        _engine = engine;
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<WorkflowService>.Instance;
    }

    public Task<WorkflowDefinition> RegisterAsync(WorkflowDefinition definition, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(definition);
        if (string.IsNullOrWhiteSpace(definition.Name))
        {
            throw new AgentPlatformException("Workflow name is required.", "WORKFLOW_INVALID");
        }

        if (definition.Version <= 0)
        {
            throw new AgentPlatformException("Workflow version must be positive.", "WORKFLOW_INVALID");
        }

        _definitions[Key(definition.Name, definition.Version)] = definition;
        _logger.LogInformation("Registered workflow '{Name}' v{Version}", definition.Name, definition.Version);
        return Task.FromResult(definition);
    }

    public Task<WorkflowDefinition?> GetAsync(string name, int? version = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        WorkflowDefinition? result;
        if (version is not null)
        {
            result = _definitions.TryGetValue(Key(name, version.Value), out var match) ? match : null;
        }
        else
        {
            result = _definitions.Values
                .Where(d => d.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(d => d.Version)
                .FirstOrDefault();
        }

        return Task.FromResult(result);
    }

    public Task<IReadOnlyList<WorkflowDefinition>> ListDefinitionsAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult<IReadOnlyList<WorkflowDefinition>>(_definitions.Values.ToList());
    }

    public async Task<WorkflowExecution> StartAsync(string name, int? version, WorkflowStartOptions? options = null, CancellationToken cancellationToken = default)
    {
        var definition = await GetAsync(name, version, cancellationToken)
            ?? throw new WorkflowNotFoundException(name);

        return await _engine.StartAsync(definition, options, cancellationToken);
    }

    public async Task<WorkflowExecution> StartAsync(WorkflowDefinition definition, WorkflowStartOptions? options = null, CancellationToken cancellationToken = default)
    {
        await RegisterAsync(definition, cancellationToken);
        return await _engine.StartAsync(definition, options, cancellationToken);
    }

    public Task<WorkflowExecution> ResumeAsync(Guid executionId, CancellationToken cancellationToken = default) =>
        _engine.ResumeAsync(executionId, cancellationToken);

    public Task<WorkflowExecution> CancelAsync(Guid executionId, string? reason = null, CancellationToken cancellationToken = default) =>
        _engine.CancelAsync(executionId, reason, cancellationToken);

    public Task<WorkflowExecution?> GetExecutionAsync(Guid executionId, CancellationToken cancellationToken = default) =>
        _engine.GetAsync(executionId, cancellationToken);

    public Task<IReadOnlyList<WorkflowExecution>> ListExecutionsAsync(Guid? tenantId = null, CancellationToken cancellationToken = default) =>
        _engine.ListAsync(tenantId, cancellationToken);

    private static string Key(string name, int version) => $"{name}@{version}";
}
