using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.TournamentManagement.Commands.CompleteMatch;
using SportsGurukul.Application.Features.TournamentManagement.Commands.RecordForfeit;
using SportsGurukul.Application.Features.TournamentManagement.Commands.RecordWalkover;
using SportsGurukul.Application.Features.TournamentManagement.Commands.RescheduleMatch;
using SportsGurukul.Application.Features.TournamentManagement.Commands.StartMatch;
using SportsGurukul.Application.Features.TournamentManagement.Commands.UpdateScore;
using SportsGurukul.Application.Features.TournamentManagement.DTOs;
using SportsGurukul.Application.Features.TournamentManagement.Queries.GetTournamentMatches;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Api.Controllers.V1;

/// <summary>
/// Manages tournament matches - score updates, state transitions, and retrieval.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
[Tags("Tournament Matches")]
public class TournamentMatchesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TournamentMatchesController> _logger;

    public TournamentMatchesController(IMediator mediator, ILogger<TournamentMatchesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Gets all matches for a tournament.
    /// </summary>
    [HttpGet("~/api/v1/tournaments/{id:guid}/matches")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<MatchDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTournamentMatches(
        Guid id,
        [FromQuery] MatchStatus? status,
        [FromQuery] Guid? roundId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching matches for tournament: {TournamentId}", id);

        var query = new GetTournamentMatchesQuery
        {
            TournamentId = id,
            Status = status,
            RoundId = roundId
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<IReadOnlyList<MatchDto>>.SuccessResult(result.Value!, "Matches retrieved successfully."));
    }

    /// <summary>
    /// Gets a specific match by its unique identifier.
    /// </summary>
    [HttpGet("~/api/v1/matches/{matchId:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<MatchDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMatchById(
        Guid matchId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching match: {MatchId}", matchId);

        var result = await _mediator.Send(new GetTournamentMatchesQuery { TournamentId = Guid.Empty }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        var match = result.Value!.FirstOrDefault(m => m.Id == matchId);
        if (match is null)
            return NotFound(new ProblemDetails { Status = StatusCodes.Status404NotFound, Title = "Not Found", Detail = $"Match {matchId} not found.", Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4" });

        return Ok(ApiResponse<MatchDto>.SuccessResult(match, "Match retrieved successfully."));
    }

    /// <summary>
    /// Starts a scheduled match.
    /// </summary>
    [HttpPost("~/api/v1/matches/{matchId:guid}/start")]
    [Authorize(Roles = "System Admin,Academy Admin,Tournament Manager,Coach")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> StartMatch(Guid matchId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting match: {MatchId}", matchId);

        var result = await _mediator.Send(new StartMatchCommand { MatchId = matchId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<object>.SuccessResult(new { MatchId = matchId }, "Match started successfully."));
    }

    /// <summary>
    /// Updates the score of an in-progress match.
    /// </summary>
    [HttpPut("~/api/v1/matches/{matchId:guid}/score")]
    [Authorize(Roles = "System Admin,Academy Admin,Tournament Manager,Coach")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateScore(
        Guid matchId,
        [FromBody] UpdateScoreRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating score for match: {MatchId}", matchId);

        var command = new UpdateScoreCommand
        {
            MatchId = matchId,
            HomeScore = request.HomeScore,
            AwayScore = request.AwayScore,
            ScoreDetails = request.ScoreDetails
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<object>.SuccessResult(new { MatchId = matchId }, "Score updated successfully."));
    }

    /// <summary>
    /// Completes an in-progress match.
    /// </summary>
    [HttpPost("~/api/v1/matches/{matchId:guid}/complete")]
    [Authorize(Roles = "System Admin,Academy Admin,Tournament Manager,Coach")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CompleteMatch(
        Guid matchId,
        [FromBody] CompleteMatchRequest? request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Completing match: {MatchId}", matchId);

        var command = new CompleteMatchCommand
        {
            MatchId = matchId,
            WinnerId = request?.WinnerId
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<object>.SuccessResult(new { MatchId = matchId }, "Match completed successfully."));
    }

    /// <summary>
    /// Records a walkover for a match.
    /// </summary>
    [HttpPost("~/api/v1/matches/{matchId:guid}/walkover")]
    [Authorize(Roles = "System Admin,Academy Admin,Tournament Manager")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RecordWalkover(
        Guid matchId,
        [FromBody] RecordWalkoverRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Recording walkover for match: {MatchId}", matchId);

        var command = new RecordWalkoverCommand
        {
            MatchId = matchId,
            WinnerId = request.WinnerId,
            Notes = request.Notes
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<object>.SuccessResult(new { MatchId = matchId }, "Walkover recorded successfully."));
    }

    /// <summary>
    /// Records a forfeit for a match.
    /// </summary>
    [HttpPost("~/api/v1/matches/{matchId:guid}/forfeit")]
    [Authorize(Roles = "System Admin,Academy Admin,Tournament Manager")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RecordForfeit(
        Guid matchId,
        [FromBody] RecordForfeitRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Recording forfeit for match: {MatchId}", matchId);

        var command = new RecordForfeitCommand
        {
            MatchId = matchId,
            WinnerId = request.WinnerId,
            Notes = request.Notes
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<object>.SuccessResult(new { MatchId = matchId }, "Forfeit recorded successfully."));
    }

    #region Helpers

    private IActionResult HandleFailure(string error)
    {
        if (error.Contains("not found", StringComparison.OrdinalIgnoreCase))
            return NotFound(new ProblemDetails { Status = StatusCodes.Status404NotFound, Title = "Not Found", Detail = error, Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4" });
        if (error.Contains("already", StringComparison.OrdinalIgnoreCase) || error.Contains("conflict", StringComparison.OrdinalIgnoreCase))
            return Conflict(new ProblemDetails { Status = StatusCodes.Status409Conflict, Title = "Conflict", Detail = error, Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8" });
        return BadRequest(new ProblemDetails { Status = StatusCodes.Status400BadRequest, Title = "Bad Request", Detail = error, Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1" });
    }

    #endregion
}

public record UpdateScoreRequest(int HomeScore, int AwayScore, string? ScoreDetails);
public record CompleteMatchRequest(Guid? WinnerId);
public record RecordWalkoverRequest(Guid WinnerId, string? Notes);
public record RecordForfeitRequest(Guid WinnerId, string? Notes);
