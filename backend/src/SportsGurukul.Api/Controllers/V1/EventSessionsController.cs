using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.EventManagement.Commands.AssignCoach;
using SportsGurukul.Application.Features.EventManagement.Commands.AssignSpeaker;
using SportsGurukul.Application.Features.EventManagement.Commands.AssignVenue;
using SportsGurukul.Application.Features.EventManagement.Commands.CancelSession;
using SportsGurukul.Application.Features.EventManagement.Commands.CreateSession;
using SportsGurukul.Application.Features.EventManagement.Commands.RescheduleSession;
using SportsGurukul.Application.Features.EventManagement.Commands.UpdateSession;
using SportsGurukul.Application.Features.EventManagement.DTOs;
using SportsGurukul.Application.Features.EventManagement.Queries.GetSessionsByEvent;

namespace SportsGurukul.Api.Controllers.V1;

/// <summary>
/// Manages event sessions — creation, updates, assignments, rescheduling, and cancellation.
/// </summary>
[ApiController]
[Route("api/v1/events/{eventId:guid}/sessions")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
[Tags("Event Sessions")]
public class EventSessionsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<EventSessionsController> _logger;

    public EventSessionsController(IMediator mediator, ILogger<EventSessionsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new session for an event.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "System Admin,Academy Admin,Event Manager,Coach")]
    [ProducesResponseType(typeof(ApiResponse<EventSessionDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateSession(
        [FromRoute] Guid eventId,
        [FromBody] CreateSessionCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating session for event {EventId}: {Title}", eventId, command.Title);

        command.EventId = eventId;

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Session created: {SessionId}", result.Value!.Id);

        return CreatedAtAction(
            nameof(GetSessions),
            new { eventId, version = "1.0" },
            ApiResponse<EventSessionDto>.SuccessResult(result.Value!, "Session created successfully."));
    }

    /// <summary>
    /// Gets all sessions for a specific event.
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<List<EventSessionDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSessions(
        [FromRoute] Guid eventId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching sessions for event {EventId}", eventId);

        var result = await _mediator.Send(new GetSessionsByEventQuery { EventId = eventId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<List<EventSessionDto>>.SuccessResult(result.Value!, "Sessions retrieved successfully."));
    }

    /// <summary>
    /// Updates an existing session.
    /// </summary>
    [HttpPut("{sessionId:guid}")]
    [Authorize(Roles = "System Admin,Academy Admin,Event Manager,Coach")]
    [ProducesResponseType(typeof(ApiResponse<EventSessionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateSession(
        [FromRoute] Guid eventId,
        [FromRoute] Guid sessionId,
        [FromBody] UpdateSessionCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating session {SessionId} for event {EventId}", sessionId, eventId);

        command.SessionId = sessionId;

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Session updated: {SessionId}", sessionId);

        return Ok(ApiResponse<EventSessionDto>.SuccessResult(result.Value!, "Session updated successfully."));
    }

    /// <summary>
    /// Cancels an existing session.
    /// </summary>
    [HttpDelete("{sessionId:guid}")]
    [Authorize(Roles = "System Admin,Academy Admin,Event Manager,Coach")]
    [ProducesResponseType(typeof(ApiResponse<EventSessionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelSession(
        [FromRoute] Guid eventId,
        [FromRoute] Guid sessionId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Cancelling session {SessionId} for event {EventId}", sessionId, eventId);

        var result = await _mediator.Send(new CancelSessionCommand { SessionId = sessionId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Session cancelled: {SessionId}", sessionId);

        return Ok(ApiResponse<EventSessionDto>.SuccessResult(result.Value!, "Session cancelled successfully."));
    }

    /// <summary>
    /// Assigns a coach to a session.
    /// </summary>
    [HttpPost("{sessionId:guid}/assign-coach")]
    [Authorize(Roles = "System Admin,Academy Admin,Event Manager")]
    [ProducesResponseType(typeof(ApiResponse<EventSessionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignCoach(
        [FromRoute] Guid eventId,
        [FromRoute] Guid sessionId,
        [FromBody] AssignCoachCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Assigning coach to session {SessionId} for event {EventId}", sessionId, eventId);

        command.SessionId = sessionId;

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Coach assigned to session {SessionId}", sessionId);

        return Ok(ApiResponse<EventSessionDto>.SuccessResult(result.Value!, "Coach assigned successfully."));
    }

    /// <summary>
    /// Assigns a speaker to a session.
    /// </summary>
    [HttpPost("{sessionId:guid}/assign-speaker")]
    [Authorize(Roles = "System Admin,Academy Admin,Event Manager")]
    [ProducesResponseType(typeof(ApiResponse<EventSessionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignSpeaker(
        [FromRoute] Guid eventId,
        [FromRoute] Guid sessionId,
        [FromBody] AssignSpeakerCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Assigning speaker to session {SessionId} for event {EventId}", sessionId, eventId);

        command.SessionId = sessionId;

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Speaker assigned to session {SessionId}", sessionId);

        return Ok(ApiResponse<EventSessionDto>.SuccessResult(result.Value!, "Speaker assigned successfully."));
    }

    /// <summary>
    /// Assigns a venue to a session.
    /// </summary>
    [HttpPost("{sessionId:guid}/assign-venue")]
    [Authorize(Roles = "System Admin,Academy Admin,Event Manager")]
    [ProducesResponseType(typeof(ApiResponse<EventSessionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignVenue(
        [FromRoute] Guid eventId,
        [FromRoute] Guid sessionId,
        [FromBody] AssignVenueCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Assigning venue to session {SessionId} for event {EventId}", sessionId, eventId);

        command.SessionId = sessionId;

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Venue assigned to session {SessionId}", sessionId);

        return Ok(ApiResponse<EventSessionDto>.SuccessResult(result.Value!, "Venue assigned successfully."));
    }

    /// <summary>
    /// Reschedules a session to a new date and time.
    /// </summary>
    [HttpPost("{sessionId:guid}/reschedule")]
    [Authorize(Roles = "System Admin,Academy Admin,Event Manager")]
    [ProducesResponseType(typeof(ApiResponse<EventSessionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RescheduleSession(
        [FromRoute] Guid eventId,
        [FromRoute] Guid sessionId,
        [FromBody] RescheduleSessionCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Rescheduling session {SessionId} for event {EventId}", sessionId, eventId);

        command.SessionId = sessionId;

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Session rescheduled: {SessionId}", sessionId);

        return Ok(ApiResponse<EventSessionDto>.SuccessResult(result.Value!, "Session rescheduled successfully."));
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
