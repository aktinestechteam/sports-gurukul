using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.ModelRouting;

namespace SportsGurukul.Application.Features.AIManagement.Queries;

public class GetModelByIdQueryHandler : IRequestHandler<GetModelByIdQuery, Result<ModelCandidate>>
{
    private readonly IModelRoutingService _modelRoutingService;

    public GetModelByIdQueryHandler(IModelRoutingService modelRoutingService)
    {
        _modelRoutingService = modelRoutingService;
    }

    public async Task<Result<ModelCandidate>> Handle(GetModelByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await _modelRoutingService.GetModelCandidateAsync(request.ModelId, cancellationToken);
        if (!result.IsSuccess || result.Value is null)
            return Result<ModelCandidate>.Failure(result.Error ?? "Model not found");

        return Result<ModelCandidate>.Success(result.Value);
    }
}
