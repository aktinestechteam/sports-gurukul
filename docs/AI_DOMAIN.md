# AI & Intelligence Domain — Sports Gurukul

## Overview

The AI & Intelligence Domain provides the data foundation for the platform's AI capabilities — AI Coaching, intelligent assistants, knowledge retrieval, semantic search, agents and workflows. It models **providers & models** (multi-vendor LLM catalog), **assistants & prompts**, **conversations & memory**, **knowledge bases & embeddings** (RAG), **semantic search**, **tools**, **agents & workflows**, and cross-cutting **token usage** and **audit** tracking. This deliverable covers **Domain & Persistence only** (entities, repository contracts, EF Core configurations, seed data). No CQRS, controllers, AI provider SDKs, or business/vector-search logic is included.

## Entity Relationships

```
AIProvider (1) ──< (N) AIModel
AIProvider (1) ──< (N) AIModelConfiguration
AIProvider (1) ──< (N) AIRoutingPolicy
AIProvider (1) ──< (N) AITokenUsage

AIModel (1) ──< (N) AIModelConfiguration
AIModel (1) ──< (N) AIRoutingPolicy
AIModel (1) ──< (N) AITokenUsage
AIModel (1) ──< (N) AIAssistant            (default model)
AIModel (1) ──< (N) AgentDefinition         (default model)
AIModel (1) ──< (N) KnowledgeBase           (embedding model)
AIModel (1) ──< (N) Embedding

AIAssistant (1) ──< (N) Conversation
AIAssistant (1) ──< (N) PromptTemplate
AIAssistant (1) ──< (N) AIModelConfiguration
AIAssistant (1) ──< (N) AITokenUsage

PromptTemplate (1) ──< (N) PromptVersion

Conversation (1) ──< (N) ConversationMessage
Conversation (1) ──< (N) ConversationMemory
Conversation (1) ──< (N) SemanticSearchRequest
Conversation (1) ──< (N) AITokenUsage

KnowledgeBase (1) ── (1) VectorIndex
KnowledgeBase (1) ──< (N) KnowledgeSource
KnowledgeBase (1) ──< (N) KnowledgeDocument
KnowledgeBase (1) ──< (N) EmbeddingChunk
KnowledgeBase (1) ──< (N) Embedding
KnowledgeBase (1) ──< (N) SemanticSearchRequest

KnowledgeSource (1) ──< (N) KnowledgeDocument
KnowledgeDocument (1) ──< (N) EmbeddingChunk
EmbeddingChunk (1) ── (1) Embedding

VectorIndex (1) ──< (N) SemanticSearchRequest
SemanticSearchRequest (1) ──< (N) SemanticSearchResult
SemanticSearchResult (N) >── (1) KnowledgeDocument
SemanticSearchResult (N) >── (1) EmbeddingChunk

AgentDefinition (1) ──< (N) AgentExecution
AgentDefinition (1) ──< (N) ToolDefinition
AgentDefinition (1) ──< (N) AIModelConfiguration
AgentDefinition (N) >── (1) WorkflowDefinition
WorkflowDefinition (1) ──< (N) WorkflowExecution
WorkflowExecution (N) >── (1) AgentExecution
ToolDefinition (1) ──< (N) ToolExecution
ToolExecution (N) >── (1) AgentExecution
ToolExecution (N) >── (1) WorkflowExecution
```

## Aggregate Boundaries

| Aggregate Root        | Entities                                    | Description                                             |
|-----------------------|---------------------------------------------|---------------------------------------------------------|
| AIProvider            | AIModel, AIModelConfiguration, AIRoutingPolicy | Multi-vendor LLM provider catalog and model registry |
| AIAssistant           | PromptTemplate, PromptVersion, Conversation, ConversationMessage, ConversationMemory | Reusable assistant definitions with prompt & chat state |
| KnowledgeBase         | KnowledgeSource, KnowledgeDocument, EmbeddingChunk, Embedding, VectorIndex | RAG knowledge graph and vector store configuration |
| SemanticSearchRequest | SemanticSearchResult                        | Semantic query execution and ranked results             |
| AgentDefinition       | ToolDefinition, AgentExecution, ToolExecution | Autonomous agent with tool bindings and executions    |
| WorkflowDefinition    | WorkflowExecution, AgentExecution, ToolExecution | Orchestrated multi-step AI workflows                   |
| AIModelConfiguration  | —                                           | Deployment-scoped model settings (API key, temperature, tokens) |
| AITokenUsage          | —                                           | Token/usage accounting per user, conversation, assistant |
| AIAuditLog            | —                                           | Audit trail for AI operations                          |

## Key Lifecycles

### Document Ingestion
```
Pending → Processing → Embedded → Indexed
                → Failed
```

### Embedding
```
Pending → Embedding → Completed
                → Failed
```

### Vector Index
```
Pending → Building → Ready
                → Failed → Rebuilding
```

### Semantic Search
```
Pending → Executing → Completed
                → Failed
```

### Conversation
```
Active → Archived
      → Closed
```

### Tool / Agent / Workflow Execution
```
Pending → Running → Succeeded
                 → Failed → Retrying
                 → Cancelled
```

## Extension Points

1. **Providers** — Add a new `AIProvider` to connect additional LLM vendors (e.g., Mistral, Cohere). Provider type and auth are extensible enums.
2. **Models** — Register `AIModel` under a provider; new model families extend `AIModelFamily`.
3. **Assistants** — Create `AIAssistant` per use case (AI Coach, Analyst, Moderator). Assistant types extend `AIAssistantType`.
4. **Prompts** — `PromptTemplate` with versioned `PromptVersion` content. Prompt types extend `AIPromptType`.
5. **Knowledge** — Link `KnowledgeBase` to any domain resource via `OwnerType`/`OwnerId`; ingest via `KnowledgeSource` (URL, upload, API).
6. **Vector Stores** — `VectorIndex` abstracts the backing store (`AIVectorIndexProvider` defaults to PgVector); distance metric is configurable.
7. **Tools** — `ToolDefinition` binds platform functions to agents; execution results captured in `ToolExecution`.
8. **Agents & Workflows** — Compose `AgentDefinition` inside `WorkflowDefinition`; trigger types extend `AITriggerType`.
9. **Routing** — `AIRoutingPolicy` defines provider/model selection (`AIRoutingStrategy`: priority, balanced, cost, latency).
10. **Model Configurations** — `AIModelConfiguration` stores per-deployment settings incl. encrypted API keys and override base URLs.

## Repository Interfaces

| Interface             | Methods |
|-----------------------|---------|
| IAIProviderRepository | GetByName, GetByType, GetActive, GetByIdWithModels |
| IAssistantRepository  | GetByName, GetByType, GetByOwner, GetActive, GetByIdWithDetails |
| IConversationRepository| GetByIdWithMessages, GetByIdWithDetails, GetByAssistantId, GetActiveByAssistant, GetByParticipant, GetByStatus |
| IPromptRepository     | GetByName, GetByType, GetByAssistantId, GetActiveByAssistant, GetDefaultByAssistant, GetByIdWithVersions |
| IKnowledgeBaseRepository| GetByName, GetByType, GetByOwner, GetByIdWithDetails, GetByVectorIndex |
| IDocumentRepository   | GetByContentHash, GetBySource, GetByKnowledgeBase, GetByStatus, GetByIdWithChunks |
| IEmbeddingRepository  | GetByChunkId, GetByKnowledgeBase, GetByModel, GetByStatus, CountByKnowledgeBase |
| IVectorIndexRepository| GetByName, GetByProvider, GetByStatus, GetActive |
| IAgentRepository      | GetByName, GetByType, GetByWorkflow, GetActive, GetByIdWithTools |
| IWorkflowRepository   | GetByName, GetByType, GetActive, GetPublished, GetByIdWithAgents |
| ITokenUsageRepository | GetByUser, GetByType, GetByModel, GetByProvider, GetByAssistant, GetByConversation, GetByDateRange |
| IAuditRepository      | GetByEntity, GetByActor, GetByAction, GetBySeverity, GetByDateRange |

## Seed Data

- **6 AI providers**: OpenAI, Azure OpenAI, Anthropic (Claude), Google (Gemini), Ollama (local), OpenRouter — seeded via `AIProviderConfiguration`.
- **15 sample models**: GPT-4o, GPT-4o Mini, GPT-3.5 Turbo, text-embedding-3-small/large, Azure GPT-4o/mini, Claude Sonnet 4, Claude Haiku 4.5, Gemini 2.0 Pro/Flash, Llama 3.1 8B / 3.3 70B, OpenRouter GPT-4o + Claude Sonnet 4 — seeded via `AIModelConfiguration` (config for the `AIModel` entity).

## Platform Services Reused

- **Identity Platform** — `UserId`/participant lookups on conversations, token usage, audit actor.
- **Reference Data Platform** — `AIProvider`, `AIModel`, `AIResourceOwnerType` for polymorphic ownership of knowledge bases, tools and workflows.
- **Document Platform** — `KnowledgeDocument.DocumentId`/content hashing for deduplication during ingestion.
- **Audit Platform** — `AIAuditLog` for compliance and debugging of AI operations.

## Files Created

### Domain Layer (`SportsGurukul.Domain`)
| Item | Path |
|------|------|
| 32 AI enums (AIProviderType, AIModelFamily, AIAssistantType, AIMessageRole, AIDocumentStatus, AIEmbeddingStatus, AISearchStatus, AIWorkflowType, AIAgentType, AIAuditAction, AIUsageType, AIRoutingStrategy, …) | `Enums/AI/*.cs` |
| AIProvider | `Entities/AI/AIProvider.cs` |
| AIModel | `Entities/AI/AIModel.cs` |
| AIAssistant | `Entities/AI/AIAssistant.cs` |
| PromptTemplate | `Entities/AI/PromptTemplate.cs` |
| PromptVersion | `Entities/AI/PromptVersion.cs` |
| Conversation | `Entities/AI/Conversation.cs` |
| ConversationMessage | `Entities/AI/ConversationMessage.cs` |
| ConversationMemory | `Entities/AI/ConversationMemory.cs` |
| KnowledgeBase | `Entities/AI/KnowledgeBase.cs` |
| KnowledgeSource | `Entities/AI/KnowledgeSource.cs` |
| KnowledgeDocument | `Entities/AI/KnowledgeDocument.cs` |
| Embedding | `Entities/AI/Embedding.cs` |
| EmbeddingChunk | `Entities/AI/EmbeddingChunk.cs` |
| VectorIndex | `Entities/AI/VectorIndex.cs` |
| SemanticSearchRequest | `Entities/AI/SemanticSearchRequest.cs` |
| SemanticSearchResult | `Entities/AI/SemanticSearchResult.cs` |
| ToolDefinition | `Entities/AI/ToolDefinition.cs` |
| ToolExecution | `Entities/AI/ToolExecution.cs` |
| WorkflowDefinition | `Entities/AI/WorkflowDefinition.cs` |
| WorkflowExecution | `Entities/AI/WorkflowExecution.cs` |
| AgentDefinition | `Entities/AI/AgentDefinition.cs` |
| AgentExecution | `Entities/AI/AgentExecution.cs` |
| AIAuditLog | `Entities/AI/AIAuditLog.cs` |
| AITokenUsage | `Entities/AI/AITokenUsage.cs` |
| AIModelConfiguration | `Entities/AI/AIModelConfiguration.cs` |
| AIRoutingPolicy | `Entities/AI/AIRoutingPolicy.cs` |

### Application Layer (`SportsGurukul.Application`)
| Item | Path |
|------|------|
| IAIProviderRepository | `Common/Interfaces/AI/IAIProviderRepository.cs` |
| IAssistantRepository | `Common/Interfaces/AI/IAssistantRepository.cs` |
| IConversationRepository | `Common/Interfaces/AI/IConversationRepository.cs` |
| IPromptRepository | `Common/Interfaces/AI/IPromptRepository.cs` |
| IKnowledgeBaseRepository | `Common/Interfaces/AI/IKnowledgeBaseRepository.cs` |
| IDocumentRepository | `Common/Interfaces/AI/IDocumentRepository.cs` |
| IEmbeddingRepository | `Common/Interfaces/AI/IEmbeddingRepository.cs` |
| IVectorIndexRepository | `Common/Interfaces/AI/IVectorIndexRepository.cs` |
| IAgentRepository | `Common/Interfaces/AI/IAgentRepository.cs` |
| IWorkflowRepository | `Common/Interfaces/AI/IWorkflowRepository.cs` |
| ITokenUsageRepository | `Common/Interfaces/AI/ITokenUsageRepository.cs` |
| IAuditRepository | `Common/Interfaces/AI/IAuditRepository.cs` |

### Infrastructure Layer (`SportsGurukul.Infrastructure`)
| Item | Path |
|------|------|
| 26 EF configurations (one per entity, incl. relationships, indexes, concurrency row version, soft-delete filters, seed data) | `Persistence/Configurations/AI/*.cs` |
| 12 repository implementations (AIProviderRepository, AssistantRepository, ConversationRepository, PromptRepository, KnowledgeBaseRepository, DocumentRepository, EmbeddingRepository, VectorIndexRepository, AgentRepository, WorkflowRepository, TokenUsageRepository, AuditRepository) | `Persistence/Repositories/AI/*.cs` |
| Migration | `Persistence/Migrations/20260802125208_AddAIDomain.cs` |

### Modified Files
| File | Change |
|------|--------|
| `ApplicationDbContext.cs` | Added 26 DbSet properties for AI entities |
| `IApplicationDbContext.cs` | Added 26 DbSet properties for AI entities |
| `DependencyInjection.cs` | Added 12 AI repository DI registrations (aliased `IAuditRepository`/`AuditRepository` to resolve Notification-domain name collision) |
| `tests/SportsGurukul.IntegrationTests/TestApplicationFactory.cs` | `InMemoryUnitOfWork` stubs for all `IApplicationDbContext` DbSets (Finance, Notification, AI) so the test project conforms to the interface |
