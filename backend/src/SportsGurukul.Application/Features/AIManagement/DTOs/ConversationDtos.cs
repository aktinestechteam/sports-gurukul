using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Features.AIManagement.DTOs;

public record ConversationDto(
    Guid Id,
    Guid AssistantId,
    string Title,
    string? Summary,
    AIConversationStatus Status,
    AIResourceOwnerType ParticipantType,
    Guid? ParticipantUserId,
    DateTime? StartedAt,
    DateTime? LastMessageAt,
    int MessageCount,
    int TokenCount,
    List<Guid> KnowledgeBaseIds,
    DateTime? ArchivedAt,
    DateTime CreatedAt
);

public record ConversationSummaryDto(
    Guid Id,
    Guid AssistantId,
    string Title,
    AIConversationStatus Status,
    Guid? ParticipantUserId,
    int MessageCount,
    int TokenCount,
    DateTime? LastMessageAt,
    DateTime? UpdatedAt
);

public record MessageDto(
    Guid Id,
    Guid ConversationId,
    int SequenceNumber,
    AIMessageRole Role,
    AIMessageContentType ContentType,
    string Content,
    string? ModelName,
    int? PromptVersionUsed,
    int? InputTokenCount,
    int? OutputTokenCount,
    long? LatencyMs,
    string? ToolCallsJson,
    string? ToolResultsJson,
    DateTime CreatedAt
);

public record ConversationMemoryDto(
    Guid Id,
    Guid ConversationId,
    AIMemoryType MemoryType,
    string Key,
    string Content,
    int Importance,
    DateTime? ExpiresAt,
    DateTime CreatedAt
);

public record CreateConversationRequest(
    Guid AssistantId,
    string Title,
    AIResourceOwnerType ParticipantType,
    Guid? ParticipantUserId,
    List<Guid>? KnowledgeBaseIds,
    string? ContextMetadataJson
);

public record AddMessageRequest(
    Guid ConversationId,
    AIMessageRole Role,
    AIMessageContentType ContentType,
    string Content,
    string? ModelName,
    int? PromptVersionUsed,
    int? InputTokenCount,
    int? OutputTokenCount,
    long? LatencyMs,
    string? ToolCallsJson,
    string? ToolResultsJson
);

public record SummarizeConversationRequest(
    Guid ConversationId,
    string Summary
);
