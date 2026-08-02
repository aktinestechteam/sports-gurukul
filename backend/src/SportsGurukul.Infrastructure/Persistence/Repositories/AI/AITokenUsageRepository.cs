using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces.AI;

namespace SportsGurukul.Infrastructure.Persistence.Repositories.AI;

public class AITokenUsageRepository : Repository<Domain.Entities.AI.AITokenUsage>, IAITokenUsageRepository
{
    public AITokenUsageRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Domain.Entities.AI.AITokenUsage?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.AITokenUsage>()
            .AsNoTracking()
            .Include(t => t.Conversation)
            .Include(t => t.Message)
            .AsSplitQuery()
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.AI.AITokenUsage>> GetByConversationIdAsync(Guid conversationId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.AITokenUsage>()
            .AsNoTracking()
            .Where(t => t.ConversationId == conversationId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.AI.AITokenUsage>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.AITokenUsage>()
            .AsNoTracking()
            .Where(t => t.UserId == userId.ToString())
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.AI.AITokenUsage>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.AITokenUsage>()
            .AsNoTracking()
            .Where(t => t.CreatedAt >= startDate && t.CreatedAt <= endDate)
            .ToListAsync(cancellationToken);
    }
}
