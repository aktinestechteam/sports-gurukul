using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Infrastructure.Persistence.Repositories.AI;

public class AIProviderRepository : Repository<AIProvider>, IAIProviderRepository
{
    public AIProviderRepository(ApplicationDbContext context) : base(context) { }

    public async Task<AIProvider?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await Context.Set<AIProvider>()
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Name == name, cancellationToken);
    }

    public async Task<AIProvider?> GetByIdWithModelsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<AIProvider>()
            .AsNoTracking()
            .Include(p => p.Models)
            .AsSplitQuery()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<AIProvider>> GetByTypeAsync(AIProviderType providerType, CancellationToken cancellationToken = default)
    {
        return await Context.Set<AIProvider>()
            .AsNoTracking()
            .Where(p => p.ProviderType == providerType)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AIProvider>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Set<AIProvider>()
            .AsNoTracking()
            .Where(p => p.IsActive)
            .OrderBy(p => p.DisplayName)
            .ToListAsync(cancellationToken);
    }
}
