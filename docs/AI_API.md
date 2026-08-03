# AI Platform REST API — Sports Gurukul

Version: 1.0

## Scope

This deliverable exposes the **AI & Intelligence Domain** Application layer (`docs/AI_APPLICATION.md`, `docs/AI_DOMAIN.md`) as REST endpoints under `/api/v1/ai`. It covers conversation/message management, assistant configuration, prompt template versioning, knowledge bases and documents, agents, workflow discovery, token usage accounting, model catalog discovery, and audit log ingestion.

Out of scope for this phase: external LLM provider calls, embeddings, vector search, agent execution, and streaming — those are later sprints.

## Standards

- Base URL: `/api/v1/ai`
- Content-Type / Accept: `application/json`
- Authentication: JWT Bearer (see `docs/authentication`). Every endpoint requires an authenticated user; none are anonymous.
- Versioning: URL segment `v1` + `X-Api-Version` header; all AI endpoints are version `1.0`.
- Pagination: `page` (1-based, default 1) and `pageSize` (default 20, max 100; model catalog default 50, max 200) query parameters on every search endpoint.

## Common Response Format

Success (all actions return HTTP 200 with the envelope; no 201s):

```json
{
  "success": true,
  "message": "Conversations retrieved successfully.",
  "data": {}
}
```

Failure (validation, not found, conflict, forbidden) is returned as RFC 7807 `ProblemDetails`, mapped by `ApiExceptionFilterAttribute` or the controllers' failure helper:

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Bad Request",
  "status": 400,
  "detail": "Validation failed: ..."
}
```

Status codes used: `200` success, `400` validation/domain error, `401` unauthenticated, `403` forbidden, `404` not found, `409` conflict (e.g. duplicate), `429` rate limited.

## Authorization Matrix

All AI endpoints require `[Authorize]`. Role membership is enforced server-side via `[Authorize(Roles = "...")]`.

| Surface | Roles |
|---|---|
| AI Conversations (`/ai/conversations`) | Coach, Athlete, Academy Admin, AI Administrator, System Admin |
| AI Messages (`/ai/conversations/{id}/messages`) | Coach, Athlete, Academy Admin, AI Administrator, System Admin |
| AI Workflows (`/ai/workflows`) | Coach, Athlete, Academy Admin, AI Administrator, System Admin |
| AI Assistants (`/ai/assistants`) | AI Administrator, System Admin |
| AI Prompt Templates (`/ai/prompt-templates`) | AI Administrator, System Admin |
| AI Knowledge Bases (`/ai/knowledge-bases`) | AI Administrator, System Admin |
| AI Knowledge Documents (`/ai/knowledge-bases/{id}/documents`) | AI Administrator, System Admin |
| AI Agents (`/ai/agents`) | AI Administrator, System Admin |
| AI Token Usage (`/ai/token-usage`) | AI Administrator, System Admin |
| AI Model Catalog (`/ai/models`) | AI Administrator, System Admin |
| AI Audit Logs (`/ai/audit-logs`) | AI Administrator, System Admin |

The `AI Administrator` role is a seeded role (`RoleType.AIAdministrator`, migration `20260803041321_AddAIAdministratorRole`) dedicated to managing assistants, prompts, knowledge bases, agents, workflows, and model usage.

## Endpoint Catalog

### AI Conversations

Base: `/api/v1/ai/conversations`

| Method | Route | Action | Request | Response data |
|---|---|---|---|---|
| POST | `/` | Create conversation | `CreateConversationCommand` | `ConversationDto` |
| GET | `/` | Search conversations | query: `searchTerm, assistantId, participantUserId, status, page, pageSize` | `IReadOnlyList<ConversationSummaryDto>` |
| GET | `/{conversationId}` | Get conversation | — | `ConversationDto` |
| PATCH | `/{conversationId}` | Rename | `RenameConversationCommand` (title) | `ConversationDto` |
| POST | `/{conversationId}/archive` | Archive | — | `ConversationDto` |
| DELETE | `/{conversationId}` | Delete | — | `bool` |
| POST | `/{conversationId}/summarize` | Store summary | `SummarizeConversationCommand` | `ConversationDto` |
| DELETE | `/{conversationId}/memory` | Clear memory | — | `bool` |
| GET | `/{conversationId}/memory` | Get memory | — | `IReadOnlyList<ConversationMemoryDto>` |

`status` is an `AIConversationStatus` enum value.

### AI Messages

Base: `/api/v1/ai/conversations/{conversationId}/messages`

| Method | Route | Action | Request | Response data |
|---|---|---|---|---|
| GET | `/` | Get history | — | `IReadOnlyList<MessageDto>` |
| POST | `/` | Add message | `AddMessageCommand` | `MessageDto` |
| POST | `/regenerate` | Regenerate response | — | `MessageDto` |

### AI Assistants

Base: `/api/v1/ai/assistants`

| Method | Route | Action | Request | Response data |
|---|---|---|---|---|
| POST | `/` | Create | `CreateAssistantCommand` | `AssistantDto` |
| GET | `/` | Search | query: `searchTerm, assistantType, ownerUserId, isActive, page, pageSize` | `IReadOnlyList<AssistantDto>` |
| GET | `/{assistantId}` | Get by id | — | `AssistantDto` |
| PATCH | `/{assistantId}` | Update | `UpdateAssistantCommand` | `AssistantDto` |
| POST | `/{assistantId}/publish` | Publish | — | `AssistantDto` |
| POST | `/{assistantId}/archive` | Archive | — | `AssistantDto` |
| PUT | `/{assistantId}/knowledge-bases` | Assign knowledge bases | `AssignKnowledgeBaseCommand` | `AssistantDto` |
| PUT | `/{assistantId}/tools` | Assign tools | `AssignToolsCommand` | `AssistantDto` |

### AI Prompt Templates

Base: `/api/v1/ai/prompt-templates`

| Method | Route | Action | Request | Response data |
|---|---|---|---|---|
| POST | `/` | Create | `CreatePromptTemplateCommand` | `PromptTemplateDto` |
| GET | `/` | Search | query: `searchTerm, assistantId, promptType, isActive, page, pageSize` | `IReadOnlyList<PromptTemplateDto>` |
| GET | `/{promptTemplateId}` | Get by id | — | `PromptTemplateDto` |
| PATCH | `/{promptTemplateId}` | Update | `UpdatePromptTemplateCommand` | `PromptTemplateDto` |
| POST | `/{promptTemplateId}/publish` | Publish version | `PublishPromptTemplateCommand` | `PromptTemplateDto` |
| POST | `/{promptTemplateId}/rollback` | Rollback version | `RollbackPromptVersionCommand` (versionNumber) | `PromptTemplateDto` |
| POST | `/{promptTemplateId}/clone` | Clone | `ClonePromptCommand` | `PromptTemplateDto` |

### AI Knowledge Bases

Base: `/api/v1/ai/knowledge-bases`

| Method | Route | Action | Request | Response data |
|---|---|---|---|---|
| POST | `/` | Create | `CreateKnowledgeBaseCommand` | `KnowledgeBaseDto` |
| GET | `/` | Search | query: `searchTerm, knowledgeBaseType, ownerUserId, isActive, page, pageSize` | `IReadOnlyList<KnowledgeBaseDto>` |
| GET | `/{knowledgeBaseId}` | Get by id | — | `KnowledgeBaseDto` |
| PATCH | `/{knowledgeBaseId}` | Update | `UpdateKnowledgeBaseCommand` | `KnowledgeBaseDto` |
| POST | `/{knowledgeBaseId}/rebuild-index` | Rebuild index | — | `KnowledgeBaseDto` |

### AI Knowledge Documents

Base: `/api/v1/ai/knowledge-bases/{knowledgeBaseId}/documents`

| Method | Route | Action | Request | Response data |
|---|---|---|---|---|
| GET | `/` | List documents | — | `IReadOnlyList<KnowledgeDocumentDto>` |
| POST | `/` | Attach document | `AttachDocumentCommand` | `KnowledgeDocumentDto` |
| DELETE | `/{documentId}` | Detach document | — | `bool` |

### AI Agents

Base: `/api/v1/ai/agents`

| Method | Route | Action | Request | Response data |
|---|---|---|---|---|
| POST | `/` | Create | `CreateAgentCommand` | `AgentDto` |
| GET | `/` | Search | query: `searchTerm, agentType, workflowId, isActive, page, pageSize` | `IReadOnlyList<AgentDto>` |
| GET | `/{agentId}` | Get by id | — | `AgentDto` |
| PATCH | `/{agentId}` | Update | `UpdateAgentCommand` | `AgentDto` |
| POST | `/{agentId}/enable` | Enable | — | `AgentDto` |
| POST | `/{agentId}/disable` | Disable | — | `AgentDto` |
| PUT | `/{agentId}/workflow` | Assign workflow | `AssignWorkflowCommand` | `AgentDto` |

### AI Workflows

Base: `/api/v1/ai/workflows`

| Method | Route | Action | Request | Response data |
|---|---|---|---|---|
| GET | `/` | Search | query: `searchTerm, workflowType, isActive, isPublished, page, pageSize` | `IReadOnlyList<WorkflowDto>` |
| GET | `/published` | Published workflows | — | `IReadOnlyList<WorkflowDto>` |
| GET | `/{workflowId}` | Get by id | — | `WorkflowDto` |

### AI Token Usage

Base: `/api/v1/ai/token-usage`

| Method | Route | Action | Request | Response data |
|---|---|---|---|---|
| POST | `/` | Record usage | `RecordTokenUsageCommand` | `TokenUsageDto` |
| GET | `/` | Search | query: `assistantId, conversationId, userId, usageType, from, to, page, pageSize` | `IReadOnlyList<TokenUsageDto>` |
| GET | `/summary` | Usage summary | query: `assistantId, conversationId, userId, from, to, usageType` | `TokenUsageSummaryDto` |

### AI Model Catalog

Base: `/api/v1/ai/models`

| Method | Route | Action | Request | Response data |
|---|---|---|---|---|
| GET | `/` | List models | query: `searchTerm, family, providerId, supportsChat, supportsFunctionCalling, supportsVision, supportsJsonMode, page, pageSize` | `IReadOnlyList<ModelCandidate>` |
| GET | `/available` | Routing candidates | query: `routingStrategy, assistantId, agentDefinitionId, conversationId, estimatedInputTokens, maxOutputTokens, requiresFunctionCalling, requiresVision, requiresJsonMode, maxCostPerRequest, maxLatencyMs, preferredModelIds, fallbackModelIds` | `IReadOnlyList<ModelCandidate>` |
| GET | `/{modelId}` | Get by id | — | `ModelCandidate` |

`routingStrategy` defaults to `Balanced` (`AIRoutingStrategy` enum: Manual, Cost, Speed, Accuracy, Balanced, Fallback).

### AI Audit Logs

Base: `/api/v1/ai/audit-logs`

| Method | Route | Action | Request | Response data |
|---|---|---|---|---|
| POST | `/` | Write entry | `WriteAuditLogCommand` | `AuditLogDto` |
| GET | `/` | Search | query: `entityType, entityId, action, actorUserId, severity, from, to, page, pageSize` | `IReadOnlyList<AuditLogDto>` |

## Request Models

Controllers bind the CQRS command records directly (`[FromBody]`), so request JSON mirrors each command's positional parameters (camelCase by default serializer configuration). Route ids override the corresponding id field on the body when both exist (e.g. `conversationId`, `assistantId`, `promptTemplateId`, `knowledgeBaseId`, `agentId`, `modelId`, `documentId`).

Enums (`AIAssistantType`, `AIPromptType`, `AIKnowledgeBaseType`, `AIAgentType`, `AIWorkflowType`, `AIUsageType`, `AIAuditAction`, `AIAuditSeverity`, `AIConversationStatus`, `AIResourceOwnerType`, `AIMessageRole`, `AIMessageContentType`, `AIKnowledgeDocumentType`, `AIChunkingStrategy`, `AIModelFamily`) are serialized as their numeric values and accepted as integers in JSON bodies and query strings.

## Error Handling

- Validation failures (`ValidationBehavior`) and domain failures surface as `Result<T>.Failure`; controllers map them to `ProblemDetails`.
- Not-found errors return 404; duplicate/conflict errors return 409; permission errors return 403; everything else 400.
- Structured logging only — prompts, message content, user data, tokens, and API keys are never logged.

## Verification

- Build: `dotnet build backend/src/SportsGurukul.Api/SportsGurukul.Api.csproj` → 0 errors.
- Tests: `dotnet test backend/tests/SportsGurukul.Application.Tests --filter "FullyQualifiedName~AI"` → 575 passed / 0 failed.
- OpenAPI: all AI tags (`AI Conversations`, `AI Messages`, `AI Assistants`, `AI Prompt Templates`, `AI Knowledge Bases`, `AI Knowledge Documents`, `AI Agents`, `AI Workflows`, `AI Token Usage`, `AI Model Catalog`, `AI Audit Logs`) appear under `/swagger/v1/swagger.json`.
