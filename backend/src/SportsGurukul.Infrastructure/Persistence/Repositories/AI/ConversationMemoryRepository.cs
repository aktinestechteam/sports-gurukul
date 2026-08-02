using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Infrastructure.Persistence.Repositories.AI;

public class ConversationMemoryRepository : Repository<Domain.Entities.AI.ConversationMemory>, IConversationMemoryRepository
{
    public ConversationMemoryRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Domain.Entities.AI.ConversationMemory?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.ConversationMemory>()
            .AsNoTracking()
            .Include(m => m.Conversation)
            .AsSplitQuery()
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.AI.ConversationMemory>> GetByConversationIdAsync(Guid conversationId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Domain.Entities.AI.ConversationMemory>()
            .AsNoTracking()
            .Where(m => m.ConversationId == conversationId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.AI.ConversationMemory>> GetByTypeAndImportanceAsync(string memoryType, int minImportance, CancellationToken cancellationToken = default)
    {
        var type = Enum.Parse<MemoryType>(memoryType);
        return await Context.Set<Domain.Entities.AI.ConversationMemory>()
            .AsNoTracking()
            .Where(m => m.Type == type && (int)m.Importance >= minImportance)
            .ToListAsync(cancellationToken);
    }
}
