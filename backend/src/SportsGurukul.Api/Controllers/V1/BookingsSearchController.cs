using System.Net.Mime;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Search.Commands.DeleteSavedBookingSearch;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Search.Commands.RecordBookingSearch;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Search.Commands.SaveBookingSearch;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Search.DTOs;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Search.Queries.AdvancedSearchBookings;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Search.Queries.GetBookingSuggestions;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Search.Queries.GetRecentBookingSearches;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Search.Queries.GetSavedBookingSearches;

namespace SportsGurukul.Api.Controllers.V1;

/// <summary>
/// Advanced booking search with filtering, autocomplete, saved searches, and recent search history.
/// </summary>
[ApiController]
[Route("api/v1/bookings/search")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
[Tags("Booking Search")]
public class BookingsSearchController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<BookingsSearchController> _logger;

    public BookingsSearchController(IMediator mediator, ILogger<BookingsSearchController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Advanced search with multiple filters, date range, sorting, and pagination.
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Academy Admin,Coach,Athlete,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<BookingSearchPageResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AdvancedSearch(
        [FromQuery] string? searchTerm,
        [FromQuery] string? bookingNumber,
        [FromQuery] string? title,
        [FromQuery] Guid? academyId,
        [FromQuery] Guid? branchId,
        [FromQuery] Guid? facilityId,
        [FromQuery] Guid? coachId,
        [FromQuery] Guid? athleteId,
        [FromQuery] string? bookingType,
        [FromQuery] string? status,
        [FromQuery] string? approvalStatus,
        [FromQuery] DateTime? dateFrom,
        [FromQuery] DateTime? dateTo,
        [FromQuery] TimeSpan? startTimeFrom,
        [FromQuery] TimeSpan? startTimeTo,
        [FromQuery] string? sortBy,
        [FromQuery] bool sortDescending = true,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Advanced booking search: Page={Page}, Term={SearchTerm}", page, searchTerm);

        var query = new AdvancedSearchBookingsQuery
        {
            SearchTerm = searchTerm,
            BookingNumber = bookingNumber,
            Title = title,
            AcademyId = academyId,
            BranchId = branchId,
            FacilityId = facilityId,
            CoachId = coachId,
            AthleteId = athleteId,
            BookingType = bookingType,
            Status = status,
            ApprovalStatus = approvalStatus,
            DateFrom = dateFrom,
            DateTo = dateTo,
            StartTimeFrom = startTimeFrom,
            StartTimeTo = startTimeTo,
            SortBy = sortBy,
            SortDescending = sortDescending,
            Page = page,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        var userId = GetUserId();
        if (userId.HasValue && !string.IsNullOrWhiteSpace(searchTerm))
        {
            _ = _mediator.Send(new RecordBookingSearchCommand
            {
                UserId = userId.Value,
                SearchTerm = searchTerm,
                AcademyId = academyId,
                FacilityId = facilityId,
                BookingType = bookingType,
                Status = status,
                ResultCount = result.Value!.TotalRecords
            }, cancellationToken);
        }

        return Ok(ApiResponse<BookingSearchPageResultDto>.SuccessResult(
            result.Value!, "Search completed successfully."));
    }

    /// <summary>
    /// Autocomplete suggestions for booking searches.
    /// </summary>
    [HttpGet("suggestions")]
    [Authorize(Roles = "Academy Admin,Coach,Athlete,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<BookingSuggestionDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetSuggestions(
        [FromQuery] string prefix,
        [FromQuery] Guid? academyId,
        [FromQuery] int limit = 10,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Booking suggestions for prefix: {Prefix}", prefix);

        var result = await _mediator.Send(
            new GetBookingSuggestionsQuery
            {
                Prefix = prefix,
                AcademyId = academyId,
                Limit = limit
            }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<IReadOnlyList<BookingSuggestionDto>>.SuccessResult(
            result.Value!, "Suggestions retrieved successfully."));
    }

    /// <summary>
    /// Gets all saved searches for the current user.
    /// </summary>
    [HttpGet("saved")]
    [Authorize(Roles = "Academy Admin,Coach,Athlete,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<SavedBookingSearchDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetSavedSearches(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null)
            return Unauthorized();

        _logger.LogInformation("Getting saved searches for user {UserId}", userId);

        var result = await _mediator.Send(
            new GetSavedBookingSearchesQuery { UserId = userId.Value }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<IReadOnlyList<SavedBookingSearchDto>>.SuccessResult(
            result.Value!, "Saved searches retrieved successfully."));
    }

    /// <summary>
    /// Saves a search filter combination for quick reuse.
    /// </summary>
    [HttpPost("saved")]
    [Authorize(Roles = "Academy Admin,Coach,Athlete,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<SavedBookingSearchDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SaveSearch(
        [FromBody] SaveBookingSearchApiRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null)
            return Unauthorized();

        _logger.LogInformation("Saving search '{Name}' for user {UserId}", request.Name, userId);

        var command = new SaveBookingSearchCommand
        {
            UserId = userId.Value,
            Name = request.Name,
            SearchTerm = request.SearchTerm,
            BookingNumber = request.BookingNumber,
            Title = request.Title,
            AcademyId = request.AcademyId,
            BranchId = request.BranchId,
            FacilityId = request.FacilityId,
            CoachId = request.CoachId,
            AthleteId = request.AthleteId,
            BookingType = request.BookingType,
            Status = request.Status,
            ApprovalStatus = request.ApprovalStatus,
            DateFrom = request.DateFrom,
            DateTo = request.DateTo,
            StartTimeFrom = request.StartTimeFrom,
            StartTimeTo = request.StartTimeTo
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return CreatedAtAction(
            nameof(GetSavedSearches),
            ApiResponse<SavedBookingSearchDto>.SuccessResult(result.Value!, "Search saved successfully."));
    }

    /// <summary>
    /// Deletes a saved search by its identifier.
    /// </summary>
    [HttpDelete("saved/{savedSearchId:guid}")]
    [Authorize(Roles = "Academy Admin,Coach,Athlete,System Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSavedSearch(
        Guid savedSearchId,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null)
            return Unauthorized();

        _logger.LogInformation("Deleting saved search {SearchId} for user {UserId}", savedSearchId, userId);

        var result = await _mediator.Send(
            new DeleteSavedBookingSearchCommand
            {
                UserId = userId.Value,
                SavedSearchId = savedSearchId
            }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return NoContent();
    }

    /// <summary>
    /// Gets recent search history for the current user.
    /// </summary>
    [HttpGet("recent")]
    [Authorize(Roles = "Academy Admin,Coach,Athlete,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<RecentBookingSearchDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetRecentSearches(
        [FromQuery] int limit = 10,
        CancellationToken cancellationToken = default)
    {
        var userId = GetUserId();
        if (userId is null)
            return Unauthorized();

        _logger.LogInformation("Getting recent searches for user {UserId}", userId);

        var result = await _mediator.Send(
            new GetRecentBookingSearchesQuery
            {
                UserId = userId.Value,
                Limit = limit
            }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<IReadOnlyList<RecentBookingSearchDto>>.SuccessResult(
            result.Value!, "Recent searches retrieved successfully."));
    }

    #region Helpers

    private Guid? GetUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return null;
        return userId;
    }

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

/// <summary>
/// API request model for saving a booking search.
/// </summary>
public class SaveBookingSearchApiRequest
{
    public string Name { get; set; } = string.Empty;
    public string? SearchTerm { get; set; }
    public string? BookingNumber { get; set; }
    public string? Title { get; set; }
    public Guid? AcademyId { get; set; }
    public Guid? BranchId { get; set; }
    public Guid? FacilityId { get; set; }
    public Guid? CoachId { get; set; }
    public Guid? AthleteId { get; set; }
    public string? BookingType { get; set; }
    public string? Status { get; set; }
    public string? ApprovalStatus { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public TimeSpan? StartTimeFrom { get; set; }
    public TimeSpan? StartTimeTo { get; set; }
}
