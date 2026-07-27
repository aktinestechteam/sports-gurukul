using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.TournamentManagement.DTOs;
using SportsGurukul.Application.Features.TournamentManagement.Queries.GetParticipantStatistics;
using SportsGurukul.Application.Features.TournamentManagement.Queries.SearchTournaments;

namespace SportsGurukul.Api.Controllers.V1;

/// <summary>
/// Provides tournament statistics and participant analytics.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
[Tags("Tournament Statistics")]
public class TournamentStatisticsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TournamentStatisticsController> _logger;

    public TournamentStatisticsController(IMediator mediator, ILogger<TournamentStatisticsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Gets participant statistics for a specific tournament and participant.
    /// </summary>
    [HttpGet("~/api/v1/tournaments/{id:guid}/statistics")]
    [Authorize(Roles = "System Admin,Academy Admin,Tournament Manager,Coach,Athlete")]
    [ProducesResponseType(typeof(ApiResponse<ParticipantStatisticsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetParticipantStatistics(
        Guid id,
        [FromQuery] Guid participantId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching statistics for participant: {ParticipantId} in tournament: {TournamentId}", participantId, id);

        var query = new GetParticipantStatisticsQuery
        {
            TournamentId = id,
            ParticipantId = participantId
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<ParticipantStatisticsDto>.SuccessResult(result.Value!, "Statistics retrieved successfully."));
    }

    /// <summary>
    /// Gets aggregate tournament statistics across all tournaments.
    /// </summary>
    [HttpGet("~/api/v1/tournaments/statistics")]
    [Authorize(Roles = "System Admin")]
    [ProducesResponseType(typeof(ApiResponse<TournamentSearchResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetTournamentStatistics(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching aggregate tournament statistics");

        var query = new SearchTournamentsQuery
        {
            Page = 1,
            PageSize = 1
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        var stats = new
        {
            TotalTournaments = result.Value!.TotalRecords,
            result.Value.TotalPages,
            result.Value.CurrentPage,
            result.Value.PageSize
        };

        return Ok(ApiResponse<object>.SuccessResult(stats, "Tournament statistics retrieved successfully."));
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
