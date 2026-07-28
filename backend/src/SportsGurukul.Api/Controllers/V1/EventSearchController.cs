using System.Net.Mime;
using System.Threading.RateLimiting;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Api.Common.Models.SwaggerExamples;
using SportsGurukul.Application.Features.EventSearchDiscovery.Commands.DeleteSavedSearch;
using SportsGurukul.Application.Features.EventSearchDiscovery.Commands.SaveSearch;
using SportsGurukul.Application.Features.EventSearchDiscovery.Commands.TrackRecentlyViewed;
using SportsGurukul.Application.Features.EventSearchDiscovery.DTOs;
using SportsGurukul.Application.Features.EventSearchDiscovery.Queries.Autocomplete;
using SportsGurukul.Application.Features.EventSearchDiscovery.Queries.CalendarEvents;
using SportsGurukul.Application.Features.EventSearchDiscovery.Queries.FeaturedEvents;
using SportsGurukul.Application.Features.EventSearchDiscovery.Queries.NearbyEvents;
using SportsGurukul.Application.Features.EventSearchDiscovery.Queries.RecommendedEvents;
using SportsGurukul.Application.Features.EventSearchDiscovery.Queries.SearchEvents;
using SportsGurukul.Application.Features.EventSearchDiscovery.Queries.TrendingEvents;
using SportsGurukul.Application.Features.EventSearchDiscovery.Queries.UpcomingEvents;
using Swashbuckle.AspNetCore.Filters;

namespace SportsGurukul.Api.Controllers.V1;

/// <summary>
/// Event search, discovery, and recommendation APIs.
/// Supports global search, advanced filtering, nearby events, trending, featured, calendar views, and autocomplete.
/// </summary>
[ApiController]
[Route("api/v1/event-discovery")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Event Search & Discovery")]
public class EventSearchController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<EventSearchController> _logger;

    public EventSearchController(IMediator mediator, ILogger<EventSearchController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Advanced event search with comprehensive filtering, sorting, and pagination.
    /// </summary>
    /// <remarks>
    /// Supports full-text search across event name, code, description, and tags.
    /// Filter by sport, academy, coach, speaker, venue, location, date range, price, event type, category, skill level, age group, availability, registration status, rating, and language.
    /// </remarks>
    /// <param name="request">Search parameters including filters, sort, and pagination</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of event cards with search metadata</returns>
    /// <response code="200">Events retrieved successfully</response>
    /// <response code="400">Validation error</response>
    [HttpGet("search")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<EventSearchPageResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(EventSearchResponseExample))]
    public async Task<IActionResult> SearchEvents(
        [FromQuery] SearchEventsQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Event search: Term='{Term}', Page={Page}, PageSize={PageSize}",
            request.SearchTerm, request.Page, request.PageSize);

        var result = await _mediator.Send(request, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Bad Request",
                Detail = result.Error,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
            });

        return Ok(ApiResponse<EventSearchPageResultDto>.SuccessResult(result.Value!, "Events retrieved successfully."));
    }

    /// <summary>
    /// Gets upcoming events, optionally filtered by city or academy.
    /// </summary>
    /// <param name="city">Optional city filter</param>
    /// <param name="academyId">Optional academy filter</param>
    /// <param name="limit">Maximum number of events to return (1-100, default 20)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of upcoming event cards</returns>
    /// <response code="200">Upcoming events retrieved successfully</response>
    [HttpGet("upcoming")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<EventCardDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetUpcomingEvents(
        [FromQuery] string? city = null,
        [FromQuery] Guid? academyId = null,
        [FromQuery] int limit = 20,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Upcoming events: City={City}, Limit={Limit}", city, limit);

        var result = await _mediator.Send(new UpcomingEventsQuery
        {
            City = city,
            AcademyId = academyId,
            Limit = limit
        }, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Bad Request",
                Detail = result.Error
            });

        return Ok(ApiResponse<IReadOnlyList<EventCardDto>>.SuccessResult(result.Value!, "Upcoming events retrieved successfully."));
    }

    /// <summary>
    /// Gets trending events based on popularity and engagement metrics.
    /// </summary>
    /// <param name="city">Optional city filter</param>
    /// <param name="limit">Maximum number of events (1-100, default 20)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of trending event cards</returns>
    /// <response code="200">Trending events retrieved successfully</response>
    [HttpGet("trending")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<TrendingEventDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetTrendingEvents(
        [FromQuery] string? city = null,
        [FromQuery] int limit = 20,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Trending events: City={City}, Limit={Limit}", city, limit);

        var result = await _mediator.Send(new TrendingEventsQuery
        {
            City = city,
            Limit = limit
        }, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Bad Request",
                Detail = result.Error
            });

        return Ok(ApiResponse<IReadOnlyList<TrendingEventDto>>.SuccessResult(result.Value!, "Trending events retrieved successfully."));
    }

    /// <summary>
    /// Gets featured/highlighted events curated by the platform.
    /// </summary>
    /// <param name="city">Optional city filter</param>
    /// <param name="sportName">Optional sport filter</param>
    /// <param name="limit">Maximum number of events (1-100, default 20)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of featured event cards</returns>
    /// <response code="200">Featured events retrieved successfully</response>
    [HttpGet("featured")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<FeaturedEventDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetFeaturedEvents(
        [FromQuery] string? city = null,
        [FromQuery] string? sportName = null,
        [FromQuery] int limit = 20,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Featured events: City={City}, Sport={Sport}", city, sportName);

        var result = await _mediator.Send(new FeaturedEventsQuery
        {
            City = city,
            SportName = sportName,
            Limit = limit
        }, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Bad Request",
                Detail = result.Error
            });

        return Ok(ApiResponse<IReadOnlyList<FeaturedEventDto>>.SuccessResult(result.Value!, "Featured events retrieved successfully."));
    }

    /// <summary>
    /// Gets personalized event recommendations.
    /// </summary>
    /// <remarks>
    /// For authenticated users, returns personalized recommendations based on history and preferences.
    /// For anonymous users, returns general recommendations based on location and popular events.
    /// </remarks>
    /// <param name="city">Optional city preference</param>
    /// <param name="latitude">Optional latitude for proximity-based recommendations</param>
    /// <param name="longitude">Optional longitude for proximity-based recommendations</param>
    /// <param name="limit">Maximum number of recommendations (1-50, default 20)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of recommended events with relevance scores</returns>
    /// <response code="200">Recommendations retrieved successfully</response>
    [HttpGet("recommended")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<RecommendationDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetRecommendedEvents(
        [FromQuery] string? city = null,
        [FromQuery] decimal? latitude = null,
        [FromQuery] decimal? longitude = null,
        [FromQuery] int limit = 20,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Recommended events: City={City}", city);

        Guid? userId = null;
        if (User.Identity?.IsAuthenticated == true)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var parsed))
                userId = parsed;
        }

        var result = await _mediator.Send(new RecommendedEventsQuery
        {
            UserId = userId,
            City = city,
            Latitude = latitude,
            Longitude = longitude,
            Limit = limit
        }, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Bad Request",
                Detail = result.Error
            });

        return Ok(ApiResponse<IReadOnlyList<RecommendationDto>>.SuccessResult(result.Value!, "Recommendations retrieved successfully."));
    }

    /// <summary>
    /// Gets events near a geographic location with distance information.
    /// </summary>
    /// <param name="latitude">Latitude coordinate (-90 to 90)</param>
    /// <param name="longitude">Longitude coordinate (-180 to 180)</param>
    /// <param name="radiusKm">Search radius in kilometers (0.1-500, default 10)</param>
    /// <param name="limit">Maximum number of events (1-100, default 20)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of nearby events with distance from location</returns>
    /// <response code="200">Nearby events retrieved successfully</response>
    /// <response code="400">Validation error</response>
    [HttpGet("nearby")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<NearbyEventDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetNearbyEvents(
        [FromQuery] decimal latitude,
        [FromQuery] decimal longitude,
        [FromQuery] decimal radiusKm = 10,
        [FromQuery] int limit = 20,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Nearby events: Lat={Lat}, Lng={Lng}, Radius={Radius}km", latitude, longitude, radiusKm);

        var result = await _mediator.Send(new NearbyEventsQuery
        {
            Latitude = latitude,
            Longitude = longitude,
            RadiusKm = radiusKm,
            Limit = limit
        }, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Bad Request",
                Detail = result.Error
            });

        return Ok(ApiResponse<IReadOnlyList<NearbyEventDto>>.SuccessResult(result.Value!, "Nearby events retrieved successfully."));
    }

    /// <summary>
    /// Gets events in calendar format for a date range.
    /// </summary>
    /// <param name="FromDate">Start date for calendar range</param>
    /// <param name="ToDate">End date for calendar range</param>
    /// <param name="academyId">Optional academy filter</param>
    /// <param name="viewType">Calendar view type: Daily, Weekly, Monthly, Agenda, Timeline</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of calendar event entries</returns>
    /// <response code="200">Calendar events retrieved successfully</response>
    /// <response code="400">Validation error</response>
    [HttpGet("calendar")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<CalendarEventDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetCalendarEvents(
        [FromQuery] DateTime FromDate,
        [FromQuery] DateTime ToDate,
        [FromQuery] Guid? academyId = null,
        [FromQuery] CalendarViewType viewType = CalendarViewType.Monthly,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Calendar events: {FromDate} to {ToDate}, View={View}", FromDate, ToDate, viewType);

        var result = await _mediator.Send(new CalendarEventsQuery
        {
            FromDate = FromDate,
            ToDate = ToDate,
            AcademyId = academyId,
            ViewType = viewType
        }, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Bad Request",
                Detail = result.Error
            });

        return Ok(ApiResponse<IReadOnlyList<CalendarEventDto>>.SuccessResult(result.Value!, "Calendar events retrieved successfully."));
    }

    /// <summary>
    /// Gets autocomplete suggestions for event search.
    /// </summary>
    /// <remarks>
    /// Returns matching event names and codes based on the prefix.
    /// Minimum 2 characters required. Results are cached for 5 minutes.
    /// </remarks>
    /// <param name="Prefix">Search prefix (minimum 2 characters)</param>
    /// <param name="Limit">Maximum suggestions (1-20, default 10)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of autocomplete suggestions with highlighted text</returns>
    /// <response code="200">Suggestions retrieved successfully</response>
    /// <response code="400">Validation error</response>
    [HttpGet("autocomplete")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<EventAutocompleteSuggestionDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAutocompleteSuggestions(
        [FromQuery] string Prefix,
        [FromQuery] int Limit = 10,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Autocomplete: prefix='{Prefix}', limit={Limit}", Prefix, Limit);

        var result = await _mediator.Send(new AutocompleteQuery
        {
            Prefix = Prefix,
            Limit = Limit
        }, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Bad Request",
                Detail = result.Error
            });

        return Ok(ApiResponse<IReadOnlyList<EventAutocompleteSuggestionDto>>.SuccessResult(result.Value!, "Suggestions retrieved successfully."));
    }

    /// <summary>
    /// Saves a search query for later use.
    /// </summary>
    /// <param name="request">Save search request with name and filter parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The saved search details</returns>
    /// <response code="200">Search saved successfully</response>
    /// <response code="400">Validation error</response>
    /// <response code="401">Unauthorized</response>
    [HttpPost("saved-searches")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<SavedEventSearchDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SaveSearch(
        [FromBody] SaveSearchCommand request,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null)
            return Unauthorized();

        request.UserId = userId.Value;

        _logger.LogInformation("Saving search '{SearchName}' for user {UserId}", request.SearchName, userId);

        var result = await _mediator.Send(request, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Bad Request",
                Detail = result.Error
            });

        return Ok(ApiResponse<SavedEventSearchDto>.SuccessResult(result.Value!, "Search saved successfully."));
    }

    /// <summary>
    /// Deletes a saved search.
    /// </summary>
    /// <param name="id">Saved search ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="204">Search deleted successfully</response>
    /// <response code="404">Saved search not found</response>
    /// <response code="401">Unauthorized</response>
    [HttpDelete("saved-searches/{id:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteSavedSearch(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null)
            return Unauthorized();

        _logger.LogInformation("Deleting saved search {SearchId} for user {UserId}", id, userId);

        var result = await _mediator.Send(new DeleteSavedSearchCommand
        {
            SavedSearchId = id,
            UserId = userId.Value
        }, cancellationToken);

        if (!result.IsSuccess)
            return NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Not Found",
                Detail = result.Error
            });

        return NoContent();
    }

    /// <summary>
    /// Tracks an event view for recently viewed events and analytics.
    /// </summary>
    /// <param name="eventId">Event ID to track</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="200">View tracked successfully</response>
    /// <response code="400">Validation error</response>
    [HttpPost("track-view/{eventId:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> TrackEventView(
        [FromRoute] Guid eventId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Tracking view for event {EventId}", eventId);

        Guid? userId = null;
        if (User.Identity?.IsAuthenticated == true)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var parsed))
                userId = parsed;
        }

        var result = await _mediator.Send(new TrackRecentlyViewedCommand
        {
            EventId = eventId,
            UserId = userId,
            Source = "api"
        }, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Bad Request",
                Detail = result.Error
            });

        return Ok(ApiResponse<bool>.SuccessResult(true, "View tracked successfully."));
    }

    private Guid? GetUserId()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
            return userId;
        return null;
    }
}
