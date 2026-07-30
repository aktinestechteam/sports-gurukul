using SportsGurukul.Application.Features.NotificationManagement.DTOs;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Platform.Communication.Analytics.DTOs;

public enum TemplateCategory
{
    General,
    Welcome,
    Verification,
    PasswordReset,
    Promotional,
    Transactional,
    Alert,
    Reminder,
    Report,
    Invoice,
    EventInvite,
    Feedback,
    Onboarding,
    Milestone,
    Custom
}

public enum TemplateStatus
{
    Draft,
    Published,
    Archived
}

public record TemplateDetailDto(
    Guid Id,
    string Name,
    string? Description,
    NotificationChannelType ChannelType,
    TemplateCategory Category,
    TemplateStatus Status,
    string SubjectTemplate,
    string BodyTemplate,
    bool IsActive,
    int CurrentVersion,
    DateTime CreatedAt,
    DateTime? PublishedAt,
    DateTime? ArchivedAt,
    string? CreatedBy,
    string? PublishedBy,
    List<TemplateVersionDetailDto> Versions,
    List<TemplateVariableDetailDto> Variables,
    List<TemplateLocalizationDto> Localizations,
    List<TemplatePartialDto> Partials,
    List<TemplateAttachmentMetaDto> Attachments,
    Dictionary<string, string>? Metadata
);

public record TemplateVersionDetailDto(
    Guid Id,
    int VersionNumber,
    string SubjectTemplate,
    string BodyTemplate,
    string? ChangeNotes,
    TemplateStatus Status,
    string? PublishedBy,
    DateTime CreatedAt,
    DateTime? PublishedAt
);

public record TemplateVariableDetailDto(
    Guid Id,
    string Name,
    string? Description,
    bool IsRequired,
    string? DefaultValue,
    string DataType,
    string? Group,
    int? MaxLength,
    string? RegexPattern,
    List<string>? AllowedValues
);

public record TemplateLocalizationDto(
    Guid Id,
    string Locale,
    string? SubjectTemplate,
    string? BodyTemplate,
    bool IsComplete,
    DateTime CreatedAt
);

public record TemplatePartialDto(
    Guid Id,
    string Name,
    string Content,
    string? Description,
    NotificationChannelType? ChannelType,
    Dictionary<string, string>? Variables,
    DateTime CreatedAt
);

public record TemplateAttachmentMetaDto(
    Guid Id,
    string FileName,
    string ContentType,
    long SizeBytes,
    bool IsRequired,
    string? Description
);

public record CreateTemplateFullRequest(
    string Name,
    string? Description,
    NotificationChannelType ChannelType,
    TemplateCategory Category,
    string SubjectTemplate,
    string BodyTemplate,
    List<CreateTemplateVariableRequest>? Variables,
    List<CreateLocalizationRequest>? Localizations,
    List<string>? PartialNames,
    List<CreateAttachmentMetaRequest>? Attachments,
    Dictionary<string, string>? Metadata
);

public record UpdateTemplateFullRequest(
    string? Name,
    string? Description,
    TemplateCategory? Category,
    string? SubjectTemplate,
    string? BodyTemplate,
    List<CreateTemplateVariableRequest>? Variables,
    List<CreateLocalizationRequest>? Localizations,
    List<string>? PartialNames,
    List<CreateAttachmentMetaRequest>? Attachments,
    Dictionary<string, string>? Metadata
);

public record CreateLocalizationRequest(
    string Locale,
    string? SubjectTemplate,
    string? BodyTemplate
);

public record CreateAttachmentMetaRequest(
    string FileName,
    string ContentType,
    long SizeBytes,
    bool IsRequired,
    string? Description
);

public record CloneTemplateRequest(
    string NewName,
    string? NewDescription,
    TemplateCategory? NewCategory,
    bool IncludeVariables,
    bool IncludeLocalizations,
    bool IncludePartials
);

public record RollbackTemplateRequest(
    int TargetVersion,
    string? ChangeNotes
);

public record TemplateVersionCompareDto(
    int FromVersion,
    int ToVersion,
    string SubjectDiff,
    string BodyDiff,
    List<string> ChangedFields
);

public record TemplateRenderPreviewRequest(
    string SubjectTemplate,
    string BodyTemplate,
    Dictionary<string, object?> TestData,
    string? Locale
);

public record TemplateRenderPreviewResult(
    string RenderedSubject,
    string RenderedBody,
    long RenderTimeMs,
    List<string> ResolvedVariables,
    List<string> UnresolvedVariables,
    List<string> Warnings
);

public record TemplateSearchCriteria(
    string? Query,
    TemplateCategory? Category,
    TemplateStatus? Status,
    NotificationChannelType? ChannelType,
    DateTime? CreatedAfter,
    DateTime? CreatedBefore,
    string? CreatedBy,
    bool? HasLocalizations,
    int PageNumber = 1,
    int PageSize = 20
);

public record TemplateSearchResult(
    List<TemplateDetailDto> Items,
    int TotalCount,
    int PageNumber,
    int PageSize,
    bool HasNextPage
);
