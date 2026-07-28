using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.EventManagement.Commands.CheckInParticipant;
using SportsGurukul.Application.Features.EventManagement.Commands.CheckOutParticipant;
using SportsGurukul.Application.Features.EventManagement.Commands.SearchAttendance;
using SportsGurukul.Application.Features.EventManagement.DTOs;
using SportsGurukul.Application.Features.EventManagement.Queries.GetAttendanceByEvent;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Api.Controllers.V1;

/// <summary>
/// Manages event attendance — check-in, check-out, attendance tracking, and reporting.
/// </summary>
[ApiController]
[Route("api/v1/events/{eventId:guid}/attendance")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
[Tags("Event Attendance")]
public class EventAttendanceController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<EventAttendanceController> _logger;

    public EventAttendanceController(IMediator mediator, ILogger<EventAttendanceController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Checks in a participant for an event or session.
    /// </summary>
    [HttpPost("check-in")]
    [Authorize(Roles = "System Admin,Academy Admin,Event Manager,Coach")]
    [ProducesResponseType(typeof(ApiResponse<AttendanceDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CheckIn(
        [FromRoute] Guid eventId,
        [FromBody] CheckInParticipantCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Checking in participant {ParticipantId} for event {EventId}", command.ParticipantId, eventId);

        command.EventId = eventId;

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Participant checked in: {AttendanceId}", result.Value!.Id);

        return CreatedAtAction(
            nameof(GetAttendance),
            new { eventId, version = "1.0" },
            ApiResponse<AttendanceDto>.SuccessResult(result.Value!, "Check-in recorded successfully."));
    }

    /// <summary>
    /// Checks out a participant from an event or session.
    /// </summary>
    [HttpPost("check-out")]
    [Authorize(Roles = "System Admin,Academy Admin,Event Manager,Coach")]
    [ProducesResponseType(typeof(ApiResponse<AttendanceDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CheckOut(
        [FromRoute] Guid eventId,
        [FromBody] CheckOutParticipantCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Checking out participant for event {EventId}, attendance {AttendanceId}", eventId, command.AttendanceId);

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Participant checked out: {AttendanceId}", result.Value!.Id);

        return Ok(ApiResponse<AttendanceDto>.SuccessResult(result.Value!, "Check-out recorded successfully."));
    }

    /// <summary>
    /// Gets paginated attendance records for a specific event.
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "System Admin,Academy Admin,Event Manager,Coach")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<AttendanceDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAttendance(
        [FromRoute] Guid eventId,
        [FromQuery] Guid? sessionId,
        [FromQuery] EventAttendanceStatus? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching attendance for event {EventId} - Page: {Page}, PageSize: {PageSize}", eventId, page, pageSize);

        var query = new GetAttendanceByEventQuery
        {
            EventId = eventId,
            SessionId = sessionId,
            Status = status,
            Page = page,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<PagedResult<AttendanceDto>>.SuccessResult(result.Value!, "Attendance retrieved successfully."));
    }

    /// <summary>
    /// Gets a paginated attendance report for a specific event.
    /// </summary>
    [HttpGet("report")]
    [Authorize(Roles = "System Admin,Academy Admin,Event Manager")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<AttendanceDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAttendanceReport(
        [FromRoute] Guid eventId,
        [FromQuery] Guid? sessionId,
        [FromQuery] EventAttendanceStatus? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Generating attendance report for event {EventId} - Page: {Page}, PageSize: {PageSize}", eventId, page, pageSize);

        var query = new SearchAttendanceQuery
        {
            EventId = eventId,
            SessionId = sessionId,
            Status = status,
            Page = page,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<PagedResult<AttendanceDto>>.SuccessResult(result.Value!, "Attendance report retrieved successfully."));
    }

    #region Helpers

    private IActionResult HandleFailure(string error)
    {
        if (error.Contains("not found", StringComparison.OrdinalIgnoreCase))
            return NotFound(new ProblemDetails { Status = StatusCodes.Status404NotFound, Title = "Not Found", Detail = error, Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4" });
        if (error.Contains("already exists", StringComparison.OrdinalIgnoreCase) || error.Contains("conflict", StringComparison.OrdinalIgnoreCase))
            return Conflict(new ProblemDetails { Status = StatusCodes.Status409Conflict, Title = "Conflict", Detail = error, Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8" });
        return BadRequest(new ProblemDetails { Status = StatusCodes.Status400BadRequest, Title = "Bad Request", Detail = error, Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1" });
    }

    #endregion
}
