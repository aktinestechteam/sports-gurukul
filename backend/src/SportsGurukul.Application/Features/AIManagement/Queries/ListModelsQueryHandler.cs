using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.ModelRouting;

namespace SportsGurukul.Application.Features.AIManagement.Queries;

public class ListModelsQueryHandler : IRequestHandler<ListModelsQuery, Result<IReadOnlyList<ModelCandidate>>>
{
    private readonly IAIProviderRepository _providerRepository;

    public ListModelsQueryHandler(IAIProviderRepository providerRepository)
    {
        _providerRepository = providerRepository;
    }

    public async Task<Result<IReadOnlyList<ModelCandidate>>> Handle(ListModelsQuery request, CancellationToken cancellationToken)
    {
        var providers = await _providerRepository.GetActiveAsync(cancellationToken);
        var candidates = new List<ModelCandidate>();

        foreach (var provider in providers)
        {
            if (request.ProviderId.HasValue && provider.Id != request.ProviderId.Value)
                continue;

            var providerWithModels = await _providerRepository.GetByIdWithModelsAsync(provider.Id, cancellationToken);
            if (providerWithModels is null)
                continue;

            foreach (var model in providerWithModels.Models.Where(m => m.IsActive && !m.IsDeleted))
            {
                if (request.Family.HasValue && model.Family != request.Family.Value)
                    continue;
                if (request.SupportsChat.HasValue && model.SupportsChat != request.SupportsChat.Value)
                    continue;
                if (request.SupportsFunctionCalling.HasValue && model.SupportsFunctionCalling != request.SupportsFunctionCalling.Value)
                    continue;
                if (request.SupportsVision.HasValue && model.SupportsVision != request.SupportsVision.Value)
                    continue;
                if (request.SupportsJsonMode.HasValue && model.SupportsJsonMode != request.SupportsJsonMode.Value)
                    continue;

                var candidate = ModelAvailabilityService.Map(model, providerWithModels);

                if (!string.IsNullOrWhiteSpace(request.SearchTerm) &&
                    !candidate.ModelName.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase))
                    continue;

                candidates.Add(candidate);
            }
        }

        var paged = candidates
            .OrderBy(c => c.ProviderName)
            .ThenBy(c => c.ModelName)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return Result<IReadOnlyList<ModelCandidate>>.Success(paged);
    }
}
