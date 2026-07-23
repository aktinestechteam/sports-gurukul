using System.Net.Mime;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Api.Common.Models.SwaggerExamples;
using SportsGurukul.Application.Features.AthleteManagement.Commands.CreateSavedSearch;
using SportsGurukul.Application.Features.AthleteManagement.Commands.DeleteSavedSearch;
using SportsGurukul.Application.Features.AthleteManagement.Commands.RecordRecentSearch;
using SportsGurukul.Application.Features.AthleteManagement.DTOs;
using SportsGurukul.Application.Features.AthleteManagement.Queries.AdvancedSearchAthletes;
using SportsGurukul.Application.Features.AthleteManagement.Queries.GetAthleteSuggestions;
using SportsGurukul.Application.Features.AthleteManagement.Queries.GetRecentSearches;
using SportsGurukul.Application.Features.AthleteManagement.Queries.GetSavedSearches;
using SportsGurukul.Application.Features.Authentication.DTOs.Responses;
using Swashbuckle.AspNetCore.Filters;

namespace SportsGurukul.Api.Controllers.V1;

/// <summary>
/// Provides athlete search, autocomplete suggestions, saved searches, and recent search history.
/// </summary>
[ApiController]
[Route("api/v1/athletes/search")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
[Tags("Athlete Search & Discovery")]
public class AthleteSearchController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AthleteSearchController> _logger;

    public AthleteSearchController(IMediator mediator, ILogger<AthleteSearchController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Advanced athlete search with 30+ filter criteria, sorting, and cursor/offset pagination.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<AthleteSearchResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AthleteSearchResponse>), StatusCodes.Status400BadRequest)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(AdvancedSearchResponseExample))]
    public async Task<IActionResult> SearchAthletes(
        [FromQuery] AdvancedSearchRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Advanced athlete search: Page={Page}, PageSize={PageSize}", request.Page, request.PageSize);

        var query = new AdvancedSearchAthletesQuery
        {
            SearchTerm = request.SearchTerm,
            Name = request.Name,
            AthleteCode = request.AthleteCode,
            Email = request.Email,
            Mobile = request.Mobile,
            SportName = request.SportName,
            SportCategory = request.SportCategory,
            IsPrimarySport = request.IsPrimarySport,
            SportIds = request.SportIds,
            City = request.City,
            State = request.State,
            Country = request.Country,
            District = request.District,
            PostalCode = request.PostalCode,
            CurrentLevel = request.CurrentLevel,
            Ranking = request.Ranking,
            StateRank = request.StateRank,
            NationalRank = request.NationalRank,
            InternationalRank = request.InternationalRank,
            Gender = request.Gender,
            MinAge = request.MinAge,
            MaxAge = request.MaxAge,
            MinHeight = request.MinHeight,
            MaxHeight = request.MaxHeight,
            MinWeight = request.MinWeight,
            MaxWeight = request.MaxWeight,
            BloodGroup = request.BloodGroup,
            MinExperience = request.MinExperience,
            MaxExperience = request.MaxExperience,
            Status = request.Status,
            IsVerified = request.IsVerified,
            HasMedicalProfile = request.HasMedicalProfile,
            MinAchievementLevel = request.MinAchievementLevel,
            CreatedFrom = request.CreatedFrom,
            CreatedTo = request.CreatedTo,
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

        return Ok(ApiResponse<AthleteSearchResponse>.SuccessResult(result.Value!, "Athletes retrieved successfully."));
    }

    /// <summary>
    /// Get autocomplete suggestions based on a search prefix.
    /// Returns matching athlete names, athlete codes, and sport names.
    /// </summary>
    [HttpGet("suggestions")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<AthleteSearchSuggestionDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<AthleteSearchSuggestionDto>>), StatusCodes.Status400BadRequest)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(SuggestionsResponseExample))]
    public async Task<IActionResult> GetSuggestions(
        [FromQuery] string prefix,
        [FromQuery] int limit = 10,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Search suggestions: prefix={Prefix}, limit={Limit}", prefix, limit);

        var query = new GetAthleteSuggestionsQuery
        {
            Prefix = prefix,
            Limit = limit
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<IReadOnlyList<AthleteSearchSuggestionDto>>.SuccessResult(result.Value!, "Suggestions retrieved successfully."));
    }

    /// <summary>
    /// Get the user's saved search configurations.
    /// </summary>
    [HttpGet("saved")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<SavedSearchDto>>), StatusCodes.Status200OK)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(SavedSearchesResponseExample))]
    public async Task<IActionResult> GetSavedSearches(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        _logger.LogInformation("Getting saved searches for user: {UserId}", userId);

        var query = new GetSavedSearchesQuery { UserId = userId };

        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<IReadOnlyList<SavedSearchDto>>.SuccessResult(result.Value!, "Saved searches retrieved successfully."));
    }

    /// <summary>
    /// Save a search configuration for reuse.
    /// </summary>
    [HttpPost("saved")]
    [ProducesResponseType(typeof(ApiResponse<SavedSearchDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<SavedSearchDto>), StatusCodes.Status400BadRequest)]
    [SwaggerRequestExample(typeof(CreateSavedSearchRequest), typeof(CreateSavedSearchRequestExample))]
    [SwaggerResponseExample(StatusCodes.Status201Created, typeof(CreateSavedSearchResponseExample))]
    public async Task<IActionResult> CreateSavedSearch(
        [FromBody] CreateSavedSearchRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        _logger.LogInformation("Creating saved search for user: {UserId}, Name: {Name}", userId, request.Name);

        var command = new CreateSavedSearchCommand
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
    /// Delete a saved search configuration.
    /// </summary>
    [HttpDelete("saved/{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<Unit>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<Unit>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSavedSearch(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        _logger.LogInformation("Deleting saved search: {Id} for user: {UserId}", id, userId);

        var command = new DeleteSavedSearchCommand { Id = id, UserId = userId };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<Unit>.SuccessResult(Unit.Value, "Saved search deleted successfully."));
    }

    /// <summary>
    /// Get the user's recent search history.
    /// </summary>
    [HttpGet("recent")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<RecentSearchDto>>), StatusCodes.Status200OK)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(RecentSearchesResponseExample))]
    public async Task<IActionResult> GetRecentSearches(
        [FromQuery] int limit = 10,
        CancellationToken cancellationToken = default)
    {
        var userId = GetUserId();
        _logger.LogInformation("Getting recent searches for user: {UserId}", userId);

        var query = new GetRecentSearchesQuery
        {
            UserId = userId,
            Limit = limit
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<IReadOnlyList<RecentSearchDto>>.SuccessResult(result.Value!, "Recent searches retrieved successfully."));
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
