using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Common.Interfaces.AI.Services;

public interface IAuditService
{
    Task<Result<AuditLogDto>> WriteAsync(WriteAuditRequest request, CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<AuditLogDto>>> GetByEntityAsync(string entityType, Guid entityId, CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<AuditLogDto>>> GetByActionAsync(AIAuditAction action, CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<AuditLogDto>>> GetByActorAsync(Guid actorUserId, CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<AuditLogDto>>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<AuditLogDto>>> SearchAsync(
        string? entityType,
        Guid? entityId,
        AIAuditAction? action,
        Guid? actorUserId,
        AIAuditSeverity? severity,
        DateTime? from,
        DateTime? to,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}
