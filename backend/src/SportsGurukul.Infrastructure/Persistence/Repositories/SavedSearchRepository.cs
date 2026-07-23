using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Repositories;

public class SavedSearchRepository : Repository<SavedSearch>, ISavedSearchRepository
{
    public SavedSearchRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<SavedSearch>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await Context.SavedSearches
            .AsNoTracking()
            .Where(s => s.UserId == userId && !s.IsDeleted)
            .OrderByDescending(s => s.UsageCount)
            .ThenByDescending(s => s.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<SavedSearch?> GetByIdAndUserAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        return await Context.SavedSearches
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId && !s.IsDeleted, cancellationToken);
    }
}
