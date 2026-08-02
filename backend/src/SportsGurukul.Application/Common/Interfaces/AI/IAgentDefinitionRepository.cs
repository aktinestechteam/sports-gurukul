using SportsGurukul.Domain.Entities.AI;

namespace SportsGurukul.Application.Common.Interfaces.AI;

public interface IAgentDefinitionRepository : IRepository<AgentDefinition>
{
    Task<AgentDefinition?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AgentDefinition>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AgentDefinition>> GetByAssistantIdAsync(Guid assistantId, CancellationToken cancellationToken = default);
}
