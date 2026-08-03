namespace SportsGurukul.Application.Features.AIManagement.ModelRouting;

public record ModelSelectionResult(
    Guid ModelId,
    Guid ProviderId,
    string ModelName,
    string ProviderName,
    int Priority,
    decimal? EstimatedCost,
    int? EstimatedLatencyMs,
    string Reason
);
