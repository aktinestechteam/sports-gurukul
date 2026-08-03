using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Features.AIManagement.ModelRouting;

public record ModelCandidate(
    Guid ModelId,
    Guid ProviderId,
    string ModelName,
    string ProviderName,
    AIModelFamily Family,
    int? ContextWindow,
    int? MaxOutputTokens,
    decimal? InputCostPerMillionTokens,
    decimal? OutputCostPerMillionTokens,
    string Currency,
    bool SupportsChat,
    bool SupportsFunctionCalling,
    bool SupportsVision,
    bool SupportsJsonMode,
    int? RateLimitPerMinute,
    long? ExpectedLatencyMs
);
