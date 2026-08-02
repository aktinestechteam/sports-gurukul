using SportsGurukul.Application.Common.Interfaces.AI.Models;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Entities.AI;

namespace SportsGurukul.Application.Common.Interfaces.AI.Services;

public interface IAgentService
{
    Task<Result<AgentDefinition>> CreateAsync(CreateAgentRequest request, CancellationToken cancellationToken = default);
    Task<Result<AgentDefinition>> UpdateAsync(UpdateAgentRequest request, CancellationToken cancellationToken = default);
    Task<Result<AgentDefinition>> EnableAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<AgentDefinition>> DisableAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<bool>> AssignWorkflowAsync(Guid agentId, Guid workflowId, CancellationToken cancellationToken = default);
    Task<Result<AgentDefinition>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<AgentDefinition>>> SearchAsync(SearchAgentsRequest request, CancellationToken cancellationToken = default);
}
