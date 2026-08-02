using SportsGurukul.Domain.Entities.AI;

namespace SportsGurukul.Application.Common.Interfaces.AI;

public interface ISemanticSearchRequestRepository : IRepository<SemanticSearchRequest>
{
    Task<SemanticSearchRequest?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SemanticSearchRequest>> GetByStatusAsync(string status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SemanticSearchRequest>> GetRecentAsync(int count, CancellationToken cancellationToken = default);
}
