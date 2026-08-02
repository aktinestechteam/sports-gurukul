using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Interfaces.HumanInTheLoop;

public interface IApprovalCoordinator
{
    Task<IReadOnlyList<ApprovalRequest>> EvaluateTimeoutsAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ApprovalRequest>> EvaluateEscalationsAsync(CancellationToken cancellationToken = default);

    Task<ApprovalDecision> ResolveAsync(Guid requestId, bool approved, string? decidedBy = null, string? reason = null, CancellationToken cancellationToken = default);
}
