using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.TournamentManagement.Commands.AwardMedals;
using SportsGurukul.Application.Features.TournamentManagement.DTOs;

namespace SportsGurukul.Api.Controllers.V1;

/// <summary>
/// Manages tournament awards and medal ceremonies.
/// </summary>
[ApiController]
[Route("api/v1/tournaments/{id:guid}/awards")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
[Tags("Tournament Awards")]
public class TournamentAwardsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TournamentAwardsController> _logger;

    public TournamentAwardsController(IMediator mediator, ILogger<TournamentAwardsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Awards medals (top 3) for a completed tournament.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "System Admin,Academy Admin,Tournament Manager")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<AwardDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AwardMedals(
        Guid id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Awarding medals for tournament: {TournamentId}", id);

        var result = await _mediator.Send(new AwardMedalsCommand { TournamentId = id }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Medals awarded for tournament: {TournamentId}, Count: {Count}", id, result.Value!.Count);

        return Ok(ApiResponse<IReadOnlyList<AwardDto>>.SuccessResult(result.Value, "Medals awarded successfully."));
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
