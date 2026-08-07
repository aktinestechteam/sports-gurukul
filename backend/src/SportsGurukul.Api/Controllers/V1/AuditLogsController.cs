using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.AIManagement.Commands.Audit;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Application.Features.AIManagement.Queries;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Api.Controllers.V1;

/// <summary>
/// Provides AI audit log ingestion and search.
/// </summary>
[ApiController]
[Route("api/v1/ai/audit-logs")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize(Roles = "AI Administrator,System Admin")]
[Tags("AI Audit Logs")]
public class AuditLogsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AuditLogsController> _logger;

    public AuditLogsController(IMediator mediator, ILogger<AuditLogsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Writes an audit log entry.
    /// </summary>
    /// <param name="command">Audit log details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created audit log entry</returns>
    /// <response code="200">Audit log written successfully</response>
    /// <response code="400">Validation error</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<AuditLogDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> WriteAuditLog(
        [FromBody] WriteAuditLogCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Writing audit log: {EntityType}, action={Action}", command.EntityType, command.Action);

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<AuditLogDto>.SuccessResult(result.Value!, "Audit log written successfully."));
    }

    /// <summary>
    /// Searches audit log entries with optional filters and pagination.
    /// </summary>
    /// <param name="entityType">Filter by entity type</param>
    /// <param name="entityId">Filter by entity id</param>
    /// <param name="action">Filter by action</param>
    /// <param name="actorUserId">Filter by actor user</param>
    /// <param name="severity">Filter by severity</param>
    /// <param name="from">Start of the date range</param>
    /// <param name="to">End of the date range</param>
    /// <param name="page">Page number (1-based, default 1)</param>
    /// <param name="pageSize">Items per page (default 20, max 100)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of audit log entries</returns>
    /// <response code="200">Audit log entries retrieved successfully</response>
    /// <response code="400">Validation error</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<AuditLogDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SearchAuditLogs(
        [FromQuery] string? entityType = null,
        [FromQuery] Guid? entityId = null,
        [FromQuery] AIAuditAction? action = null,
        [FromQuery] Guid? actorUserId = null,
        [FromQuery] AIAuditSeverity? severity = null,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Audit log search: Page={Page}, PageSize={PageSize}", page, pageSize);

        var result = await _mediator.Send(new SearchAuditLogsQuery(
            entityType,
            entityId,
            action,
            actorUserId,
            severity,
            from,
            to,
            page,
            pageSize), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<IReadOnlyList<AuditLogDto>>.SuccessResult(
            result.Value!, "Audit log entries retrieved successfully."));
    }

    #region Helpers

    private IActionResult HandleFailure(string error)
    {
        if (error.Contains("not found", StringComparison.OrdinalIgnoreCase))
        {
            return NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Not Found",
                Detail = error,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4"
            });
        }

        if (error.Contains("already exists", StringComparison.OrdinalIgnoreCase))
        {
            return Conflict(new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Conflict",
                Detail = error,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8"
            });
        }

        if (error.Contains("insufficient permissions", StringComparison.OrdinalIgnoreCase) ||
            error.Contains("unauthorized", StringComparison.OrdinalIgnoreCase))
        {
            return StatusCode(StatusCodes.Status403Forbidden, new ProblemDetails
            {
                Status = StatusCodes.Status403Forbidden,
                Title = "Forbidden",
                Detail = error,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3"
            });
        }

        return BadRequest(new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Bad Request",
            Detail = error,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
        });
    }

    #endregion
}
