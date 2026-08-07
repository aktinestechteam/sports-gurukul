using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Common.Interfaces.AI;

public interface IConversationRepository : IRepository<Conversation>
{
    Task<Conversation?> GetByIdWithMessagesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Conversation?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Conversation>> GetByAssistantIdAsync(Guid assistantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Conversation>> GetByParticipantAsync(Guid participantUserId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Conversation>> GetByStatusAsync(AIConversationStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Conversation>> GetActiveByAssistantAsync(Guid assistantId, CancellationToken cancellationToken = default);
}
