using System.Net.Mime;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Api.Common.Models.SwaggerExamples;
using SportsGurukul.Application.Features.CoachManagement.Commands.DeleteSavedCoachSearch;
using SportsGurukul.Application.Features.CoachManagement.Commands.RecordCoachRecentSearch;
using SportsGurukul.Application.Features.CoachManagement.Commands.SaveCoachSearch;
using SportsGurukul.Application.Features.CoachManagement.DTOs;
using SportsGurukul.Application.Features.CoachManagement.Queries.AdvancedSearchCoaches;
using SportsGurukul.Application.Features.CoachManagement.Queries.GetCoachSuggestions;
using SportsGurukul.Application.Features.CoachManagement.Queries.GetRecentCoachSearches;
using SportsGurukul.Application.Features.CoachManagement.Queries.GetSavedCoachSearches;
using SportsGurukul.Application.Features.CoachManagement.Queries.GetSimilarCoaches;
using Swashbuckle.AspNetCore.Filters;

namespace SportsGurukul.Api.Controllers.V1;

/// <summary>
/// Provides coach search, autocomplete suggestions, similar coaches, saved searches, and recent search history.
/// </summary>
[ApiController]
[Route("api/v1/coaches/search")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Coach Search & Discovery")]
public class CoachSearchController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CoachSearchController> _logger;

    public CoachSearchController(IMediator mediator, ILogger<CoachSearchController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Advanced coach search with 30+ filter criteria, sorting, and cursor/offset pagination.
    /// Supports radius search by latitude/longitude, availability filters, certification filters, and more.
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<AdvancedCoachSearchResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AdvancedCoachSearchResponse>), StatusCodes.Status400BadRequest)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(CoachAdvancedSearchResponseExample))]
    public async Task<IActionResult> SearchCoaches(
        [FromQuery] CoachAdvancedSearchRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Advanced coach search: Page={Page}, PageSize={PageSize}", request.Page, request.PageSize);

        var query = new AdvancedSearchCoachesQuery
        {
            SearchTerm = request.SearchTerm,
            Name = request.Name,
            CoachCode = request.CoachCode,
            Email = request.Email,
            Mobile = request.Mobile,
            SportName = request.SportName,
            SportIds = request.SportIds,
            SportCategory = request.SportCategory,
            CoachingLevel = request.CoachingLevel,
            MinExperience = request.MinExperience,
            MaxExperience = request.MaxExperience,
            CertificationName = request.CertificationName,
            CertificationStatus = request.CertificationStatus,
            CurrentOrganization = request.CurrentOrganization,
            HighestQualification = request.HighestQualification,
            Country = request.Country,
            State = request.State,
            City = request.City,
            District = request.District,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            RadiusKm = request.RadiusKm,
            AvailableToday = request.AvailableToday,
            OnlineAvailable = request.OnlineAvailable,
            OfflineAvailable = request.OfflineAvailable,
            IsVerified = request.IsVerified,
            BackgroundVerified = request.BackgroundVerified,
            Language = request.Language,
            SortBy = request.SortBy,
            SortDescending = request.SortDescending,
            Page = request.Page,
            PageSize = request.PageSize,
            Cursor = request.Cursor,
            UseCursorPagination = request.UseCursorPagination
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<AdvancedCoachSearchResponse>.SuccessResult(result.Value!, "Coaches retrieved successfully."));
    }

    /// <summary>
    /// Get autocomplete suggestions based on a search prefix.
    /// Returns matching coach names and coach codes.
    /// </summary>
    [HttpGet("suggestions")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<CoachSearchSuggestionDto>>), StatusCodes.Status200OK)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(CoachSuggestionsResponseExample))]
    public async Task<IActionResult> GetSuggestions(
        [FromQuery] string prefix,
        [FromQuery] int limit = 10,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Coach search suggestions: prefix={Prefix}, limit={Limit}", prefix, limit);

        var query = new GetCoachSuggestionsQuery { Prefix = prefix, Limit = limit };
        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<IReadOnlyList<CoachSearchSuggestionDto>>.SuccessResult(result.Value!, "Suggestions retrieved successfully."));
    }

    /// <summary>
    /// Get coaches similar to a given coach based on sports, experience, and verification status.
    /// </summary>
    [HttpGet("similar/{coachId:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<SimilarCoachDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<SimilarCoachDto>>), StatusCodes.Status404NotFound)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(SimilarCoachesResponseExample))]
    public async Task<IActionResult> GetSimilarCoaches(
        Guid coachId,
        [FromQuery] int limit = 5,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching similar coaches for: {CoachId}", coachId);

        var query = new GetSimilarCoachesQuery { CoachId = coachId, Limit = limit };
        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<IReadOnlyList<SimilarCoachDto>>.SuccessResult(result.Value!, "Similar coaches retrieved successfully."));
    }

    /// <summary>
    /// Save a coach search configuration for reuse.
    /// </summary>
    [HttpPost("saved")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<SavedSearchDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<SavedSearchDto>), StatusCodes.Status400BadRequest)]
    [SwaggerRequestExample(typeof(CoachSaveSearchRequest), typeof(CoachSaveSearchRequestExample))]
    [SwaggerResponseExample(StatusCodes.Status201Created, typeof(CoachSavedSearchResponseExample))]
    public async Task<IActionResult> CreateSavedSearch(
        [FromBody] CoachSaveSearchRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        _logger.LogInformation("Creating saved coach search for user: {UserId}, Name: {Name}", userId, request.Name);

        var command = new SaveCoachSearchCommand
        {
            UserId = userId,
            Name = request.Name,
            FiltersJson = request.FiltersJson
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return StatusCode(StatusCodes.Status201Created,
            ApiResponse<SavedSearchDto>.SuccessResult(result.Value!, "Saved search created successfully."));
    }

    /// <summary>
    /// Get the user's saved coach search configurations.
    /// </summary>
    [HttpGet("saved")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<SavedSearchDto>>), StatusCodes.Status200OK)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(CoachSavedSearchesResponseExample))]
    public async Task<IActionResult> GetSavedSearches(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        _logger.LogInformation("Getting saved coach searches for user: {UserId}", userId);

        var query = new GetSavedCoachSearchesQuery { UserId = userId };
        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<IReadOnlyList<SavedSearchDto>>.SuccessResult(result.Value!, "Saved searches retrieved successfully."));
    }

    /// <summary>
    /// Delete a saved coach search configuration.
    /// </summary>
    [HttpDelete("saved/{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSavedSearch(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        _logger.LogInformation("Deleting saved coach search: {Id} for user: {UserId}", id, userId);

        var command = new DeleteSavedCoachSearchCommand { Id = id, UserId = userId };
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<object>.SuccessResult(new { }, "Saved search deleted successfully."));
    }

    /// <summary>
    /// Get the user's recent coach search history.
    /// </summary>
    [HttpGet("recent")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<RecentSearchDto>>), StatusCodes.Status200OK)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(CoachRecentSearchesResponseExample))]
    public async Task<IActionResult> GetRecentSearches(
        [FromQuery] int limit = 10,
        CancellationToken cancellationToken = default)
    {
        var userId = GetUserId();
        _logger.LogInformation("Getting recent coach searches for user: {UserId}", userId);

        var query = new GetRecentCoachSearchesQuery { UserId = userId, Limit = limit };
        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<IReadOnlyList<RecentSearchDto>>.SuccessResult(result.Value!, "Recent searches retrieved successfully."));
    }

    /// <summary>
    /// Record a coach search in the user's recent search history.
    /// </summary>
    [HttpPost("recent")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status201Created)]
    public async Task<IActionResult> RecordRecentSearch(
        [FromBody] CoachRecordRecentSearchRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        _logger.LogInformation("Recording recent coach search for user: {UserId}", userId);

        var command = new RecordCoachRecentSearchCommand
        {
            UserId = userId,
            QueryText = request.QueryText,
            FiltersJson = request.FiltersJson,
            ResultCount = request.ResultCount
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return StatusCode(StatusCodes.Status201Created,
            ApiResponse<object>.SuccessResult(new { }, "Recent search recorded successfully."));
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedAccessException("User ID not found in token.");
        return userId;
    }

    private IActionResult HandleFailure(string error)
    {
        return BadRequest(ApiResponse<object>.FailureResult(error));
    }
}
