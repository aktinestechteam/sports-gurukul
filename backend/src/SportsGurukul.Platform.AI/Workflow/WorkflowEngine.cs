using System.Diagnostics;
using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.AI.Interfaces.HumanInTheLoop;
using SportsGurukul.Platform.AI.Interfaces.Observability;
using SportsGurukul.Platform.AI.Interfaces.Streaming;
using SportsGurukul.Platform.AI.Interfaces.Tools;
using SportsGurukul.Platform.AI.Interfaces.Workflow;
using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Workflow;

public class WorkflowEngine : IWorkflowEngine
{
    private readonly IWorkflowStore _store;
    private readonly IToolExecutor _toolExecutor;
    private readonly IApprovalService _approvalService;
    private readonly IConditionEvaluator _conditionEvaluator;
    private readonly IAgentEventStream _stream;
    private readonly IMetricsCollector _metrics;
    private readonly AIPlatformOptions _options;
    private readonly ILogger<WorkflowEngine> _logger;

    public WorkflowEngine(
        IWorkflowStore store,
        IToolExecutor toolExecutor,
        IApprovalService approvalService,
        IConditionEvaluator conditionEvaluator,
        IAgentEventStream stream,
        IMetricsCollector metrics,
        AIPlatformOptions options,
        ILogger<WorkflowEngine>? logger = null)
    {
        _store = store;
        _toolExecutor = toolExecutor;
        _approvalService = approvalService;
        _conditionEvaluator = conditionEvaluator;
        _stream = stream;
        _metrics = metrics;
        _options = options;
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<WorkflowEngine>.Instance;
    }

    public async Task<WorkflowExecution> StartAsync(WorkflowDefinition definition, WorkflowStartOptions? options = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(definition);
        if (string.IsNullOrWhiteSpace(definition.Name))
        {
            throw new AgentPlatformException("Workflow definition must have a name.", "WORKFLOW_INVALID");
        }

        var execution = new WorkflowExecution
        {
            DefinitionName = definition.Name,
            DefinitionVersion = definition.Version,
            Status = WorkflowStatus.Running,
            Definition = definition,
            TenantId = options?.TenantId,
            CreatedBy = options?.CreatedBy,
            CorrelationId = options?.CorrelationId,
            State = options?.InitialState is null ? new() : new Dictionary<string, object?>(options.InitialState),
            StartedAt = DateTime.UtcNow
        };

        if (options?.Input is not null)
        {
            foreach (var (key, value) in options.Input)
            {
                execution.State[$"input.{key}"] = value;
            }
        }

        foreach (var stepDef in definition.Steps)
        {
            execution.Steps.Add(new WorkflowStepExecution { StepId = stepDef.Id, Name = stepDef.Name });
        }

        await _store.SaveAsync(execution, cancellationToken);
        await PublishStatusAsync(execution, cancellationToken);

        var stopwatch = Stopwatch.StartNew();
        await RunAsync(execution, cancellationToken);
        stopwatch.Stop();

        _metrics.RecordWorkflow(execution.DefinitionName, stopwatch.ElapsedMilliseconds, execution.Status == WorkflowStatus.Completed);
        await PublishStatusAsync(execution, cancellationToken);
        return execution;
    }

    public async Task<WorkflowExecution> ResumeAsync(Guid executionId, CancellationToken cancellationToken = default)
    {
        var execution = await _store.GetAsync(executionId, cancellationToken)
            ?? throw new WorkflowExecutionNotFoundException(executionId);

        if (execution.Status is WorkflowStatus.Completed or WorkflowStatus.Cancelled or WorkflowStatus.Failed)
        {
            return execution;
        }

        await RunAsync(execution, cancellationToken);
        await PublishStatusAsync(execution, cancellationToken);
        return execution;
    }

    public async Task<WorkflowExecution> CancelAsync(Guid executionId, string? reason = null, CancellationToken cancellationToken = default)
    {
        var execution = await _store.GetAsync(executionId, cancellationToken)
            ?? throw new WorkflowExecutionNotFoundException(executionId);

        if (execution.Status is WorkflowStatus.Completed or WorkflowStatus.Cancelled)
        {
            return execution;
        }

        execution.Status = WorkflowStatus.Cancelled;
        execution.CompletedAt = DateTime.UtcNow;
        execution.FailureReason = reason;

        foreach (var step in execution.Steps.Where(s =>
                     s.Status is WorkflowStepStatus.Pending or WorkflowStepStatus.Ready or WorkflowStepStatus.Running or WorkflowStepStatus.WaitingForApproval))
        {
            step.Status = WorkflowStepStatus.Cancelled;
            step.CompletedAt = DateTime.UtcNow;
        }

        await _store.SaveAsync(execution, cancellationToken);
        _metrics.RecordWorkflow(execution.DefinitionName, 0, false, "cancelled");
        return execution;
    }

    public Task<WorkflowExecution?> GetAsync(Guid executionId, CancellationToken cancellationToken = default) =>
        _store.GetAsync(executionId, cancellationToken);

    public async Task<WorkflowCheckpoint> CheckpointAsync(Guid executionId, CancellationToken cancellationToken = default)
    {
        var execution = await _store.GetAsync(executionId, cancellationToken)
            ?? throw new WorkflowExecutionNotFoundException(executionId);

        var checkpoint = new WorkflowCheckpoint
        {
            ExecutionId = execution.Id,
            State = new Dictionary<string, object?>(execution.State),
            Steps = execution.Steps.Select(CloneStep).ToList()
        };

        await _store.SaveCheckpointAsync(checkpoint, cancellationToken);
        return checkpoint;
    }

    public Task<IReadOnlyList<WorkflowExecution>> ListAsync(Guid? tenantId = null, CancellationToken cancellationToken = default) =>
        _store.ListAsync(tenantId, cancellationToken);

    private async Task RunAsync(WorkflowExecution execution, CancellationToken cancellationToken)
    {
        var definition = execution.Definition
            ?? throw new AgentPlatformException($"Workflow execution {execution.Id} has no definition.", "WORKFLOW_DEFINITION_MISSING");

        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (execution.Status is WorkflowStatus.Cancelled or WorkflowStatus.Completed or WorkflowStatus.Failed)
            {
                return;
            }

            await ResolveApprovalsAsync(execution, definition, cancellationToken);
            if (execution.Status is WorkflowStatus.Failed or WorkflowStatus.Completed or WorkflowStatus.Cancelled)
            {
                return;
            }

            var ready = GetReadySteps(execution, definition);

            if (ready.Count == 0)
            {
                if (execution.Steps.Any(s => s.Status == WorkflowStepStatus.WaitingForApproval))
                {
                    execution.Status = WorkflowStatus.WaitingForApproval;
                    await _store.SaveAsync(execution, cancellationToken);
                    return;
                }

                if (await CompleteParallelParentsAsync(execution, definition, cancellationToken))
                {
                    continue;
                }

                if (execution.Steps.All(s => IsTerminalSuccess(s.Status)))
                {
                    execution.Status = WorkflowStatus.Completed;
                    execution.CompletedAt = DateTime.UtcNow;
                    await _store.SaveAsync(execution, cancellationToken);
                    return;
                }

                if (execution.Steps.Any(s => s.Status == WorkflowStepStatus.Failed))
                {
                    var failed = execution.Steps.First(s => s.Status == WorkflowStepStatus.Failed);
                    await FailExecutionAsync(execution, definition, failed, cancellationToken);
                    return;
                }

                execution.Status = WorkflowStatus.Failed;
                execution.CompletedAt = DateTime.UtcNow;
                execution.FailureReason = "Workflow cannot progress (dependency cycle or blocked steps).";
                await _store.SaveAsync(execution, cancellationToken);
                return;
            }

            var runParallel = _options.RunWorkflowStepsInParallel || ready.Count > 1;
            if (runParallel)
            {
                await Task.WhenAll(ready.Select(stepDef => ExecuteStepAsync(execution, definition, stepDef, cancellationToken)));
            }
            else
            {
                foreach (var stepDef in ready)
                {
                    await ExecuteStepAsync(execution, definition, stepDef, cancellationToken);
                    if (execution.Status == WorkflowStatus.WaitingForApproval)
                    {
                        await _store.SaveAsync(execution, cancellationToken);
                        return;
                    }

                    if (execution.Status == WorkflowStatus.Failed)
                    {
                        return;
                    }
                }
            }

            await _store.SaveAsync(execution, cancellationToken);
            await CheckpointAsync(execution.Id, cancellationToken);
        }
    }

    private async Task ResolveApprovalsAsync(WorkflowExecution execution, WorkflowDefinition definition, CancellationToken cancellationToken)
    {
        var waiting = execution.Steps.Where(s => s.Status == WorkflowStepStatus.WaitingForApproval).ToList();
        if (waiting.Count == 0)
        {
            return;
        }

        foreach (var step in waiting)
        {
            if (step.ApprovalRequestId is null || !Guid.TryParse(step.ApprovalRequestId, out var requestId))
            {
                step.Status = WorkflowStepStatus.Ready;
                continue;
            }

            var approval = await _approvalService.GetAsync(requestId, cancellationToken);
            if (approval is null)
            {
                step.Status = WorkflowStepStatus.Ready;
                continue;
            }

            switch (approval.Status)
            {
                case ApprovalStatus.Approved:
                    var approvedDef = definition.Steps.FirstOrDefault(d => d.Id == step.StepId);
                    if (approvedDef?.Type == WorkflowStepType.Approval)
                    {
                        step.Status = WorkflowStepStatus.Succeeded;
                        step.CompletedAt = DateTime.UtcNow;
                    }
                    else
                    {
                        step.Status = WorkflowStepStatus.Ready;
                    }

                    break;
                case ApprovalStatus.Rejected:
                case ApprovalStatus.Cancelled:
                    step.Status = WorkflowStepStatus.Failed;
                    step.Error = approval.DecisionReason ?? approval.Status.ToString();
                    step.CompletedAt = DateTime.UtcNow;
                    await FailExecutionAsync(execution, definition, step, cancellationToken);
                    return;
                default:
                    break;
            }
        }

        if (execution.Status == WorkflowStatus.WaitingForApproval
            && !execution.Steps.Any(s => s.Status == WorkflowStepStatus.WaitingForApproval))
        {
            execution.Status = WorkflowStatus.Running;
        }
    }

    private List<WorkflowStepDefinition> GetReadySteps(WorkflowExecution execution, WorkflowDefinition definition)
    {
        var ready = new List<WorkflowStepDefinition>();

        foreach (var stepDef in definition.Steps)
        {
            var step = execution.Steps.FirstOrDefault(s => s.StepId == stepDef.Id);
            if (step is null)
            {
                continue;
            }

            if (step.Status is not (WorkflowStepStatus.Pending or WorkflowStepStatus.Ready))
            {
                continue;
            }

            if (!DependenciesSatisfied(execution, stepDef))
            {
                continue;
            }

            if (stepDef.Condition is not null && !_conditionEvaluator.Evaluate(stepDef.Condition, execution.State))
            {
                step.Status = WorkflowStepStatus.Skipped;
                step.CompletedAt = DateTime.UtcNow;
                continue;
            }

            ready.Add(stepDef);
        }

        return ready;
    }

    private static bool DependenciesSatisfied(WorkflowExecution execution, WorkflowStepDefinition stepDef)
    {
        foreach (var dep in stepDef.DependsOn)
        {
            var depStep = execution.Steps.FirstOrDefault(s => s.StepId == dep);
            if (depStep is null || !IsTerminalSuccess(depStep.Status))
            {
                return false;
            }
        }

        return true;
    }

    private async Task ExecuteStepAsync(WorkflowExecution execution, WorkflowDefinition definition, WorkflowStepDefinition stepDef, CancellationToken cancellationToken)
    {
        var step = execution.Steps.First(s => s.StepId == stepDef.Id);
        step.Status = WorkflowStepStatus.Running;
        step.StartedAt = DateTime.UtcNow;
        step.Input = stepDef.ToolArguments;

        switch (stepDef.Type)
        {
            case WorkflowStepType.Task:
                await ExecuteTaskStepAsync(execution, definition, stepDef, step, cancellationToken);
                break;
            case WorkflowStepType.Approval:
                await RequestApprovalAsync(execution, stepDef, step, cancellationToken);
                break;
            case WorkflowStepType.Parallel:
                foreach (var branchId in stepDef.BranchStepIds)
                {
                    var branch = execution.Steps.FirstOrDefault(s => s.StepId == branchId);
                    if (branch is not null && branch.Status == WorkflowStepStatus.Pending)
                    {
                        branch.Status = WorkflowStepStatus.Ready;
                    }
                }

                step.Status = WorkflowStepStatus.Running;
                break;
            case WorkflowStepType.Condition:
                step.Status = _conditionEvaluator.Evaluate(stepDef.Condition, execution.State)
                    ? WorkflowStepStatus.Succeeded
                    : WorkflowStepStatus.Skipped;
                step.CompletedAt = DateTime.UtcNow;
                break;
            case WorkflowStepType.Wait:
                var seconds = stepDef.ToolArguments.TryGetValue("seconds", out var raw) && int.TryParse(raw?.ToString(), out var parsed) ? parsed : 0;
                if (seconds > 0)
                {
                    await Task.Delay(TimeSpan.FromSeconds(seconds), cancellationToken);
                }

                step.Status = WorkflowStepStatus.Succeeded;
                step.CompletedAt = DateTime.UtcNow;
                break;
            default:
                step.Status = WorkflowStepStatus.Skipped;
                step.CompletedAt = DateTime.UtcNow;
                break;
        }

        await PublishStatusAsync(execution, cancellationToken);
    }

    private async Task ExecuteTaskStepAsync(WorkflowExecution execution, WorkflowDefinition definition, WorkflowStepDefinition stepDef, WorkflowStepExecution step, CancellationToken cancellationToken)
    {
        if (stepDef.RequiresApproval)
        {
            await RequestApprovalAsync(execution, stepDef, step, cancellationToken);
            return;
        }

        if (string.IsNullOrWhiteSpace(stepDef.ToolName))
        {
            step.Status = WorkflowStepStatus.Skipped;
            step.CompletedAt = DateTime.UtcNow;
            return;
        }

        var context = new ToolExecutionContext
        {
            AgentId = null,
            RunId = execution.Id.ToString(),
            SessionId = null,
            TenantId = execution.TenantId,
            UserId = execution.CreatedBy,
            CorrelationId = execution.CorrelationId
        };

        step.Attempts++;
        var result = await _toolExecutor.ExecuteAsync(stepDef.ToolName, stepDef.ToolArguments, context, cancellationToken);

        if (result.Success)
        {
            step.Status = WorkflowStepStatus.Succeeded;
            step.Output = result.Data;
            step.Error = null;
            step.CompletedAt = DateTime.UtcNow;
            execution.State[stepDef.Id] = result.Data;
            return;
        }

        step.Error = result.Error;

        if (step.Attempts <= stepDef.RetryMax)
        {
            _logger.LogWarning("Step '{Step}' failed (attempt {Attempt}/{Max}); scheduling retry. {Error}", stepDef.Id, step.Attempts, stepDef.RetryMax, result.Error);
            var delay = GetRetryDelaySeconds(stepDef, step.Attempts);
            if (delay > 0)
            {
                await Task.Delay(TimeSpan.FromSeconds(delay), cancellationToken);
            }

            step.Status = WorkflowStepStatus.Pending;
            step.CompletedAt = null;
            return;
        }

        step.Status = WorkflowStepStatus.Failed;
        step.CompletedAt = DateTime.UtcNow;
        await FailExecutionAsync(execution, definition, step, cancellationToken);
    }

    private async Task RequestApprovalAsync(WorkflowExecution execution, WorkflowStepDefinition stepDef, WorkflowStepExecution step, CancellationToken cancellationToken)
    {
        var approval = await _approvalService.RequestAsync(new CreateApprovalRequest
        {
            Type = stepDef.Type == WorkflowStepType.Approval ? ApprovalType.WorkflowStep : ApprovalType.ToolCall,
            Title = $"Approve workflow step: {stepDef.Name ?? stepDef.Id}",
            Description = $"Workflow {execution.DefinitionName} v{execution.DefinitionVersion} requires approval for step '{stepDef.Id}'.",
            Action = stepDef.ToolName ?? stepDef.Id,
            Payload = stepDef.ToolArguments,
            RequestedBy = execution.CreatedBy,
            RequiredRole = stepDef.ApprovalRequiredRole,
            TenantId = execution.TenantId,
            CorrelationId = execution.CorrelationId,
            RunId = execution.Id.ToString(),
            ExpiresIn = TimeSpan.FromMinutes(_options.ApprovalDefaultTimeoutMinutes),
            EscalationThreshold = TimeSpan.FromMinutes(_options.ApprovalEscalationThresholdMinutes)
        }, cancellationToken);

        step.Status = WorkflowStepStatus.WaitingForApproval;
        step.ApprovalRequestId = approval.Id.ToString();
        step.CompletedAt = null;
        execution.Status = WorkflowStatus.WaitingForApproval;
    }

    private async Task<bool> CompleteParallelParentsAsync(WorkflowExecution execution, WorkflowDefinition definition, CancellationToken cancellationToken)
    {
        var changed = false;
        foreach (var stepDef in definition.Steps.Where(s => s.Type == WorkflowStepType.Parallel))
        {
            var parent = execution.Steps.FirstOrDefault(s => s.StepId == stepDef.Id);
            if (parent is null || parent.Status != WorkflowStepStatus.Running)
            {
                continue;
            }

            var branches = stepDef.BranchStepIds
                .Select(id => execution.Steps.FirstOrDefault(s => s.StepId == id))
                .ToList();

            if (branches.Count > 0 && branches.All(b => b is not null && IsTerminalSuccess(b.Status)))
            {
                parent.Status = WorkflowStepStatus.Succeeded;
                parent.CompletedAt = DateTime.UtcNow;
                changed = true;
            }
        }

        if (changed)
        {
            await _store.SaveAsync(execution, cancellationToken);
        }

        return changed;
    }

    private async Task FailExecutionAsync(WorkflowExecution execution, WorkflowDefinition definition, WorkflowStepExecution failedStep, CancellationToken cancellationToken)
    {
        await CompensateAsync(execution, definition, cancellationToken);

        execution.Status = WorkflowStatus.Failed;
        execution.CompletedAt = DateTime.UtcNow;
        execution.FailureReason = $"Step '{failedStep.StepId}' failed: {failedStep.Error}";
        await _store.SaveAsync(execution, cancellationToken);
        _metrics.RecordWorkflow(execution.DefinitionName, 0, false, failedStep.Error);
        await PublishStatusAsync(execution, cancellationToken);
    }

    private async Task CompensateAsync(WorkflowExecution execution, WorkflowDefinition definition, CancellationToken cancellationToken)
    {
        var succeeded = execution.Steps
            .Where(s => s.Status == WorkflowStepStatus.Succeeded)
            .OrderByDescending(s => definition.Steps.FindIndex(d => d.Id == s.StepId))
            .ToList();

        foreach (var step in succeeded)
        {
            var stepDef = definition.Steps.FirstOrDefault(d => d.Id == step.StepId);
            var compId = stepDef?.CompensatingStepId;
            if (stepDef is null || string.IsNullOrWhiteSpace(compId))
            {
                continue;
            }

            var compDef = definition.Steps.FirstOrDefault(d => d.Id == compId);
            if (compDef is null || string.IsNullOrWhiteSpace(compDef.ToolName))
            {
                continue;
            }

            step.Status = WorkflowStepStatus.Compensating;
            await _store.SaveAsync(execution, cancellationToken);

            var context = new ToolExecutionContext
            {
                RunId = execution.Id.ToString(),
                TenantId = execution.TenantId,
                UserId = execution.CreatedBy,
                CorrelationId = execution.CorrelationId
            };

            var arguments = new Dictionary<string, object?>(compDef.ToolArguments)
            {
                ["compensatingFor"] = stepDef.Id,
                ["originalOutput"] = step.Output
            };

            var result = await _toolExecutor.ExecuteAsync(compDef.ToolName, arguments, context, cancellationToken);
            step.Status = result.Success ? WorkflowStepStatus.Compensated : WorkflowStepStatus.Compensating;
            step.CompletedAt = DateTime.UtcNow;
            step.Error = result.Success ? null : result.Error;
        }

        await _store.SaveAsync(execution, cancellationToken);
    }

    private static int GetRetryDelaySeconds(WorkflowStepDefinition stepDef, int attempt) => stepDef.RetryPolicy switch
    {
        RetryPolicy.Fixed => stepDef.RetryDelaySeconds,
        RetryPolicy.Linear => stepDef.RetryDelaySeconds * attempt,
        RetryPolicy.Exponential => stepDef.RetryDelaySeconds * (1 << Math.Min(attempt, 5)),
        _ => 0
    };

    private static bool IsTerminalSuccess(WorkflowStepStatus status) =>
        status is WorkflowStepStatus.Succeeded or WorkflowStepStatus.Skipped or WorkflowStepStatus.Compensated;

    private static WorkflowStepExecution CloneStep(WorkflowStepExecution source) => new()
    {
        StepId = source.StepId,
        Name = source.Name,
        Status = source.Status,
        Attempts = source.Attempts,
        Input = source.Input,
        Output = source.Output,
        Error = source.Error,
        ApprovalRequestId = source.ApprovalRequestId,
        StartedAt = source.StartedAt,
        CompletedAt = source.CompletedAt
    };

    private async Task PublishStatusAsync(WorkflowExecution execution, CancellationToken cancellationToken)
    {
        if (_options.EnableStreaming)
        {
            await _stream.PublishAsync(AgentStreamEvent.Workflow(execution.Id, $"{execution.DefinitionName}: {execution.Status}", execution.CorrelationId), cancellationToken);
        }
    }
}
