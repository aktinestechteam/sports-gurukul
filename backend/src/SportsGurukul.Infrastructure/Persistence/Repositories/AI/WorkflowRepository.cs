using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Infrastructure.Persistence.Repositories.AI;

public class WorkflowRepository : Repository<WorkflowDefinition>, IWorkflowRepository
{
    public WorkflowRepository(ApplicationDbContext context) : base(context) { }

    public async Task<WorkflowDefinition?> GetByIdWithAgentsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<WorkflowDefinition>()
            .AsNoTracking()
            .Include(w => w.Agents)
            .ThenInclude(a => a.Tools.Where(t => t.IsActive))
            .AsSplitQuery()
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
    }

    public async Task<WorkflowDefinition?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await Context.Set<WorkflowDefinition>()
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.Name == name, cancellationToken);
    }

    public async Task<IReadOnlyList<WorkflowDefinition>> GetByTypeAsync(AIWorkflowType workflowType, CancellationToken cancellationToken = default)
    {
        return await Context.Set<WorkflowDefinition>()
            .AsNoTracking()
            .Where(w => w.WorkflowType == workflowType)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<WorkflowDefinition>> GetPublishedAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Set<WorkflowDefinition>()
            .AsNoTracking()
            .Where(w => w.IsPublished && w.IsActive)
            .OrderBy(w => w.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<WorkflowDefinition>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Set<WorkflowDefinition>()
            .AsNoTracking()
            .Where(w => w.IsActive)
            .OrderBy(w => w.Name)
            .ToListAsync(cancellationToken);
    }
}
