using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Interfaces.Workflow;

public interface IWorkflowStore
{
    Task<WorkflowExecution> SaveAsync(WorkflowExecution execution, CancellationToken cancellationToken = default);

    Task<WorkflowExecution?> GetAsync(Guid executionId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<WorkflowExecution>> ListAsync(Guid? tenantId = null, CancellationToken cancellationToken = default);

    Task SaveCheckpointAsync(WorkflowCheckpoint checkpoint, CancellationToken cancellationToken = default);

    Task<WorkflowCheckpoint?> GetCheckpointAsync(Guid executionId, CancellationToken cancellationToken = default);
}
