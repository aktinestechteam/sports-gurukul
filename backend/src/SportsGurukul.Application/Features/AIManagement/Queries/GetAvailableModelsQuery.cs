using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.ModelRouting;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Features.AIManagement.Queries;

public record GetAvailableModelsQuery(
    AIRoutingStrategy RoutingStrategy,
    Guid? AssistantId,
    Guid? AgentDefinitionId,
    Guid? ConversationId,
    int? EstimatedInputTokens,
    int? MaxOutputTokens,
    bool RequiresFunctionCalling,
    bool RequiresVision,
    bool RequiresJsonMode,
    decimal? MaxCostPerRequest,
    int? MaxLatencyMs,
    IReadOnlyList<Guid>? PreferredModelIds,
    IReadOnlyList<Guid>? FallbackModelIds
) : IRequest<Result<IReadOnlyList<ModelCandidate>>>;
