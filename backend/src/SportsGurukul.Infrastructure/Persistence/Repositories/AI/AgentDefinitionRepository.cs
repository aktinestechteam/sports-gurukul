using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Infrastructure.Persistence.Repositories.AI;

public class AgentDefinitionRepository : Repository<Domain.Entities.AI.AgentDefinition>, IAgentDefinitionRepository
{
    public AgentDefinitionRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Domain.Entities.AI.AgentDefinition?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.AgentDefinition>()
            .AsNoTracking()
            .Include(a => a.Assistant)
            .Include(a => a.Executions)
            .AsSplitQuery()
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.AI.AgentDefinition>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.AgentDefinition>()
            .AsNoTracking()
            .Where(a => a.Status == AgentStatus.Active)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.AI.AgentDefinition>> GetByAssistantIdAsync(Guid assistantId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.AgentDefinition>()
            .AsNoTracking()
            .Where(a => a.AssistantId == assistantId)
            .ToListAsync(cancellationToken);
    }
}
