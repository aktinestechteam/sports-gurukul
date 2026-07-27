using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.TournamentManagement.Commands.PublishResults;
using SportsGurukul.Application.Features.TournamentManagement.DTOs;
using SportsGurukul.Application.Features.TournamentManagement.Queries.GetTournamentResults;

namespace SportsGurukul.Api.Controllers.V1;

/// <summary>
/// Manages tournament results and publications.
/// </summary>
[ApiController]
[Route("api/v1/tournaments/{id:guid}/results")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
[Tags("Tournament Results")]
public class TournamentResultsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TournamentResultsController> _logger;

    public TournamentResultsController(IMediator mediator, ILogger<TournamentResultsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Gets all results for a tournament.
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ResultDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetResults(
        Guid id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching results for tournament: {TournamentId}", id);

        var result = await _mediator.Send(new GetTournamentResultsQuery { TournamentId = id }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<IReadOnlyList<ResultDto>>.SuccessResult(result.Value!, "Results retrieved successfully."));
    }

    /// <summary>
    /// Publishes results for a completed tournament.
    /// </summary>
    [HttpPost("publish")]
    [Authorize(Roles = "System Admin,Academy Admin,Tournament Manager")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PublishResults(
        Guid id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Publishing results for tournament: {TournamentId}", id);

        var result = await _mediator.Send(new PublishResultsCommand { TournamentId = id }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<object>.SuccessResult(new { TournamentId = id }, "Results published successfully."));
    }

    #region Helpers

    private IActionResult HandleFailure(string error)
    {
        if (error.Contains("not found", StringComparison.OrdinalIgnoreCase))
            return NotFound(new ProblemDetails { Status = StatusCodes.Status404NotFound, Title = "Not Found", Detail = error, Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4" });
        return BadRequest(new ProblemDetails { Status = StatusCodes.Status400BadRequest, Title = "Bad Request", Detail = error, Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1" });
    }

    #endregion
}
