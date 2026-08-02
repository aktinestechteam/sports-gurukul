using SportsGurukul.Application.Common.Interfaces.AI.Models;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Entities.AI;

namespace SportsGurukul.Application.Common.Interfaces.AI.Services;

public interface ITokenUsageService
{
    Task<Result<AITokenUsage>> RecordUsageAsync(RecordTokenUsageRequest request, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<AITokenUsage>>> GetByConversationAsync(Guid conversationId, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<AITokenUsage>>> GetByUserAsync(string userId, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<AITokenUsage>>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<AITokenUsage>>> SearchAsync(SearchTokenUsageRequest request, CancellationToken cancellationToken = default);
}
