using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.EventManagement.DTOs;
using SportsGurukul.Application.Features.EventManagement.Queries.GetEventStatistics;
using SportsGurukul.Application.Features.EventManagement.Queries.SearchEvents;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Api.Controllers.V1;

[ApiController]
[Route("api/v1/event-statistics")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
[Tags("Event Statistics")]
public class EventStatisticsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<EventStatisticsController> _logger;

    public EventStatisticsController(IMediator mediator, ILogger<EventStatisticsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Gets statistics for a specific event.
    /// </summary>
    /// <param name="eventId">The event identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The event statistics.</returns>
    [HttpGet("{eventId:guid}/statistics")]
    [Authorize(Roles = "Admin,AcademyAdmin,EventManager")]
    [ProducesResponseType(typeof(ApiResponse<StatisticsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetEventStatistics(
        [FromRoute] Guid eventId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting statistics for event {EventId}", eventId);
        var result = await _mediator.Send(new GetEventStatisticsQuery { EventId = eventId }, cancellationToken);
        if (!result.IsSuccess)
            return HandleFailure(result.Error!);
        return Ok(ApiResponse<StatisticsDto>.SuccessResult(result.Value!, "Event statistics retrieved successfully"));
    }

    /// <summary>
    /// Gets a summary of statistics across all events.
    /// </summary>
    /// <param name="academyId">Optional academy filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paged list of event summaries with statistics.</returns>
    [HttpGet("statistics")]
    [Authorize(Roles = "Admin,AcademyAdmin,EventManager")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<EventSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetEventsStatisticsOverview(
        [FromQuery] Guid? academyId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting events statistics overview for academy {AcademyId}", academyId);
        var result = await _mediator.Send(new SearchEventsQuery
        {
            AcademyId = academyId,
            Page = 1,
            PageSize = 100
        }, cancellationToken);
        if (!result.IsSuccess)
            return HandleFailure(result.Error!);
        return Ok(ApiResponse<PagedResult<EventSummaryDto>>.SuccessResult(result.Value!, "Events statistics overview retrieved successfully"));
    }

    /// <summary>
    /// Gets upcoming events.
    /// </summary>
    /// <param name="academyId">Optional academy filter.</param>
    /// <param name="limit">Maximum number of events to return.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paged list of upcoming events.</returns>
    [HttpGet("upcoming")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<EventSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetUpcomingEvents(
        [FromQuery] Guid? academyId,
        [FromQuery] int limit = 10,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting upcoming events for academy {AcademyId} with limit {Limit}", academyId, limit);
        var result = await _mediator.Send(new SearchEventsQuery
        {
            AcademyId = academyId,
            Page = 1,
            PageSize = limit
        }, cancellationToken);
        if (!result.IsSuccess)
            return HandleFailure(result.Error!);
        return Ok(ApiResponse<PagedResult<EventSummaryDto>>.SuccessResult(result.Value!, "Upcoming events retrieved successfully"));
    }

    /// <summary>
    /// Gets events for calendar view.
    /// </summary>
    /// <param name="academyId">Optional academy filter.</param>
    /// <param name="sportId">Optional sport filter.</param>
    /// <param name="status">Optional status filter.</param>
    /// <param name="eventType">Optional event type filter.</param>
    /// <param name="searchTerm">Optional search term.</param>
    /// <param name="page">Page number.</param>
    /// <param name="pageSize">Page size.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paged list of events for calendar view.</returns>
    [HttpGet("calendar")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<EventSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetEventCalendar(
        [FromQuery] Guid? academyId,
        [FromQuery] Guid? sportId,
        [FromQuery] string? status,
        [FromQuery] string? eventType,
        [FromQuery] string? searchTerm,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting event calendar with filters");
        Enum.TryParse<EventStatus>(status, true, out var parsedStatus);
        Enum.TryParse<EventType>(eventType, true, out var parsedEventType);
        var result = await _mediator.Send(new SearchEventsQuery
        {
            AcademyId = academyId,
            SportId = sportId,
            Status = parsedStatus,
            EventType = parsedEventType,
            SearchTerm = searchTerm,
            Page = page,
            PageSize = pageSize
        }, cancellationToken);
        if (!result.IsSuccess)
            return HandleFailure(result.Error!);
        return Ok(ApiResponse<PagedResult<EventSummaryDto>>.SuccessResult(result.Value!, "Event calendar retrieved successfully"));
    }

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
        return BadRequest(new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Bad Request",
            Detail = error,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
        });
    }
}
