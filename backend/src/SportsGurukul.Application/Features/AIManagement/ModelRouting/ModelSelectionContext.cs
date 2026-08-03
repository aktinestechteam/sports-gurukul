using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Features.AIManagement.ModelRouting;

public record ModelSelectionContext(
    Guid? AssistantId,
    Guid? AgentDefinitionId,
    Guid? ConversationId,
    AIRoutingStrategy RoutingStrategy,
    int? EstimatedInputTokens,
    int? MaxOutputTokens,
    bool RequiresFunctionCalling,
    bool RequiresVision,
    bool RequiresJsonMode,
    decimal? MaxCostPerRequest,
    int? MaxLatencyMs,
    IReadOnlyList<Guid>? PreferredModelIds,
    IReadOnlyList<Guid>? FallbackModelIds
);
