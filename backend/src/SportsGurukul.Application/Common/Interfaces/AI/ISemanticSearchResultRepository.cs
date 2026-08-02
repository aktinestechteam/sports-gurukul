using SportsGurukul.Domain.Entities.AI;

namespace SportsGurukul.Application.Common.Interfaces.AI;

public interface ISemanticSearchResultRepository : IRepository<SemanticSearchResult>
{
    Task<SemanticSearchResult?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SemanticSearchResult>> GetBySearchRequestIdAsync(Guid searchRequestId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SemanticSearchResult>> GetByMinScoreAsync(Guid searchRequestId, double minScore, CancellationToken cancellationToken = default);
}
