using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Infrastructure.Persistence.Repositories.AI;

public class AssistantRepository : Repository<AIAssistant>, IAssistantRepository
{
    public AssistantRepository(ApplicationDbContext context) : base(context) { }

    public async Task<AIAssistant?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<AIAssistant>()
            .AsNoTracking()
            .Include(a => a.Model)
            .Include(a => a.PromptTemplates)
            .Include(a => a.Conversations)
            .AsSplitQuery()
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<AIAssistant?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await Context.Set<AIAssistant>()
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Name == name, cancellationToken);
    }

    public async Task<IReadOnlyList<AIAssistant>> GetByOwnerAsync(Guid ownerUserId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<AIAssistant>()
            .AsNoTracking()
            .Where(a => a.OwnerUserId == ownerUserId)
            .OrderBy(a => a.DisplayName)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AIAssistant>> GetByTypeAsync(AIAssistantType assistantType, CancellationToken cancellationToken = default)
    {
        return await Context.Set<AIAssistant>()
            .AsNoTracking()
            .Where(a => a.AssistantType == assistantType)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AIAssistant>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Set<AIAssistant>()
            .AsNoTracking()
            .Where(a => a.IsActive)
            .OrderBy(a => a.DisplayName)
            .ToListAsync(cancellationToken);
    }
}
