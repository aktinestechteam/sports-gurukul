# AI Agent Platform

Version: 1.0
Project: `SportsGurukul.Platform.AI`

## Overview

`SportsGurukul.Platform.AI` is a provider-agnostic agent platform that composes planning, tool execution,
workflows, human-in-the-loop approvals, memory, observability, security, streaming, multi-agent
coordination, and MCP into a single service library. It intentionally has **no hardcoded dependency on
LangGraph, DSPy, or a specific MCP vendor** — every capability is behind an interface with an in-memory
default so the platform runs end-to-end out of the box and each integration point can be swapped.

Everything is registered through one extension method:

```csharp
services.AddAIPlatform(options => {
    options.EnableSelfEvaluation = true;
    options.EnableStreaming = true;
});
```

## Module Map

| Module | Interfaces | Default implementation |
| --- | --- | --- |
| Runtime | `IAgentService`, `IAgentRuntime`, `IAgentExecutor`, `IAgentPlanner`, `IAgentRegistry`, `IAgentLifecycleService` | `AgentService`, `AgentRuntime`, `AgentExecutor`, `DefaultAgentPlanner`, `InMemoryAgentRegistry`, `AgentLifecycleService` |
| Planning | `IPlanningService`, `IReflectionService` | `PlanningService`, `ReflectionService` |
| Tools | `ITool`, `IToolRegistry`, `IToolExecutor`, `IToolAuthorization`, `IToolService` | `InMemoryToolRegistry`, `DefaultToolExecutor`, `DefaultToolAuthorization`, `ToolService` |
| Built-in tools | — | `InternalApiTool`, `DatabaseTool`, `KnowledgeSearchTool`, `NotificationTool`, `FinanceTool`, `SchedulingTool` |
| Gateways | `IInternalApiGateway`, `IRestApiClient`, `IDatabaseQueryExecutor`, `IKnowledgeSearcher`, `INotificationGateway`, `IFinanceGateway`, `ISchedulingGateway` | `StubInternalApiGateway`, `DefaultRestApiClient`, `StubDatabaseQueryExecutor`, `StubKnowledgeSearcher`, `StubNotificationGateway`, `StubFinanceGateway`, `StubSchedulingGateway` |
| Workflow | `IWorkflowEngine`, `IWorkflowService`, `IWorkflowStore`, `IConditionEvaluator` | `WorkflowEngine`, `WorkflowService`, `InMemoryWorkflowStore`, `SimpleConditionEvaluator` |
| Human-in-the-loop | `IApprovalService`, `IApprovalStore`, `IApprovalCoordinator` | `ApprovalService`, `InMemoryApprovalStore`, `ApprovalCoordinator` |
| Memory | `IMemoryService`, `IWorkingMemoryStore`, `ISessionMemoryStore`, `ILongTermMemoryStore`, `ISemanticMemoryStore`, `IEpisodicMemoryStore`, `IEmbeddingProvider` | `MemoryService`, `InMemoryMemoryStore`, `HashedEmbeddingProvider` |
| Model | `ILanguageModelFactory`, `ILanguageModel` | `InMemoryLanguageModelFactory`, `StubLanguageModel` |
| Observability | `IMetricsCollector`, `IMetricsReporter`, `IObservabilityService` | `InMemoryMetricsCollector`, `ObservabilityService` |
| Security | `ITenantIsolation`, `ITenantContextAccessor`, `IPromptInjectionGuard`, `IOutputValidator`, `IAuditLogger` | `DefaultTenantIsolation`, `AsyncLocalTenantContextAccessor`, `DefaultPromptInjectionGuard`, `DefaultOutputValidator`, `InMemoryAuditLogger` |
| Streaming | `IAgentEventStream` | `InMemoryAgentEventStream` |
| Multi-agent | `IAgentRouter`, `IResultAggregator`, `ISupervisorAgent`, `ICollaborationCoordinator`, `IWorkerAgent` | `AgentRouter`, `ResultAggregator`, `SupervisorAgent`, `CollaborationCoordinator` |
| MCP | `IMcpClientFactory`, `IMcpToolAdapter`, `IMcpServerRegistry`, `IMcpClient`, `IMcpServer` | `McpClientFactory`, `DefaultMcpToolAdapter`, `InMemoryMcpServerRegistry` |
| Events | `IDomainEventPublisher` | `MediatRDomainEventPublisher` |

## Agent Run Flow

```
RunAsync(request)
   │
   ├─ IAgentRegistry.GetAsync(agentId)            → AgentNotFoundException if missing
   ├─ IAgentLifecycleService.StartAsync            → session + run id
   ├─ IAgentPlanner.CreatePlanAsync(goal, ctx)
   │     ├─ IPlanningService.DecomposeAsync        → split goal into steps
   │     ├─ IPlanningService.PrioritizeAsync       → order by priority (Critical=0 … Low=3)
   │     └─ AssignToolsAsync                       → map steps to registered tools + approval flags
   ├─ loop while steps pending (bounded by MaxIterations / MaxToolCalls)
   │     ├─ step has tool?
   │     │     ├─ IPromptInjectionGuard.InspectAsync
   │     │     ├─ RequiresApproval? → ApprovalService.Request + WaitForResolution
   │     │     └─ IToolExecutor.ExecuteAsync
   │     │           ├─ IToolAuthorization.AuthorizeAsync
   │     │           ├─ (approval if required)
   │     │           └─ ITool.ExecuteAsync (timeout + retry)
   │     ├─ step failed → IReflectionService.ReflectAsync → replan or stop
   │     └─ periodic reflection gate
   ├─ optional IReflectionService.EvaluateAsync   (self-evaluation)
   ├─ IAgentLifecycleService.CompleteAsync
   ├─ IMetricsCollector.RecordAgent / IAuditLogger
   └─ IAgentEventStream: status → plan → step → tool → done
```

The executor treats a run as **completed only if every step succeeded**; otherwise the result is `Failed`
with the failing step's error surfaced through `IReflectionService` for replanning.

## Tool Assignment Heuristic

`DefaultAgentPlanner` assigns a tool to a free-text step by keyword scoring:

- `+3` if the step title contains the tool name
- `+2` per matching tag
- `+2` if the title contains the tool description

A tool is only assigned if the score is positive, and then any missing tool `Parameters` are populated
from the step title. A step can also name a tool directly with a `tool: args` prefix (e.g.
`finance: process payroll`), which is parsed by `PlanningService.TryExtractTool`.

## Workflow Approval Flow

```
StartAsync(definition)
   │
   ├─ create steps, mark approval steps WaitingForApproval
   ├─ execution.Status = WaitingForApproval       (awaiting human decision)
   └─ return execution

ApprovalService.ApproveAsync(requestId, ...)
   │
   └─ ApprovalCoordinator.ResolveAsync → store update + Signal

ResumeAsync(executionId)
   │
   ├─ ResolveApprovalsAsync
   │     └─ Approved  → approval step Succeeded, downstream steps Ready
   │        Rejected  → step Failed → workflow Failed
   ├─ status resets WaitingForApproval → Running when no step awaits approval
   ├─ run ready steps (sequential or parallel per options)
   └─ all terminal-success → Completed
```

`InMemoryApprovalStore.WaitAsync` re-checks the request status after registering its waiter to close the
race where an approval resolves between the lookup and the wait, preventing waiters from hanging.

## Multi-Agent Patterns

- **Router**: `IAgentRouter.RouteAsync` assigns a `DelegatedTask` to the best `IWorkerAgent` by capability.
- **Aggregator**: `IResultAggregator.AggregateAsync` merges `DelegatedTaskResult`s with a strategy
  (`Sequential`, `Parallel`, `Vote`, etc.).
- **Supervisor**: `ISupervisorAgent.RunAsync` / `ICollaborationCoordinator.CoordinateAsync` orchestrate
  worker groups with fallback routing when a worker is unavailable.

## Streaming

`IAgentEventStream` gives each run a replayable event history plus a live channel:

- `WatchAsync(runId)` replays history first (ordered by `Sequence`), then streams live events.
- `PublishAsync` assigns monotonically increasing sequence numbers and appends to history.
- `CompleteAsync(runId)` emits a `Done` event and completes the channel; watchers stop on `Done`.
- Cancellation is honored via `CancellationToken`.

## Memory

`IMemoryService` fans out by `MemoryCategory` into the working / session / long-term / semantic /
episodic stores. Semantic entries are embedded automatically on write. The default
`HashedEmbeddingProvider` produces **deterministic** word-level hashed vectors (FNV-1a) that are stable
across processes, so persisted embeddings remain comparable after restarts. `ISemanticMemoryStore.SearchAsync`
ranks candidates by cosine similarity and filters below `InMemoryMemoryStore.MinimumSimilarity` (0.1).

## Security

- **Tenant isolation**: `ITenantIsolation.VerifyAccess` guards scope/tenant pairs; the accessor is
  `AsyncLocal`-scoped so tenant context propagates through async work.
- **Prompt injection**: `IPromptInjectionGuard.InspectAsync` flags/block risky step content.
- **Output validation**: `IOutputValidator` redacts sensitive patterns (API keys, credentials).
- **Audit**: `IAuditLogger` records tool calls, agent runs, and denials with tenant/correlation context.

## Observability

`InMemoryMetricsCollector` records per-agent, per-tool, per-workflow, and per-model metrics and exposes
snapshots through `IMetricsReporter`. `ObservabilityService.IsHealthyAsync` computes the failure rate
across agents + tools + workflows and reports unhealthy at ≥50% failures.

## Provider Agnosticism

- **Model**: register a provider factory on `ILanguageModelFactory.Register("provider", () => model)`;
  `AgentDefinition.Provider`/`Model` select it at run time. Unknown providers fall back to the stub.
- **MCP**: `IMcpClientFactory` creates servers/clients from `McpServerInfo`; `DefaultMcpToolAdapter`
  adapts an `IMcpServer` into an `ITool` discoverable via `IMcpServerRegistry`.
- **Tools**: register any `ITool` on `IToolRegistry`; approval and authorization metadata travel with the
  tool registration.

## Configuration (`AIPlatformOptions`)

| Option | Default | Purpose |
| --- | --- | --- |
| `EnableSelfEvaluation` | `true` | run self-evaluation after each run |
| `EnableReflection` / `ReflectionFrequency` | `true` / `3` | periodic plan reflection |
| `EnableStreaming` | `true` | publish stream events |
| `DefaultToolTimeoutSeconds` / `ToolRetryMax` | `30` / `1` | tool execution limits |
| `RunWorkflowStepsInParallel` | `false` | parallel workflow step execution |
| `ApprovalDefaultTimeoutMinutes` / `ApprovalEscalationThresholdMinutes` | `60` / `30` | approval lifecycle |

## Tests

`SportsGurukul.Platform.AI.Tests` covers 80 scenarios across runtime, planning, tools, workflows,
approvals, memory, multi-agent, MCP, security, observability, streaming, and DI registration.
Run them with:

```powershell
dotnet test backend\tests\SportsGurukul.Platform.AI.Tests\SportsGurukul.Platform.AI.Tests.csproj
```
