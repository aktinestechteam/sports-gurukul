using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.TournamentManagement.Commands.GenerateRankings;
using SportsGurukul.Application.Features.TournamentManagement.DTOs;
using SportsGurukul.Application.Features.TournamentManagement.Queries.GetTournamentRankings;

namespace SportsGurukul.Api.Controllers.V1;

/// <summary>
/// Manages tournament rankings - generation and retrieval.
/// </summary>
[ApiController]
[Route("api/v1/tournaments/{id:guid}/rankings")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
[Tags("Tournament Rankings")]
public class TournamentRankingsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TournamentRankingsController> _logger;

    public TournamentRankingsController(IMediator mediator, ILogger<TournamentRankingsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Gets rankings for a tournament.
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<RankingDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRankings(
        Guid id,
        [FromQuery] Guid? categoryId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching rankings for tournament: {TournamentId}", id);

        var query = new GetTournamentRankingsQuery
        {
            TournamentId = id,
            CategoryId = categoryId
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<IReadOnlyList<RankingDto>>.SuccessResult(result.Value!, "Rankings retrieved successfully."));
    }

    /// <summary>
    /// Generates rankings for a tournament.
    /// </summary>
    [HttpPost("generate")]
    [Authorize(Roles = "System Admin,Academy Admin,Tournament Manager")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<RankingDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GenerateRankings(
        Guid id,
        [FromQuery] Guid? categoryId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Generating rankings for tournament: {TournamentId}", id);

        var command = new GenerateRankingsCommand
        {
            TournamentId = id,
            CategoryId = categoryId
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Rankings generated for tournament: {TournamentId}, Count: {Count}", id, result.Value!.Count);

        return Ok(ApiResponse<IReadOnlyList<RankingDto>>.SuccessResult(result.Value, "Rankings generated successfully."));
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
