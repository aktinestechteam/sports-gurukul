using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Domain.Entities.AI;

namespace SportsGurukul.Application.Features.AIManagement.ModelRouting;

public class ModelAvailabilityService : IModelAvailabilityService
{
    private readonly IAIProviderRepository _providerRepository;

    public ModelAvailabilityService(IAIProviderRepository providerRepository)
    {
        _providerRepository = providerRepository;
    }

    public async Task<bool> IsAvailableAsync(Guid modelId, CancellationToken cancellationToken = default)
    {
        var providers = await _providerRepository.GetActiveAsync(cancellationToken);
        foreach (var provider in providers)
        {
            var providerWithModels = await _providerRepository.GetByIdWithModelsAsync(provider.Id, cancellationToken);
            if (providerWithModels?.Models.Any(m => m.Id == modelId && m.IsActive) == true)
                return true;
        }

        return false;
    }

    public async Task<IReadOnlyList<ModelCandidate>> GetAvailableCandidatesAsync(
        ModelSelectionContext context,
        CancellationToken cancellationToken = default)
    {
        var providers = await _providerRepository.GetActiveAsync(cancellationToken);
        var candidates = new List<ModelCandidate>();

        foreach (var provider in providers)
        {
            var providerWithModels = await _providerRepository.GetByIdWithModelsAsync(provider.Id, cancellationToken);
            if (providerWithModels is null)
                continue;

            foreach (var model in providerWithModels.Models.Where(m => m.IsActive))
            {
                var candidate = Map(model, providerWithModels);
                if (ModelSelectionCalculator.MeetsCapabilities(candidate, context))
                    candidates.Add(candidate);
            }
        }

        if (context.PreferredModelIds is { Count: > 0 })
        {
            candidates = candidates
                .OrderBy(c => IndexOf(context.PreferredModelIds, c.ModelId))
                .ToList();
        }

        return candidates;
    }

    internal static ModelCandidate Map(AIModel model, AIProvider provider)
        => new(
            model.Id,
            model.ProviderId,
            model.Name,
            provider.DisplayName,
            model.Family,
            model.ContextWindow,
            model.MaxOutputTokens,
            model.InputCostPerMillionTokens,
            model.OutputCostPerMillionTokens,
            model.Currency,
            model.SupportsChat,
            model.SupportsFunctionCalling,
            model.SupportsVision,
            model.SupportsJsonMode,
            model.RateLimitPerMinute,
            null);

    private static int IndexOf(IReadOnlyList<Guid> ids, Guid modelId)
    {
        for (var i = 0; i < ids.Count; i++)
        {
            if (ids[i] == modelId)
                return i;
        }

        return int.MaxValue;
    }
}
