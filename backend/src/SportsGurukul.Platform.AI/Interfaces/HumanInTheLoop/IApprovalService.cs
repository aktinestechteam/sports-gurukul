using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Interfaces.HumanInTheLoop;

public interface IApprovalService
{
    Task<ApprovalRequest> RequestAsync(CreateApprovalRequest request, CancellationToken cancellationToken = default);

    Task<ApprovalDecision> ApproveAsync(Guid requestId, string? decidedBy = null, string? reason = null, CancellationToken cancellationToken = default);

    Task<ApprovalDecision> RejectAsync(Guid requestId, string? decidedBy = null, string? reason = null, CancellationToken cancellationToken = default);

    Task<ApprovalRequest> CancelAsync(Guid requestId, string? reason = null, CancellationToken cancellationToken = default);

    Task<ApprovalRequest?> GetAsync(Guid requestId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ApprovalRequest>> GetPendingAsync(string? tenantId = null, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ApprovalRequest>> ListAsync(string? tenantId = null, CancellationToken cancellationToken = default);

    Task<ApprovalSummary> GetSummaryAsync(string? tenantId = null, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ApprovalRequest>> EscalateAsync(Guid requestId, string? escalatedTo = null, CancellationToken cancellationToken = default);

    Task<ApprovalRequest> WaitForResolutionAsync(Guid requestId, TimeSpan? timeout = null, CancellationToken cancellationToken = default);
}
