using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Common.Interfaces.AI;

public interface ITokenUsageRepository : IRepository<AITokenUsage>
{
    Task<IReadOnlyList<AITokenUsage>> GetByProviderAsync(Guid providerId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AITokenUsage>> GetByModelAsync(Guid modelId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AITokenUsage>> GetByAssistantAsync(Guid assistantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AITokenUsage>> GetByConversationAsync(Guid conversationId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AITokenUsage>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AITokenUsage>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AITokenUsage>> GetByTypeAsync(AIUsageType usageType, CancellationToken cancellationToken = default);
}
