using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.AI.Interfaces.HumanInTheLoop;
using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.HumanInTheLoop;

public class ApprovalService : IApprovalService
{
    private readonly IApprovalStore _store;
    private readonly IApprovalCoordinator _coordinator;
    private readonly AIPlatformOptions _options;
    private readonly ILogger<ApprovalService> _logger;

    public ApprovalService(
        IApprovalStore store,
        IApprovalCoordinator coordinator,
        AIPlatformOptions options,
        ILogger<ApprovalService>? logger = null)
    {
        _store = store;
        _coordinator = coordinator;
        _options = options;
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<ApprovalService>.Instance;
    }

    public async Task<ApprovalRequest> RequestAsync(CreateApprovalRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new AgentPlatformException("Approval title is required.", "APPROVAL_INVALID");
        }

        var created = DateTime.UtcNow;
        var approval = new ApprovalRequest
        {
            Id = Guid.NewGuid(),
            Type = request.Type,
            Title = request.Title,
            Description = request.Description,
            Action = request.Action,
            Payload = request.Payload,
            Priority = request.Priority,
            RequestedBy = request.RequestedBy,
            ApproverId = request.ApproverId,
            RequiredRole = request.RequiredRole,
            TenantId = request.TenantId,
            CorrelationId = request.CorrelationId,
            RunId = request.RunId,
            EscalationTarget = request.EscalationTarget,
            CreatedAt = created,
            ExpiresAt = created.Add(request.ExpiresIn ?? TimeSpan.FromMinutes(_options.ApprovalDefaultTimeoutMinutes))
        };

        await _store.AddAsync(approval, cancellationToken);
        _logger.LogInformation("Created approval request '{Id}' of type '{Type}'", approval.Id, approval.Type);
        return approval;
    }

    public Task<ApprovalDecision> ApproveAsync(Guid requestId, string? decidedBy = null, string? reason = null, CancellationToken cancellationToken = default) =>
        _coordinator.ResolveAsync(requestId, true, decidedBy, reason, cancellationToken);

    public Task<ApprovalDecision> RejectAsync(Guid requestId, string? decidedBy = null, string? reason = null, CancellationToken cancellationToken = default) =>
        _coordinator.ResolveAsync(requestId, false, decidedBy, reason, cancellationToken);

    public async Task<ApprovalRequest> CancelAsync(Guid requestId, string? reason = null, CancellationToken cancellationToken = default)
    {
        var request = await _store.GetAsync(requestId, cancellationToken)
            ?? throw new AgentPlatformException($"Approval request '{requestId}' not found.", "APPROVAL_NOT_FOUND");

        if (request.Status == ApprovalStatus.Pending)
        {
            request.Status = ApprovalStatus.Cancelled;
            request.DecisionReason = reason;
            request.ResolvedAt = DateTime.UtcNow;
            await _store.UpdateAsync(request, cancellationToken);
            _store.Signal(requestId);
        }

        return request;
    }

    public Task<ApprovalRequest?> GetAsync(Guid requestId, CancellationToken cancellationToken = default) =>
        _store.GetAsync(requestId, cancellationToken);

    public Task<IReadOnlyList<ApprovalRequest>> GetPendingAsync(string? tenantId = null, CancellationToken cancellationToken = default) =>
        _store.GetPendingAsync(tenantId, cancellationToken);

    public Task<IReadOnlyList<ApprovalRequest>> ListAsync(string? tenantId = null, CancellationToken cancellationToken = default) =>
        _store.ListAsync(tenantId, cancellationToken);

    public async Task<ApprovalSummary> GetSummaryAsync(string? tenantId = null, CancellationToken cancellationToken = default)
    {
        var all = await _store.ListAsync(tenantId, cancellationToken);
        return new ApprovalSummary
        {
            Pending = all.Count(r => r.Status == ApprovalStatus.Pending),
            Approved = all.Count(r => r.Status == ApprovalStatus.Approved),
            Rejected = all.Count(r => r.Status == ApprovalStatus.Rejected),
            TimedOut = all.Count(r => r.Status == ApprovalStatus.TimedOut),
            Escalated = all.Count(r => r.Status == ApprovalStatus.Escalated)
        };
    }

    public async Task<IReadOnlyList<ApprovalRequest>> EscalateAsync(Guid requestId, string? escalatedTo = null, CancellationToken cancellationToken = default)
    {
        var request = await _store.GetAsync(requestId, cancellationToken)
            ?? throw new AgentPlatformException($"Approval request '{requestId}' not found.", "APPROVAL_NOT_FOUND");

        if (request.Status == ApprovalStatus.Pending)
        {
            request.EscalationLevel++;
            request.Status = ApprovalStatus.Escalated;
            request.EscalationTarget = escalatedTo ?? request.EscalationTarget;
            request.ResolvedAt = DateTime.UtcNow;
            await _store.UpdateAsync(request, cancellationToken);
            _store.Signal(requestId);
        }

        return await _store.GetPendingAsync(request.TenantId, cancellationToken);
    }

    public Task<ApprovalRequest> WaitForResolutionAsync(Guid requestId, TimeSpan? timeout = null, CancellationToken cancellationToken = default) =>
        _store.WaitAsync(requestId, timeout, cancellationToken);
}
