using Microsoft.Extensions.Logging.Abstractions;
using SportsGurukul.Platform.AI.HumanInTheLoop;
using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Tests;

public class ApprovalTests
{
    private readonly InMemoryApprovalStore _store = new();
    private readonly ApprovalCoordinator _coordinator;
    private readonly ApprovalService _service;

    public ApprovalTests()
    {
        _coordinator = new ApprovalCoordinator(_store, NullLogger<ApprovalCoordinator>.Instance);
        _service = new ApprovalService(
            _store,
            _coordinator,
            new AIPlatformOptions { ApprovalDefaultTimeoutMinutes = 60 });
    }

    [Fact]
    public async Task RequestAsync_CreatesPendingRequest()
    {
        var request = await _service.RequestAsync(new CreateApprovalRequest
        {
            Title = "Approve finance operation",
            Type = ApprovalType.ToolCall,
            TenantId = "t1"
        });

        Assert.NotEqual(Guid.Empty, request.Id);
        Assert.Equal(ApprovalStatus.Pending, request.Status);
        Assert.NotNull(request.ExpiresAt);
    }

    [Fact]
    public async Task ApproveAsync_ResolvesAndSignalsWaiter()
    {
        var request = await _service.RequestAsync(new CreateApprovalRequest { Title = "Approve payment" });

        var waiting = _service.WaitForResolutionAsync(request.Id, TimeSpan.FromSeconds(5));

        var decision = await _service.ApproveAsync(request.Id, decidedBy: "manager-1", reason: "Looks good");

        var resolved = await waiting;

        Assert.True(decision.Approved);
        Assert.Equal(ApprovalStatus.Approved, resolved.Status);
        Assert.Equal("manager-1", resolved.ApproverId);
    }

    [Fact]
    public async Task RejectAsync_ResolvesAsRejected()
    {
        var request = await _service.RequestAsync(new CreateApprovalRequest { Title = "Reject me" });

        var decision = await _service.RejectAsync(request.Id, reason: "Not allowed");

        var stored = await _service.GetAsync(request.Id);

        Assert.False(decision.Approved);
        Assert.NotNull(stored);
        Assert.Equal(ApprovalStatus.Rejected, stored!.Status);
        Assert.Equal("Not allowed", stored.DecisionReason);
    }

    [Fact]
    public async Task CancelAsync_CancelsPendingRequest()
    {
        var request = await _service.RequestAsync(new CreateApprovalRequest { Title = "Cancel me" });

        var cancelled = await _service.CancelAsync(request.Id, "No longer needed");

        Assert.Equal(ApprovalStatus.Cancelled, cancelled.Status);
    }

    [Fact]
    public async Task Coordinator_MarksExpiredAsTimedOut()
    {
        var request = await _service.RequestAsync(new CreateApprovalRequest
        {
            Title = "Expire me",
            ExpiresIn = TimeSpan.FromMilliseconds(10)
        });

        request.ExpiresAt = DateTime.UtcNow.AddSeconds(-1);
        await _store.UpdateAsync(request);

        var timedOut = await _coordinator.EvaluateTimeoutsAsync();

        Assert.Contains(timedOut, r => r.Id == request.Id && r.Status == ApprovalStatus.TimedOut);
    }

    [Fact]
    public async Task GetSummaryAsync_CountsByStatus()
    {
        await _service.RequestAsync(new CreateApprovalRequest { Title = "Pending one" });
        var second = await _service.RequestAsync(new CreateApprovalRequest { Title = "Approve me" });
        await _service.ApproveAsync(second.Id);

        var summary = await _service.GetSummaryAsync();

        Assert.Equal(1, summary.Pending);
        Assert.Equal(1, summary.Approved);
    }

    [Fact]
    public async Task EscalateAsync_EscalatesPendingRequest()
    {
        var request = await _service.RequestAsync(new CreateApprovalRequest { Title = "Escalate me" });

        await _service.EscalateAsync(request.Id, "director-1");

        var stored = await _service.GetAsync(request.Id);
        Assert.NotNull(stored);
        Assert.Equal(ApprovalStatus.Escalated, stored!.Status);
    }
}
