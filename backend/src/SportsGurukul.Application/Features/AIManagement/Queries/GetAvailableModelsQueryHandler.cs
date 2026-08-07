using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.ModelRouting;

namespace SportsGurukul.Application.Features.AIManagement.Queries;

public class GetAvailableModelsQueryHandler : IRequestHandler<GetAvailableModelsQuery, Result<IReadOnlyList<ModelCandidate>>>
{
    private readonly IModelAvailabilityService _availabilityService;

    public GetAvailableModelsQueryHandler(IModelAvailabilityService availabilityService)
    {
        _availabilityService = availabilityService;
    }

    public async Task<Result<IReadOnlyList<ModelCandidate>>> Handle(GetAvailableModelsQuery request, CancellationToken cancellationToken)
    {
        var context = new ModelSelectionContext(
            request.AssistantId,
            request.AgentDefinitionId,
            request.ConversationId,
            request.RoutingStrategy,
            request.EstimatedInputTokens,
            request.MaxOutputTokens,
            request.RequiresFunctionCalling,
            request.RequiresVision,
            request.RequiresJsonMode,
            request.MaxCostPerRequest,
            request.MaxLatencyMs,
            request.PreferredModelIds,
            request.FallbackModelIds);

        var candidates = await _availabilityService.GetAvailableCandidatesAsync(context, cancellationToken);
        return Result<IReadOnlyList<ModelCandidate>>.Success(candidates);
    }
}
