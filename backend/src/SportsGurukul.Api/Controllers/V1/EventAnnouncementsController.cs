using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.EventManagement.Commands.ArchiveAnnouncement;
using SportsGurukul.Application.Features.EventManagement.Commands.PublishAnnouncement;
using SportsGurukul.Application.Features.EventManagement.Commands.UpdateAnnouncement;
using SportsGurukul.Application.Features.EventManagement.DTOs;
using SportsGurukul.Application.Features.EventManagement.Queries.GetAnnouncementsByEvent;

namespace SportsGurukul.Api.Controllers.V1;

/// <summary>
/// Manages event announcements — publishing, updating, archiving, and retrieval.
/// </summary>
[ApiController]
[Route("api/v1/events/{eventId}/announcements")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
[Tags("Event Announcements")]
public class EventAnnouncementsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<EventAnnouncementsController> _logger;

    public EventAnnouncementsController(IMediator mediator, ILogger<EventAnnouncementsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Publishes a new announcement for an event.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "System Admin,Academy Admin,Event Manager")]
    [ProducesResponseType(typeof(ApiResponse<AnnouncementDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> PublishAnnouncement(
        [FromRoute] Guid eventId,
        [FromBody] PublishAnnouncementCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Publishing announcement for event: {EventId}", eventId);

        command.EventId = eventId;

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Announcement published for event: {EventId}", eventId);

        return CreatedAtAction(
            nameof(GetAnnouncements),
            new { eventId },
            ApiResponse<AnnouncementDto>.SuccessResult(result.Value!, "Announcement published successfully."));
    }

    /// <summary>
    /// Updates an existing announcement.
    /// </summary>
    [HttpPut("{announcementId:guid}")]
    [Authorize(Roles = "System Admin,Academy Admin,Event Manager")]
    [ProducesResponseType(typeof(ApiResponse<AnnouncementDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateAnnouncement(
        [FromRoute] Guid eventId,
        [FromRoute] Guid announcementId,
        [FromBody] UpdateAnnouncementCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating announcement: {AnnouncementId} for event: {EventId}", announcementId, eventId);

        command.AnnouncementId = announcementId;

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Announcement updated: {AnnouncementId}", announcementId);

        return Ok(ApiResponse<AnnouncementDto>.SuccessResult(result.Value!, "Announcement updated successfully."));
    }

    /// <summary>
    /// Archives an announcement.
    /// </summary>
    [HttpDelete("{announcementId:guid}")]
    [Authorize(Roles = "System Admin,Academy Admin,Event Manager")]
    [ProducesResponseType(typeof(ApiResponse<AnnouncementDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ArchiveAnnouncement(
        [FromRoute] Guid eventId,
        [FromRoute] Guid announcementId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Archiving announcement: {AnnouncementId} for event: {EventId}", announcementId, eventId);

        var command = new ArchiveAnnouncementCommand { AnnouncementId = announcementId };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Announcement archived: {AnnouncementId}", announcementId);

        return Ok(ApiResponse<AnnouncementDto>.SuccessResult(result.Value!, "Announcement archived successfully."));
    }

    /// <summary>
    /// Retrieves all announcements for a specific event.
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<List<AnnouncementDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAnnouncements(
        [FromRoute] Guid eventId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving announcements for event: {EventId}", eventId);

        var query = new GetAnnouncementsByEventQuery { EventId = eventId };

        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<List<AnnouncementDto>>.SuccessResult(result.Value!, "Announcements retrieved successfully."));
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
