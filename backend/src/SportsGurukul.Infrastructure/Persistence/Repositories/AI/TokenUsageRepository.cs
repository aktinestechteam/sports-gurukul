using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Infrastructure.Persistence.Repositories.AI;

public class TokenUsageRepository : Repository<AITokenUsage>, ITokenUsageRepository
{
    public TokenUsageRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<AITokenUsage>> GetByProviderAsync(Guid providerId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<AITokenUsage>()
            .AsNoTracking()
            .Where(t => t.ProviderId == providerId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AITokenUsage>> GetByModelAsync(Guid modelId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<AITokenUsage>()
            .AsNoTracking()
            .Where(t => t.ModelId == modelId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AITokenUsage>> GetByAssistantAsync(Guid assistantId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<AITokenUsage>()
            .AsNoTracking()
            .Where(t => t.AssistantId == assistantId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AITokenUsage>> GetByConversationAsync(Guid conversationId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<AITokenUsage>()
            .AsNoTracking()
            .Where(t => t.ConversationId == conversationId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AITokenUsage>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<AITokenUsage>()
            .AsNoTracking()
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AITokenUsage>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        return await Context.Set<AITokenUsage>()
            .AsNoTracking()
            .Where(t => t.CreatedAt >= from && t.CreatedAt < to)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AITokenUsage>> GetByTypeAsync(AIUsageType usageType, CancellationToken cancellationToken = default)
    {
        return await Context.Set<AITokenUsage>()
            .AsNoTracking()
            .Where(t => t.UsageType == usageType)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
