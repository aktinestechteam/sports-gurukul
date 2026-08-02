using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces.AI;

namespace SportsGurukul.Infrastructure.Persistence.Repositories.AI;

public class ConversationMessageRepository : Repository<Domain.Entities.AI.ConversationMessage>, IConversationMessageRepository
{
    public ConversationMessageRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Domain.Entities.AI.ConversationMessage?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.ConversationMessage>()
            .AsNoTracking()
            .Include(m => m.Conversation)
            .AsSplitQuery()
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.AI.ConversationMessage>> GetByConversationIdAsync(Guid conversationId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.ConversationMessage>()
            .AsNoTracking()
            .Where(m => m.ConversationId == conversationId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.AI.ConversationMessage>> GetRecentByConversationIdAsync(Guid conversationId, int count, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.ConversationMessage>()
            .AsNoTracking()
            .Where(m => m.ConversationId == conversationId)
            .OrderByDescending(m => m.CreatedAt)
            .Take(count)
            .ToListAsync(cancellationToken);
    }
}
