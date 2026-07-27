using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.TournamentManagement.Commands.ApproveRegistration;
using SportsGurukul.Application.Features.TournamentManagement.Commands.RejectRegistration;
using SportsGurukul.Application.Features.TournamentManagement.Commands.RegisterParticipant;
using SportsGurukul.Application.Features.TournamentManagement.Commands.WithdrawParticipant;
using SportsGurukul.Application.Features.TournamentManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Api.Controllers.V1;

/// <summary>
/// Manages tournament registrations — participant registration, approval, rejection, and withdrawal.
/// </summary>
[ApiController]
[Route("api/v1/tournaments/{id:guid}/registrations")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
[Tags("Tournament Registrations")]
public class TournamentRegistrationsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TournamentRegistrationsController> _logger;

    public TournamentRegistrationsController(IMediator mediator, ILogger<TournamentRegistrationsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Registers a participant for a tournament.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "System Admin,Academy Admin,Tournament Manager,Coach,Athlete")]
    [ProducesResponseType(typeof(ApiResponse<ParticipantDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegisterParticipant(
        Guid id,
        [FromBody] RegisterParticipantRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Registering participant for tournament: {TournamentId}", id);

        var command = new RegisterParticipantCommand
        {
            TournamentId = id,
            CategoryId = request.CategoryId,
            ParticipantType = request.ParticipantType,
            AthleteId = request.AthleteId,
            TeamId = request.TeamId,
            AcademyId = request.AcademyId,
            RegistrantName = request.RegistrantName,
            Email = request.Email,
            Phone = request.Phone,
            Notes = request.Notes
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Participant registered: {ParticipantId}", result.Value!.Id);

        return CreatedAtAction(
            null,
            new { id, version = "1.0" },
            ApiResponse<ParticipantDto>.SuccessResult(result.Value, "Participant registered successfully."));
    }

    /// <summary>
    /// Withdraws a participant from a tournament.
    /// </summary>
    [HttpDelete("{registrationId:guid}")]
    [Authorize(Roles = "System Admin,Academy Admin,Tournament Manager,Coach,Athlete")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> WithdrawParticipant(
        Guid id,
        Guid registrationId,
        [FromQuery] string? reason,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Withdrawing participant: {RegistrationId} from tournament: {TournamentId}", registrationId, id);

        var command = new WithdrawParticipantCommand
        {
            TournamentId = id,
            ParticipantId = registrationId,
            Reason = reason
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Participant withdrawn: {RegistrationId}", registrationId);

        return Ok(ApiResponse<object>.SuccessResult(new { RegistrationId = registrationId }, "Participant withdrawn successfully."));
    }

    /// <summary>
    /// Approves a pending registration.
    /// </summary>
    [HttpPost("{registrationId:guid}/approve")]
    [Authorize(Roles = "System Admin,Academy Admin,Tournament Manager")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ApproveRegistration(
        Guid id,
        Guid registrationId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Approving registration: {RegistrationId} for tournament: {TournamentId}", registrationId, id);

        var result = await _mediator.Send(new ApproveRegistrationCommand { RegistrationId = registrationId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Registration approved: {RegistrationId}", registrationId);

        return Ok(ApiResponse<object>.SuccessResult(new { RegistrationId = registrationId }, "Registration approved successfully."));
    }

    /// <summary>
    /// Rejects a pending registration.
    /// </summary>
    [HttpPost("{registrationId:guid}/reject")]
    [Authorize(Roles = "System Admin,Academy Admin,Tournament Manager")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RejectRegistration(
        Guid id,
        Guid registrationId,
        [FromBody] RejectRegistrationRequest? request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Rejecting registration: {RegistrationId} for tournament: {TournamentId}", registrationId, id);

        var command = new RejectRegistrationCommand
        {
            RegistrationId = registrationId,
            Reason = request?.Reason
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Registration rejected: {RegistrationId}", registrationId);

        return Ok(ApiResponse<object>.SuccessResult(new { RegistrationId = registrationId }, "Registration rejected successfully."));
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

public record RegisterParticipantRequest(
    Guid? CategoryId,
    TournamentParticipantType ParticipantType,
    Guid? AthleteId,
    Guid? TeamId,
    Guid? AcademyId,
    string RegistrantName,
    string? Email,
    string? Phone,
    string? Notes);

public record RejectRegistrationRequest(string? Reason);
