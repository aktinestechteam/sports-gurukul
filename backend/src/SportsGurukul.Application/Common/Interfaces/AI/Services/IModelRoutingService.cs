using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Application.Features.AIManagement.ModelRouting;

namespace SportsGurukul.Application.Common.Interfaces.AI.Services;

public interface IModelRoutingService
{
    Task<Result<ModelSelectionResult>> SelectModelAsync(ModelSelectionContext context, CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<ModelSelectionResult>>> ResolveFallbackChainAsync(ModelSelectionContext context, CancellationToken cancellationToken = default);

    Task<Result<bool>> IsModelAvailableAsync(Guid modelId, CancellationToken cancellationToken = default);

    Task<Result<ModelCandidate?>> GetModelCandidateAsync(Guid modelId, CancellationToken cancellationToken = default);
}
