using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Common.Interfaces.AI;

public interface IPromptRepository : IRepository<PromptTemplate>
{
    Task<PromptTemplate?> GetByIdWithVersionsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PromptTemplate?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PromptTemplate>> GetByAssistantIdAsync(Guid assistantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PromptTemplate>> GetActiveByAssistantAsync(Guid assistantId, CancellationToken cancellationToken = default);
    Task<PromptTemplate?> GetDefaultByAssistantAsync(Guid assistantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PromptTemplate>> GetByTypeAsync(AIPromptType promptType, CancellationToken cancellationToken = default);
}
