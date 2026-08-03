using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Common.Interfaces.AI;

public interface IAssistantRepository : IRepository<AIAssistant>
{
    Task<AIAssistant?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AIAssistant?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AIAssistant>> GetByOwnerAsync(Guid ownerUserId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AIAssistant>> GetByTypeAsync(AIAssistantType assistantType, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AIAssistant>> GetActiveAsync(CancellationToken cancellationToken = default);
}
