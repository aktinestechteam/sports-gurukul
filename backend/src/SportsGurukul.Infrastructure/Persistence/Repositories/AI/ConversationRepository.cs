using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Infrastructure.Persistence.Repositories.AI;

public class ConversationRepository : Repository<Conversation>, IConversationRepository
{
    public ConversationRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Conversation?> GetByIdWithMessagesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Conversation>()
            .AsNoTracking()
            .Include(c => c.Messages.OrderBy(m => m.SequenceNumber))
            .AsSplitQuery()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Conversation?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Conversation>()
            .AsNoTracking()
            .Include(c => c.Assistant)
            .Include(c => c.Messages.OrderBy(m => m.SequenceNumber))
            .Include(c => c.Memories)
            .AsSplitQuery()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Conversation>> GetByAssistantIdAsync(Guid assistantId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Conversation>()
            .AsNoTracking()
            .Where(c => c.AssistantId == assistantId)
            .OrderByDescending(c => c.LastMessageAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Conversation>> GetByParticipantAsync(Guid participantUserId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Conversation>()
            .AsNoTracking()
            .Where(c => c.ParticipantUserId == participantUserId)
            .OrderByDescending(c => c.LastMessageAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Conversation>> GetByStatusAsync(AIConversationStatus status, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Conversation>()
            .AsNoTracking()
            .Where(c => c.Status == status)
            .OrderByDescending(c => c.LastMessageAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Conversation>> GetActiveByAssistantAsync(Guid assistantId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Conversation>()
            .AsNoTracking()
            .Where(c => c.AssistantId == assistantId && c.Status == AIConversationStatus.Active)
            .OrderByDescending(c => c.LastMessageAt)
            .ToListAsync(cancellationToken);
    }
}
