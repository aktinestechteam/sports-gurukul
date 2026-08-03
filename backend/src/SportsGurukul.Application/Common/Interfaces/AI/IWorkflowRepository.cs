using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Common.Interfaces.AI;

public interface IWorkflowRepository : IRepository<WorkflowDefinition>
{
    Task<WorkflowDefinition?> GetByIdWithAgentsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<WorkflowDefinition?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WorkflowDefinition>> GetByTypeAsync(AIWorkflowType workflowType, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WorkflowDefinition>> GetPublishedAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WorkflowDefinition>> GetActiveAsync(CancellationToken cancellationToken = default);
}
