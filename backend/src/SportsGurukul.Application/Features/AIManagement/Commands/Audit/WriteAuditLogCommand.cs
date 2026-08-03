using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Audit;

public record WriteAuditLogCommand(
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
) : IRequest<Result<AuditLogDto>>;
