using SportsGurukul.Domain.Entities.AI;

namespace SportsGurukul.Application.Common.Interfaces.AI;

public interface IAIAssistantRepository : IRepository<AIAssistant>
{
    Task<AIAssistant?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AIAssistant>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AIAssistant>> GetByTypeAsync(string assistantType, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AIAssistant>> GetPublicAsync(CancellationToken cancellationToken = default);
}
