using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Api.Common.Models.SwaggerExamples;
using SportsGurukul.Application.Features.AcademyManagement.Commands.RegisterAthlete;
using SportsGurukul.Application.Features.AcademyManagement.Commands.RemoveAthlete;
using SportsGurukul.Application.Features.AcademyManagement.Commands.TransferAthlete;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;
using SportsGurukul.Application.Features.AcademyManagement.Queries.GetRegisteredAthletes;
using SportsGurukul.Application.Features.Authentication.DTOs.Responses;
using Swashbuckle.AspNetCore.Filters;

namespace SportsGurukul.Api.Controllers.V1;

/// <summary>
/// Manages athlete registrations to an academy — register, remove, transfer, and list registered athletes.
/// </summary>
[ApiController]
[Route("api/v1/academies/{academyId:guid}/athletes")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
[Tags("Academy Athletes")]
public class AcademyAthletesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AcademyAthletesController> _logger;

    public AcademyAthletesController(IMediator mediator, ILogger<AcademyAthletesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Registers an athlete with an academy.
    /// </summary>
    /// <param name="academyId">The academy's unique identifier</param>
    /// <param name="athleteId">The athlete's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created athlete registration summary</returns>
    /// <response code="201">Athlete registered successfully</response>
    /// <response code="400">Validation error</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Academy or athlete not found</response>
    /// <response code="409">Athlete already registered with this academy</response>
    [HttpPost("{athleteId:guid}")]
    [Authorize(Roles = "Academy Admin,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<AcademyAthleteSummaryDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegisterAthlete(
        Guid academyId,
        Guid athleteId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Registering athlete {AthleteId} with academy: {AcademyId}", athleteId, academyId);

        var command = new RegisterAthleteCommand
        {
            AcademyId = academyId,
            AthleteId = athleteId
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Athlete registered with academy: {AcademyId}", academyId);

        return CreatedAtAction(
            nameof(GetRegisteredAthletes),
            new { academyId, version = "1.0" },
            ApiResponse<AcademyAthleteSummaryDto>.SuccessResult(result.Value!, "Athlete registered successfully."));
    }

    /// <summary>
    /// Removes an athlete registration from an academy.
    /// </summary>
    /// <param name="academyId">The academy's unique identifier</param>
    /// <param name="athleteId">The athlete's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content</returns>
    /// <response code="204">Athlete removed successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Athlete registration not found</response>
    [HttpDelete("{athleteId:guid}")]
    [Authorize(Roles = "Academy Admin,System Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveAthlete(
        Guid academyId,
        Guid athleteId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Removing athlete {AthleteId} from academy: {AcademyId}", athleteId, academyId);

        var result = await _mediator.Send(
            new RemoveAthleteCommand { AcademyId = academyId, AthleteId = athleteId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Athlete removed from academy: {AcademyId}", academyId);

        return NoContent();
    }

    /// <summary>
    /// Transfers an athlete from this academy to another academy.
    /// </summary>
    /// <param name="academyId">The source academy's unique identifier</param>
    /// <param name="athleteId">The athlete's unique identifier</param>
    /// <param name="request">Transfer details with destination academy ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated athlete registration summary in the destination academy</returns>
    /// <response code="200">Athlete transferred successfully</response>
    /// <response code="400">Validation error or athlete not registered with this academy</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Academy or athlete not found</response>
    /// <response code="409">Athlete already registered with destination academy</response>
    [HttpPost("{athleteId:guid}/transfer")]
    [Authorize(Roles = "Academy Admin,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<AcademyAthleteSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [SwaggerRequestExample(typeof(TransferAthleteRequest), typeof(TransferAthleteRequestExample))]
    public async Task<IActionResult> TransferAthlete(
        Guid academyId,
        Guid athleteId,
        [FromBody] TransferAthleteRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Transferring athlete {AthleteId} from academy {AcademyId} to academy {ToAcademyId}",
            athleteId, academyId, request.ToAcademyId);

        var command = new TransferAthleteCommand
        {
            FromAcademyId = academyId,
            ToAcademyId = request.ToAcademyId,
            AthleteId = athleteId
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Athlete transferred from academy {AcademyId} to academy {ToAcademyId}",
            academyId, request.ToAcademyId);

        return Ok(ApiResponse<AcademyAthleteSummaryDto>.SuccessResult(result.Value!, "Athlete transferred successfully."));
    }

    /// <summary>
    /// Gets all athletes registered with an academy.
    /// </summary>
    /// <param name="academyId">The academy's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of registered athletes</returns>
    /// <response code="200">Athletes retrieved successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="404">Academy not found</response>
    [HttpGet]
    [Authorize(Roles = "Academy Admin,Coach,Athlete,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<AcademyAthleteSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(AcademyAthleteSummaryDtoExample))]
    public async Task<IActionResult> GetRegisteredAthletes(
        Guid academyId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching registered athletes for academy: {AcademyId}", academyId);

        var result = await _mediator.Send(new GetRegisteredAthletesQuery { AcademyId = academyId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<IReadOnlyList<AcademyAthleteSummaryDto>>.SuccessResult(result.Value!, "Athletes retrieved successfully."));
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
            error.Contains("already assigned", StringComparison.OrdinalIgnoreCase) ||
            error.Contains("already registered", StringComparison.OrdinalIgnoreCase))
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
