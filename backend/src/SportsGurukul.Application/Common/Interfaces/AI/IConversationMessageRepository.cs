using SportsGurukul.Domain.Entities.AI;

namespace SportsGurukul.Application.Common.Interfaces.AI;

public interface IConversationMessageRepository : IRepository<ConversationMessage>
{
    Task<ConversationMessage?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ConversationMessage>> GetByConversationIdAsync(Guid conversationId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ConversationMessage>> GetRecentByConversationIdAsync(Guid conversationId, int count, CancellationToken cancellationToken = default);
}
