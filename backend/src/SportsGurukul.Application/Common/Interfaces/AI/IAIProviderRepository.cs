using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Common.Interfaces.AI;

public interface IAIProviderRepository : IRepository<AIProvider>
{
    Task<AIProvider?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<AIProvider?> GetByIdWithModelsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AIProvider>> GetByTypeAsync(AIProviderType providerType, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AIProvider>> GetActiveAsync(CancellationToken cancellationToken = default);
}
