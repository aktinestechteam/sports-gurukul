# AI Application Layer — Sports Gurukul

Version: 1.0

## Scope

This deliverable implements the **Application (Business Logic) layer** for the AI & Intelligence Domain, on top of the persistence layer documented in `docs/AI_DOMAIN.md`. It provides CQRS commands/queries with handlers and FluentValidation validators, reusable platform services, model-routing and tool-calling abstractions, domain events, and DI registration. It does **not** call any external LLM provider (OpenAI/Gemini), does not generate embeddings, and does not implement vector search — those are later sprints.

## Conventions

- **Repository pattern only** — handlers/services never touch `ApplicationDbContext`; all reads go through `IRepository<T>` / feature repository interfaces, all mutations through `IRepository<T>.AddAsync/Update/Remove` + `IUnitOfWork.SaveChangesAsync`.
- **Async/await** throughout, with `CancellationToken` propagated.
- Every operation returns `Result<T>` (`SportsGurukul.Application.Common.Models.Result<T>`) with `Success`/`Failure`.
- **Structured logging only** — prompts, message content, and user data are never logged.
- Entities are loaded with `AsNoTracking()`; mutations re-attach via `Update`.
- Validation runs through `ValidationBehavior` (`FluentValidation`); domain/availability failures are returned as `Result<T>.Failure`, never thrown.

## Layer Map

```
Features/AIManagement/
├── DTOs/                 request & response records per aggregate
├── Commands/             CQRS commands + handlers + validators
│   ├── Conversation/     (8 commands)
│   ├── Assistant/        (6 commands)
│   ├── Prompt/           (5 commands)
│   ├── Knowledge/        (5 commands)
│   └── Agent/            (5 commands)
├── Queries/              (10 read queries + handlers)
├── Events/               domain events (INotification)
├── Services/             platform services behind Common/Interfaces/AI/Services
├── ModelRouting/         model selection strategies, availability, fallback
├── ToolCalling/          tool registry, resolver, authorization, executor
├── AiJson.cs             internal System.Text.Json helpers
└── AssistantAssignmentStore.cs  knowledge-base/tool assignment metadata helper
```

## Platform Services

Implemented in `Features/AIManagement/Services/`, exposed via `Common/Interfaces/AI/Services/`:

| Interface | Implementation | Responsibility |
|---|---|---|
| `IConversationService` | `ConversationService` | Create/rename/archive/delete, add & regenerate messages, summarize, history, search. Enforces active conversation + assistant rules. Trims older messages when estimated tokens exceed the routed model's context window. |
| `IConversationMemoryService` | `ConversationMemoryService` | Store/clear/read conversation memories; filters out deleted/expired entries. |
| `IAssistantService` | `AssistantService` | CRUD, publish/archive, assign knowledge bases & tools (via `AssistantAssignmentStore` metadata keys `knowledge_base_ids`, `tool_ids`). |
| `IPromptService` | `PromptService` | Create/update, versioned publish (increments `CurrentVersion`, snapshots `PromptVersion`), rollback, clone. |
| `IPromptRenderer` | `PromptRenderer` | Replaces `{{key}}` placeholders; resolves default/active template for an assistant. |
| `IKnowledgeService` | `KnowledgeService` | Knowledge-base CRUD, document attach/detach (SHA-256 content hash), re-index queue (sets documents back to `Pending`). |
| `IAgentService` | `AgentService` | Agent CRUD, enable/disable, assign workflow. |
| `IWorkflowService` | `WorkflowService` | Read-only lookup of workflows (published list). |
| `ITokenUsageService` | `TokenUsageService` | Records usage, updates `Conversation.TokenCount`, summaries & filtered reads. |
| `IAuditService` | `AuditService` | Write/query audit log entries. |
| `IAIService` | `AIService` | Facade bundling conversation, model routing, and token usage for a single AI entry point. |
| `IModelRoutingService` | `ModelRoutingService` | See Model Routing below. |

## Model Routing

Location: `Features/AIManagement/ModelRouting/`

- `ModelSelectionContext` — capabilities requested (function calling, vision, JSON mode), token estimates, preferred/fallback model ids, routing strategy.
- `ModelCandidate` — flattened, provider-aware view of an `AIModel` + `AIProvider`.
- `IModelSelectionStrategy` — one per `AIRoutingStrategy` (`Balanced`, `Cost`, `Speed`, `Accuracy`):
  - `CostBasedModelSelectionStrategy`
  - `LatencyBasedModelSelectionStrategy`
  - `CapabilityBasedModelSelectionStrategy`
  - `BalancedModelSelectionStrategy` (default when the requested strategy is not configured)
- `IModelAvailabilityService` / `ModelAvailabilityService` — enumerates active providers/models and filters to candidates that meet requested capabilities; honors `PreferredModelIds` ordering.
- `IFallbackStrategy` / `FallbackStrategy` — builds a fallback chain (requested fallback ids first, then remaining candidates).
- `ModelSelectionCalculator` — pure static scoring (`EstimateCost`, `EstimateLatency`, `CapabilityScore`, `MeetsCapabilities`, `BalancedScore`, `ToSelectionResult`).
- `ModelRoutingService` — entry point: `SelectModelAsync`, `ResolveFallbackChainAsync`, `IsModelAvailableAsync`, `GetModelCandidateAsync`.

## Tool Calling

Location: `Features/AIManagement/ToolCalling/`

- `IToolRegistry` / `DefaultToolRegistry` — in-memory, thread-safe registry of `ToolDescriptor`s.
- `IToolResolver` / `ToolResolver` — resolves by name; for an agent, combines the agent's active `ToolDefinition`s (no executor) with registered system tools.
- `IToolAuthorizationService` / `ToolAuthorizationService` — denies tools that `RequiresApproval`, requires an authenticated user for non-system tools.
- `IToolExecutor` / `ToolExecutor` — times execution, enforces authorization, invokes `ToolDescriptor.Executor`, maps exceptions to `ToolCallResult.Failure`.

Executors are registered at runtime by the caller (web/worker); the registry is empty at startup.

## Domain Events

`Features/AIManagement/Events/` — published via `IMediator.Publish` after a successful unit of work:

`ConversationCreatedEvent`, `MessageAddedEvent`, `ConversationArchivedEvent`, `PromptPublishedEvent`, `KnowledgeBaseUpdatedEvent`, `AgentCreatedEvent`, `WorkflowAssignedEvent`, `TokenUsageRecordedEvent`.

## CQRS Surface

### Conversation commands
`CreateConversationCommand`, `RenameConversationCommand`, `ArchiveConversationCommand`, `DeleteConversationCommand`, `AddMessageCommand`, `RegenerateResponseCommand`, `ClearConversationMemoryCommand`, `SummarizeConversationCommand`

### Assistant commands
`CreateAssistantCommand`, `UpdateAssistantCommand`, `PublishAssistantCommand`, `ArchiveAssistantCommand`, `AssignKnowledgeBaseCommand`, `AssignToolsCommand`

### Prompt commands
`CreatePromptTemplateCommand`, `UpdatePromptTemplateCommand`, `PublishPromptTemplateCommand`, `RollbackPromptVersionCommand`, `ClonePromptCommand`

### Knowledge commands
`CreateKnowledgeBaseCommand`, `UpdateKnowledgeBaseCommand`, `AttachDocumentCommand`, `DetachDocumentCommand`, `RebuildKnowledgeIndexCommand`

### Agent commands
`CreateAgentCommand`, `UpdateAgentCommand`, `EnableAgentCommand`, `DisableAgentCommand`, `AssignWorkflowCommand`

### Queries
`GetConversationByIdQuery`, `GetConversationHistoryQuery`, `SearchConversationsQuery`, `GetAssistantByIdQuery`, `GetPromptTemplateByIdQuery`, `GetKnowledgeBaseByIdQuery`, `GetKnowledgeBaseDocumentsQuery`, `GetAgentByIdQuery`, `GetWorkflowByIdQuery`, `GetPublishedWorkflowsQuery`

## Key Business Rules

1. Conversations can only be created against **active** assistants; messages can only be added to **active** conversations.
2. Archived conversations cannot be renamed; deleting a conversation soft-deletes it (`Status = Deleted`, `IsDeleted = true`).
3. Messages carry a monotonic `SequenceNumber`; estimated token count is `ceil(length/4)` and is used to trim older non-system messages when it exceeds the routed model's context window.
4. Publishing a prompt template always creates a new immutable `PromptVersion` and bumps `CurrentVersion`; rollback restores `TemplateText` from a version and marks it active.
5. Assistant → knowledge-base/tool assignments live in `AIAssistant.MetadataJson` (`knowledge_base_ids`, `tool_ids`) because the entity has no navigation collections for them.
6. Documents attached to a knowledge base are stored with a SHA-256 content hash and start in `Pending` status; re-indexing resets all documents to `Pending` for the ingestion pipeline.
7. Token usage is recorded per request; `Conversation.TokenCount` is updated in the same unit of work.
8. Model selection never returns a model that fails required capabilities; the routing service falls back to the Balanced strategy when the requested strategy is unregistered.

## DI Registration

`Application/DependencyInjection.cs` → `RegisterAIServices(services)` registers all AI services, model-routing strategies (as `IEnumerable<IModelSelectionStrategy>`), and tool-calling components as transient. MediatR handlers and FluentValidation validators are auto-discovered from the assembly.

## Not Included (Later Sprints)

- OpenAI / Gemini client calls and completions
- Embedding generation and vector search
- LangGraph-style workflow execution engine
- Streaming / SSE
- Guardrail enforcement runtime
