using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Features.AIManagement.DTOs;

public record PromptTemplateDto(
    Guid Id,
    Guid AssistantId,
    string Name,
    string? Description,
    AIPromptType PromptType,
    string TemplateText,
    string? InputSchemaJson,
    string? OutputSchemaJson,
    string? VariablesJson,
    int CurrentVersion,
    bool IsActive,
    bool IsDefault,
    List<PromptVersionDto> Versions,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record PromptVersionDto(
    Guid Id,
    Guid PromptTemplateId,
    int VersionNumber,
    string Content,
    string? ChangeSummary,
    string? Notes,
    Guid? CreatedByUserId,
    bool IsActive,
    DateTime? DeployedAt,
    DateTime CreatedAt
);

public record CreatePromptTemplateRequest(
    Guid AssistantId,
    string Name,
    string? Description,
    AIPromptType PromptType,
    string TemplateText,
    string? InputSchemaJson,
    string? OutputSchemaJson,
    string? VariablesJson,
    bool IsDefault
);

public record UpdatePromptTemplateRequest(
    Guid PromptTemplateId,
    string? Name,
    string? Description,
    string? TemplateText,
    string? InputSchemaJson,
    string? OutputSchemaJson,
    string? VariablesJson,
    bool? IsActive,
    byte[]? ExpectedRowVersion
);

public record PublishPromptTemplateRequest(
    Guid PromptTemplateId,
    string? ChangeSummary,
    string? Notes
);

public record RollbackPromptVersionRequest(
    Guid PromptTemplateId,
    int VersionNumber
);

public record ClonePromptRequest(
    Guid SourcePromptId,
    Guid? TargetAssistantId,
    string? NewName
);
