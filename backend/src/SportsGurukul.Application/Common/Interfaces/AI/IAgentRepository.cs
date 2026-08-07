using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Common.Interfaces.AI;

public interface IAgentRepository : IRepository<AgentDefinition>
{
    Task<AgentDefinition?> GetByIdWithToolsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AgentDefinition?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AgentDefinition>> GetByWorkflowAsync(Guid workflowId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AgentDefinition>> GetByTypeAsync(AIAgentType agentType, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AgentDefinition>> GetActiveAsync(CancellationToken cancellationToken = default);
}
