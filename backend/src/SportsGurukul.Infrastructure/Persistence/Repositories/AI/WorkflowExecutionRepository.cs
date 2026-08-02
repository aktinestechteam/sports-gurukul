using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Infrastructure.Persistence.Repositories.AI;

public class WorkflowExecutionRepository : Repository<Domain.Entities.AI.WorkflowExecution>, IWorkflowExecutionRepository
{
    public WorkflowExecutionRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Domain.Entities.AI.WorkflowExecution?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.WorkflowExecution>()
            .AsNoTracking()
            .Include(e => e.WorkflowDefinition)
            .AsSplitQuery()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.AI.WorkflowExecution>> GetByWorkflowDefinitionIdAsync(Guid workflowDefinitionId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.WorkflowExecution>()
            .AsNoTracking()
            .Where(e => e.WorkflowDefinitionId == workflowDefinitionId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.AI.WorkflowExecution>> GetByStatusAsync(string status, CancellationToken cancellationToken = default)
    {
        var execStatus = Enum.Parse<WorkflowExecutionStatus>(status);
        return await Context.Set<Domain.Entities.AI.WorkflowExecution>()
            .AsNoTracking()
            .Where(e => e.Status == execStatus)
            .ToListAsync(cancellationToken);
    }
}
