using SportsGurukul.Domain.Entities.AI;

namespace SportsGurukul.Application.Common.Interfaces.AI;

public interface IWorkflowDefinitionRepository : IRepository<WorkflowDefinition>
{
    Task<WorkflowDefinition?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WorkflowDefinition>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WorkflowDefinition>> GetByStatusAsync(string status, CancellationToken cancellationToken = default);
}
