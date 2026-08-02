using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces.AI;

namespace SportsGurukul.Infrastructure.Persistence.Repositories.AI;

public class ToolExecutionRepository : Repository<Domain.Entities.AI.ToolExecution>, IToolExecutionRepository
{
    public ToolExecutionRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Domain.Entities.AI.ToolExecution?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.ToolExecution>()
            .AsNoTracking()
            .Include(e => e.ToolDefinition)
            .Include(e => e.Conversation)
            .AsSplitQuery()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.AI.ToolExecution>> GetByToolDefinitionIdAsync(Guid toolDefinitionId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.ToolExecution>()
            .AsNoTracking()
            .Where(e => e.ToolDefinitionId == toolDefinitionId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.AI.ToolExecution>> GetByConversationIdAsync(Guid conversationId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.ToolExecution>()
            .AsNoTracking()
            .Where(e => e.ConversationId == conversationId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.AI.ToolExecution>> GetRecentAsync(Guid conversationId, int count, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.ToolExecution>()
            .AsNoTracking()
            .Where(e => e.ConversationId == conversationId)
            .OrderByDescending(e => e.CreatedAt)
            .Take(count)
            .ToListAsync(cancellationToken);
    }
}
