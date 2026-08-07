namespace SportsGurukul.Application.Features.AIManagement.ModelRouting;

internal static class ModelSelectionCalculator
{
    internal static decimal? EstimateCost(ModelCandidate candidate, ModelSelectionContext context)
    {
        if (candidate.InputCostPerMillionTokens is null && candidate.OutputCostPerMillionTokens is null)
            return null;

        var inputTokens = context.EstimatedInputTokens ?? 0;
        var outputTokens = context.MaxOutputTokens ?? 0;

        var inputCost = (candidate.InputCostPerMillionTokens ?? 0m) * inputTokens / 1_000_000m;
        var outputCost = (candidate.OutputCostPerMillionTokens ?? 0m) * outputTokens / 1_000_000m;
        return inputCost + outputCost;
    }

    internal static int? EstimateLatency(ModelCandidate candidate)
        => candidate.ExpectedLatencyMs is null ? null : (int)candidate.ExpectedLatencyMs.Value;

    internal static int CapabilityScore(ModelCandidate candidate, ModelSelectionContext context)
    {
        var score = 0;
        if (!context.RequiresFunctionCalling || candidate.SupportsFunctionCalling) score++;
        if (!context.RequiresVision || candidate.SupportsVision) score++;
        if (!context.RequiresJsonMode || candidate.SupportsJsonMode) score++;
        if (candidate.SupportsChat) score++;
        return score;
    }

    internal static bool MeetsCapabilities(ModelCandidate candidate, ModelSelectionContext context)
    {
        if (context.RequiresFunctionCalling && !candidate.SupportsFunctionCalling) return false;
        if (context.RequiresVision && !candidate.SupportsVision) return false;
        if (context.RequiresJsonMode && !candidate.SupportsJsonMode) return false;
        if (!candidate.SupportsChat) return false;
        return true;
    }

    internal static ModelSelectionResult ToSelectionResult(
        ModelCandidate candidate,
        ModelSelectionContext context,
        int priority,
        string reason)
        => new(
            candidate.ModelId,
            candidate.ProviderId,
            candidate.ModelName,
            candidate.ProviderName,
            priority,
            EstimateCost(candidate, context),
            EstimateLatency(candidate),
            reason);

    internal static double BalancedScore(ModelCandidate candidate, ModelSelectionContext context)
    {
        var score = 0d;

        var cost = EstimateCost(candidate, context);
        if (cost.HasValue)
            score += 50d * (1d / (1d + (double)cost.Value * 100_000d));

        var latency = EstimateLatency(candidate);
        if (latency.HasValue)
            score += 30d * (1d / (1d + latency.Value / 1000d));

        score += 10d * CapabilityScore(candidate, context);
        score += (candidate.ContextWindow ?? 0) / 100_000d;

        return score;
    }
}
