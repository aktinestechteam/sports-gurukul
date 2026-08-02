using SportsGurukul.Application.Common.Interfaces.AI.Models;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Entities.AI;

namespace SportsGurukul.Application.Common.Interfaces.AI.Services;

public interface IWorkflowService
{
    Task<Result<WorkflowDefinition>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<WorkflowDefinition>>> SearchAsync(SearchWorkflowsRequest request, CancellationToken cancellationToken = default);
}
