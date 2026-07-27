using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.TournamentManagement.Commands.GenerateFixtures;
using SportsGurukul.Application.Features.TournamentManagement.Commands.RegenerateFixtures;
using SportsGurukul.Application.Features.TournamentManagement.DTOs;
using SportsGurukul.Application.Features.TournamentManagement.Queries.GetTournamentFixtures;

namespace SportsGurukul.Api.Controllers.V1;

/// <summary>
/// Manages tournament fixture generation, regeneration, and retrieval.
/// </summary>
[ApiController]
[Route("api/v1/tournaments/{id:guid}/fixtures")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
[Tags("Tournament Fixtures")]
public class TournamentFixturesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TournamentFixturesController> _logger;

    public TournamentFixturesController(IMediator mediator, ILogger<TournamentFixturesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Generates fixtures for a tournament.
    /// </summary>
    [HttpPost("generate")]
    [Authorize(Roles = "System Admin,Academy Admin,Tournament Manager")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<FixtureDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GenerateFixtures(
        Guid id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Generating fixtures for tournament: {TournamentId}", id);

        var result = await _mediator.Send(new GenerateFixturesCommand { TournamentId = id }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Fixtures generated for tournament: {TournamentId}, Count: {Count}", id, result.Value!.Count);

        return Ok(ApiResponse<IReadOnlyList<FixtureDto>>.SuccessResult(result.Value, "Fixtures generated successfully."));
    }

    /// <summary>
    /// Regenerates fixtures for a tournament (must be in FixtureGeneration status).
    /// </summary>
    [HttpPost("regenerate")]
    [Authorize(Roles = "System Admin,Academy Admin,Tournament Manager")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<FixtureDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RegenerateFixtures(
        Guid id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Regenerating fixtures for tournament: {TournamentId}", id);

        var result = await _mediator.Send(new RegenerateFixturesCommand { TournamentId = id }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Fixtures regenerated for tournament: {TournamentId}, Count: {Count}", id, result.Value!.Count);

        return Ok(ApiResponse<IReadOnlyList<FixtureDto>>.SuccessResult(result.Value, "Fixtures regenerated successfully."));
    }

    /// <summary>
    /// Gets all fixtures for a tournament.
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<FixtureDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFixtures(
        Guid id,
        [FromQuery] Guid? stageId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching fixtures for tournament: {TournamentId}", id);

        var query = new GetTournamentFixturesQuery
        {
            TournamentId = id,
            StageId = stageId
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<IReadOnlyList<FixtureDto>>.SuccessResult(result.Value!, "Fixtures retrieved successfully."));
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
