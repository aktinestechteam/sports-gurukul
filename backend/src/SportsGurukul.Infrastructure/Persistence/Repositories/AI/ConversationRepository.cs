using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Infrastructure.Persistence.Repositories.AI;

public class ConversationRepository : Repository<Domain.Entities.AI.Conversation>, IConversationRepository
{
    public ConversationRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Domain.Entities.AI.Conversation?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.Conversation>()
            .AsNoTracking()
            .Include(c => c.Assistant)
            .Include(c => c.Messages)
            .Include(c => c.Memories)
            .AsSplitQuery()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.AI.Conversation>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.Conversation>()
            .AsNoTracking()
            .Where(c => c.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.AI.Conversation>> GetByAssistantIdAsync(Guid assistantId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.Conversation>()
            .AsNoTracking()
            .Where(c => c.AssistantId == assistantId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.AI.Conversation>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.Conversation>()
            .AsNoTracking()
            .Where(c => c.Status == ConversationStatus.Active)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.AI.Conversation>> GetByStatusAsync(string status, CancellationToken cancellationToken = default)
    {
        var conversationStatus = Enum.Parse<ConversationStatus>(status);
        return await Context.Set<Domain.Entities.AI.Conversation>()
            .AsNoTracking()
            .Where(c => c.Status == conversationStatus)
            .ToListAsync(cancellationToken);
    }
}
