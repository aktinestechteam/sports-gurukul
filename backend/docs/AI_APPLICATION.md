# AI & Intelligence Platform - Application Layer

## CQRS Architecture

The AI Platform follows Clean Architecture with CQRS pattern using MediatR:

```
┌─────────────────────────────────────────────────┐
│                  API Layer                       │
│  Controllers → Send Commands/Queries via IMediator │
└──────────────────┬──────────────────────────────┘
                   │
┌──────────────────▼──────────────────────────────┐
│              Application Layer                    │
│                                                   │
│  Commands ──→ Command Handlers ──→ Services       │
│  Queries  ──→ Query Handlers  ──→ Repositories    │
│                                                   │
│  Validators (FluentValidation, auto-registered)   │
│  Domain Events (MediatR INotification)            │
└──────────────────┬──────────────────────────────┘
                   │
┌──────────────────▼──────────────────────────────┐
│              Infrastructure Layer                 │
│  Repository implementations, EF Core, Providers  │
└─────────────────────────────────────────────────┘
```

## Command Flow

```
Client → Controller → Command (IRequest<Result<TDto>>)
                      ↓
            ValidationBehavior (auto pipeline)
                      ↓
            CommandHandler (IRequestHandler)
                      ↓
            Service (business logic + domain events)
                      ↓
            Repository (persistence)
                      ↓
            Result<TDto> → Controller → Response
```

## Query Flow

```
Client → Controller → Query (IRequest<Result<TDto>>)
                      ↓
            QueryHandler (IRequestHandler)
                      ↓
            Repository (read) ← for search/list queries
            Service (read)    ← for single-entity queries
                      ↓
            Result<TDto> → Controller → Response
```

## Commands

### Conversation (8 commands)
| Command | Handler | Description |
|---------|---------|-------------|
| `CreateConversationCommand` | `CreateConversationCommandHandler` | Creates a new conversation session |
| `RenameConversationCommand` | `RenameConversationCommandHandler` | Updates conversation title |
| `ArchiveConversationCommand` | `ArchiveConversationCommandHandler` | Marks conversation as archived |
| `DeleteConversationCommand` | `DeleteConversationCommandHandler` | Soft-deletes a conversation |
| `AddMessageCommand` | `AddMessageCommandHandler` | Adds a user/assistant message |
| `RegenerateResponseCommand` | `RegenerateResponseCommandHandler` | Re-triggers last AI response |
| `ClearConversationMemoryCommand` | `ClearConversationMemoryCommandHandler` | Clears conversation memory |
| `SummarizeConversationCommand` | `SummarizeConversationCommandHandler` | Generates conversation summary |

### Assistant (6 commands)
| Command | Handler | Description |
|---------|---------|-------------|
| `CreateAssistantCommand` | `CreateAssistantCommandHandler` | Creates an AI assistant definition |
| `UpdateAssistantCommand` | `UpdateAssistantCommandHandler` | Updates assistant configuration |
| `PublishAssistantCommand` | `PublishAssistantCommandHandler` | Activates an assistant |
| `ArchiveAssistantCommand` | `ArchiveAssistantCommandHandler` | Deactivates an assistant |
| `AssignKnowledgeBaseCommand` | `AssignKnowledgeBaseCommandHandler` | Links knowledge base to assistant |
| `AssignToolsCommand` | `AssignToolsCommandHandler` | Links tools to assistant |

### Prompt (5 commands)
| Command | Handler | Description |
|---------|---------|-------------|
| `CreatePromptTemplateCommand` | `CreatePromptTemplateCommandHandler` | Creates a prompt template |
| `UpdatePromptTemplateCommand` | `UpdatePromptTemplateCommandHandler` | Creates new version of template |
| `PublishPromptTemplateCommand` | `PublishPromptTemplateCommandHandler` | Activates a prompt template |
| `RollbackPromptVersionCommand` | `RollbackPromptVersionCommandHandler` | Rolls back to previous version |
| `ClonePromptCommand` | `ClonePromptCommandHandler` | Duplicates an existing template |

### Knowledge (5 commands)
| Command | Handler | Description |
|---------|---------|-------------|
| `CreateKnowledgeBaseCommand` | `CreateKnowledgeBaseCommandHandler` | Creates a knowledge base |
| `UpdateKnowledgeBaseCommand` | `UpdateKnowledgeBaseCommandHandler` | Updates knowledge base metadata |
| `AttachDocumentCommand` | `AttachDocumentCommandHandler` | Attaches document to knowledge base |
| `DetachDocumentCommand` | `DetachDocumentCommandHandler` | Detaches document from knowledge base |
| `RebuildKnowledgeIndexCommand` | `RebuildKnowledgeIndexCommandHandler` | Triggers index rebuild |

### Agent (5 commands)
| Command | Handler | Description |
|---------|---------|-------------|
| `CreateAgentCommand` | `CreateAgentCommandHandler` | Creates an autonomous agent |
| `UpdateAgentCommand` | `UpdateAgentCommandHandler` | Updates agent configuration |
| `EnableAgentCommand` | `EnableAgentCommandHandler` | Activates an agent |
| `DisableAgentCommand` | `DisableAgentCommandHandler` | Deactivates an agent |
| `AssignWorkflowCommand` | `AssignWorkflowCommandHandler` | Assigns workflow to agent |

## Queries

| Query | Handler | Returns |
|-------|---------|---------|
| `GetConversationQuery` | `GetConversationQueryHandler` | `ConversationDto` |
| `ConversationHistoryQuery` | `ConversationHistoryQueryHandler` | `PaginatedResult<MessageDto>` |
| `SearchConversationsQuery` | `SearchConversationsQueryHandler` | `PaginatedResult<ConversationSummaryDto>` |
| `AssistantQuery` | `AssistantQueryHandler` | `AssistantDto` |
| `SearchAssistantsQuery` | `SearchAssistantsQueryHandler` | `PaginatedResult<AssistantSummaryDto>` |
| `KnowledgeBaseQuery` | `KnowledgeBaseQueryHandler` | `KnowledgeBaseDto` |
| `SearchKnowledgeBasesQuery` | `SearchKnowledgeBasesQueryHandler` | `PaginatedResult<KnowledgeBaseSummaryDto>` |
| `PromptQuery` | `PromptQueryHandler` | `PromptTemplateDto` |
| `SearchPromptsQuery` | `SearchPromptsQueryHandler` | `PaginatedResult<PromptSummaryDto>` |
| `AgentQuery` | `AgentQueryHandler` | `AgentDto` |
| `SearchAgentsQuery` | `SearchAgentsQueryHandler` | `PaginatedResult<AgentSummaryDto>` |
| `WorkflowQuery` | `WorkflowQueryHandler` | `WorkflowDto` |
| `SearchWorkflowsQuery` | `SearchWorkflowsQueryHandler` | `PaginatedResult<WorkflowSummaryDto>` |
| `TokenUsageQuery` | `TokenUsageQueryHandler` | `PaginatedResult<TokenUsageSummaryDto>` |
| `AuditLogQuery` | `AuditLogQueryHandler` | `PaginatedResult<AuditLogDto>` |

## Validators

| Validator | Validates |
|-----------|-----------|
| `CreateConversationCommandValidator` | Title length |
| `RenameConversationCommandValidator` | Title required, max length |
| `AddMessageCommandValidator` | ConversationId, Content (not empty, max 100000), Role |
| `CreateAssistantCommandValidator` | Name required, max length; SystemPrompt max length |
| `CreatePromptTemplateCommandValidator` | Name required, max length; TemplateContent required |
| `ClonePromptCommandValidator` | NewName required, max length |
| `CreateKnowledgeBaseCommandValidator` | Name required, max length |
| `CreateAgentCommandValidator` | Name required, max length |

## DTOs

Single file `AIDtos.cs` containing all DTO records:
- `ConversationDto`, `ConversationSummaryDto`, `MessageDto`
- `AssistantDto`, `AssistantSummaryDto`
- `PromptTemplateDto`, `PromptVersionDto`, `PromptSummaryDto`
- `KnowledgeBaseDto`, `KnowledgeBaseSummaryDto`, `KnowledgeSourceSummaryDto`, `KnowledgeDocumentDto`
- `AgentDto`, `AgentSummaryDto`
- `WorkflowDto`, `WorkflowSummaryDto`
- `ToolDefinitionDto`
- `TokenUsageDto`, `TokenUsageSummaryDto`
- `AuditLogDto`
- `PaginatedResult<T>`

## Platform Services

| Interface | Implementation | Responsibility |
|-----------|---------------|----------------|
| `IConversationService` | `ConversationService` | Conversation lifecycle, message management |
| `IAssistantService` | `AssistantService` | Assistant CRUD, knowledge/tool assignment |
| `IPromptService` | `PromptService` | Prompt templates, versioning, publishing |
| `IKnowledgeService` | `KnowledgeService` | Knowledge bases, document management |
| `IAgentService` | `AgentService` | Agent lifecycle, workflow assignment |
| `IWorkflowService` | `WorkflowService` | Workflow queries |
| `ITokenUsageService` | `TokenUsageService` | Token/cost tracking |
| `IAuditService` | `AuditService` | Audit logging |
| `IAIService` | `AIService` | AI facade (stub - provider integration in later sprint) |

## Model Routing (Abstractions)

| Interface | Responsibility |
|-----------|---------------|
| `IModelRoutingService` | Model selection, fallback, cost/latency/capability-based routing |
| `IRoutingPolicyService` | Active routing policy management |

## Tool Calling (Abstractions)

| Interface | Responsibility |
|-----------|---------------|
| `IToolRegistry` | Tool registration and discovery |
| `IToolExecutor` | Tool execution |
| `IToolResolver` | Tool resolution by name/conversation/assistant |
| `IToolAuthorizationService` | Tool access control |

## Domain Events

| Event | Published When |
|-------|---------------|
| `ConversationCreatedEvent` | New conversation created |
| `MessageAddedEvent` | Message added to conversation |
| `ConversationArchivedEvent` | Conversation archived |
| `PromptPublishedEvent` | Prompt template published |
| `KnowledgeBaseUpdatedEvent` | Knowledge base updated |
| `AgentCreatedEvent` | New agent created |
| `WorkflowAssignedEvent` | Workflow assigned to agent |
| `TokenUsageRecordedEvent` | Token usage recorded |

## Business Rules

| Rule | Implementation |
|------|---------------|
| Conversation Lifecycle | Active → Archived/Deleted (soft) |
| Conversation Memory | Cleared per request, context window managed |
| Prompt Version Resolution | Rollback creates new version with old content |
| Assistant Configuration | Validated on create/update |
| Knowledge Assignment | KB linked by ID, count tracked |
| Agent Enable/Disable | Status toggled, no cascading deletes |
| Workflow Assignment | Linked by ID to agent metadata |
| Token Usage Tracking | Recorded per conversation/message |
| Soft Delete | `IsDeleted = true` on all entities |
| Optimistic Concurrency | `RowVersion` byte array on entities |

## Extension Points

1. **New AI Provider**: Add enum value to `AIProviderType`, create provider entry in seed data, implement in provider SDK sprint
2. **New Tool Type**: Add enum to `ToolType`, extend `IToolRegistry`, implement executor
3. **New Model Routing Strategy**: Add to `RoutingStrategy`, extend `IModelRoutingService`
4. **New Domain Event**: Create record implementing `INotification`, publish via `IPublisher` in service
5. **New Command/Query**: Create record + handler + validator, auto-registered via MediatR/Validators assembly scanning
6. **New Assistant Type**: Add enum to `AIAssistantType`, update seed data

## File Structure

```
Features/AIManagement/
├── Commands/
│   ├── Conversation/       (16 files: 8 commands + 8 handlers)
│   ├── Assistant/          (12 files: 6 commands + 6 handlers)
│   ├── Prompt/             (10 files: 5 commands + 5 handlers)
│   ├── Knowledge/          (10 files: 5 commands + 5 handlers)
│   └── Agent/              (10 files: 5 commands + 5 handlers)
├── Queries/                (30 files: 15 queries + 15 handlers)
├── Validators/             (8 files)
├── DTOs/                   (1 file, all DTO records)
├── Services/               (9 files: 9 service implementations)
└── DomainEvents/           (8 files)
```

## DI Registration

All services registered as Transient in `DependencyInjection.cs`:
```csharp
services.AddTransient<IConversationService, ConversationService>();
services.AddTransient<IAssistantService, AssistantService>();
// ... all 9 services
```

MediatR and FluentValidation auto-registered via assembly scanning.
