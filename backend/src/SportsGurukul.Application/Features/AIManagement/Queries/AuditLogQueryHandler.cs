using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Queries;

public class AuditLogQueryHandler
    : IRequestHandler<AuditLogQuery, Result<PaginatedResult<AuditLogDto>>>
{
    private readonly IAIAuditLogRepository _auditLogRepo;

    public AuditLogQueryHandler(IAIAuditLogRepository auditLogRepo)
    {
        _auditLogRepo = auditLogRepo;
    }

    public async Task<Result<PaginatedResult<AuditLogDto>>> Handle(AuditLogQuery request, CancellationToken cancellationToken)
    {
        var query = await _auditLogRepo.FindAsync(al => true, cancellationToken);

        var filtered = query.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(request.EntityType))
            filtered = filtered.Where(al => al.EntityType.Equals(request.EntityType, StringComparison.OrdinalIgnoreCase));

        if (request.EntityId.HasValue)
            filtered = filtered.Where(al => al.EntityId == request.EntityId.Value);

        if (request.EventType.HasValue)
            filtered = filtered.Where(al => al.EventType == request.EventType.Value);

        if (request.Severity.HasValue)
            filtered = filtered.Where(al => al.Severity == request.Severity.Value);

        if (request.FromDate.HasValue)
            filtered = filtered.Where(al => al.CreatedAt >= request.FromDate.Value);

        if (request.ToDate.HasValue)
            filtered = filtered.Where(al => al.CreatedAt <= request.ToDate.Value);

        var list = filtered.ToList();
        var total = list.Count;
        var paged = list
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(al => new AuditLogDto(
                al.Id, al.EntityId, al.EntityType, al.EventType, al.Severity,
                al.Action, al.ActorId, al.ActorType, al.IpAddress,
                al.Message, al.Metadata, al.CreatedAt
            ))
            .ToList();

        return Result<PaginatedResult<AuditLogDto>>.Success(
            new PaginatedResult<AuditLogDto>(paged, total, request.Page, request.PageSize));
    }
}
