using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Audit;

public class WriteAuditLogCommandHandler : IRequestHandler<WriteAuditLogCommand, Result<AuditLogDto>>
{
    private readonly IAuditService _auditService;

    public WriteAuditLogCommandHandler(IAuditService auditService)
    {
        _auditService = auditService;
    }

    public async Task<Result<AuditLogDto>> Handle(WriteAuditLogCommand request, CancellationToken cancellationToken)
    {
        var auditRequest = new WriteAuditRequest(
            request.ActorUserId,
            request.ActorType,
            request.Action,
            request.EntityType,
            request.EntityId,
            request.DetailsJson,
            request.BeforeJson,
            request.AfterJson,
            request.ChangedFieldsJson,
            request.IpAddress,
            request.UserAgent,
            request.CorrelationId,
            request.Severity);

        return await _auditService.WriteAsync(auditRequest, cancellationToken);
    }
}
