using SportsGurukul.Domain.Entities.AI;

namespace SportsGurukul.Application.Common.Interfaces.AI;

public interface IAgentExecutionRepository : IRepository<AgentExecution>
{
    Task<AgentExecution?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AgentExecution>> GetByAgentDefinitionIdAsync(Guid agentDefinitionId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AgentExecution>> GetByStatusAsync(string status, CancellationToken cancellationToken = default);
}
