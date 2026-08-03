using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Features.AIManagement.DTOs;

public record AssistantDto(
    Guid Id,
    string Name,
    string DisplayName,
    string? Description,
    AIAssistantType AssistantType,
    string? SystemPrompt,
    Guid? ModelId,
    string? ModelName,
    double? Temperature,
    double? TopP,
    int? MaxTokens,
    bool MemoryEnabled,
    bool StreamingEnabled,
    bool IsActive,
    AIResourceOwnerType OwnerType,
    Guid? OwnerUserId,
    string? AvatarUrl,
    string? GuardrailsJson,
    List<Guid> AssignedKnowledgeBaseIds,
    List<Guid> AssignedToolIds,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record CreateAssistantRequest(
    string Name,
    string DisplayName,
    string? Description,
    AIAssistantType AssistantType,
    string? SystemPrompt,
    Guid? ModelId,
    double? Temperature,
    double? TopP,
    int? MaxTokens,
    bool MemoryEnabled,
    bool StreamingEnabled,
    AIResourceOwnerType OwnerType,
    Guid? OwnerUserId,
    string? AvatarUrl,
    string? GuardrailsJson,
    string? MetadataJson
);

public record UpdateAssistantRequest(
    Guid AssistantId,
    string? Name,
    string? DisplayName,
    string? Description,
    AIAssistantType? AssistantType,
    string? SystemPrompt,
    Guid? ModelId,
    double? Temperature,
    double? TopP,
    int? MaxTokens,
    bool? MemoryEnabled,
    bool? StreamingEnabled,
    string? AvatarUrl,
    string? GuardrailsJson,
    byte[]? ExpectedRowVersion
);

public record AssignKnowledgeBaseRequest(
    Guid AssistantId,
    List<Guid> KnowledgeBaseIds,
    bool ClearExisting
);

public record AssignToolsRequest(
    Guid AssistantId,
    List<Guid> ToolDefinitionIds,
    bool ClearExisting
);
