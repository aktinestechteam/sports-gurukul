using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Repositories;

public class SportRepository : Repository<Sport>, ISportRepository
{
    public SportRepository(ApplicationDbContext context) : base(context) { }

    public override async Task<Sport?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Sports
            .AsNoTracking()
            .Include(s => s.SportCategory)
            .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted, cancellationToken);
    }

    public async Task<Sport?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await Context.Sports
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Name == name, cancellationToken);
    }

    public async Task<Sport?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await Context.Sports
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Code == code, cancellationToken);
    }

    public async Task<IReadOnlyList<Sport>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        return await Context.Sports
            .AsNoTracking()
            .Where(s => s.SportCategoryId == categoryId)
            .ToListAsync(cancellationToken);
    }
}
