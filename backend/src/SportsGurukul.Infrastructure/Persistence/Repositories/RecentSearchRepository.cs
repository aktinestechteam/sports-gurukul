using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Repositories;

public class RecentSearchRepository : Repository<RecentSearch>, IRecentSearchRepository
{
    public RecentSearchRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<RecentSearch>> GetByUserIdAsync(Guid userId, int limit = 10, CancellationToken cancellationToken = default)
    {
        return await Context.RecentSearches
            .AsNoTracking()
            .Where(s => s.UserId == userId && !s.IsDeleted)
            .OrderByDescending(s => s.SearchedAt)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task DeleteOlderThanAsync(Guid userId, int keepCount, CancellationToken cancellationToken = default)
    {
        var toDelete = await Context.RecentSearches
            .Where(s => s.UserId == userId && !s.IsDeleted)
            .OrderByDescending(s => s.SearchedAt)
            .Skip(keepCount)
            .ToListAsync(cancellationToken);

        foreach (var entity in toDelete)
        {
            entity.IsDeleted = true;
            Context.RecentSearches.Update(entity);
        }
    }
}
