using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Infrastructure.Persistence.Repositories.AI;

public class ToolDefinitionRepository : Repository<Domain.Entities.AI.ToolDefinition>, IToolDefinitionRepository
{
    public ToolDefinitionRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Domain.Entities.AI.ToolDefinition?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.ToolDefinition>()
            .AsNoTracking()
            .Include(t => t.Executions)
            .AsSplitQuery()
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.AI.ToolDefinition>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.ToolDefinition>()
            .AsNoTracking()
            .Where(t => t.Status == ToolStatus.Active)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.AI.ToolDefinition>> GetByTypeAsync(string toolType, CancellationToken cancellationToken = default)
    {
        var type = Enum.Parse<ToolType>(toolType);
        return await Context.Set<Domain.Entities.AI.ToolDefinition>()
            .AsNoTracking()
            .Where(t => t.Type == type)
            .ToListAsync(cancellationToken);
    }
}
