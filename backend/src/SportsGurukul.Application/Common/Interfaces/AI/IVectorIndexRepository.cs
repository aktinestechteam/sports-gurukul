using SportsGurukul.Domain.Entities.AI;

namespace SportsGurukul.Application.Common.Interfaces.AI;

public interface IVectorIndexRepository : IRepository<VectorIndex>
{
    Task<VectorIndex?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<VectorIndex>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<VectorIndex>> GetByStatusAsync(string status, CancellationToken cancellationToken = default);
}
