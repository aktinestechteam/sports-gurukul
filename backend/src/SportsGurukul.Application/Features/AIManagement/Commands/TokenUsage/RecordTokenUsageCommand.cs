using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Features.AIManagement.Commands.TokenUsage;

public record RecordTokenUsageCommand(
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
) : IRequest<Result<TokenUsageDto>>;
