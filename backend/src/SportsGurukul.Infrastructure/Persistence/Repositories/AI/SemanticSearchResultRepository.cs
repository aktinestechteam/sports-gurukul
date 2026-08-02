using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces.AI;

namespace SportsGurukul.Infrastructure.Persistence.Repositories.AI;

public class SemanticSearchResultRepository : Repository<Domain.Entities.AI.SemanticSearchResult>, ISemanticSearchResultRepository
{
    public SemanticSearchResultRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Domain.Entities.AI.SemanticSearchResult?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.SemanticSearchResult>()
            .AsNoTracking()
            .Include(r => r.SearchRequest)
            .Include(r => r.Document)
            .AsSplitQuery()
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.AI.SemanticSearchResult>> GetBySearchRequestIdAsync(Guid searchRequestId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.SemanticSearchResult>()
            .AsNoTracking()
            .Where(r => r.SearchRequestId == searchRequestId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.AI.SemanticSearchResult>> GetByMinScoreAsync(Guid searchRequestId, double minScore, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.SemanticSearchResult>()
            .AsNoTracking()
            .Where(r => r.SearchRequestId == searchRequestId && r.Score >= minScore)
            .OrderByDescending(r => r.Score)
            .ToListAsync(cancellationToken);
    }
}
