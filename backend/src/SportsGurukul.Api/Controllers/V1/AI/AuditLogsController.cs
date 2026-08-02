using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Application.Features.AIManagement.Queries;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Api.Controllers.V1.AI;

[Route("api/v1/audit-logs")]
[Authorize(Roles = "Platform Administrator,AI Administrator")]
[Tags("Audit Logs")]
public class AuditLogsController : AIControllerBase
{
    public AuditLogsController(IMediator mediator, ILogger<AuditLogsController> logger)
        : base(mediator, logger)
    {
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<AuditLogDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLogs(
        [FromQuery] string? entityType,
        [FromQuery] Guid? entityId,
        [FromQuery] AuditEventType? eventType,
        [FromQuery] AuditSeverity? severity,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        Logger.LogInformation("Fetching audit logs");

        var query = new AuditLogQuery(entityType, entityId, eventType, severity, fromDate, toDate, page, pageSize);
        var result = await Mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<PaginatedResult<AuditLogDto>>.SuccessResult(
            result.Value!, "Audit logs retrieved successfully."));
    }
}
