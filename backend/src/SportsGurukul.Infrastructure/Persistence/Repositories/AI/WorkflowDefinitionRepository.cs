using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Infrastructure.Persistence.Repositories.AI;

public class WorkflowDefinitionRepository : Repository<Domain.Entities.AI.WorkflowDefinition>, IWorkflowDefinitionRepository
{
    public WorkflowDefinitionRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Domain.Entities.AI.WorkflowDefinition?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.WorkflowDefinition>()
            .AsNoTracking()
            .Include(w => w.Executions)
            .AsSplitQuery()
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.AI.WorkflowDefinition>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.WorkflowDefinition>()
            .AsNoTracking()
            .Where(w => w.Status == WorkflowStatus.Active)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.AI.WorkflowDefinition>> GetByStatusAsync(string status, CancellationToken cancellationToken = default)
    {
        var workflowStatus = Enum.Parse<WorkflowStatus>(status);
        return await Context.Set<Domain.Entities.AI.WorkflowDefinition>()
            .AsNoTracking()
            .Where(w => w.Status == workflowStatus)
            .ToListAsync(cancellationToken);
    }
}
