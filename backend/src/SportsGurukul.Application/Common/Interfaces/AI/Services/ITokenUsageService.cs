using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Common.Interfaces.AI.Services;

public interface ITokenUsageService
{
    Task<Result<TokenUsageDto>> RecordAsync(RecordTokenUsageRequest request, CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<TokenUsageDto>>> GetByConversationAsync(Guid conversationId, CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<TokenUsageDto>>> GetByAssistantAsync(Guid assistantId, CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<TokenUsageDto>>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<TokenUsageDto>>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);

    Task<Result<TokenUsageSummaryDto>> GetSummaryAsync(
        Guid? assistantId,
        Guid? conversationId,
        Guid? userId,
        DateTime? from,
        DateTime? to,
        AIUsageType? usageType,
        CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<TokenUsageDto>>> SearchAsync(
        Guid? assistantId,
        Guid? conversationId,
        Guid? userId,
        AIUsageType? usageType,
        DateTime? from,
        DateTime? to,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}
