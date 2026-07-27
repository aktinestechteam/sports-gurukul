using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.TournamentManagement.Commands.ArchiveTournament;
using SportsGurukul.Application.Features.TournamentManagement.Commands.CancelTournament;
using SportsGurukul.Application.Features.TournamentManagement.Commands.CloseRegistration;
using SportsGurukul.Application.Features.TournamentManagement.Commands.CreateTournament;
using SportsGurukul.Application.Features.TournamentManagement.Commands.OpenRegistration;
using SportsGurukul.Application.Features.TournamentManagement.Commands.PublishTournament;
using SportsGurukul.Application.Features.TournamentManagement.Commands.UpdateTournament;
using SportsGurukul.Application.Features.TournamentManagement.DTOs;
using SportsGurukul.Application.Features.TournamentManagement.Queries.GetTournamentById;
using SportsGurukul.Application.Features.TournamentManagement.Queries.SearchTournaments;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Api.Controllers.V1;

/// <summary>
/// Manages tournament lifecycle — creation, publishing, registration windows, archival, and search.
/// </summary>
[ApiController]
[Route("api/v1/tournaments")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
[Tags("Tournaments")]
public class TournamentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TournamentsController> _logger;

    public TournamentsController(IMediator mediator, ILogger<TournamentsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new tournament.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "System Admin,Academy Admin,Tournament Manager")]
    [ProducesResponseType(typeof(ApiResponse<TournamentDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateTournament(
        [FromBody] CreateTournamentCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating tournament: {TournamentName}", request.TournamentName);

        var result = await _mediator.Send(request, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Tournament created: {TournamentId}", result.Value!.Id);

        return CreatedAtAction(
            nameof(GetTournamentById),
            new { id = result.Value.Id, version = "1.0" },
            ApiResponse<TournamentDto>.SuccessResult(result.Value, "Tournament created successfully."));
    }

    /// <summary>
    /// Searches tournaments with filtering and pagination.
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<TournamentSearchResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SearchTournaments(
        [FromQuery] Guid? academyId,
        [FromQuery] TournamentStatus? status,
        [FromQuery] TournamentType? tournamentType,
        [FromQuery] string? searchTerm,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Searching tournaments - Page: {Page}, PageSize: {PageSize}", page, pageSize);

        var query = new SearchTournamentsQuery
        {
            AcademyId = academyId,
            Status = status,
            TournamentType = tournamentType,
            SearchTerm = searchTerm,
            Page = page,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<TournamentSearchResponse>.SuccessResult(result.Value!, "Tournaments retrieved successfully."));
    }

    /// <summary>
    /// Searches tournaments (explicit /search route alias).
    /// </summary>
    [HttpGet("search")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<TournamentSearchResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SearchTournamentsAlias(
        [FromQuery] Guid? academyId,
        [FromQuery] TournamentStatus? status,
        [FromQuery] TournamentType? tournamentType,
        [FromQuery] string? searchTerm,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new SearchTournamentsQuery
        {
            AcademyId = academyId,
            Status = status,
            TournamentType = tournamentType,
            SearchTerm = searchTerm,
            Page = page,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<TournamentSearchResponse>.SuccessResult(result.Value!, "Tournaments retrieved successfully."));
    }

    /// <summary>
    /// Gets a specific tournament by its unique identifier.
    /// </summary>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<TournamentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTournamentById(
        Guid id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching tournament: {TournamentId}", id);

        var result = await _mediator.Send(new GetTournamentByIdQuery { TournamentId = id }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<TournamentDto>.SuccessResult(result.Value!, "Tournament retrieved successfully."));
    }

    /// <summary>
    /// Updates a tournament. Only editable in Draft status.
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "System Admin,Academy Admin,Tournament Manager")]
    [ProducesResponseType(typeof(ApiResponse<TournamentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateTournament(
        Guid id,
        [FromBody] UpdateTournamentCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating tournament: {TournamentId}", id);

        request.TournamentId = id;

        var result = await _mediator.Send(request, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Tournament updated: {TournamentId}", id);

        return Ok(ApiResponse<TournamentDto>.SuccessResult(result.Value!, "Tournament updated successfully."));
    }

    /// <summary>
    /// Soft-deletes a tournament.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "System Admin")]
    [ProducesResponseType(typeof(ApiResponse<TournamentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTournament(
        Guid id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting tournament: {TournamentId}", id);

        var result = await _mediator.Send(new CancelTournamentCommand { TournamentId = id }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Tournament deleted: {TournamentId}", id);

        return Ok(ApiResponse<object>.SuccessResult(new { TournamentId = id }, "Tournament deleted successfully."));
    }

    /// <summary>
    /// Publishes a tournament, transitioning it from Draft.
    /// </summary>
    [HttpPost("{id:guid}/publish")]
    [Authorize(Roles = "System Admin,Academy Admin,Tournament Manager")]
    [ProducesResponseType(typeof(ApiResponse<TournamentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PublishTournament(
        Guid id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Publishing tournament: {TournamentId}", id);

        var result = await _mediator.Send(new PublishTournamentCommand { TournamentId = id }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Tournament published: {TournamentId}", id);

        return Ok(ApiResponse<TournamentDto>.SuccessResult(result.Value!, "Tournament published successfully."));
    }

    /// <summary>
    /// Opens registration for a published tournament.
    /// </summary>
    [HttpPost("{id:guid}/registration/open")]
    [Authorize(Roles = "System Admin,Academy Admin,Tournament Manager")]
    [ProducesResponseType(typeof(ApiResponse<TournamentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> OpenRegistration(
        Guid id,
        [FromBody] OpenRegistrationRequest? request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Opening registration for tournament: {TournamentId}", id);

        var command = new OpenRegistrationCommand
        {
            TournamentId = id,
            RegistrationCloseDate = request?.RegistrationCloseDate
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Registration opened for tournament: {TournamentId}", id);

        return Ok(ApiResponse<TournamentDto>.SuccessResult(result.Value!, "Registration opened successfully."));
    }

    /// <summary>
    /// Closes registration for a tournament.
    /// </summary>
    [HttpPost("{id:guid}/registration/close")]
    [Authorize(Roles = "System Admin,Academy Admin,Tournament Manager")]
    [ProducesResponseType(typeof(ApiResponse<TournamentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CloseRegistration(
        Guid id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Closing registration for tournament: {TournamentId}", id);

        var result = await _mediator.Send(new CloseRegistrationCommand { TournamentId = id }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Registration closed for tournament: {TournamentId}", id);

        return Ok(ApiResponse<TournamentDto>.SuccessResult(result.Value!, "Registration closed successfully."));
    }

    /// <summary>
    /// Cancels a tournament.
    /// </summary>
    [HttpPost("{id:guid}/cancel")]
    [Authorize(Roles = "System Admin,Academy Admin,Tournament Manager")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelTournament(
        Guid id,
        [FromBody] CancelTournamentRequest? request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Cancelling tournament: {TournamentId}", id);

        var command = new CancelTournamentCommand
        {
            TournamentId = id,
            Reason = request?.Reason
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Tournament cancelled: {TournamentId}", id);

        return Ok(ApiResponse<object>.SuccessResult(new { TournamentId = id }, "Tournament cancelled successfully."));
    }

    /// <summary>
    /// Archives a completed tournament.
    /// </summary>
    [HttpPost("{id:guid}/archive")]
    [Authorize(Roles = "System Admin,Academy Admin,Tournament Manager")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ArchiveTournament(
        Guid id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Archiving tournament: {TournamentId}", id);

        var result = await _mediator.Send(new ArchiveTournamentCommand { TournamentId = id }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Tournament archived: {TournamentId}", id);

        return Ok(ApiResponse<object>.SuccessResult(new { TournamentId = id }, "Tournament archived successfully."));
    }

    #region Helpers

    private IActionResult HandleFailure(string error)
    {
        if (error.Contains("not found", StringComparison.OrdinalIgnoreCase))
            return NotFound(new ProblemDetails { Status = StatusCodes.Status404NotFound, Title = "Not Found", Detail = error, Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4" });
        if (error.Contains("already exists", StringComparison.OrdinalIgnoreCase) || error.Contains("already", StringComparison.OrdinalIgnoreCase) || error.Contains("conflict", StringComparison.OrdinalIgnoreCase))
            return Conflict(new ProblemDetails { Status = StatusCodes.Status409Conflict, Title = "Conflict", Detail = error, Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8" });
        if (error.Contains("only be", StringComparison.OrdinalIgnoreCase) || error.Contains("cannot", StringComparison.OrdinalIgnoreCase) || error.Contains("must be", StringComparison.OrdinalIgnoreCase) || error.Contains("invalid", StringComparison.OrdinalIgnoreCase))
            return BadRequest(new ProblemDetails { Status = StatusCodes.Status400BadRequest, Title = "Bad Request", Detail = error, Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1" });
        return BadRequest(new ProblemDetails { Status = StatusCodes.Status400BadRequest, Title = "Bad Request", Detail = error, Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1" });
    }

    #endregion
}

public record OpenRegistrationRequest(DateTime? RegistrationCloseDate);
public record CancelTournamentRequest(string? Reason);
