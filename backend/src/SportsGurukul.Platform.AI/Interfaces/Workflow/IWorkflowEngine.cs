using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Interfaces.Workflow;

public interface IWorkflowEngine
{
    Task<WorkflowExecution> StartAsync(WorkflowDefinition definition, WorkflowStartOptions? options = null, CancellationToken cancellationToken = default);

    Task<WorkflowExecution> ResumeAsync(Guid executionId, CancellationToken cancellationToken = default);

    Task<WorkflowExecution> CancelAsync(Guid executionId, string? reason = null, CancellationToken cancellationToken = default);

    Task<WorkflowExecution?> GetAsync(Guid executionId, CancellationToken cancellationToken = default);

    Task<WorkflowCheckpoint> CheckpointAsync(Guid executionId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<WorkflowExecution>> ListAsync(Guid? tenantId = null, CancellationToken cancellationToken = default);
}
