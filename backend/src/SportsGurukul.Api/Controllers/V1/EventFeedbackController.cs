using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.EventManagement.Commands.ApproveFeedback;
using SportsGurukul.Application.Features.EventManagement.Commands.RejectFeedback;
using SportsGurukul.Application.Features.EventManagement.Commands.SubmitFeedback;
using SportsGurukul.Application.Features.EventManagement.DTOs;
using SportsGurukul.Application.Features.EventManagement.Queries.GetFeedbackByEvent;

namespace SportsGurukul.Api.Controllers.V1;

/// <summary>
/// Manages event feedback — submission, approval, rejection, and retrieval.
/// </summary>
[ApiController]
[Route("api/v1/events/{eventId}/feedback")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
[Tags("Event Feedback")]
public class EventFeedbackController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<EventFeedbackController> _logger;

    public EventFeedbackController(IMediator mediator, ILogger<EventFeedbackController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Submits feedback for an event.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "System Admin,Academy Admin,Athlete")]
    [ProducesResponseType(typeof(ApiResponse<FeedbackDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> SubmitFeedback(
        [FromRoute] Guid eventId,
        [FromBody] SubmitFeedbackCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Submitting feedback for event: {EventId}", eventId);

        command.EventId = eventId;

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Feedback submitted for event: {EventId}", eventId);

        return CreatedAtAction(
            nameof(GetFeedback),
            new { eventId },
            ApiResponse<FeedbackDto>.SuccessResult(result.Value!, "Feedback submitted successfully."));
    }

    /// <summary>
    /// Retrieves all feedback for a specific event.
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "System Admin,Academy Admin,Event Manager")]
    [ProducesResponseType(typeof(ApiResponse<List<FeedbackDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetFeedback(
        [FromRoute] Guid eventId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving feedback for event: {EventId}", eventId);

        var query = new GetFeedbackByEventQuery { EventId = eventId };

        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<List<FeedbackDto>>.SuccessResult(result.Value!, "Feedback retrieved successfully."));
    }

    /// <summary>
    /// Approves a specific feedback submission.
    /// </summary>
    [HttpPost("{feedbackId:guid}/approve")]
    [Authorize(Roles = "System Admin,Academy Admin,Event Manager")]
    [ProducesResponseType(typeof(ApiResponse<FeedbackDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ApproveFeedback(
        [FromRoute] Guid eventId,
        [FromRoute] Guid feedbackId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Approving feedback: {FeedbackId} for event: {EventId}", feedbackId, eventId);

        var command = new ApproveFeedbackCommand { FeedbackId = feedbackId };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Feedback approved: {FeedbackId}", feedbackId);

        return Ok(ApiResponse<FeedbackDto>.SuccessResult(result.Value!, "Feedback approved successfully."));
    }

    /// <summary>
    /// Rejects a specific feedback submission.
    /// </summary>
    [HttpPost("{feedbackId:guid}/reject")]
    [Authorize(Roles = "System Admin,Academy Admin,Event Manager")]
    [ProducesResponseType(typeof(ApiResponse<FeedbackDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> RejectFeedback(
        [FromRoute] Guid eventId,
        [FromRoute] Guid feedbackId,
        [FromBody] RejectFeedbackRequest? request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Rejecting feedback: {FeedbackId} for event: {EventId}", feedbackId, eventId);

        var command = new RejectFeedbackCommand
        {
            FeedbackId = feedbackId,
            Reason = request?.Reason
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Feedback rejected: {FeedbackId}", feedbackId);

        return Ok(ApiResponse<FeedbackDto>.SuccessResult(result.Value!, "Feedback rejected successfully."));
    }

    private IActionResult HandleFailure(string error)
    {
        if (error.Contains("not found", StringComparison.OrdinalIgnoreCase))
            return NotFound(new ProblemDetails { Status = StatusCodes.Status404NotFound, Title = "Not Found", Detail = error, Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4" });
        if (error.Contains("already exists", StringComparison.OrdinalIgnoreCase) || error.Contains("conflict", StringComparison.OrdinalIgnoreCase))
            return Conflict(new ProblemDetails { Status = StatusCodes.Status409Conflict, Title = "Conflict", Detail = error, Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8" });
        return BadRequest(new ProblemDetails { Status = StatusCodes.Status400BadRequest, Title = "Bad Request", Detail = error, Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1" });
    }
}

public record RejectFeedbackRequest(string? Reason);
