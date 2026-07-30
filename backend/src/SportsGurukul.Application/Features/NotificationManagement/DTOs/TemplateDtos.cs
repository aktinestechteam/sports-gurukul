using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Application.Features.NotificationManagement.DTOs;

public record TemplateDto(
    Guid Id,
    string Name,
    string? Description,
    NotificationChannelType ChannelType,
    string SubjectTemplate,
    string BodyTemplate,
    bool IsActive,
    int CurrentVersion,
    DateTime CreatedAt,
    List<TemplateVersionDto> Versions,
    List<TemplateVariableDto> Variables
);

public record TemplateVersionDto(
    Guid Id,
    int VersionNumber,
    string SubjectTemplate,
    string BodyTemplate,
    string? ChangeNotes,
    DateTime PublishedAt
);

public record TemplateVariableDto(
    Guid Id,
    string Name,
    string? Description,
    bool IsRequired,
    string? DefaultValue,
    string DataType
);

public record CreateTemplateRequest(
    string Name,
    string? Description,
    NotificationChannelType ChannelType,
    string SubjectTemplate,
    string BodyTemplate,
    List<CreateTemplateVariableRequest>? Variables
);

public record UpdateTemplateRequest(
    Guid Id,
    string? Name,
    string? Description,
    string? SubjectTemplate,
    string? BodyTemplate,
    List<CreateTemplateVariableRequest>? Variables
);

public record CreateTemplateVariableRequest(
    string Name,
    string? Description,
    bool IsRequired,
    string? DefaultValue,
    string DataType
);

public record CreateTemplateVersionRequest(
    Guid TemplateId,
    string SubjectTemplate,
    string BodyTemplate,
    string? ChangeNotes
);
