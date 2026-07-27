using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Session.CreateTrainingSession;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Session.UpdateTrainingSession;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Session.RescheduleTrainingSession;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Session.CompleteTrainingSession;
using SportsGurukul.Application.Features.TrainingProgramManagement.Queries.GetSessionByIdQuery;
using SportsGurukul.Application.Features.TrainingProgramManagement.Queries.GetTrainingSessionsQuery;
using SportsGurukul.Application.Features.TrainingProgramManagement.Queries.GetUpcomingSessionsQuery;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Api.Controllers.V1;

/// <summary>
/// Manages training session scheduling, rescheduling, completion, and facility assignment.
/// </summary>
[ApiController]
[Route("api/v1/training-sessions")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
[Tags("Training Sessions")]
public class TrainingSessionsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TrainingSessionsController> _logger;

    public TrainingSessionsController(IMediator mediator, ILogger<TrainingSessionsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Request body for creating a training session.
    /// </summary>
    public record CreateSessionRequest(
        string SessionTitle,
        SessionType SessionType,
        DateTime SessionDate,
        TimeSpan StartTime,
        TimeSpan EndTime,
        Guid? FacilityId,
        Guid CoachId);

    /// <summary>
    /// Request body for rescheduling a training session.
    /// </summary>
    public record RescheduleSessionRequest(
        DateTime SessionDate,
        TimeSpan StartTime,
        TimeSpan EndTime);

    /// <summary>
    /// Creates a new training session under a training batch.
    /// </summary>
    /// <param name="batchId">The parent training batch's unique identifier</param>
    /// <param name="request">Session details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created training session</returns>
    /// <response code="201">Training session created successfully</response>
    /// <response code="400">Validation error</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Training batch not found</response>
    /// <response code="409">Session code already exists</response>
    [HttpPost("~/api/v1/training-batches/{batchId:guid}/sessions")]
    [Authorize(Roles = "Coach,System Admin,Academy Admin")]
    [ProducesResponseType(typeof(ApiResponse<TrainingSessionDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateSession(
        Guid batchId,
        [FromBody] CreateSessionRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating training session for batch: {BatchId}", batchId);

        var command = new CreateTrainingSessionCommand(
            batchId,
            request.SessionTitle,
            request.SessionType,
            request.SessionDate,
            request.StartTime,
            request.EndTime,
            request.FacilityId,
            request.CoachId);

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Training session created: {Id}", result.Value!.Id);

        return CreatedAtAction(
            nameof(GetSession),
            new { sessionId = result.Value.Id, version = "1.0" },
            ApiResponse<TrainingSessionDto>.SuccessResult(result.Value, "Training session created successfully."));
    }

    /// <summary>
    /// Gets all training sessions for a specific training batch.
    /// </summary>
    /// <param name="batchId">The parent training batch's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of training sessions</returns>
    /// <response code="200">Training sessions retrieved successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="404">Training batch not found</response>
    [HttpGet("~/api/v1/training-batches/{batchId:guid}/sessions")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<TrainingSessionDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSessionsByBatch(
        Guid batchId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetTrainingSessionsQuery { BatchId = batchId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<IReadOnlyList<TrainingSessionDto>>.SuccessResult(result.Value!, "Training sessions retrieved successfully."));
    }

    /// <summary>
    /// Gets upcoming training sessions, optionally filtered by coach or batch.
    /// </summary>
    /// <param name="coachId">Optional coach ID filter</param>
    /// <param name="batchId">Optional batch ID filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of upcoming training sessions</returns>
    /// <response code="200">Upcoming sessions retrieved successfully</response>
    /// <response code="401">Not authenticated</response>
    [HttpGet("upcoming")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<TrainingSessionDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUpcomingSessions(
        [FromQuery] Guid? coachId,
        [FromQuery] Guid? batchId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetUpcomingSessionsQuery { CoachId = coachId, BatchId = batchId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<IReadOnlyList<TrainingSessionDto>>.SuccessResult(result.Value!, "Upcoming sessions retrieved successfully."));
    }

    /// <summary>
    /// Gets a training session by its unique identifier.
    /// </summary>
    /// <param name="sessionId">The training session's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Training session details</returns>
    /// <response code="200">Training session retrieved successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="404">Training session not found</response>
    [HttpGet("{sessionId:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<TrainingSessionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSession(
        Guid sessionId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetSessionByIdQuery { Id = sessionId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<TrainingSessionDto>.SuccessResult(result.Value!, "Training session retrieved successfully."));
    }

    /// <summary>
    /// Updates a training session's title, type, date, and time.
    /// </summary>
    /// <param name="sessionId">The training session's unique identifier</param>
    /// <param name="request">Session fields to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated training session</returns>
    /// <response code="200">Training session updated successfully</response>
    /// <response code="400">Validation error</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Training session not found</response>
    [HttpPut("{sessionId:guid}")]
    [Authorize(Roles = "Coach,System Admin,Academy Admin")]
    [ProducesResponseType(typeof(ApiResponse<TrainingSessionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateSession(
        Guid sessionId,
        [FromBody] UpdateTrainingSessionCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating training session: {SessionId}", sessionId);

        var command = request with { Id = sessionId };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Training session updated: {SessionId}", sessionId);

        return Ok(ApiResponse<TrainingSessionDto>.SuccessResult(result.Value!, "Training session updated successfully."));
    }

    /// <summary>
    /// Reschedules a training session to a new date and time.
    /// </summary>
    /// <param name="sessionId">The training session's unique identifier</param>
    /// <param name="request">New schedule details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Rescheduled training session</returns>
    /// <response code="200">Training session rescheduled successfully</response>
    /// <response code="400">Validation error</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Training session not found</response>
    [HttpPost("{sessionId:guid}/reschedule")]
    [Authorize(Roles = "Coach,System Admin,Academy Admin")]
    [ProducesResponseType(typeof(ApiResponse<TrainingSessionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RescheduleSession(
        Guid sessionId,
        [FromBody] RescheduleSessionRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Rescheduling training session: {SessionId}", sessionId);

        var command = new RescheduleTrainingSessionCommand(sessionId, request.SessionDate, request.StartTime, request.EndTime);

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Training session rescheduled: {SessionId}", sessionId);

        return Ok(ApiResponse<TrainingSessionDto>.SuccessResult(result.Value!, "Training session rescheduled successfully."));
    }

    /// <summary>
    /// Marks a training session as completed.
    /// </summary>
    /// <param name="sessionId">The training session's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Completed training session</returns>
    /// <response code="200">Training session completed successfully</response>
    /// <response code="400">Training session is not in a completable state</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Training session not found</response>
    [HttpPost("{sessionId:guid}/complete")]
    [Authorize(Roles = "Coach,System Admin,Academy Admin")]
    [ProducesResponseType(typeof(ApiResponse<TrainingSessionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CompleteSession(
        Guid sessionId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Completing training session: {SessionId}", sessionId);

        var result = await _mediator.Send(new CompleteTrainingSessionCommand(sessionId), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Training session completed: {SessionId}", sessionId);

        return Ok(ApiResponse<TrainingSessionDto>.SuccessResult(result.Value!, "Training session completed successfully."));
    }

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
