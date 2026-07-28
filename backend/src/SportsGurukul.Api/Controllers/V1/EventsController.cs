using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.EventManagement.Commands.ArchiveEvent;
using SportsGurukul.Application.Features.EventManagement.Commands.CancelEvent;
using SportsGurukul.Application.Features.EventManagement.Commands.CompleteEvent;
using SportsGurukul.Application.Features.EventManagement.Commands.CreateEvent;
using SportsGurukul.Application.Features.EventManagement.Commands.PublishEvent;
using SportsGurukul.Application.Features.EventManagement.Commands.UpdateEvent;
using SportsGurukul.Application.Features.EventManagement.DTOs;
using SportsGurukul.Application.Features.EventManagement.Queries.GetEventById;
using SportsGurukul.Application.Features.EventManagement.Queries.SearchEvents;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Api.Controllers.V1;

/// <summary>
/// Manages event lifecycle — creation, publishing, registration windows, archival, and search.
/// </summary>
[ApiController]
[Route("api/v1/events")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
[Tags("Events")]
public class EventsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<EventsController> _logger;

    public EventsController(IMediator mediator, ILogger<EventsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new event.
    /// </summary>
    /// <param name="command">The request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created resource.</returns>
    [HttpPost]
    [Authorize(Roles = "Admin,Academy Admin,Event Manager")]
    [ProducesResponseType(typeof(ApiResponse<EventDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateEvent(
        [FromBody] CreateEventCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating event: {EventName}", command.EventName);

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Event created: {EventId}", result.Value!.Id);

        return CreatedAtAction(
            nameof(GetEventById),
            new { eventId = result.Value.Id, version = "1.0" },
            ApiResponse<EventDto>.SuccessResult(result.Value, "Event created successfully."));
    }

    /// <summary>
    /// Searches events with filtering and pagination.
    /// </summary>
    /// <param name="academyId">Filter by academy.</param>
    /// <param name="sportId">Filter by sport.</param>
    /// <param name="status">Filter by event status.</param>
    /// <param name="eventType">Filter by event type.</param>
    /// <param name="searchTerm">Search term for name/description.</param>
    /// <param name="page">Page number.</param>
    /// <param name="pageSize">Page size.</param>
    /// <param name="sortBy">Sort field.</param>
    /// <param name="sortDescending">Sort descending.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paged list of events.</returns>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<EventSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SearchEvents(
        [FromQuery] Guid? academyId,
        [FromQuery] Guid? sportId,
        [FromQuery] string? status,
        [FromQuery] string? eventType,
        [FromQuery] string? searchTerm,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Searching events with filters");

        EventStatus? eventStatus = null;
        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<EventStatus>(status, true, out var parsedStatus))
            eventStatus = parsedStatus;

        EventType? eventTypeEnum = null;
        if (!string.IsNullOrWhiteSpace(eventType) && Enum.TryParse<EventType>(eventType, true, out var parsedEventType))
            eventTypeEnum = parsedEventType;

        var query = new SearchEventsQuery
        {
            AcademyId = academyId,
            SportId = sportId,
            Status = eventStatus,
            EventType = eventTypeEnum,
            SearchTerm = searchTerm,
            Page = page,
            PageSize = pageSize,
            SortBy = sortBy,
            SortDescending = sortDescending
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<PagedResult<EventSummaryDto>>.SuccessResult(result.Value!, "Events retrieved successfully."));
    }

    /// <summary>
    /// Searches events (explicit /search route alias).
    /// </summary>
    /// <param name="academyId">Filter by academy.</param>
    /// <param name="sportId">Filter by sport.</param>
    /// <param name="status">Filter by event status.</param>
    /// <param name="eventType">Filter by event type.</param>
    /// <param name="searchTerm">Search term for name/description.</param>
    /// <param name="page">Page number.</param>
    /// <param name="pageSize">Page size.</param>
    /// <param name="sortBy">Sort field.</param>
    /// <param name="sortDescending">Sort descending.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paged list of events.</returns>
    [HttpGet("search")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<EventSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SearchEventsAlias(
        [FromQuery] Guid? academyId,
        [FromQuery] Guid? sportId,
        [FromQuery] string? status,
        [FromQuery] string? eventType,
        [FromQuery] string? searchTerm,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Searching events with filters (alias)");

        EventStatus? eventStatus = null;
        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<EventStatus>(status, true, out var parsedStatus))
            eventStatus = parsedStatus;

        EventType? eventTypeEnum = null;
        if (!string.IsNullOrWhiteSpace(eventType) && Enum.TryParse<EventType>(eventType, true, out var parsedEventType))
            eventTypeEnum = parsedEventType;

        var query = new SearchEventsQuery
        {
            AcademyId = academyId,
            SportId = sportId,
            Status = eventStatus,
            EventType = eventTypeEnum,
            SearchTerm = searchTerm,
            Page = page,
            PageSize = pageSize,
            SortBy = sortBy,
            SortDescending = sortDescending
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<PagedResult<EventSummaryDto>>.SuccessResult(result.Value!, "Events retrieved successfully."));
    }

    /// <summary>
    /// Gets upcoming events filtered by published status.
    /// </summary>
    /// <param name="academyId">Filter by academy.</param>
    /// <param name="limit">Maximum number of results.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of upcoming events.</returns>
    [HttpGet("upcoming")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<EventSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetUpcomingEvents(
        [FromQuery] Guid? academyId,
        [FromQuery] int limit = 10,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching upcoming events");

        var query = new SearchEventsQuery
        {
            AcademyId = academyId,
            Status = EventStatus.Published,
            Page = 1,
            PageSize = limit,
            SortBy = "StartDate",
            SortDescending = false
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<PagedResult<EventSummaryDto>>.SuccessResult(result.Value!, "Upcoming events retrieved successfully."));
    }

    /// <summary>
    /// Gets a specific event by its unique identifier.
    /// </summary>
    /// <param name="eventId">The event identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The event.</returns>
    [HttpGet("{eventId:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<EventDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEventById(
        Guid eventId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching event: {EventId}", eventId);

        var result = await _mediator.Send(new GetEventByIdQuery { EventId = eventId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<EventDto>.SuccessResult(result.Value!, "Event retrieved successfully."));
    }

    /// <summary>
    /// Updates an event. Only editable in Draft status.
    /// </summary>
    /// <param name="eventId">The event identifier.</param>
    /// <param name="command">The update request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated event.</returns>
    [HttpPut("{eventId:guid}")]
    [Authorize(Roles = "Admin,Academy Admin,Event Manager")]
    [ProducesResponseType(typeof(ApiResponse<EventDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateEvent(
        Guid eventId,
        [FromBody] UpdateEventCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating event: {EventId}", eventId);

        command.EventId = eventId;

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Event updated: {EventId}", eventId);

        return Ok(ApiResponse<EventDto>.SuccessResult(result.Value!, "Event updated successfully."));
    }

    /// <summary>
    /// Soft-deletes an event via archival.
    /// </summary>
    /// <param name="eventId">The event identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Success message.</returns>
    [HttpDelete("{eventId:guid}")]
    [Authorize(Roles = "Admin,Academy Admin")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ArchiveEvent(
        Guid eventId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Archiving event: {EventId}", eventId);

        var result = await _mediator.Send(new ArchiveEventCommand { EventId = eventId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Event archived: {EventId}", eventId);

        return Ok(ApiResponse<object>.SuccessResult(new { EventId = eventId }, "Event archived successfully."));
    }

    /// <summary>
    /// Publishes an event, transitioning it from Draft.
    /// </summary>
    /// <param name="eventId">The event identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The published event.</returns>
    [HttpPost("{eventId:guid}/publish")]
    [Authorize(Roles = "Admin,Academy Admin,Event Manager")]
    [ProducesResponseType(typeof(ApiResponse<EventDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PublishEvent(
        Guid eventId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Publishing event: {EventId}", eventId);

        var result = await _mediator.Send(new PublishEventCommand { EventId = eventId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Event published: {EventId}", eventId);

        return Ok(ApiResponse<EventDto>.SuccessResult(result.Value!, "Event published successfully."));
    }

    /// <summary>
    /// Cancels an event.
    /// </summary>
    /// <param name="eventId">The event identifier.</param>
    /// <param name="request">Optional cancellation reason.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The cancelled event.</returns>
    [HttpPost("{eventId:guid}/cancel")]
    [Authorize(Roles = "Admin,Academy Admin,Event Manager")]
    [ProducesResponseType(typeof(ApiResponse<EventDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelEvent(
        Guid eventId,
        [FromBody] CancelEventRequest? request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Cancelling event: {EventId}", eventId);

        var command = new CancelEventCommand
        {
            EventId = eventId,
            Reason = request?.Reason
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Event cancelled: {EventId}", eventId);

        return Ok(ApiResponse<EventDto>.SuccessResult(result.Value!, "Event cancelled successfully."));
    }

    /// <summary>
    /// Marks an event as completed.
    /// </summary>
    /// <param name="eventId">The event identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The completed event.</returns>
    [HttpPost("{eventId:guid}/complete")]
    [Authorize(Roles = "Admin,Academy Admin,Event Manager")]
    [ProducesResponseType(typeof(ApiResponse<EventDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CompleteEvent(
        Guid eventId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Completing event: {EventId}", eventId);

        var result = await _mediator.Send(new CompleteEventCommand { EventId = eventId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Event completed: {EventId}", eventId);

        return Ok(ApiResponse<EventDto>.SuccessResult(result.Value!, "Event completed successfully."));
    }

    /// <summary>
    /// Archives a completed event.
    /// </summary>
    /// <param name="eventId">The event identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The archived event.</returns>
    [HttpPost("{eventId:guid}/archive")]
    [Authorize(Roles = "Admin,Academy Admin")]
    [ProducesResponseType(typeof(ApiResponse<EventDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ArchiveEventAction(
        Guid eventId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Archiving event: {EventId}", eventId);

        var result = await _mediator.Send(new ArchiveEventCommand { EventId = eventId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Event archived: {EventId}", eventId);

        return Ok(ApiResponse<EventDto>.SuccessResult(result.Value!, "Event archived successfully."));
    }

    #region Helpers

    private IActionResult HandleFailure(string error)
    {
        if (error.Contains("not found", StringComparison.OrdinalIgnoreCase))
            return NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Not Found",
                Detail = error,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4"
            });

        if (error.Contains("already exists", StringComparison.OrdinalIgnoreCase) ||
            error.Contains("already", StringComparison.OrdinalIgnoreCase) ||
            error.Contains("conflict", StringComparison.OrdinalIgnoreCase))
            return Conflict(new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Conflict",
                Detail = error,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8"
            });

        if (error.Contains("cannot", StringComparison.OrdinalIgnoreCase) ||
            error.Contains("invalid", StringComparison.OrdinalIgnoreCase) ||
            error.Contains("must", StringComparison.OrdinalIgnoreCase) ||
            error.Contains("no eligible", StringComparison.OrdinalIgnoreCase) ||
            error.Contains("not eligible", StringComparison.OrdinalIgnoreCase) ||
            error.Contains("only", StringComparison.OrdinalIgnoreCase) ||
            error.Contains("duplicate", StringComparison.OrdinalIgnoreCase) ||
            error.Contains("capacity", StringComparison.OrdinalIgnoreCase) ||
            error.Contains("registration", StringComparison.OrdinalIgnoreCase))
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Bad Request",
                Detail = error,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
            });

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

public record CancelEventRequest(string? Reason);
