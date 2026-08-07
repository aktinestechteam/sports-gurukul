using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Infrastructure.Persistence.Repositories.AI;

public class AgentRepository : Repository<AgentDefinition>, IAgentRepository
{
    public AgentRepository(ApplicationDbContext context) : base(context) { }

    public async Task<AgentDefinition?> GetByIdWithToolsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<AgentDefinition>()
            .AsNoTracking()
            .Include(a => a.Tools.Where(t => t.IsActive))
            .Include(a => a.Model)
            .AsSplitQuery()
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<AgentDefinition?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await Context.Set<AgentDefinition>()
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Name == name, cancellationToken);
    }

    public async Task<IReadOnlyList<AgentDefinition>> GetByWorkflowAsync(Guid workflowId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<AgentDefinition>()
            .AsNoTracking()
            .Where(a => a.WorkflowId == workflowId)
            .OrderBy(a => a.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AgentDefinition>> GetByTypeAsync(AIAgentType agentType, CancellationToken cancellationToken = default)
    {
        return await Context.Set<AgentDefinition>()
            .AsNoTracking()
            .Where(a => a.AgentType == agentType)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AgentDefinition>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Set<AgentDefinition>()
            .AsNoTracking()
            .Where(a => a.IsActive)
            .OrderBy(a => a.Name)
            .ToListAsync(cancellationToken);
    }
}
