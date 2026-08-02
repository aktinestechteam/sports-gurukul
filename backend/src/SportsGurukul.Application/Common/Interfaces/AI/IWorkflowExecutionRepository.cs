using SportsGurukul.Domain.Entities.AI;

namespace SportsGurukul.Application.Common.Interfaces.AI;

public interface IWorkflowExecutionRepository : IRepository<WorkflowExecution>
{
    Task<WorkflowExecution?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WorkflowExecution>> GetByWorkflowDefinitionIdAsync(Guid workflowDefinitionId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WorkflowExecution>> GetByStatusAsync(string status, CancellationToken cancellationToken = default);
}
