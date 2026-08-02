using SportsGurukul.Domain.Entities.AI;

namespace SportsGurukul.Application.Common.Interfaces.AI;

public interface IAIModelRepository : IRepository<AIModel>
{
    Task<AIModel?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AIModel>> GetByProviderAsync(Guid providerId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AIModel>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AIModel>> GetByCapabilityAsync(string capability, CancellationToken cancellationToken = default);
}
