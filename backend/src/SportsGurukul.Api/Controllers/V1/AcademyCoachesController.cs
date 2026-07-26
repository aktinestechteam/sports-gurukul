using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Api.Common.Models.SwaggerExamples;
using SportsGurukul.Application.Features.AcademyManagement.Commands.AssignCoach;
using SportsGurukul.Application.Features.AcademyManagement.Commands.RemoveCoach;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;
using SportsGurukul.Application.Features.AcademyManagement.Queries.GetAssignedCoaches;
using SportsGurukul.Application.Features.Authentication.DTOs.Responses;
using Swashbuckle.AspNetCore.Filters;

namespace SportsGurukul.Api.Controllers.V1;

/// <summary>
/// Manages coach assignments to an academy — assign, remove, and list assigned coaches.
/// </summary>
[ApiController]
[Route("api/v1/academies/{academyId:guid}/coaches")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
[Tags("Academy Coaches")]
public class AcademyCoachesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AcademyCoachesController> _logger;

    public AcademyCoachesController(IMediator mediator, ILogger<AcademyCoachesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Assigns a coach to an academy.
    /// </summary>
    /// <param name="academyId">The academy's unique identifier</param>
    /// <param name="coachId">The coach's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created coach assignment summary</returns>
    /// <response code="201">Coach assigned successfully</response>
    /// <response code="400">Validation error</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Academy or coach not found</response>
    /// <response code="409">Coach already assigned to this academy</response>
    [HttpPost("{coachId:guid}")]
    [Authorize(Roles = "Academy Admin,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<AcademyCoachSummaryDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AssignCoach(
        Guid academyId,
        Guid coachId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Assigning coach {CoachId} to academy: {AcademyId}", coachId, academyId);

        var command = new AssignCoachCommand
        {
            AcademyId = academyId,
            CoachId = coachId
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Coach assigned to academy: {AcademyId}", academyId);

        return CreatedAtAction(
            nameof(GetAssignedCoaches),
            new { academyId, version = "1.0" },
            ApiResponse<AcademyCoachSummaryDto>.SuccessResult(result.Value!, "Coach assigned successfully."));
    }

    /// <summary>
    /// Removes a coach assignment from an academy.
    /// </summary>
    /// <param name="academyId">The academy's unique identifier</param>
    /// <param name="coachId">The coach's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content</returns>
    /// <response code="204">Coach removed successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Coach assignment not found</response>
    [HttpDelete("{coachId:guid}")]
    [Authorize(Roles = "Academy Admin,System Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveCoach(
        Guid academyId,
        Guid coachId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Removing coach {CoachId} from academy: {AcademyId}", coachId, academyId);

        var result = await _mediator.Send(
            new RemoveCoachCommand { AcademyId = academyId, CoachId = coachId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Coach removed from academy: {AcademyId}", academyId);

        return NoContent();
    }

    /// <summary>
    /// Gets all coaches assigned to an academy.
    /// </summary>
    /// <param name="academyId">The academy's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of assigned coaches</returns>
    /// <response code="200">Coaches retrieved successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="404">Academy not found</response>
    [HttpGet]
    [Authorize(Roles = "Academy Admin,Coach,Athlete,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<AcademyCoachSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(AcademyCoachSummaryDtoExample))]
    public async Task<IActionResult> GetAssignedCoaches(
        Guid academyId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching assigned coaches for academy: {AcademyId}", academyId);

        var result = await _mediator.Send(new GetAssignedCoachesQuery { AcademyId = academyId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<IReadOnlyList<AcademyCoachSummaryDto>>.SuccessResult(result.Value!, "Coaches retrieved successfully."));
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

        if (error.Contains("already exists", StringComparison.OrdinalIgnoreCase) ||
            error.Contains("already associated", StringComparison.OrdinalIgnoreCase) ||
            error.Contains("already assigned", StringComparison.OrdinalIgnoreCase))
        {
            return Conflict(new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Conflict",
                Detail = error,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8"
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
