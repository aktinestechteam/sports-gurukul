using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Features.AIManagement.ModelRouting;

public class ModelRoutingService : IModelRoutingService
{
    private readonly IEnumerable<IModelSelectionStrategy> _strategies;
    private readonly IModelAvailabilityService _availabilityService;
    private readonly IFallbackStrategy _fallbackStrategy;
    private readonly IAIProviderRepository _providerRepository;
    private readonly ILogger<ModelRoutingService> _logger;

    public ModelRoutingService(
        IEnumerable<IModelSelectionStrategy> strategies,
        IModelAvailabilityService availabilityService,
        IFallbackStrategy fallbackStrategy,
        IAIProviderRepository providerRepository,
        ILogger<ModelRoutingService> logger)
    {
        _strategies = strategies;
        _availabilityService = availabilityService;
        _fallbackStrategy = fallbackStrategy;
        _providerRepository = providerRepository;
        _logger = logger;
    }

    public async Task<Result<ModelSelectionResult>> SelectModelAsync(
        ModelSelectionContext context,
        CancellationToken cancellationToken = default)
    {
        var candidates = await _availabilityService.GetAvailableCandidatesAsync(context, cancellationToken);
        if (candidates.Count == 0)
            return Result<ModelSelectionResult>.Failure("No models are currently available for the request");

        var strategy = _strategies.FirstOrDefault(s => s.Strategy == context.RoutingStrategy)
            ?? _strategies.FirstOrDefault(s => s.Strategy == AIRoutingStrategy.Balanced);

        if (strategy is null)
            return Result<ModelSelectionResult>.Failure("No routing strategy is configured for the requested strategy");

        var selection = await strategy.SelectAsync(candidates, context, cancellationToken);
        if (!selection.IsSuccess)
        {
            _logger.LogWarning("Model routing failed for strategy {Strategy}: {Reason}", context.RoutingStrategy, selection.Error);
            return selection;
        }

        return Result<ModelSelectionResult>.Success(selection.Value!);
    }

    public async Task<Result<IReadOnlyList<ModelSelectionResult>>> ResolveFallbackChainAsync(
        ModelSelectionContext context,
        CancellationToken cancellationToken = default)
    {
        var candidates = await _availabilityService.GetAvailableCandidatesAsync(context, cancellationToken);
        if (candidates.Count == 0)
            return Result<IReadOnlyList<ModelSelectionResult>>.Failure("No models are currently available for the request");

        var chain = await _fallbackStrategy.ResolveFallbackChainAsync(candidates, context, cancellationToken);

        var results = new List<ModelSelectionResult>();
        var priority = 1;
        foreach (var modelId in chain)
        {
            var candidate = candidates.FirstOrDefault(c => c.ModelId == modelId);
            if (candidate is null)
                continue;

            results.Add(ModelSelectionCalculator.ToSelectionResult(candidate, context, priority++, "Fallback candidate"));
        }

        return Result<IReadOnlyList<ModelSelectionResult>>.Success(results);
    }

    public async Task<Result<bool>> IsModelAvailableAsync(Guid modelId, CancellationToken cancellationToken = default)
    {
        var available = await _availabilityService.IsAvailableAsync(modelId, cancellationToken);
        return Result<bool>.Success(available);
    }

    public async Task<Result<ModelCandidate?>> GetModelCandidateAsync(Guid modelId, CancellationToken cancellationToken = default)
    {
        var providers = await _providerRepository.GetActiveAsync(cancellationToken);
        foreach (var provider in providers)
        {
            var providerWithModels = await _providerRepository.GetByIdWithModelsAsync(provider.Id, cancellationToken);
            var model = providerWithModels?.Models.FirstOrDefault(m => m.Id == modelId);
            if (model is not null)
                return Result<ModelCandidate?>.Success(ModelAvailabilityService.Map(model, providerWithModels!));
        }

        return Result<ModelCandidate?>.Failure("Model not found");
    }
}
