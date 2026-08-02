using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Infrastructure.Persistence.Repositories.AI;

public class AIProviderRepository : Repository<Domain.Entities.AI.AIProvider>, IAIProviderRepository
{
    public AIProviderRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Domain.Entities.AI.AIProvider?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.AIProvider>()
            .AsNoTracking()
            .Include(p => p.Models)
            .AsSplitQuery()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.AI.AIProvider>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.AIProvider>()
            .AsNoTracking()
            .Where(p => p.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.AI.AIProvider>> GetByTypeAsync(string providerType, CancellationToken cancellationToken = default)
    {
        var type = Enum.Parse<AIProviderType>(providerType);
        return await Context.Set<Domain.Entities.AI.AIProvider>()
            .AsNoTracking()
            .Where(p => p.Type == type)
            .ToListAsync(cancellationToken);
    }
}
