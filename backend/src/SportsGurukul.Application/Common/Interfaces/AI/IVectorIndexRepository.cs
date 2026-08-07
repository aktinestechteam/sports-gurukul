using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Common.Interfaces.AI;

public interface IVectorIndexRepository : IRepository<VectorIndex>
{
    Task<VectorIndex?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<VectorIndex>> GetByProviderAsync(AIVectorIndexProvider provider, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<VectorIndex>> GetByStatusAsync(AIVectorIndexStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<VectorIndex>> GetActiveAsync(CancellationToken cancellationToken = default);
}
