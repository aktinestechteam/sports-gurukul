using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Infrastructure.Persistence.Repositories.AI;

public class AgentExecutionRepository : Repository<Domain.Entities.AI.AgentExecution>, IAgentExecutionRepository
{
    public AgentExecutionRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Domain.Entities.AI.AgentExecution?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.AgentExecution>()
            .AsNoTracking()
            .Include(e => e.AgentDefinition)
            .AsSplitQuery()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.AI.AgentExecution>> GetByAgentDefinitionIdAsync(Guid agentDefinitionId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.AgentExecution>()
            .AsNoTracking()
            .Where(e => e.AgentDefinitionId == agentDefinitionId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.AI.AgentExecution>> GetByStatusAsync(string status, CancellationToken cancellationToken = default)
    {
        var execStatus = Enum.Parse<AgentExecutionStatus>(status);
        return await Context.Set<Domain.Entities.AI.AgentExecution>()
            .AsNoTracking()
            .Where(e => e.Status == execStatus)
            .ToListAsync(cancellationToken);
    }
}
