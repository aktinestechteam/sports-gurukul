using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Features.AIManagement.DTOs;

public record ConversationDto(
    Guid Id,
    string? Title,
    Guid? AssistantId,
    string? AssistantName,
    Guid? UserId,
    ConversationStatus Status,
    string? ContextSummary,
    int? TokenCount,
    int MessageCount,
    DateTime? LastActivityAt,
    string? Metadata,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    List<MessageDto> Messages
);

public record ConversationSummaryDto(
    Guid Id,
    string? Title,
    Guid? AssistantId,
    string? AssistantName,
    ConversationStatus Status,
    int MessageCount,
    DateTime? LastActivityAt,
    DateTime CreatedAt
);

public record MessageDto(
    Guid Id,
    Guid ConversationId,
    MessageRole Role,
    MessageStatus Status,
    string Content,
    int? TokensUsed,
    string? ToolCalls,
    string? ToolResults,
    string? ErrorMessage,
    decimal? Cost,
    double? LatencyMs,
    string? Metadata,
    DateTime CreatedAt
);

public record AssistantDto(
    Guid Id,
    string Name,
    string? Description,
    AIAssistantType AssistantType,
    AIAssistantPersonality Personality,
    string? SystemPrompt,
    string? GreetingMessage,
    string? AvatarUrl,
    bool IsActive,
    bool IsPublic,
    int? MaxHistoryLength,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    List<KnowledgeBaseSummaryDto>? KnowledgeBases,
    List<ToolDefinitionDto>? Tools
);

public record AssistantSummaryDto(
    Guid Id,
    string Name,
    string? Description,
    AIAssistantType AssistantType,
    AIAssistantPersonality Personality,
    bool IsActive,
    bool IsPublic,
    DateTime CreatedAt
);

public record PromptTemplateDto(
    Guid Id,
    string Name,
    string? Description,
    PromptType Type,
    PromptStatus Status,
    string TemplateContent,
    string? Variables,
    string? Tags,
    string? Category,
    int CurrentVersion,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    List<PromptVersionDto> Versions
);

public record PromptVersionDto(
    Guid Id,
    Guid PromptTemplateId,
    int VersionNumber,
    string Content,
    string? ChangeNotes,
    string? Hash,
    DateTime CreatedAt
);

public record PromptSummaryDto(
    Guid Id,
    string Name,
    PromptType Type,
    PromptStatus Status,
    int CurrentVersion,
    string? Category,
    DateTime CreatedAt
);

public record KnowledgeBaseDto(
    Guid Id,
    string Name,
    string? Description,
    KnowledgeBaseVisibility Visibility,
    KnowledgeBaseStatus Status,
    string? Category,
    string? Tags,
    string? IconUrl,
    int TotalSources,
    int TotalDocuments,
    long? TotalSizeBytes,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    List<KnowledgeSourceSummaryDto>? Sources
);

public record KnowledgeBaseSummaryDto(
    Guid Id,
    string Name,
    string? Description,
    KnowledgeBaseVisibility Visibility,
    KnowledgeBaseStatus Status,
    int TotalDocuments,
    DateTime CreatedAt
);

public record KnowledgeSourceSummaryDto(
    Guid Id,
    Guid KnowledgeBaseId,
    string Name,
    KnowledgeSourceType SourceType,
    SourceStatus Status,
    int DocumentCount,
    DateTime? LastSyncAt
);

public record KnowledgeDocumentDto(
    Guid Id,
    Guid KnowledgeSourceId,
    DocumentType Type,
    string Title,
    string? Description,
    string? FileName,
    long? FileSizeBytes,
    string? ContentType,
    int? PageCount,
    string? Metadata,
    EmbeddingStatus EmbeddingStatus,
    DateTime? IndexedAt,
    DateTime CreatedAt
);

public record AgentDto(
    Guid Id,
    string Name,
    string? Description,
    Guid? AssistantId,
    string? AssistantName,
    AgentStatus Status,
    string? Configuration,
    string? Tools,
    string? Rules,
    string? Constraints,
    int MaxIterations,
    bool RequiresApproval,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    List<WorkflowSummaryDto>? Workflows
);

public record AgentSummaryDto(
    Guid Id,
    string Name,
    string? Description,
    Guid? AssistantId,
    AgentStatus Status,
    DateTime CreatedAt
);

public record WorkflowDto(
    Guid Id,
    string Name,
    string? Description,
    WorkflowStatus Status,
    string? Steps,
    string? Triggers,
    string? Conditions,
    string? Variables,
    int Version,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record WorkflowSummaryDto(
    Guid Id,
    string Name,
    string? Description,
    WorkflowStatus Status,
    int Version,
    DateTime CreatedAt
);

public record ToolDefinitionDto(
    Guid Id,
    string Name,
    string? Description,
    ToolType Type,
    ToolStatus Status,
    string? Schema,
    string? EndpointUrl,
    string? Parameters,
    string? ReturnType,
    bool RequiresApproval,
    int? TimeoutSeconds,
    DateTime CreatedAt
);

public record TokenUsageDto(
    Guid Id,
    Guid? ConversationId,
    Guid? MessageId,
    string ModelName,
    string? ProviderName,
    int PromptTokens,
    int CompletionTokens,
    int TotalTokens,
    decimal? Cost,
    string? UserId,
    string? SessionId,
    string? RequestType,
    DateTime CreatedAt
);

public record TokenUsageSummaryDto(
    Guid Id,
    string ModelName,
    int TotalTokens,
    decimal? Cost,
    string? RequestType,
    DateTime CreatedAt
);

public record AuditLogDto(
    Guid Id,
    Guid? EntityId,
    string EntityType,
    AuditEventType EventType,
    AuditSeverity Severity,
    string? Action,
    string? ActorId,
    string? ActorType,
    string? IpAddress,
    string? Message,
    string? Metadata,
    DateTime CreatedAt
);

public record ModelCatalogDto(
    Guid Id,
    Guid ProviderId,
    string ProviderName,
    string Name,
    string? DisplayName,
    string? Description,
    AIModelCapability Capabilities,
    AIModelStatus Status,
    int? MaxTokens,
    int? MaxContextLength,
    decimal? CostPerInputToken,
    decimal? CostPerOutputToken,
    double DefaultTemperature,
    bool SupportsStreaming,
    bool SupportsFunctionCalling,
    bool SupportsVision,
    bool SupportsEmbeddings,
    string? ModelVersion,
    DateTime? ReleasedAt,
    DateTime CreatedAt
);

public record PaginatedResult<T>(
    IReadOnlyList<T> Items,
    int TotalCount,
    int Page,
    int PageSize
);
