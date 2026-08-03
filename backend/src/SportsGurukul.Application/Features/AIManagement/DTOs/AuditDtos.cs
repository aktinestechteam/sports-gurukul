using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Features.AIManagement.DTOs;

public record AuditLogDto(
    Guid Id,
    Guid? ActorUserId,
    AIResourceOwnerType ActorType,
    AIAuditAction Action,
    string EntityType,
    Guid? EntityId,
    string? DetailsJson,
    string? BeforeJson,
    string? AfterJson,
    string? ChangedFieldsJson,
    string? IpAddress,
    string? UserAgent,
    string? CorrelationId,
    AIAuditSeverity Severity,
    DateTime CreatedAt
);

public record WriteAuditRequest(
    Guid? ActorUserId,
    AIResourceOwnerType ActorType,
    AIAuditAction Action,
    string EntityType,
    Guid? EntityId,
    string? DetailsJson,
    string? BeforeJson,
    string? AfterJson,
    string? ChangedFieldsJson,
    string? IpAddress,
    string? UserAgent,
    string? CorrelationId,
    AIAuditSeverity Severity
);
