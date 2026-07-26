using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Api.Common.Models.SwaggerExamples;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;
using SportsGurukul.Application.Features.AcademyManagement.Queries.GetPagedAcademies;
using SportsGurukul.Application.Features.AcademyManagement.Queries.SearchAcademies;
using Swashbuckle.AspNetCore.Filters;

namespace SportsGurukul.Api.Controllers.V1;

/// <summary>
/// Provides academy search, filtering, pagination, and autocomplete suggestions.
/// </summary>
[ApiController]
[Route("api/v1/academies")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Academy Search & Discovery")]
public class AcademySearchController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AcademySearchController> _logger;

    public AcademySearchController(IMediator mediator, ILogger<AcademySearchController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Advanced academy search with filtering, sorting, and pagination.
    /// </summary>
    /// <remarks>
    /// Supports full-text search across academy name, code, and description.
    /// Filter by name, city, state, sport, verification status, membership type, and facility type.
    /// </remarks>
    /// <param name="request">Search parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of academy summaries</returns>
    /// <response code="200">Academies retrieved successfully</response>
    /// <response code="400">Validation error</response>
    [HttpGet("search")]
    [Authorize(Roles = "Academy Admin,Coach,Athlete,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<AcademySearchResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(AcademySearchResponseExample))]
    public async Task<IActionResult> SearchAcademies(
        [FromQuery] AcademySearchRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Academy search: Page={Page}, PageSize={PageSize}", request.Page, request.PageSize);

        var query = new SearchAcademiesQuery
        {
            SearchTerm = request.SearchTerm,
            Name = request.Name,
            City = request.City,
            State = request.State,
            SportName = request.SportName,
            VerificationStatus = request.VerificationStatus,
            MembershipType = request.MembershipType,
            FacilityType = request.FacilityType,
            Page = request.Page,
            PageSize = request.PageSize
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Bad Request",
                Detail = result.Error,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
            });
        }

        return Ok(ApiResponse<AcademySearchResponse>.SuccessResult(result.Value!, "Academies retrieved successfully."));
    }

    /// <summary>
    /// Gets paginated academy list with optional search term.
    /// </summary>
    /// <param name="searchTerm">Optional search term</param>
    /// <param name="page">Page number (1-based, default 1)</param>
    /// <param name="pageSize">Items per page (default 20, max 100)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of academy summaries</returns>
    /// <response code="200">Academies retrieved successfully</response>
    /// <response code="400">Validation error</response>
    [HttpGet]
    [Authorize(Roles = "Academy Admin,Coach,Athlete,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<AcademySearchResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(AcademySearchResponseExample))]
    public async Task<IActionResult> GetAcademies(
        [FromQuery] string? searchTerm = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Academy listing: Page={Page}, PageSize={PageSize}", page, pageSize);

        var query = new GetPagedAcademiesQuery
        {
            SearchTerm = searchTerm,
            Page = page,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Bad Request",
                Detail = result.Error,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
            });
        }

        return Ok(ApiResponse<AcademySearchResponse>.SuccessResult(result.Value!, "Academies retrieved successfully."));
    }

    /// <summary>
    /// Gets academy autocomplete suggestions based on a search prefix.
    /// Returns matching academy names and codes.
    /// </summary>
    /// <param name="prefix">Search prefix for autocomplete</param>
    /// <param name="limit">Maximum number of suggestions (default 10)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of academy summary suggestions</returns>
    /// <response code="200">Suggestions retrieved successfully</response>
    /// <response code="400">Validation error</response>
    [HttpGet("suggestions")]
    [Authorize(Roles = "Academy Admin,Coach,Athlete,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<AcademySummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetSuggestions(
        [FromQuery] string prefix,
        [FromQuery] int limit = 10,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Academy suggestions: prefix={Prefix}, limit={Limit}", prefix, limit);

        var query = new SearchAcademiesQuery
        {
            SearchTerm = prefix,
            PageSize = limit
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Bad Request",
                Detail = result.Error,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
            });
        }

        return Ok(ApiResponse<IReadOnlyList<AcademySummaryDto>>.SuccessResult(
            result.Value!.Items, "Suggestions retrieved successfully."));
    }
}
