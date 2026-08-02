using SportsGurukul.Domain.Entities.AI;

namespace SportsGurukul.Application.Common.Interfaces.AI;

public interface IAIProviderRepository : IRepository<AIProvider>
{
    Task<AIProvider?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AIProvider>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AIProvider>> GetByTypeAsync(string providerType, CancellationToken cancellationToken = default);
}
