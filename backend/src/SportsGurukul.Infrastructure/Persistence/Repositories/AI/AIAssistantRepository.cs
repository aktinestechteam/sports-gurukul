using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Infrastructure.Persistence.Repositories.AI;

public class AIAssistantRepository : Repository<Domain.Entities.AI.AIAssistant>, IAIAssistantRepository
{
    public AIAssistantRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Domain.Entities.AI.AIAssistant?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.AIAssistant>()
            .AsNoTracking()
            .Include(a => a.Conversations)
            .Include(a => a.AgentDefinitions)
            .AsSplitQuery()
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.AI.AIAssistant>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.AIAssistant>()
            .AsNoTracking()
            .Where(a => a.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.AI.AIAssistant>> GetByTypeAsync(string assistantType, CancellationToken cancellationToken = default)
    {
        var type = Enum.Parse<AIAssistantType>(assistantType);
        return await Context.Set<Domain.Entities.AI.AIAssistant>()
            .AsNoTracking()
            .Where(a => a.AssistantType == type)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.AI.AIAssistant>> GetPublicAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.AIAssistant>()
            .AsNoTracking()
            .Where(a => a.IsPublic)
            .ToListAsync(cancellationToken);
    }
}
