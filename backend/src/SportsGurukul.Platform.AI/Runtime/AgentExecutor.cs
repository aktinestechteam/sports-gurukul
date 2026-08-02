using System.Diagnostics;
using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.AI.Interfaces.HumanInTheLoop;
using SportsGurukul.Platform.AI.Interfaces.Model;
using SportsGurukul.Platform.AI.Interfaces.Observability;
using SportsGurukul.Platform.AI.Interfaces.Planning;
using SportsGurukul.Platform.AI.Interfaces.Runtime;
using SportsGurukul.Platform.AI.Interfaces.Security;
using SportsGurukul.Platform.AI.Interfaces.Streaming;
using SportsGurukul.Platform.AI.Interfaces.Tools;
using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Runtime;

public class AgentExecutor : IAgentExecutor
{
    private readonly IAgentRegistry _registry;
    private readonly IAgentPlanner _planner;
    private readonly IToolExecutor _toolExecutor;
    private readonly IAgentMemory _memory;
    private readonly IAgentLifecycleService _lifecycle;
    private readonly IReflectionService _reflection;
    private readonly ILanguageModelFactory _modelFactory;
    private readonly IApprovalService _approvalService;
    private readonly IPromptInjectionGuard _promptGuard;
    private readonly IAgentEventStream _stream;
    private readonly IMetricsCollector _metrics;
    private readonly IAuditLogger _audit;
    private readonly AIPlatformOptions _options;
    private readonly ILogger<AgentExecutor> _logger;

    public AgentExecutor(
        IAgentRegistry registry,
        IAgentPlanner planner,
        IToolExecutor toolExecutor,
        IAgentMemory memory,
        IAgentLifecycleService lifecycle,
        IReflectionService reflection,
        ILanguageModelFactory modelFactory,
        IApprovalService approvalService,
        IPromptInjectionGuard promptGuard,
        IAgentEventStream stream,
        IMetricsCollector metrics,
        IAuditLogger audit,
        AIPlatformOptions options,
        ILogger<AgentExecutor>? logger = null)
    {
        _registry = registry;
        _planner = planner;
        _toolExecutor = toolExecutor;
        _memory = memory;
        _lifecycle = lifecycle;
        _reflection = reflection;
        _modelFactory = modelFactory;
        _approvalService = approvalService;
        _promptGuard = promptGuard;
        _stream = stream;
        _metrics = metrics;
        _audit = audit;
        _options = options;
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<AgentExecutor>.Instance;
    }

    public async Task<AgentRunResult> ExecuteAsync(AgentRunRequest request, CancellationToken cancellationToken = default)
    {
        var definition = await _registry.GetAsync(request.AgentId, cancellationToken)
            ?? throw new AgentNotFoundException(request.AgentId);

        var session = await _lifecycle.StartAsync(request, cancellationToken);

        var context = new AgentContext
        {
            RunId = session.RunId,
            AgentId = request.AgentId,
            SessionId = request.SessionId,
            TenantId = request.TenantId,
            UserId = request.UserId,
            CorrelationId = request.CorrelationId,
            Definition = definition
        };

        var effectiveMaxIterations = request.Options?.MaxIterations ?? definition.MaxIterations;
        var effectiveMaxToolCalls = request.Options?.MaxToolCalls ?? definition.MaxToolCalls;
        var enableReflection = request.Options?.EnableReflection ?? (definition.EnableReflection && _options.EnableReflection);
        var enableSelfEvaluation = request.Options?.EnableSelfEvaluation ?? (definition.EnableSelfEvaluation && _options.EnableSelfEvaluation);

        using var timeoutCts = new CancellationTokenSource();
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var timeout = request.Options?.Timeout;
        if (timeout is not null)
        {
            timeoutCts.CancelAfter(timeout.Value);
        }

        var ct = linkedCts.Token;

        var stopwatch = Stopwatch.StartNew();
        var startedAt = DateTime.UtcNow;
        var usage = new ModelUsage();
        decimal cost = 0;

        try
        {
            await PublishAsync(AgentStreamEvent.Status(session.RunId, AgentState.Planning.ToString(), request.CorrelationId), ct);

            var goal = new PlanningGoal
            {
                Description = request.Goal,
                Input = request.Input,
                SessionId = request.SessionId,
                Metadata = new Dictionary<string, object?>
                {
                    ["sessionId"] = request.SessionId ?? string.Empty,
                    ["tenantId"] = request.TenantId ?? string.Empty
                }
            };

            var plan = await _planner.CreatePlanAsync(goal, context, ct);
            await _memory.WriteAsync(NewEntry(MemoryCategory.Episodic, "plan", $"Plan created with {plan.Steps.Count} steps.", request.SessionId, request.TenantId), ct);
            await PublishAsync(AgentStreamEvent.Plan(session.RunId, $"Plan: {string.Join(" -> ", plan.Steps.Select(s => s.Title))}", request.CorrelationId), ct);

            var results = new List<AgentTaskResult>();
            var pending = plan.Steps.ToList();
            var iteration = 0;
            var toolCalls = 0;

            while (pending.Count > 0)
            {
                ct.ThrowIfCancellationRequested();

                if (iteration >= effectiveMaxIterations || toolCalls >= effectiveMaxToolCalls)
                {
                    _logger.LogWarning("Run {RunId} reached execution limits. iterations={Iterations} toolCalls={ToolCalls}", session.RunId, iteration, toolCalls);
                    break;
                }

                var step = pending[0];
                pending.RemoveAt(0);
                step.State = TaskState.InProgress;
                iteration++;
                session.IterationCount = iteration;

                await PublishAsync(AgentStreamEvent.Message(session.RunId, $"Executing step {iteration}: {step.Title}", request.CorrelationId), ct);

                var taskResult = await ExecuteStepAsync(step, context, request, ct);
                results.Add(taskResult);

                if (step.ToolName is not null)
                {
                    toolCalls++;
                }

                await _memory.WriteAsync(NewEntry(MemoryCategory.Episodic, "step", $"{step.Title}: {(taskResult.Succeeded ? "OK" : "FAILED")}", request.SessionId, request.TenantId), ct);

                if (taskResult.ToolResult?.Usage is { } toolUsage)
                {
                    usage.PromptTokens += toolUsage.PromptTokens;
                    usage.CompletionTokens += toolUsage.CompletionTokens;
                }

                if (!taskResult.Succeeded)
                {
                    var reflection = await _reflection.ReflectAsync(new ReflectionRequest
                    {
                        PlanId = plan.Id.ToString(),
                        Goal = request.Goal,
                        CompletedSteps = results,
                        CurrentStep = step,
                        Insight = taskResult.Error
                    }, ct);

                    if (reflection.ShouldReplan)
                    {
                        plan = await _planner.ReplanAsync(plan, step.Id, reflection.Insight, context, ct);
                        pending = plan.Steps.Where(s => s.State == TaskState.Pending || s.State == TaskState.Blocked).ToList();
                        await PublishAsync(AgentStreamEvent.Plan(session.RunId, $"Replanned: {reflection.Insight}", request.CorrelationId), ct);
                    }
                    else
                    {
                        break;
                    }
                }

                if (enableReflection && iteration % Math.Max(1, _options.ReflectionFrequency) == 0)
                {
                    var reflection = await _reflection.ReflectAsync(new ReflectionRequest
                    {
                        PlanId = plan.Id.ToString(),
                        Goal = request.Goal,
                        CompletedSteps = results,
                        CurrentStep = step
                    }, ct);

                    if (reflection.ShouldStop)
                    {
                        _logger.LogInformation("Run {RunId} stopped by reflection.", session.RunId);
                        break;
                    }

                    if (reflection.ShouldReplan)
                    {
                        plan = await _planner.ReplanAsync(plan, step.Id, reflection.Insight, context, ct);
                        pending = plan.Steps.Where(s => s.State == TaskState.Pending || s.State == TaskState.Blocked).ToList();
                    }
                }
            }

            var answer = BuildAnswer(results);
            var completedCount = results.Count(r => r.Succeeded);
            var succeeded = completedCount == results.Count && results.Count > 0;

            SelfEvaluation? evaluation = null;
            if (enableSelfEvaluation)
            {
                evaluation = await _reflection.EvaluateAsync(new SelfEvaluationRequest
                {
                    RunId = session.RunId.ToString(),
                    Goal = request.Goal,
                    Tasks = results,
                    FinalAnswer = answer
                }, ct);
                await _memory.WriteAsync(NewEntry(MemoryCategory.Session, "evaluation", $"Score {evaluation.Score:F2} ({evaluation.Verdict})", request.SessionId, request.TenantId), ct);
            }

            stopwatch.Stop();
            var result = new AgentRunResult
            {
                RunId = session.RunId,
                AgentId = request.AgentId,
                Status = succeeded ? AgentState.Completed : AgentState.Failed,
                Answer = answer,
                Tasks = results,
                Usage = usage,
                Duration = stopwatch.Elapsed,
                IterationCount = iteration,
                StartedAt = startedAt,
                CompletedAt = DateTime.UtcNow,
                Error = succeeded ? null : "One or more steps failed."
            };

            await _lifecycle.CompleteAsync(session.RunId, result, ct);
            await _memory.WriteAsync(NewEntry(MemoryCategory.Episodic, "run", $"Run {session.RunId} completed as {result.Status}.", request.SessionId, request.TenantId), ct);

            _metrics.RecordAgent(request.AgentId, stopwatch.ElapsedMilliseconds, cost, usage, succeeded, succeeded ? null : result.Error);
            await _audit.AuditAsync("agent.run.completed", "agent", request.AgentId, request.UserId, request.TenantId, succeeded ? "Info" : "Error", $"Run {session.RunId} status={result.Status}", request.CorrelationId, ct);

            await PublishAsync(AgentStreamEvent.Status(session.RunId, result.Status.ToString(), request.CorrelationId), ct);
            await PublishAsync(AgentStreamEvent.Done(session.RunId, request.CorrelationId), ct);
            await _stream.CompleteAsync(session.RunId, ct);

            return result;
        }
        catch (OperationCanceledException)
        {
            await _lifecycle.CancelAsync(session.RunId, "Cancelled", CancellationToken.None);

            return new AgentRunResult
            {
                RunId = session.RunId,
                AgentId = request.AgentId,
                Status = AgentState.Cancelled,
                Tasks = [],
                StartedAt = startedAt,
                CompletedAt = DateTime.UtcNow,
                Duration = stopwatch.Elapsed
            };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Agent run {RunId} failed.", session.RunId);
            await _lifecycle.FailAsync(session.RunId, ex.Message, CancellationToken.None);
            await _audit.AuditAsync("agent.run.failed", "agent", request.AgentId, request.UserId, request.TenantId, "Error", ex.Message, request.CorrelationId, CancellationToken.None);

            _metrics.RecordAgent(request.AgentId, stopwatch.ElapsedMilliseconds, 0, usage, false, ex.Message);

            return new AgentRunResult
            {
                RunId = session.RunId,
                AgentId = request.AgentId,
                Status = AgentState.Failed,
                Tasks = [],
                StartedAt = startedAt,
                CompletedAt = DateTime.UtcNow,
                Duration = stopwatch.Elapsed,
                Error = ex.Message
            };
        }
        finally
        {
            await PublishAsync(AgentStreamEvent.Done(session.RunId, request.CorrelationId), CancellationToken.None);
            await _stream.CompleteAsync(session.RunId, CancellationToken.None);
        }
    }

    private async Task<AgentTaskResult> ExecuteStepAsync(PlanStep step, AgentContext context, AgentRunRequest request, CancellationToken ct)
    {
        if (step.ToolName is not null)
        {
            var assessment = await _promptGuard.InspectAsync($"{step.Title} {string.Join(' ', step.Arguments.Values)}", ct);
            if (assessment.RiskLevel == SecurityRiskLevel.Blocked)
            {
                return new AgentTaskResult
                {
                    Step = step,
                    Succeeded = false,
                    Error = $"Prompt injection guard blocked step: {string.Join(", ", assessment.Indicators)}"
                };
            }

            if (step.RequiresApproval)
            {
                var approval = await RequestStepApprovalAsync(step, context, request, ct);
                if (approval.Status != ApprovalStatus.Approved)
                {
                    return new AgentTaskResult
                    {
                        Step = step,
                        Succeeded = false,
                        Error = $"Approval rejected: {approval.DecisionReason}"
                    };
                }
            }

            var call = new ToolCall
            {
                Id = Guid.NewGuid(),
                ToolName = step.ToolName,
                Arguments = step.Arguments,
                Status = ToolCallStatus.Running
            };

            var toolResult = await _toolExecutor.ExecuteAsync(call, new ToolExecutionContext
            {
                AgentId = request.AgentId,
                RunId = context.RunId.ToString(),
                SessionId = request.SessionId,
                TenantId = request.TenantId,
                UserId = request.UserId,
                CorrelationId = request.CorrelationId
            }, ct);

            step.State = toolResult.Success ? TaskState.Completed : TaskState.Failed;
            step.Result = toolResult.Data?.ToString();

            return new AgentTaskResult
            {
                Step = step,
                ToolResult = toolResult,
                Succeeded = toolResult.Success,
                Error = toolResult.Error
            };
        }

        var model = _modelFactory.Create(context.Definition.Provider, context.Definition.Model);
        var response = await model.GenerateAsync(
            new List<ModelMessage>
            {
                ModelMessage.System(context.Definition.SystemPrompt ?? "You are a helpful agent."),
                ModelMessage.User($"{step.Title}\n\nInput: {request.Input}")
            },
            new ModelOptions { Tools = null },
            ct);

        step.State = TaskState.Completed;
        step.Result = response.Content;

        return new AgentTaskResult
        {
            Step = step,
            ModelOutput = response.Content,
            Succeeded = true
        };
    }

    private async Task<ApprovalRequest> RequestStepApprovalAsync(PlanStep step, AgentContext context, AgentRunRequest request, CancellationToken ct)
    {
        var approval = await _approvalService.RequestAsync(new CreateApprovalRequest
        {
            Type = ApprovalType.AgentOutput,
            Title = $"Approve agent step: {step.Title}",
            Description = step.Description,
            Action = $"Execute step '{step.Title}' of run {context.RunId}",
            RequestedBy = request.UserId,
            RequiredRole = context.Definition.ApprovalPolicy,
            TenantId = request.TenantId,
            CorrelationId = request.CorrelationId,
            RunId = context.RunId.ToString(),
            ExpiresIn = TimeSpan.FromMinutes(_options.ApprovalDefaultTimeoutMinutes),
            EscalationThreshold = TimeSpan.FromMinutes(_options.ApprovalEscalationThresholdMinutes)
        }, ct);

        await PublishAsync(AgentStreamEvent.Approval(context.RunId, $"Approval required: {approval.Id}", request.CorrelationId), ct);
        return await _approvalService.WaitForResolutionAsync(approval.Id, cancellationToken: ct);
    }

    private static string BuildAnswer(IReadOnlyList<AgentTaskResult> results)
    {
        var parts = results
            .Where(r => r.Succeeded)
            .Select(r => r.ModelOutput ?? r.ToolResult?.Data?.ToString())
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .ToList();

        return parts.Count == 0 ? "No successful output produced." : string.Join("\n", parts);
    }

    private static MemoryEntry NewEntry(MemoryCategory category, string subject, string content, string? sessionId, string? tenantId) => new()
    {
        Category = category,
        Subject = subject,
        Content = content,
        SessionId = sessionId,
        TenantId = tenantId,
        Importance = MemoryImportance.Medium
    };

    private async Task PublishAsync(AgentStreamEvent @event, CancellationToken ct)
    {
        if (_options.EnableStreaming)
        {
            await _stream.PublishAsync(@event, ct);
        }
    }
}
