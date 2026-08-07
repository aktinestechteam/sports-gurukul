using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Features.AIManagement.DTOs;

public record TokenUsageDto(
    Guid Id,
    Guid? ProviderId,
    Guid? ModelId,
    Guid? AssistantId,
    Guid? ConversationId,
    Guid? UserId,
    AIResourceOwnerType UserType,
    AIUsageType UsageType,
    int InputTokens,
    int OutputTokens,
    int TotalTokens,
    int? CacheReadTokens,
    int? CacheWriteTokens,
    decimal? Cost,
    string Currency,
    DateTime? StartedAt,
    DateTime? EndedAt,
    long? LatencyMs,
    string? ModelName,
    DateTime CreatedAt
);

public record RecordTokenUsageRequest(
    Guid? ProviderId,
    Guid? ModelId,
    Guid? AssistantId,
    Guid? ConversationId,
    Guid? UserId,
    AIResourceOwnerType UserType,
    AIUsageType UsageType,
    int InputTokens,
    int OutputTokens,
    int? CacheReadTokens,
    int? CacheWriteTokens,
    decimal? Cost,
    string? Currency,
    DateTime? StartedAt,
    DateTime? EndedAt,
    long? LatencyMs,
    string? ModelName
);

public record TokenUsageSummaryDto(
    int TotalRequests,
    int TotalInputTokens,
    int TotalOutputTokens,
    int TotalTokens,
    decimal? TotalCost,
    string Currency,
    DateTime? From,
    DateTime? To
);
