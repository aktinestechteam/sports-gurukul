# AI & Intelligence Platform - Domain & Persistence Layer

## Overview

The AI & Intelligence Platform provides foundational infrastructure for building AI-powered features across SportsGurukul. This layer defines domain entities, repository interfaces, and EF Core persistence configurations.

## Architecture

- **Domain Layer** (`SportsGurukul.Domain.Entities.AI`) - Entity models
- **Application Layer** (`SportsGurukul.Application.Common.Interfaces.AI`) - Repository interfaces
- **Infrastructure Layer** (`SportsGurukul.Infrastructure.Persistence.Configurations.AI`, `Repositories.AI`) - EF Core configurations and repository implementations

## Domain Entities

### Provider & Models
| Entity | Table | Description |
|--------|-------|-------------|
| `AIProvider` | AIProviders | AI service providers (OpenAI, Azure, Anthropic, Google, Ollama, OpenRouter) |
| `AIModel` | AIModels | AI model definitions with capabilities and pricing |
| `AIModelConfiguration` | AIModelConfigurations | Per-model runtime configuration overrides |

### Assistants & Conversations
| Entity | Table | Description |
|--------|-------|-------------|
| `AIAssistant` | AIAssistants | Configurable AI assistant definitions |
| `Conversation` | Conversations | Chat/conversation sessions |
| `ConversationMessage` | ConversationMessages | Individual messages within conversations |
| `ConversationMemory` | ConversationMemories | Context/memory storage for conversations |

### Prompts & Templates
| Entity | Table | Description |
|--------|-------|-------------|
| `PromptTemplate` | PromptTemplates | Reusable prompt templates with versioning |
| `PromptVersion` | PromptVersions | Versioned snapshots of prompt content |

### Knowledge Management
| Entity | Table | Description |
|--------|-------|-------------|
| `KnowledgeBase` | KnowledgeBases | Organized collections of knowledge sources |
| `KnowledgeSource` | KnowledgeSources | Source connectors (documents, web pages, APIs) |
| `KnowledgeDocument` | KnowledgeDocuments | Individual documents within a source |

### Embeddings & Search
| Entity | Table | Description |
|--------|-------|-------------|
| `Embedding` | Embeddings | Vector embeddings for semantic search |
| `EmbeddingChunk` | EmbeddingChunks | Document chunks with embedding relationships |
| `VectorIndex` | VectorIndexes | Vector index configurations |
| `SemanticSearchRequest` | SemanticSearchRequests | Search query history |
| `SemanticSearchResult` | SemanticSearchResults | Individual search results |

### Tools & Workflows
| Entity | Table | Description |
|--------|-------|-------------|
| `ToolDefinition` | ToolDefinitions | Callable tool/function definitions |
| `ToolExecution` | ToolExecutions | Tool invocation history |
| `WorkflowDefinition` | WorkflowDefinitions | Multi-step workflow definitions |
| `WorkflowExecution` | WorkflowExecutions | Workflow execution history |

### Agents
| Entity | Table | Description |
|--------|-------|-------------|
| `AgentDefinition` | AgentDefinitions | Autonomous agent definitions |
| `AgentExecution` | AgentExecutions | Agent execution history |

### Observability
| Entity | Table | Description |
|--------|-------|-------------|
| `AIAuditLog` | AIAuditLogs | Audit trail for AI operations |
| `AITokenUsage` | AITokenUsages | Token usage and cost tracking |

### Routing
| Entity | Table | Description |
|--------|-------|-------------|
| `AIRoutingPolicy` | AIRoutingPolicies | Model/provider routing policies |

## Key Enums

| Enum | Values |
|------|--------|
| `AIProviderType` | OpenAI, AzureOpenAI, Anthropic, Google, Ollama, OpenRouter, Groq, Custom |
| `AIModelCapability` | TextGeneration, CodeGeneration, ImageGeneration, ImageAnalysis, AudioTranscription, Embedding, Reasoning, FunctionCalling, Vision |
| `AIAssistantType` | Coach, Mentor, Analyst, Tutor, Scheduler, Scout, Nutritionist, FitnessTrainer, General |
| `ConversationStatus` | Active, Paused, Archived, Completed |
| `MessageRole` | System, User, Assistant, Tool, Function |
| `KnowledgeBaseVisibility` | Private, Team, Academy, Public |
| `EmbeddingStatus` | Pending, Processing, Completed, Failed |

## Repository Pattern

Each entity has a corresponding repository interface in `SportsGurukul.Application.Common.Interfaces.AI` and implementation in `SportsGurukul.Infrastructure.Persistence.Repositories.AI`. All repositories extend `Repository<T>` and implement their specific interface with query methods.

### Available Repository Methods (base)
- `GetByIdAsync`, `GetAllAsync`, `FindAsync`, `AddAsync`, `Update`, `Remove`, `CountAsync`, `AnyAsync`

### Domain-Specific Query Methods
Each repository adds 3-5 specific query methods based on entity concerns (e.g., `GetByStatusAsync`, `GetActiveAsync`, `GetByProviderIdAsync`).

## Seed Data

### Providers (6)
| Provider | Type | ID |
|----------|------|-----|
| OpenAI | OpenAI | `11111111-...` |
| Azure OpenAI | AzureOpenAI | `22222222-...` |
| Anthropic | Anthropic | `33333333-...` |
| Google AI | Google | `44444444-...` |
| Ollama | Ollama | `55555555-...` |
| OpenRouter | OpenRouter | `66666666-...` |

### Models (6)
| Model | Provider | ID |
|-------|----------|-----|
| GPT-4 | OpenAI | `aaaaaaaa-...` |
| GPT-3.5 Turbo | OpenAI | `bbbbbbbb-...` |
| Claude 3 Opus | Anthropic | `cccccccc-...` |
| Claude 3 Sonnet | Anthropic | `dddddddd-...` |
| Gemini Pro | Google | `eeeeeeee-...` |
| Llama 3 | Ollama | `ffffffff-...` |

## Migration

The initial migration is `20260730150025_AI_Domain_Persistence`.

## Conventions

- All entities extend `BaseEntity` (Id, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, IsDeleted)
- All tables use soft delete via `HasQueryFilter(e => !e.IsDeleted)`
- All enums stored as strings via `HasConversion<string>()`
- All configurations ignore `CreatedBy` and `UpdatedBy` audit fields
- RowVersion used for concurrency control
- Entity type configurations auto-discovered via `ApplyConfigurationsFromAssembly`
