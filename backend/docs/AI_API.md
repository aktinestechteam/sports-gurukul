# AI & Intelligence Platform — REST API

## Overview

The AI & Intelligence Platform REST API exposes AI capabilities (conversations, assistants, prompts, knowledge bases, agents, workflows, model catalog, token usage, audit logs) as versioned REST endpoints.

- **Base URL:** `/api/v1`
- **Content-Type:** `application/json`
- **Authentication:** JWT Bearer Token
- **API Versioning:** URL segment (`/api/v1/`) + Header (`X-Api-Version`)

---

## Authentication & Authorization

### Authentication

All endpoints require a valid JWT access token in the `Authorization` header:

```
Authorization: Bearer <token>
```

Endpoints marked `[AllowAnonymous]` (e.g., search public prompts, public knowledge bases) do not require authentication.

### Authorization Matrix

| Role | Conversations | Assistants | Prompts | Knowledge | Agents | Workflows | Token Usage | Audit Logs | Models |
|---|---|---|---|---|---|---|---|---|---|
| **Platform Administrator** | Full | Full | Full | Full | Full | Full | Full | Full | Full |
| **AI Administrator** | Full | Full | Full | Full | Full | Full | Full | Full | Full |
| **Academy Administrator** | Own | View | View | View | View | View | - | - | View |
| **Coach** | Own | View | View | View | - | - | - | - | View |
| **Athlete** | Own | View | - | View | - | - | - | - | View |
| **Anonymous** | - | - | Public | Public | - | - | - | - | - |

### Policies Defined

| Policy | Roles |
|---|---|
| `AI.FullAccess` | Platform Administrator, AI Administrator |
| `AI.Admin` | Platform Administrator, AI Administrator, Academy Administrator |
| `AI.Owner` | Platform Administrator, AI Administrator, Academy Administrator, Coach, Athlete |

---

## Endpoint Catalog

### Conversations (`/api/v1/conversations`)

| Method | Route | Description | Auth |
|---|---|---|---|
| POST | `/api/v1/conversations` | Create a new conversation | JWT |
| GET | `/api/v1/conversations` | Search conversations (paginated) | JWT |
| GET | `/api/v1/conversations/search` | Search conversations alias | JWT |
| GET | `/api/v1/conversations/{id}` | Get conversation by ID | JWT |
| PUT | `/api/v1/conversations/{id}` | Rename conversation | JWT |
| DELETE | `/api/v1/conversations/{id}` | Delete conversation | JWT |
| POST | `/api/v1/conversations/{id}/summarize` | Summarize conversation | JWT |
| POST | `/api/v1/conversations/{id}/regenerate` | Regenerate last response | JWT |
| DELETE | `/api/v1/conversations/{id}/memory` | Clear conversation memory | JWT |

### Messages (`/api/v1/conversations/{conversationId}/messages`)

| Method | Route | Description | Auth |
|---|---|---|---|
| POST | `/api/v1/conversations/{conversationId}/messages` | Add a message to conversation | JWT |
| GET | `/api/v1/conversations/{conversationId}/messages` | Get conversation history | JWT |

### Assistants (`/api/v1/assistants`)

| Method | Route | Description | Auth |
|---|---|---|---|
| POST | `/api/v1/assistants` | Create a new assistant | JWT |
| GET | `/api/v1/assistants` | Search assistants | JWT |
| GET | `/api/v1/assistants/search` | Search assistants alias | JWT |
| GET | `/api/v1/assistants/{id}` | Get assistant by ID | JWT |
| PUT | `/api/v1/assistants/{id}` | Update assistant | JWT |
| POST | `/api/v1/assistants/{id}/publish` | Publish assistant | JWT |
| POST | `/api/v1/assistants/{id}/knowledge` | Attach knowledge base | JWT |
| POST | `/api/v1/assistants/{id}/tools` | Assign tools | JWT |

### Prompts (`/api/v1/prompts`)

| Method | Route | Description | Auth |
|---|---|---|---|
| POST | `/api/v1/prompts` | Create prompt template | Admin |
| GET | `/api/v1/prompts` | Search prompt templates | Public |
| GET | `/api/v1/prompts/search` | Search prompts alias | Public |
| GET | `/api/v1/prompts/{id}` | Get prompt template | Public |
| PUT | `/api/v1/prompts/{id}` | Update prompt template | Admin |
| POST | `/api/v1/prompts/{id}/publish` | Publish prompt template | Admin |
| POST | `/api/v1/prompts/{id}/rollback` | Rollback to version | Admin |
| POST | `/api/v1/prompts/{id}/clone` | Clone prompt template | Admin |

### Knowledge Bases (`/api/v1/knowledge-bases`)

| Method | Route | Description | Auth |
|---|---|---|---|
| POST | `/api/v1/knowledge-bases` | Create knowledge base | Admin |
| GET | `/api/v1/knowledge-bases` | Search knowledge bases | Public |
| GET | `/api/v1/knowledge-bases/search` | Search knowledge bases alias | Public |
| GET | `/api/v1/knowledge-bases/{id}` | Get knowledge base | Public |
| PUT | `/api/v1/knowledge-bases/{id}` | Update knowledge base | Admin |
| DELETE | `/api/v1/knowledge-bases/{id}` | Delete knowledge base | Admin |
| POST | `/api/v1/knowledge-bases/{id}/documents` | Attach document | Admin |
| DELETE | `/api/v1/knowledge-bases/{id}/documents/{documentId}` | Detach document | Admin |
| POST | `/api/v1/knowledge-bases/{id}/rebuild-index` | Rebuild search index | Admin |

### Knowledge Documents (`/api/v1/knowledge-bases/{knowledgeBaseId}/documents`)

| Method | Route | Description | Auth |
|---|---|---|---|
| GET | `/api/v1/knowledge-bases/{knowledgeBaseId}/documents` | List documents in knowledge base | Public |

### Agents (`/api/v1/agents`)

| Method | Route | Description | Auth |
|---|---|---|---|
| POST | `/api/v1/agents` | Create agent | Admin |
| GET | `/api/v1/agents` | Search agents | Admin |
| GET | `/api/v1/agents/search` | Search agents alias | Admin |
| GET | `/api/v1/agents/{id}` | Get agent | Admin |
| PUT | `/api/v1/agents/{id}` | Update agent | Admin |
| POST | `/api/v1/agents/{id}/enable` | Enable agent | Admin |
| POST | `/api/v1/agents/{id}/disable` | Disable agent | Admin |
| POST | `/api/v1/agents/{id}/workflow` | Assign workflow | Admin |

### Workflows (`/api/v1/workflows`)

| Method | Route | Description | Auth |
|---|---|---|---|
| POST | `/api/v1/workflows` | Create workflow | Admin |
| GET | `/api/v1/workflows` | Search workflows | Admin |
| GET | `/api/v1/workflows/{id}` | Get workflow | Admin |
| PUT | `/api/v1/workflows/{id}` | Update workflow | Admin |

### Model Catalog (`/api/v1/models`)

| Method | Route | Description | Auth |
|---|---|---|---|
| GET | `/api/v1/models` | List available AI models | JWT |

### Token Usage (`/api/v1/token-usage`)

| Method | Route | Description | Auth |
|---|---|---|---|
| GET | `/api/v1/token-usage` | Query token consumption | Admin |

### Audit Logs (`/api/v1/audit-logs`)

| Method | Route | Description | Auth |
|---|---|---|---|
| GET | `/api/v1/audit-logs` | Query audit trail | Admin |

---

## Swagger Tags

| Tag | Controllers |
|---|---|
| `Conversations` | ConversationsController |
| `Messages` | MessagesController |
| `Assistants` | AssistantsController |
| `Prompts` | PromptTemplatesController |
| `Knowledge Bases` | KnowledgeBasesController |
| `Knowledge Documents` | KnowledgeDocumentsController |
| `Agents` | AgentsController |
| `Workflows` | WorkflowsController |
| `Model Catalog` | ModelCatalogController |
| `Token Usage` | TokenUsageController |
| `Audit Logs` | AuditLogsController |

---

## HTTP Status Codes

| Code | Description |
|---|---|
| `200 OK` | Successful operation |
| `201 Created` | Resource created successfully |
| `400 Bad Request` | Validation failure or business rule violation |
| `401 Unauthorized` | Missing or invalid JWT token |
| `403 Forbidden` | Insufficient permissions |
| `404 Not Found` | Resource does not exist |
| `409 Conflict` | Duplicate resource or state conflict |
| `429 Too Many Requests` | Rate limit exceeded |
| `500 Internal Server Error` | Unexpected server error |

---

## Response Format

### Success Response

```json
{
  "success": true,
  "message": "Operation completed successfully.",
  "data": { ... }
}
```

### Paginated Response

```json
{
  "success": true,
  "message": "Items retrieved successfully.",
  "data": {
    "items": [ ... ],
    "totalCount": 100,
    "page": 1,
    "pageSize": 20
  }
}
```

### Error Response (RFC 7807 ProblemDetails)

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Bad Request",
  "status": 400,
  "detail": "Validation error description",
  "errors": {
    "fieldName": [ "Error message" ]
  }
}
```

---

## Controllers & Files

### Controllers (in `Controllers/V1/AI/`)

| File | Endpoints |
|---|---|
| `ConversationsController.cs` | 10 conversation CRUD + action endpoints |
| `MessagesController.cs` | 2 message sub-resource endpoints |
| `AssistantsController.cs` | 8 assistant CRUD + action endpoints |
| `PromptTemplatesController.cs` | 8 prompt template endpoints |
| `KnowledgeBasesController.cs` | 9 knowledge base endpoints |
| `KnowledgeDocumentsController.cs` | 1 document listing endpoint |
| `AgentsController.cs` | 8 agent CRUD + action endpoints |
| `WorkflowsController.cs` | 4 workflow CRUD endpoints |
| `ModelCatalogController.cs` | 1 model listing endpoint |
| `TokenUsageController.cs` | 1 token usage query endpoint |
| `AuditLogsController.cs` | 1 audit log query endpoint |

### Models (in `Common/Models/AI/`)

| File | Contents |
|---|---|
| `AIRequestModels.cs` | All 17 request DTOs |
| `AISwaggerExamples.cs` | 13 Swagger request example providers |

### New Application Layer Files

| File | Type |
|---|---|
| `Commands/Workflow/CreateWorkflowCommand.cs` | Create workflow command |
| `Commands/Workflow/CreateWorkflowCommandHandler.cs` | Handler |
| `Commands/Workflow/UpdateWorkflowCommand.cs` | Update workflow command |
| `Commands/Workflow/UpdateWorkflowCommandHandler.cs` | Handler |
| `Queries/GetModelsQuery.cs` | Model catalog query |
| `Queries/GetModelsQueryHandler.cs` | Handler |

---

## Logging

All controllers use structured logging with `ILogger<T>`. No prompts, conversation content, API keys, tokens, or other sensitive information is logged. Only metadata (IDs, counts, statuses) is recorded.

## OpenAPI Specification

The complete OpenAPI 3.0 specification is available at:

- **Swagger UI:** `/swagger` (development only)
- **OpenAPI JSON:** `/swagger/v1/swagger.json`
- **Saved file:** `docs/openapi.json`
