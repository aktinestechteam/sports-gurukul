using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Queries;

public class SearchAuditLogsQueryHandler : IRequestHandler<SearchAuditLogsQuery, Result<IReadOnlyList<AuditLogDto>>>
{
    private readonly IAuditService _auditService;

    public SearchAuditLogsQueryHandler(IAuditService auditService)
    {
        _auditService = auditService;
    }

    public Task<Result<IReadOnlyList<AuditLogDto>>> Handle(SearchAuditLogsQuery request, CancellationToken cancellationToken)
        => _auditService.SearchAsync(
            request.EntityType,
            request.EntityId,
            request.Action,
            request.ActorUserId,
            request.Severity,
            request.From,
            request.To,
            request.Page,
            request.PageSize,
            cancellationToken);
}
