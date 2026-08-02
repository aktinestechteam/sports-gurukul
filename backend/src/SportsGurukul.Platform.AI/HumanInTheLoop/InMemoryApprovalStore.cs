using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.AI.Interfaces.HumanInTheLoop;
using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.HumanInTheLoop;

public class InMemoryApprovalStore : IApprovalStore
{
    private readonly ConcurrentDictionary<Guid, ApprovalRequest> _requests = new();
    private readonly ConcurrentDictionary<Guid, TaskCompletionSource<ApprovalRequest>> _waiters = new();
    private readonly ILogger<InMemoryApprovalStore> _logger;

    public InMemoryApprovalStore(ILogger<InMemoryApprovalStore>? logger = null)
    {
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<InMemoryApprovalStore>.Instance;
    }

    public Task<ApprovalRequest> AddAsync(ApprovalRequest request, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _requests[request.Id] = request;
        return Task.FromResult(request);
    }

    public Task<ApprovalRequest?> GetAsync(Guid requestId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_requests.TryGetValue(requestId, out var request) ? request : null);
    }

    public Task<IReadOnlyList<ApprovalRequest>> ListAsync(string? tenantId = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var results = _requests.Values
            .Where(r => tenantId is null || r.TenantId == tenantId)
            .OrderByDescending(r => r.CreatedAt)
            .ToList();
        return Task.FromResult<IReadOnlyList<ApprovalRequest>>(results);
    }

    public Task<IReadOnlyList<ApprovalRequest>> GetPendingAsync(string? tenantId = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var results = _requests.Values
            .Where(r => r.Status == ApprovalStatus.Pending)
            .Where(r => tenantId is null || r.TenantId == tenantId)
            .OrderByDescending(r => r.CreatedAt)
            .ToList();
        return Task.FromResult<IReadOnlyList<ApprovalRequest>>(results);
    }

    public Task UpdateAsync(ApprovalRequest request, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _requests[request.Id] = request;
        return Task.CompletedTask;
    }

    public async Task<ApprovalRequest> WaitAsync(Guid requestId, TimeSpan? timeout, CancellationToken cancellationToken = default)
    {
        if (_requests.TryGetValue(requestId, out var existing) && existing.Status != ApprovalStatus.Pending)
        {
            return existing;
        }

        var tcs = _waiters.GetOrAdd(requestId, _ => new TaskCompletionSource<ApprovalRequest>(TaskCreationOptions.RunContinuationsAsynchronously));

        if (_requests.TryGetValue(requestId, out var current) && current.Status != ApprovalStatus.Pending)
        {
            _waiters.TryRemove(requestId, out _);
            tcs.TrySetResult(current);
        }

        var waiting = tcs.Task;
        var delay = timeout is null ? Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken) : Task.Delay(timeout.Value, cancellationToken);

        var completed = await Task.WhenAny(waiting, delay).ConfigureAwait(false);
        if (completed == delay)
        {
            return _requests.TryGetValue(requestId, out var cur) ? cur : new ApprovalRequest { Id = requestId, Status = ApprovalStatus.TimedOut };
        }

        return await waiting.ConfigureAwait(false);
    }

    public void Signal(Guid requestId)
    {
        if (_waiters.TryRemove(requestId, out var tcs) && _requests.TryGetValue(requestId, out var request))
        {
            tcs.TrySetResult(request);
        }
    }
}
