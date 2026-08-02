using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Interfaces.Workflow;

public interface IWorkflowService
{
    Task<WorkflowDefinition> RegisterAsync(WorkflowDefinition definition, CancellationToken cancellationToken = default);

    Task<WorkflowDefinition?> GetAsync(string name, int? version = null, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<WorkflowDefinition>> ListDefinitionsAsync(CancellationToken cancellationToken = default);

    Task<WorkflowExecution> StartAsync(string name, int? version, WorkflowStartOptions? options = null, CancellationToken cancellationToken = default);

    Task<WorkflowExecution> StartAsync(WorkflowDefinition definition, WorkflowStartOptions? options = null, CancellationToken cancellationToken = default);

    Task<WorkflowExecution> ResumeAsync(Guid executionId, CancellationToken cancellationToken = default);

    Task<WorkflowExecution> CancelAsync(Guid executionId, string? reason = null, CancellationToken cancellationToken = default);

    Task<WorkflowExecution?> GetExecutionAsync(Guid executionId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<WorkflowExecution>> ListExecutionsAsync(Guid? tenantId = null, CancellationToken cancellationToken = default);
}
