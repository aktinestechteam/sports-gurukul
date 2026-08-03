using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Features.AIManagement.ModelRouting;

public interface IModelSelectionStrategy
{
    AIRoutingStrategy Strategy { get; }

    Task<Result<ModelSelectionResult>> SelectAsync(
        IReadOnlyList<ModelCandidate> candidates,
        ModelSelectionContext context,
        CancellationToken cancellationToken = default);
}
