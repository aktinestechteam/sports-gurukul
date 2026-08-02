using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.AI.Interfaces.Workflow;
using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Workflow;

public class InMemoryWorkflowStore : IWorkflowStore
{
    private readonly ConcurrentDictionary<Guid, WorkflowExecution> _executions = new();
    private readonly ConcurrentDictionary<Guid, WorkflowCheckpoint> _checkpoints = new();
    private readonly ILogger<InMemoryWorkflowStore> _logger;

    public InMemoryWorkflowStore(ILogger<InMemoryWorkflowStore>? logger = null)
    {
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<InMemoryWorkflowStore>.Instance;
    }

    public Task<WorkflowExecution> SaveAsync(WorkflowExecution execution, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        execution.Revision++;
        _executions[execution.Id] = execution;
        return Task.FromResult(execution);
    }

    public Task<WorkflowExecution?> GetAsync(Guid executionId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_executions.TryGetValue(executionId, out var execution) ? execution : null);
    }

    public Task<IReadOnlyList<WorkflowExecution>> ListAsync(Guid? tenantId = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var executions = tenantId is null
            ? _executions.Values.ToList()
            : _executions.Values.Where(e => e.TenantId == tenantId.ToString()).ToList();
        return Task.FromResult<IReadOnlyList<WorkflowExecution>>(executions);
    }

    public Task SaveCheckpointAsync(WorkflowCheckpoint checkpoint, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _checkpoints[checkpoint.ExecutionId] = checkpoint;
        return Task.CompletedTask;
    }

    public Task<WorkflowCheckpoint?> GetCheckpointAsync(Guid executionId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_checkpoints.TryGetValue(executionId, out var checkpoint) ? checkpoint : null);
    }
}
