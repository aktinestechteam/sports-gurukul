using SportsGurukul.Domain.Entities.AI;

namespace SportsGurukul.Application.Common.Interfaces.AI;

public interface IConversationRepository : IRepository<Conversation>
{
    Task<Conversation?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Conversation>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Conversation>> GetByAssistantIdAsync(Guid assistantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Conversation>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Conversation>> GetByStatusAsync(string status, CancellationToken cancellationToken = default);
}
