using System.Net.Mime;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Api.Common.Models.SwaggerExamples;
using SportsGurukul.Application.Features.AcademySearchDiscovery.Commands.DeleteSavedAcademySearch;
using SportsGurukul.Application.Features.AcademySearchDiscovery.Commands.RecordAcademySearch;
using SportsGurukul.Application.Features.AcademySearchDiscovery.Commands.SaveAcademySearch;
using SportsGurukul.Application.Features.AcademySearchDiscovery.DTOs;
using SportsGurukul.Application.Features.AcademySearchDiscovery.Queries.AdvancedSearchAcademies;
using SportsGurukul.Application.Features.AcademySearchDiscovery.Queries.GetPopularAcademies;
using SportsGurukul.Application.Features.AcademySearchDiscovery.Queries.GetPopularSearchTerms;
using SportsGurukul.Application.Features.AcademySearchDiscovery.Queries.GetRecentAcademySearches;
using SportsGurukul.Application.Features.AcademySearchDiscovery.Queries.GetSavedAcademySearches;
using SportsGurukul.Application.Features.AcademySearchDiscovery.Queries.GetSimilarAcademies;
using SportsGurukul.Application.Features.AcademySearchDiscovery.Queries.NearbyAcademies;
using Swashbuckle.AspNetCore.Filters;

namespace SportsGurukul.Api.Controllers.V1;

/// <summary>
/// Provides advanced academy search, nearby discovery, similar academies,
/// saved searches, recent searches, and popular academies.
/// </summary>
[ApiController]
[Route("api/v1/academies")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Academy Search & Discovery")]
public class AcademyDiscoveryController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AcademyDiscoveryController> _logger;

    public AcademyDiscoveryController(IMediator mediator, ILogger<AcademyDiscoveryController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Advanced academy search with 40+ filter criteria, geo-radius, facility filters, and sorting.
    /// </summary>
    /// <param name="request">Advanced search parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of academy search results</returns>
    /// <response code="200">Academies retrieved successfully</response>
    /// <response code="400">Validation error</response>
    [HttpPost("advanced-search")]
    [Authorize(Roles = "Academy Admin,Coach,Athlete,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<AcademySearchPageResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [SwaggerRequestExample(typeof(AdvancedAcademySearchRequest), typeof(AdvancedAcademySearchRequestExample))]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(AcademySearchPageResultDtoExample))]
    public async Task<IActionResult> AdvancedSearch(
        [FromBody] AdvancedAcademySearchRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Advanced academy search: SearchTerm={SearchTerm}, City={City}, Page={Page}",
            request.SearchTerm, request.City, request.Page);

        var userId = GetCurrentUserId();

        var query = new AdvancedSearchAcademiesQuery
        {
            SearchTerm = request.SearchTerm,
            Name = request.Name,
            AcademyCode = request.AcademyCode,
            RegistrationNumber = request.RegistrationNumber,
            Country = request.Country,
            State = request.State,
            City = request.City,
            District = request.District,
            PinCode = request.PinCode,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            RadiusKm = request.RadiusKm,
            SportName = request.SportName,
            SportCategory = request.SportCategory,
            HasSwimmingPool = request.HasSwimmingPool,
            HasIndoorStadium = request.HasIndoorStadium,
            HasCricketGround = request.HasCricketGround,
            HasFootballGround = request.HasFootballGround,
            HasGym = request.HasGym,
            HasYogaHall = request.HasYogaHall,
            HasParking = request.HasParking,
            HasMedicalRoom = request.HasMedicalRoom,
            HasWifi = request.HasWifi,
            HasCafeteria = request.HasCafeteria,
            VerifiedOnly = request.VerifiedOnly,
            GovernmentRegisteredOnly = request.GovernmentRegisteredOnly,
            MinEstablishmentYears = request.MinEstablishmentYears,
            MinMembershipPrice = request.MinMembershipPrice,
            MaxMembershipPrice = request.MaxMembershipPrice,
            MinRating = request.MinRating,
            MinCoaches = request.MinCoaches,
            MinAthletes = request.MinAthletes,
            MinBranches = request.MinBranches,
            OpenNow = request.OpenNow,
            WeekendOpen = request.WeekendOpen,
            SortBy = request.SortBy,
            Page = request.Page,
            PageSize = request.PageSize
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return HandleFailure(result.Error!);
        }

        if (userId != Guid.Empty)
        {
            var recordCommand = new RecordAcademySearchCommand
            {
                UserId = userId,
                SearchTerm = request.SearchTerm ?? string.Empty,
                City = request.City,
                State = request.State,
                SportName = request.SportName,
                AcademyCount = result.Value!.TotalRecords
            };

            _ = _mediator.Send(recordCommand, cancellationToken);
        }

        return Ok(ApiResponse<AcademySearchPageResultDto>.SuccessResult(result.Value!, "Academies retrieved successfully."));
    }

    /// <summary>
    /// Finds academies near a geographic location within a specified radius.
    /// </summary>
    /// <param name="request">Nearby search parameters with coordinates</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of nearby academy results with distance</returns>
    /// <response code="200">Nearby academies retrieved successfully</response>
    /// <response code="400">Validation error</response>
    [HttpGet("nearby")]
    [Authorize(Roles = "Academy Admin,Coach,Athlete,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<AcademySearchResultDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [SwaggerRequestExample(typeof(NearbyAcademiesRequest), typeof(NearbyAcademiesRequestExample))]
    public async Task<IActionResult> GetNearbyAcademies(
        [FromQuery] NearbyAcademiesRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Nearby academies: Lat={Latitude}, Lng={Longitude}, Radius={RadiusKm}",
            request.Latitude, request.Longitude, request.RadiusKm);

        var query = new NearbyAcademiesQuery
        {
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            RadiusKm = request.RadiusKm,
            Limit = request.Limit,
            SportName = request.SportName
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return HandleFailure(result.Error!);
        }

        return Ok(ApiResponse<IReadOnlyList<AcademySearchResultDto>>.SuccessResult(result.Value!, "Nearby academies retrieved successfully."));
    }

    /// <summary>
    /// Gets academies similar to the specified academy based on sports, facilities, and location.
    /// </summary>
    /// <param name="academyId">Academy ID to find similar ones for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of similar academies with similarity scores</returns>
    /// <response code="200">Similar academies retrieved successfully</response>
    /// <response code="400">Validation error</response>
    /// <response code="404">Academy not found</response>
    [HttpGet("similar/{academyId:guid}")]
    [Authorize(Roles = "Academy Admin,Coach,Athlete,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<AcademySimilarDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(AcademySimilarDtoExample))]
    public async Task<IActionResult> GetSimilarAcademies(
        [FromRoute] Guid academyId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Similar academies for AcademyId={AcademyId}", academyId);

        var query = new GetSimilarAcademiesQuery
        {
            AcademyId = academyId
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return HandleFailure(result.Error!);
        }

        return Ok(ApiResponse<IReadOnlyList<AcademySimilarDto>>.SuccessResult(result.Value!, "Similar academies retrieved successfully."));
    }

    /// <summary>
    /// Saves an academy search configuration for later use.
    /// </summary>
    /// <param name="request">Search parameters to save</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The saved search details</returns>
    /// <response code="201">Search saved successfully</response>
    /// <response code="400">Validation error</response>
    [HttpPost("saved-searches")]
    [Authorize(Roles = "Academy Admin,Athlete,Coach,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<SavedAcademySearchDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [SwaggerRequestExample(typeof(SaveAcademySearchRequest), typeof(SaveAcademySearchRequestExample))]
    [SwaggerResponseExample(StatusCodes.Status201Created, typeof(SavedAcademySearchDtoExample))]
    public async Task<IActionResult> SaveAcademySearch(
        [FromBody] SaveAcademySearchRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Saving academy search: SearchName={SearchName}", request.SearchName);

        var userId = GetCurrentUserId();

        if (userId == Guid.Empty)
        {
            return Unauthorized(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Unauthorized",
                Detail = "Valid user session is required.",
                Type = "https://tools.ietf.org/html/rfc7235#section-3.1"
            });
        }

        var command = new SaveAcademySearchCommand
        {
            UserId = userId,
            SearchName = request.SearchName,
            SearchTerm = request.SearchTerm,
            City = request.City,
            State = request.State,
            Country = request.Country,
            District = request.District,
            PinCode = request.PinCode,
            SportName = request.SportName,
            SportCategory = request.SportCategory,
            FacilityType = request.FacilityType,
            HasSwimmingPool = request.HasSwimmingPool,
            HasIndoorStadium = request.HasIndoorStadium,
            HasCricketGround = request.HasCricketGround,
            HasFootballGround = request.HasFootballGround,
            HasGym = request.HasGym,
            HasYogaHall = request.HasYogaHall,
            HasParking = request.HasParking,
            HasMedicalRoom = request.HasMedicalRoom,
            HasWifi = request.HasWifi,
            HasCafeteria = request.HasCafeteria,
            VerifiedOnly = request.VerifiedOnly,
            GovernmentRegisteredOnly = request.GovernmentRegisteredOnly,
            OpenNow = request.OpenNow,
            WeekendOpen = request.WeekendOpen,
            MinMembershipPrice = request.MinMembershipPrice,
            MaxMembershipPrice = request.MaxMembershipPrice,
            MinRating = request.MinRating,
            ResultCount = request.ResultCount
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return HandleFailure(result.Error!);
        }

        return CreatedAtAction(
            nameof(GetSavedAcademySearches),
            null,
            ApiResponse<SavedAcademySearchDto>.SuccessResult(result.Value!, "Search saved successfully."));
    }

    /// <summary>
    /// Gets all saved academy searches for the current user.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of saved search configurations</returns>
    /// <response code="200">Saved searches retrieved successfully</response>
    /// <response code="400">Validation error</response>
    [HttpGet("saved-searches")]
    [Authorize(Roles = "Academy Admin,Athlete,Coach,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<SavedAcademySearchDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(SavedAcademySearchDtoExample))]
    public async Task<IActionResult> GetSavedAcademySearches(
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving saved academy searches");

        var userId = GetCurrentUserId();

        if (userId == Guid.Empty)
        {
            return Unauthorized(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Unauthorized",
                Detail = "Valid user session is required.",
                Type = "https://tools.ietf.org/html/rfc7235#section-3.1"
            });
        }

        var query = new GetSavedAcademySearchesQuery
        {
            UserId = userId
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return HandleFailure(result.Error!);
        }

        return Ok(ApiResponse<IReadOnlyList<SavedAcademySearchDto>>.SuccessResult(result.Value!, "Saved searches retrieved successfully."));
    }

    /// <summary>
    /// Deletes a saved academy search by ID.
    /// </summary>
    /// <param name="id">Saved search ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="204">Search deleted successfully</response>
    /// <response code="400">Validation error</response>
    /// <response code="404">Saved search not found</response>
    [HttpDelete("saved-searches/{id:guid}")]
    [Authorize(Roles = "Academy Admin,Athlete,Coach,System Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSavedAcademySearch(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting saved academy search: Id={Id}", id);

        var userId = GetCurrentUserId();

        if (userId == Guid.Empty)
        {
            return Unauthorized(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Unauthorized",
                Detail = "Valid user session is required.",
                Type = "https://tools.ietf.org/html/rfc7235#section-3.1"
            });
        }

        var command = new DeleteSavedAcademySearchCommand
        {
            SearchId = id,
            UserId = userId
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return HandleFailure(result.Error!);
        }

        return NoContent();
    }

    /// <summary>
    /// Gets recent academy searches for the current user.
    /// </summary>
    /// <param name="limit">Maximum number of recent searches to return (default 10)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of recent search terms with context</returns>
    /// <response code="200">Recent searches retrieved successfully</response>
    /// <response code="400">Validation error</response>
    [HttpGet("recent-searches")]
    [Authorize(Roles = "Academy Admin,Athlete,Coach,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<RecentAcademySearchDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(RecentAcademySearchDtoExample))]
    public async Task<IActionResult> GetRecentAcademySearches(
        [FromQuery] int limit = 10,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving recent academy searches: Limit={Limit}", limit);

        var userId = GetCurrentUserId();

        if (userId == Guid.Empty)
        {
            return Unauthorized(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Unauthorized",
                Detail = "Valid user session is required.",
                Type = "https://tools.ietf.org/html/rfc7235#section-3.1"
            });
        }

        var query = new GetRecentAcademySearchesQuery
        {
            UserId = userId,
            Limit = limit
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return HandleFailure(result.Error!);
        }

        return Ok(ApiResponse<IReadOnlyList<RecentAcademySearchDto>>.SuccessResult(result.Value!, "Recent searches retrieved successfully."));
    }

    /// <summary>
    /// Gets popular academies ranked by view count and rating.
    /// </summary>
    /// <param name="limit">Maximum number of popular academies (default 10)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of popular academies</returns>
    /// <response code="200">Popular academies retrieved successfully</response>
    /// <response code="400">Validation error</response>
    [HttpGet("popular")]
    [Authorize(Roles = "Academy Admin,Coach,Athlete,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<PopularAcademyDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(PopularAcademyDtoExample))]
    public async Task<IActionResult> GetPopularAcademies(
        [FromQuery] int limit = 10,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving popular academies: Limit={Limit}", limit);

        var query = new GetPopularAcademiesQuery
        {
            Limit = limit
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return HandleFailure(result.Error!);
        }

        return Ok(ApiResponse<IReadOnlyList<PopularAcademyDto>>.SuccessResult(result.Value!, "Popular academies retrieved successfully."));
    }

    /// <summary>
    /// Gets popular search terms used across the platform.
    /// </summary>
    /// <param name="limit">Maximum number of terms (default 10)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of popular search terms</returns>
    /// <response code="200">Popular search terms retrieved successfully</response>
    /// <response code="400">Validation error</response>
    [HttpGet("popular-searches")]
    [Authorize(Roles = "Academy Admin,Coach,Athlete,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<string>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPopularSearchTerms(
        [FromQuery] int limit = 10,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving popular search terms: Limit={Limit}", limit);

        var query = new GetPopularSearchTermsQuery
        {
            Limit = limit
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return HandleFailure(result.Error!);
        }

        return Ok(ApiResponse<IReadOnlyList<string>>.SuccessResult(result.Value!, "Popular search terms retrieved successfully."));
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
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

        if (error.Contains("already exists", StringComparison.OrdinalIgnoreCase))
            return Conflict(new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Conflict",
                Detail = error,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8"
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
