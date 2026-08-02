using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.AI.Interfaces.HumanInTheLoop;
using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.HumanInTheLoop;

public class ApprovalCoordinator : IApprovalCoordinator
{
    private readonly IApprovalStore _store;
    private readonly ILogger<ApprovalCoordinator> _logger;

    public ApprovalCoordinator(IApprovalStore store, ILogger<ApprovalCoordinator>? logger = null)
    {
        _store = store;
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<ApprovalCoordinator>.Instance;
    }

    public async Task<IReadOnlyList<ApprovalRequest>> EvaluateTimeoutsAsync(CancellationToken cancellationToken = default)
    {
        var pending = await _store.GetPendingAsync(null, cancellationToken);
        var now = DateTime.UtcNow;
        var timedOut = new List<ApprovalRequest>();

        foreach (var request in pending)
        {
            if (request.ExpiresAt is not null && now >= request.ExpiresAt.Value && request.Status == ApprovalStatus.Pending)
            {
                request.Status = ApprovalStatus.TimedOut;
                request.ResolvedAt = now;
                await _store.UpdateAsync(request, cancellationToken);
                timedOut.Add(request);
            }
        }

        return timedOut;
    }

    public async Task<IReadOnlyList<ApprovalRequest>> EvaluateEscalationsAsync(CancellationToken cancellationToken = default)
    {
        var pending = await _store.GetPendingAsync(null, cancellationToken);
        var now = DateTime.UtcNow;
        var escalated = new List<ApprovalRequest>();

        foreach (var request in pending)
        {
            var threshold = request.EscalationLevel + 1;
            var thresholdDuration = request.CreatedAt.AddMinutes(threshold * 5);

            if (now >= thresholdDuration && request.Status == ApprovalStatus.Pending)
            {
                request.EscalationLevel++;
                if (request.EscalationLevel == 1)
                {
                    request.Status = ApprovalStatus.Escalated;
                }

                request.ResolvedAt = now;
                await _store.UpdateAsync(request, cancellationToken);
                escalated.Add(request);
            }
        }

        return escalated;
    }

    public async Task<ApprovalDecision> ResolveAsync(Guid requestId, bool approved, string? decidedBy = null, string? reason = null, CancellationToken cancellationToken = default)
    {
        var request = await _store.GetAsync(requestId, cancellationToken)
            ?? throw new AgentPlatformException($"Approval request '{requestId}' not found.", "APPROVAL_NOT_FOUND");

        request.Status = approved ? ApprovalStatus.Approved : ApprovalStatus.Rejected;
        request.ApproverId = decidedBy ?? request.ApproverId;
        request.DecisionReason = reason;
        request.ResolvedAt = DateTime.UtcNow;

        await _store.UpdateAsync(request, cancellationToken);
        _store.Signal(requestId);

        return new ApprovalDecision
        {
            RequestId = requestId,
            Approved = approved,
            DecidedBy = decidedBy,
            Reason = reason,
            DecidedAt = DateTime.UtcNow
        };
    }
}
