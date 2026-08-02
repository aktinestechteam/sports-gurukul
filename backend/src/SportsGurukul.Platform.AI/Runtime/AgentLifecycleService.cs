using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.AI.Interfaces.Runtime;
using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Runtime;

public class AgentLifecycleService : IAgentLifecycleService
{
    private readonly ConcurrentDictionary<Guid, AgentSession> _sessions = new();
    private readonly ConcurrentDictionary<Guid, AgentRunResult> _results = new();
    private readonly ILogger<AgentLifecycleService> _logger;

    public AgentLifecycleService(ILogger<AgentLifecycleService>? logger = null)
    {
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<AgentLifecycleService>.Instance;
    }

    public Task<AgentSession> StartAsync(AgentRunRequest request, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var session = new AgentSession
        {
            RunId = Guid.NewGuid(),
            AgentId = request.AgentId,
            State = AgentState.Planning,
            Goal = request.Goal,
            SessionId = request.SessionId,
            TenantId = request.TenantId
        };

        _sessions[session.RunId] = session;
        _logger.LogInformation("Agent run started. RunId={RunId} Agent={Agent}", session.RunId, request.AgentId);
        return Task.FromResult(session);
    }

    public Task CompleteAsync(Guid runId, AgentRunResult result, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (_sessions.TryGetValue(runId, out var session))
        {
            session.State = AgentState.Completed;
            session.EndedAt = DateTime.UtcNow;
            session.IterationCount = result.IterationCount;
        }

        _results[runId] = result;
        return Task.CompletedTask;
    }

    public Task FailAsync(Guid runId, string reason, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (_sessions.TryGetValue(runId, out var session))
        {
            session.State = AgentState.Failed;
            session.EndedAt = DateTime.UtcNow;
            session.LastError = reason;
        }

        return Task.CompletedTask;
    }

    public Task CancelAsync(Guid runId, string? reason = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (_sessions.TryGetValue(runId, out var session))
        {
            session.State = AgentState.Cancelled;
            session.EndedAt = DateTime.UtcNow;
            session.LastError = reason;
        }

        return Task.CompletedTask;
    }

    public Task PauseAsync(Guid runId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (_sessions.TryGetValue(runId, out var session))
        {
            session.State = AgentState.Paused;
        }

        return Task.CompletedTask;
    }

    public Task<AgentSession?> GetSessionAsync(Guid runId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_sessions.TryGetValue(runId, out var session) ? session : null);
    }

    public Task<IReadOnlyList<AgentSession>> GetActiveSessionsAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var active = _sessions.Values.Where(s => s.State is AgentState.Planning or AgentState.Executing or AgentState.WaitingForApproval or AgentState.Paused).ToList();
        return Task.FromResult<IReadOnlyList<AgentSession>>(active);
    }

    public Task<AgentRunResult?> GetResultAsync(Guid runId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_results.TryGetValue(runId, out var result) ? result : null);
    }
}
