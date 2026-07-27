using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.LiveScoringManagement.Commands.CompleteMatch;
using SportsGurukul.Application.Features.LiveScoringManagement.Commands.GenerateLeaderboard;
using SportsGurukul.Application.Features.LiveScoringManagement.Commands.PauseMatch;
using SportsGurukul.Application.Features.LiveScoringManagement.Commands.PublishResults;
using SportsGurukul.Application.Features.LiveScoringManagement.Commands.RecordForfeit;
using SportsGurukul.Application.Features.LiveScoringManagement.Commands.RecordWalkover;
using SportsGurukul.Application.Features.LiveScoringManagement.Commands.ResumeMatch;
using SportsGurukul.Application.Features.LiveScoringManagement.Commands.StartMatch;
using SportsGurukul.Application.Features.LiveScoringManagement.Commands.UndoScore;
using SportsGurukul.Application.Features.LiveScoringManagement.Commands.UpdateLiveScore;
using SportsGurukul.Application.Features.LiveScoringManagement.DTOs;
using SportsGurukul.Application.Features.LiveScoringManagement.Queries.Leaderboard;
using SportsGurukul.Application.Features.LiveScoringManagement.Queries.LiveScore;
using SportsGurukul.Application.Features.LiveScoringManagement.Queries.MatchStatistics;
using SportsGurukul.Application.Features.LiveScoringManagement.Queries.MedalTable;
using SportsGurukul.Application.Features.LiveScoringManagement.Queries.PlayerStatistics;
using SportsGurukul.Application.Features.LiveScoringManagement.Queries.TournamentStandings;
using SportsGurukul.Platform.Competition.Models.Enums;

namespace SportsGurukul.Api.Controllers.V1;

/// <summary>
/// Live match management - real-time scoring, leaderboard, rankings, and statistics.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
[Tags("Live Scoring")]
public class LiveScoringController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<LiveScoringController> _logger;

    public LiveScoringController(IMediator mediator, ILogger<LiveScoringController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Starts a live match from its scheduled state.
    /// </summary>
    [HttpPost("~/api/v1/live/matches/{matchId:guid}/start")]
    [Authorize(Roles = "System Admin,Academy Admin,Tournament Manager,Coach")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> StartLiveMatch(
        Guid matchId,
        [FromBody] StartLiveMatchRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting live match: {MatchId}", matchId);

        var result = await _mediator.Send(new StartLiveMatchCommand
        {
            TournamentId = request.TournamentId,
            MatchId = matchId,
            SportCode = request.SportCode,
            HomeParticipantId = request.HomeParticipantId,
            HomeParticipantName = request.HomeParticipantName,
            AwayParticipantId = request.AwayParticipantId,
            AwayParticipantName = request.AwayParticipantName
        }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<object>.SuccessResult(new { LiveMatchId = result.Value, MatchId = matchId }, "Live match started successfully."));
    }

    /// <summary>
    /// Pauses an active live match.
    /// </summary>
    [HttpPost("~/api/v1/live/matches/{matchId:guid}/pause")]
    [Authorize(Roles = "System Admin,Academy Admin,Tournament Manager,Coach")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PauseMatch(Guid matchId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Pausing live match: {MatchId}", matchId);

        var result = await _mediator.Send(new PauseMatchCommand { MatchId = matchId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<object>.SuccessResult(new { MatchId = matchId }, "Match paused successfully."));
    }

    /// <summary>
    /// Resumes a paused live match.
    /// </summary>
    [HttpPost("~/api/v1/live/matches/{matchId:guid}/resume")]
    [Authorize(Roles = "System Admin,Academy Admin,Tournament Manager,Coach")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResumeMatch(Guid matchId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Resuming live match: {MatchId}", matchId);

        var result = await _mediator.Send(new ResumeMatchCommand { MatchId = matchId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<object>.SuccessResult(new { MatchId = matchId }, "Match resumed successfully."));
    }

    /// <summary>
    /// Updates the score of an in-progress live match.
    /// </summary>
    [HttpPost("~/api/v1/live/matches/{matchId:guid}/score")]
    [Authorize(Roles = "System Admin,Academy Admin,Tournament Manager,Coach")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateScore(
        Guid matchId,
        [FromBody] UpdateLiveScoreRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating live score for match: {MatchId}", matchId);

        var result = await _mediator.Send(new UpdateLiveScoreCommand
        {
            MatchId = matchId,
            ParticipantId = request.ParticipantId,
            Points = request.Points,
            Unit = request.Unit,
            PeriodNumber = request.PeriodNumber,
            Description = request.Description
        }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<object>.SuccessResult(new { MatchId = matchId }, "Score updated successfully."));
    }

    /// <summary>
    /// Undoes the last score event for a live match.
    /// </summary>
    [HttpPost("~/api/v1/live/matches/{matchId:guid}/undo")]
    [Authorize(Roles = "System Admin,Academy Admin,Tournament Manager,Coach")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UndoScore(Guid matchId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Undoing score for live match: {MatchId}", matchId);

        var result = await _mediator.Send(new UndoScoreCommand { MatchId = matchId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<object>.SuccessResult(new { MatchId = matchId }, "Score undone successfully."));
    }

    /// <summary>
    /// Completes a live match.
    /// </summary>
    [HttpPost("~/api/v1/live/matches/{matchId:guid}/complete")]
    [Authorize(Roles = "System Admin,Academy Admin,Tournament Manager")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CompleteMatch(
        Guid matchId,
        [FromBody] CompleteLiveMatchRequest? request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Completing live match: {MatchId}", matchId);

        var result = await _mediator.Send(new CompleteMatchCommand
        {
            MatchId = matchId,
            WinnerId = request?.WinnerId,
            WinnerName = request?.WinnerName
        }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<object>.SuccessResult(new { MatchId = matchId }, "Match completed successfully."));
    }

    /// <summary>
    /// Records a walkover for a live match.
    /// </summary>
    [HttpPost("~/api/v1/live/matches/{matchId:guid}/walkover")]
    [Authorize(Roles = "System Admin,Academy Admin,Tournament Manager")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RecordWalkover(
        Guid matchId,
        [FromBody] LiveScoringWalkoverRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Recording walkover for live match: {MatchId}", matchId);

        var result = await _mediator.Send(new RecordWalkoverCommand
        {
            MatchId = matchId,
            WinnerId = request.WinnerId,
            WinnerName = request.WinnerName
        }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<object>.SuccessResult(new { MatchId = matchId }, "Walkover recorded successfully."));
    }

    /// <summary>
    /// Records a forfeit for a live match.
    /// </summary>
    [HttpPost("~/api/v1/live/matches/{matchId:guid}/forfeit")]
    [Authorize(Roles = "System Admin,Academy Admin,Tournament Manager")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RecordForfeit(
        Guid matchId,
        [FromBody] LiveScoringForfeitRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Recording forfeit for live match: {MatchId}", matchId);

        var result = await _mediator.Send(new RecordForfeitCommand
        {
            MatchId = matchId,
            WinnerId = request.WinnerId,
            WinnerName = request.WinnerName
        }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<object>.SuccessResult(new { MatchId = matchId }, "Forfeit recorded successfully."));
    }

    /// <summary>
    /// Gets live score for a specific match.
    /// </summary>
    [HttpGet("~/api/v1/live/matches/{matchId:guid}/score")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<LiveScoreDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetLiveScore(Guid matchId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting live score for match: {MatchId}", matchId);

        var result = await _mediator.Send(new LiveScoreQuery { MatchId = matchId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<LiveScoreDto>.SuccessResult(result.Value!, "Live score retrieved successfully."));
    }

    /// <summary>
    /// Gets the leaderboard for a tournament.
    /// </summary>
    [HttpGet("~/api/v1/live/tournaments/{tournamentId:guid}/leaderboard")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<LeaderboardDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLeaderboard(
        Guid tournamentId,
        [FromQuery] LeaderboardType type = LeaderboardType.Tournament,
        [FromQuery] string? sportCode = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting leaderboard for tournament: {TournamentId}", tournamentId);

        var result = await _mediator.Send(new LeaderboardQuery
        {
            TournamentId = tournamentId,
            Type = type,
            SportCode = sportCode
        }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<LeaderboardDto>.SuccessResult(result.Value!, "Leaderboard retrieved successfully."));
    }

    /// <summary>
    /// Generates a leaderboard for a tournament.
    /// </summary>
    [HttpPost("~/api/v1/live/tournaments/{tournamentId:guid}/leaderboard")]
    [Authorize(Roles = "System Admin,Academy Admin,Tournament Manager")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GenerateLeaderboard(
        Guid tournamentId,
        [FromBody] GenerateLeaderboardRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Generating leaderboard for tournament: {TournamentId}", tournamentId);

        var result = await _mediator.Send(new GenerateLeaderboardCommand
        {
            TournamentId = tournamentId,
            Type = request.Type,
            SportCode = request.SportCode
        }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<object>.SuccessResult(new { TournamentId = tournamentId }, "Leaderboard generated successfully."));
    }

    /// <summary>
    /// Gets tournament standings.
    /// </summary>
    [HttpGet("~/api/v1/live/tournaments/{tournamentId:guid}/standings")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<StandingsDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStandings(
        Guid tournamentId,
        [FromQuery] string? sportCode = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting standings for tournament: {TournamentId}", tournamentId);

        var result = await _mediator.Send(new TournamentStandingsQuery
        {
            TournamentId = tournamentId,
            SportCode = sportCode
        }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<StandingsDto>.SuccessResult(result.Value!, "Standings retrieved successfully."));
    }

    /// <summary>
    /// Gets the medal table for a tournament.
    /// </summary>
    [HttpGet("~/api/v1/live/tournaments/{tournamentId:guid}/medals")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<MedalTableDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMedalTable(
        Guid tournamentId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting medal table for tournament: {TournamentId}", tournamentId);

        var result = await _mediator.Send(new MedalTableQuery { TournamentId = tournamentId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<MedalTableDto>.SuccessResult(result.Value!, "Medal table retrieved successfully."));
    }

    /// <summary>
    /// Gets match statistics.
    /// </summary>
    [HttpGet("~/api/v1/live/matches/{matchId:guid}/statistics")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<MatchStatisticsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMatchStatistics(Guid matchId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting statistics for match: {MatchId}", matchId);

        var result = await _mediator.Send(new MatchStatisticsQuery { MatchId = matchId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<MatchStatisticsDto>.SuccessResult(result.Value!, "Match statistics retrieved successfully."));
    }

    /// <summary>
    /// Gets player statistics for a tournament.
    /// </summary>
    [HttpGet("~/api/v1/live/tournaments/{tournamentId:guid}/players/{playerId:guid}/statistics")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<PlayerStatisticsDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPlayerStatistics(
        Guid tournamentId,
        Guid playerId,
        [FromQuery] string? sportCode = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting player statistics: {PlayerId} in tournament {TournamentId}", playerId, tournamentId);

        var result = await _mediator.Send(new PlayerStatisticsQuery
        {
            TournamentId = tournamentId,
            PlayerId = playerId,
            SportCode = sportCode
        }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<PlayerStatisticsDto>.SuccessResult(result.Value!, "Player statistics retrieved successfully."));
    }

    /// <summary>
    /// Publishes results for a completed match.
    /// </summary>
    [HttpPost("~/api/v1/live/tournaments/{tournamentId:guid}/matches/{matchId:guid}/publish")]
    [Authorize(Roles = "System Admin,Academy Admin,Tournament Manager")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> PublishResults(
        Guid tournamentId,
        Guid matchId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Publishing results for match {MatchId} in tournament {TournamentId}", matchId, tournamentId);

        var result = await _mediator.Send(new PublishResultsCommand
        {
            TournamentId = tournamentId,
            MatchId = matchId
        }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<object>.SuccessResult(new { TournamentId = tournamentId, MatchId = matchId }, "Results published successfully."));
    }

    #region Helpers

    private IActionResult HandleFailure(string error)
    {
        if (error.Contains("not found", StringComparison.OrdinalIgnoreCase))
            return NotFound(new ProblemDetails { Status = StatusCodes.Status404NotFound, Title = "Not Found", Detail = error, Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4" });
        if (error.Contains("already", StringComparison.OrdinalIgnoreCase) || error.Contains("conflict", StringComparison.OrdinalIgnoreCase))
            return Conflict(new ProblemDetails { Status = StatusCodes.Status409Conflict, Title = "Conflict", Detail = error, Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8" });
        return BadRequest(new ProblemDetails { Status = StatusCodes.Status400BadRequest, Title = "Bad Request", Detail = error, Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3" });
    }

    #endregion
}

public record UpdateLiveScoreRequest(Guid ParticipantId, int Points, ScoringUnit Unit, int PeriodNumber, string? Description);
public record CompleteLiveMatchRequest(Guid? WinnerId, string? WinnerName);
public record GenerateLeaderboardRequest(LeaderboardType Type, string? SportCode);
public record StartLiveMatchRequest(Guid TournamentId, string SportCode, Guid HomeParticipantId, string HomeParticipantName, Guid AwayParticipantId, string AwayParticipantName);
public record LiveScoringWalkoverRequest(Guid WinnerId, string WinnerName);
public record LiveScoringForfeitRequest(Guid WinnerId, string WinnerName);
