using SportsGurukul.Domain.Entities.AI;

namespace SportsGurukul.Application.Common.Interfaces.AI;

public interface IConversationMemoryRepository : IRepository<ConversationMemory>
{
    Task<ConversationMemory?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ConversationMemory>> GetByConversationIdAsync(Guid conversationId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ConversationMemory>> GetByTypeAndImportanceAsync(string memoryType, int minImportance, CancellationToken cancellationToken = default);
}
