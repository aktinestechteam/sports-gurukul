using System.Diagnostics;
using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.AI.Interfaces.HumanInTheLoop;
using SportsGurukul.Platform.AI.Interfaces.Observability;
using SportsGurukul.Platform.AI.Interfaces.Security;
using SportsGurukul.Platform.AI.Interfaces.Streaming;
using SportsGurukul.Platform.AI.Interfaces.Tools;
using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Tools;

public class DefaultToolExecutor : IToolExecutor
{
    private readonly IToolRegistry _registry;
    private readonly IToolAuthorization _authorization;
    private readonly IApprovalService _approvalService;
    private readonly IAgentEventStream _stream;
    private readonly IMetricsCollector _metrics;
    private readonly IAuditLogger _audit;
    private readonly AIPlatformOptions _options;
    private readonly ILogger<DefaultToolExecutor> _logger;

    public DefaultToolExecutor(
        IToolRegistry registry,
        IToolAuthorization authorization,
        IApprovalService approvalService,
        IAgentEventStream stream,
        IMetricsCollector metrics,
        IAuditLogger audit,
        AIPlatformOptions options,
        ILogger<DefaultToolExecutor>? logger = null)
    {
        _registry = registry;
        _authorization = authorization;
        _approvalService = approvalService;
        _stream = stream;
        _metrics = metrics;
        _audit = audit;
        _options = options;
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<DefaultToolExecutor>.Instance;
    }

    public async Task<ToolResult> ExecuteAsync(string toolName, IDictionary<string, object?> arguments, ToolExecutionContext context, CancellationToken cancellationToken = default)
    {
        var call = new ToolCall
        {
            ToolName = toolName,
            Arguments = arguments ?? new Dictionary<string, object?>(),
            Status = ToolCallStatus.Pending
        };

        return await ExecuteAsync(call, context, cancellationToken);
    }

    public async Task<ToolResult> ExecuteAsync(ToolCall call, ToolExecutionContext context, CancellationToken cancellationToken = default)
    {
        var tool = await _registry.GetAsync(call.ToolName, cancellationToken)
            ?? throw new ToolNotFoundException(call.ToolName);

        var runGuid = Guid.TryParse(context.RunId, out var parsedRunId) ? parsedRunId : Guid.Empty;

        var stopwatch = Stopwatch.StartNew();
        call.Status = ToolCallStatus.Running;
        call.StartedAt = DateTime.UtcNow;

        await PublishStreamAsync(AgentStreamEvent.ToolCall(runGuid, call.ToolName, context.CorrelationId), cancellationToken);

        var authorization = await _authorization.AuthorizeAsync(tool, context, cancellationToken);

        if (!authorization.Allowed)
        {
            stopwatch.Stop();
            call.Status = ToolCallStatus.Denied;
            call.DurationMs = stopwatch.ElapsedMilliseconds;

            _metrics.RecordTool(tool.Name, context.AgentId, stopwatch.ElapsedMilliseconds, false, authorization.Reason, denied: true);
            await _audit.AuditAsync("tool.denied", "tool", tool.Name, context.UserId, context.TenantId, "Warning", authorization.Reason, context.CorrelationId, cancellationToken);

            throw new ToolAuthorizationException(tool.Name, authorization.Reason ?? "Not authorized");
        }

        Guid? approvalRequestId = null;
        if (authorization.RequiresApproval || tool.RequiresApproval)
        {
            call.Status = ToolCallStatus.AwaitingApproval;

            var approval = await _approvalService.RequestAsync(new CreateApprovalRequest
            {
                Type = ApprovalType.ToolCall,
                Title = $"Approve tool call: {tool.Name}",
                Description = tool.Description,
                Action = $"Invoke '{tool.Name}' on behalf of agent {context.AgentId}",
                RequestedBy = context.UserId,
                RequiredRole = tool.Permission,
                TenantId = context.TenantId,
                CorrelationId = context.CorrelationId,
                RunId = context.RunId,
                ExpiresIn = TimeSpan.FromMinutes(_options.ApprovalDefaultTimeoutMinutes),
                EscalationThreshold = TimeSpan.FromMinutes(_options.ApprovalEscalationThresholdMinutes)
            }, cancellationToken);

            approvalRequestId = approval.Id;
            call.ApprovalRequestId = approval.Id;

            await PublishStreamAsync(AgentStreamEvent.Approval(runGuid, $"Approval required: {approval.Id}", context.CorrelationId), cancellationToken);

            approval = await _approvalService.WaitForResolutionAsync(approval.Id, cancellationToken: cancellationToken);
            if (approval.Status != ApprovalStatus.Approved)
            {
                stopwatch.Stop();
                call.Status = ToolCallStatus.Denied;
                call.DurationMs = stopwatch.ElapsedMilliseconds;

                _metrics.RecordTool(tool.Name, context.AgentId, stopwatch.ElapsedMilliseconds, false, approval.DecisionReason, denied: true);

                return ToolResult.Fail($"Tool call not approved: {approval.DecisionReason}", stopwatch.ElapsedMilliseconds);
            }
        }

        var timeout = tool.TimeoutSeconds is > 0 ? TimeSpan.FromSeconds(tool.TimeoutSeconds.Value) : TimeSpan.FromSeconds(_options.DefaultToolTimeoutSeconds);
        using var timeoutCts = new CancellationTokenSource(timeout);
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

        var retryMax = Math.Max(0, _options.ToolRetryMax);
        ToolResult? lastResult = null;

        for (var attempt = 0; attempt <= retryMax; attempt++)
        {
            call.RetryCount = attempt;

            try
            {
                lastResult = await tool.ExecuteAsync(call, linkedCts.Token);
                if (lastResult.Success)
                {
                    break;
                }
            }
            catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
            {
                stopwatch.Stop();
                call.Status = ToolCallStatus.TimedOut;
                call.DurationMs = stopwatch.ElapsedMilliseconds;

                _metrics.RecordTool(tool.Name, context.AgentId, stopwatch.ElapsedMilliseconds, false, "timeout");
                _logger.LogWarning("Tool '{Tool}' timed out after {Timeout}s.", tool.Name, timeout.TotalSeconds);

                return ToolResult.Fail($"Tool '{tool.Name}' timed out after {timeout.TotalSeconds:0}s.", stopwatch.ElapsedMilliseconds);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
        }

        stopwatch.Stop();
        call.Status = lastResult?.Success == true ? ToolCallStatus.Succeeded : ToolCallStatus.Failed;
        call.DurationMs = stopwatch.ElapsedMilliseconds;
        call.CompletedAt = DateTime.UtcNow;

        _metrics.RecordTool(tool.Name, context.AgentId, stopwatch.ElapsedMilliseconds, lastResult?.Success == true, lastResult?.Error);
        await _audit.AuditAsync(
            lastResult?.Success == true ? "tool.succeeded" : "tool.failed",
            "tool",
            tool.Name,
            context.UserId,
            context.TenantId,
            lastResult?.Success == true ? "Info" : "Error",
            lastResult?.Error ?? $"Invoked with {call.Arguments.Count} argument(s)",
            context.CorrelationId,
            cancellationToken);

        await PublishStreamAsync(AgentStreamEvent.ToolResult(runGuid, $"{tool.Name}: {(lastResult?.Success == true ? "OK" : "FAILED")}", context.CorrelationId), cancellationToken);

        return lastResult ?? ToolResult.Fail("Tool returned no result.", stopwatch.ElapsedMilliseconds);
    }

    private async Task PublishStreamAsync(AgentStreamEvent @event, CancellationToken ct)
    {
        if (_options.EnableStreaming)
        {
            await _stream.PublishAsync(@event, ct);
        }
    }
}
