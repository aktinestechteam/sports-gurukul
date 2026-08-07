namespace SportsGurukul.Application.Features.AIManagement.ModelRouting;

public interface IModelAvailabilityService
{
    Task<bool> IsAvailableAsync(Guid modelId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ModelCandidate>> GetAvailableCandidatesAsync(
        ModelSelectionContext context,
        CancellationToken cancellationToken = default);
}
