using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Api.Common.Models.SwaggerExamples;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;
using SportsGurukul.Application.Features.AcademyManagement.Queries.GetAcademyStatistics;
using Swashbuckle.AspNetCore.Filters;

namespace SportsGurukul.Api.Controllers.V1;

/// <summary>
/// Provides statistics and aggregate metrics for an academy.
/// </summary>
[ApiController]
[Route("api/v1/academies/{academyId:guid}/statistics")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
[Tags("Academy Statistics")]
public class AcademyStatisticsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AcademyStatisticsController> _logger;

    public AcademyStatisticsController(IMediator mediator, ILogger<AcademyStatisticsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Gets aggregate statistics for an academy, including counts of coaches, athletes,
    /// branches, facilities, memberships, sports, documents, and gallery images.
    /// </summary>
    /// <param name="academyId">The academy's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Academy statistics</returns>
    /// <response code="200">Statistics retrieved successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Academy not found</response>
    [HttpGet]
    [Authorize(Roles = "Academy Admin,Coach,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<AcademyStatisticsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(AcademyStatisticsDtoExample))]
    public async Task<IActionResult> GetAcademyStatistics(
        Guid academyId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching statistics for academy: {AcademyId}", academyId);

        var result = await _mediator.Send(new GetAcademyStatisticsQuery { AcademyId = academyId }, cancellationToken);

        if (!result.IsSuccess)
        {
            return HandleFailure(result.Error!);
        }

        return Ok(ApiResponse<AcademyStatisticsDto>.SuccessResult(result.Value!, "Statistics retrieved successfully."));
    }

    #region Helpers

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
