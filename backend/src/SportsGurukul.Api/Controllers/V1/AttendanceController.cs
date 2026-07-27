using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Attendance.MarkAttendance;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Attendance.CheckInAthlete;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Attendance.CheckOutAthlete;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Attendance.UpdateAttendance;
using SportsGurukul.Application.Features.TrainingProgramManagement.Queries.GetSessionAttendanceQuery;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Api.Controllers.V1;

/// <summary>
/// Manages session attendance tracking including check-in, check-out, and attendance recording.
/// </summary>
[ApiController]
[Route("api/v1/attendance")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
[Tags("Attendance")]
public class AttendanceController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AttendanceController> _logger;

    public AttendanceController(IMediator mediator, ILogger<AttendanceController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    #region Request Types

    public record MarkAttendanceRequest
    {
        public Guid AthleteId { get; init; }
        public AttendanceStatus Status { get; init; }
        public string? Remarks { get; init; }
    }

    public record CheckInRequest
    {
        public Guid AthleteId { get; init; }
    }

    public record CheckOutRequest
    {
        public Guid AthleteId { get; init; }
    }

    public record UpdateAttendanceRequest
    {
        public AttendanceStatus Status { get; init; }
        public string? Remarks { get; init; }
    }

    #endregion

    #region Attendance

    /// <summary>
    /// Records attendance for an athlete in a training session.
    /// </summary>
    /// <param name="sessionId">The training session's unique identifier</param>
    /// <param name="request">Attendance details including athlete, status, and remarks</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created attendance record</returns>
    /// <response code="201">Attendance recorded successfully</response>
    /// <response code="400">Validation error</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="409">Attendance already recorded for this athlete in this session</response>
    [HttpPost("~/api/v1/training-sessions/{sessionId:guid}/attendance")]
    [Authorize(Roles = "Coach,System Admin,Academy Admin")]
    [ProducesResponseType(typeof(ApiResponse<AttendanceDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> MarkAttendance(
        Guid sessionId,
        [FromBody] MarkAttendanceRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Recording attendance for athlete {AthleteId} in session {SessionId}", request.AthleteId, sessionId);

        var command = new MarkAttendanceCommand
        {
            SessionId = sessionId,
            AthleteId = request.AthleteId,
            Status = request.Status,
            Remarks = request.Remarks
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Attendance recorded: {AttendanceId}", result.Value!.Id);

        return CreatedAtAction(
            nameof(GetSessionAttendance),
            new { sessionId = result.Value.SessionId, version = "1.0" },
            ApiResponse<AttendanceDto>.SuccessResult(result.Value, "Attendance recorded successfully."));
    }

    /// <summary>
    /// Gets all attendance records for a specific training session.
    /// </summary>
    /// <param name="sessionId">The training session's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of attendance records for the session</returns>
    /// <response code="200">Attendance records retrieved successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Session not found</response>
    [HttpGet("~/api/v1/training-sessions/{sessionId:guid}/attendance")]
    [Authorize(Roles = "Coach,System Admin,Academy Admin")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<AttendanceDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSessionAttendance(
        Guid sessionId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching attendance for session {SessionId}", sessionId);

        var result = await _mediator.Send(new GetSessionAttendanceQuery { SessionId = sessionId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<IReadOnlyList<AttendanceDto>>.SuccessResult(result.Value!, "Attendance records retrieved successfully."));
    }

    /// <summary>
    /// Records check-in for an athlete in a training session.
    /// </summary>
    /// <param name="sessionId">The training session's unique identifier</param>
    /// <param name="request">Check-in details including athlete ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated attendance record with check-in time</returns>
    /// <response code="201">Check-in recorded successfully</response>
    /// <response code="400">Validation error or athlete not marked for attendance</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    [HttpPost("~/api/v1/training-sessions/{sessionId:guid}/check-in")]
    [Authorize(Roles = "Coach,System Admin,Academy Admin")]
    [ProducesResponseType(typeof(ApiResponse<AttendanceDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CheckInAthlete(
        Guid sessionId,
        [FromBody] CheckInRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Checking in athlete {AthleteId} for session {SessionId}", request.AthleteId, sessionId);

        var command = new CheckInAthleteCommand
        {
            SessionId = sessionId,
            AthleteId = request.AthleteId
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Athlete checked in: {AttendanceId}", result.Value!.Id);

        return CreatedAtAction(
            nameof(GetSessionAttendance),
            new { sessionId = result.Value.SessionId, version = "1.0" },
            ApiResponse<AttendanceDto>.SuccessResult(result.Value, "Check-in recorded successfully."));
    }

    /// <summary>
    /// Records check-out for an athlete in a training session.
    /// </summary>
    /// <param name="sessionId">The training session's unique identifier</param>
    /// <param name="request">Check-out details including athlete ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated attendance record with check-out time</returns>
    /// <response code="201">Check-out recorded successfully</response>
    /// <response code="400">Validation error or athlete not checked in</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    [HttpPost("~/api/v1/training-sessions/{sessionId:guid}/check-out")]
    [Authorize(Roles = "Coach,System Admin,Academy Admin")]
    [ProducesResponseType(typeof(ApiResponse<AttendanceDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CheckOutAthlete(
        Guid sessionId,
        [FromBody] CheckOutRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Checking out athlete {AthleteId} for session {SessionId}", request.AthleteId, sessionId);

        var command = new CheckOutAthleteCommand
        {
            SessionId = sessionId,
            AthleteId = request.AthleteId
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Athlete checked out: {AttendanceId}", result.Value!.Id);

        return CreatedAtAction(
            nameof(GetSessionAttendance),
            new { sessionId = result.Value.SessionId, version = "1.0" },
            ApiResponse<AttendanceDto>.SuccessResult(result.Value, "Check-out recorded successfully."));
    }

    /// <summary>
    /// Updates an existing attendance record.
    /// </summary>
    /// <param name="attendanceId">The attendance record's unique identifier</param>
    /// <param name="request">Fields to update including status and remarks</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated attendance record</returns>
    /// <response code="200">Attendance updated successfully</response>
    /// <response code="400">Validation error</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Attendance record not found</response>
    [HttpPut("{attendanceId:guid}")]
    [Authorize(Roles = "Coach,System Admin,Academy Admin")]
    [ProducesResponseType(typeof(ApiResponse<AttendanceDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAttendance(
        Guid attendanceId,
        [FromBody] UpdateAttendanceRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating attendance record: {AttendanceId}", attendanceId);

        var command = new UpdateAttendanceCommand
        {
            AttendanceId = attendanceId,
            Status = request.Status,
            Remarks = request.Remarks
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Attendance updated: {AttendanceId}", attendanceId);

        return Ok(ApiResponse<AttendanceDto>.SuccessResult(result.Value!, "Attendance updated successfully."));
    }

    #endregion

    #region Helpers

    private IActionResult HandleFailure(string error)
    {
        if (error.Contains("not found", StringComparison.OrdinalIgnoreCase))
            return NotFound(new ProblemDetails { Status = StatusCodes.Status404NotFound, Title = "Not Found", Detail = error, Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4" });
        if (error.Contains("already exists", StringComparison.OrdinalIgnoreCase) || error.Contains("already", StringComparison.OrdinalIgnoreCase))
            return Conflict(new ProblemDetails { Status = StatusCodes.Status409Conflict, Title = "Conflict", Detail = error, Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8" });
        return BadRequest(new ProblemDetails { Status = StatusCodes.Status400BadRequest, Title = "Bad Request", Detail = error, Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1" });
    }

    #endregion
}
