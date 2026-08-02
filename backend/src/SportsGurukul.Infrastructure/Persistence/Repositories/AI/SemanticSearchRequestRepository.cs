using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Infrastructure.Persistence.Repositories.AI;

public class SemanticSearchRequestRepository : Repository<Domain.Entities.AI.SemanticSearchRequest>, ISemanticSearchRequestRepository
{
    public SemanticSearchRequestRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Domain.Entities.AI.SemanticSearchRequest?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.SemanticSearchRequest>()
            .AsNoTracking()
            .Include(r => r.Results)
            .AsSplitQuery()
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.AI.SemanticSearchRequest>> GetByStatusAsync(string status, CancellationToken cancellationToken = default)
    {
        var searchStatus = Enum.Parse<SemanticSearchStatus>(status);
        return await Context.Set<Domain.Entities.AI.SemanticSearchRequest>()
            .AsNoTracking()
            .Where(r => r.Status == searchStatus)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.AI.SemanticSearchRequest>> GetRecentAsync(int count, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.SemanticSearchRequest>()
            .AsNoTracking()
            .OrderByDescending(r => r.CreatedAt)
            .Take(count)
            .ToListAsync(cancellationToken);
    }
}
