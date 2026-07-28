using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.EventManagement.Commands.ApproveRegistration;
using SportsGurukul.Application.Features.EventManagement.Commands.CancelRegistration;
using SportsGurukul.Application.Features.EventManagement.Commands.CloseRegistration;
using SportsGurukul.Application.Features.EventManagement.Commands.MoveFromWaitlist;
using SportsGurukul.Application.Features.EventManagement.Commands.OpenRegistration;
using SportsGurukul.Application.Features.EventManagement.Commands.RegisterParticipant;
using SportsGurukul.Application.Features.EventManagement.Commands.RejectRegistration;
using SportsGurukul.Application.Features.EventManagement.DTOs;
using SportsGurukul.Application.Features.EventManagement.Queries.GetRegistrationsByEvent;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Api.Controllers.V1;

/// <summary>
/// Manages event registrations — participant registration, approval, rejection, waitlist, and registration windows.
/// </summary>
[ApiController]
[Route("api/v1/events/{eventId:guid}/registrations")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
[Tags("Event Registrations")]
public class EventRegistrationsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<EventRegistrationsController> _logger;

    public EventRegistrationsController(IMediator mediator, ILogger<EventRegistrationsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Registers a participant for an event.
    /// </summary>
    /// <param name="eventId">The event identifier.</param>
    /// <param name="command">The registration request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created registration.</returns>
    [HttpPost]
    [Authorize(Roles = "Admin,Academy Admin,Athlete")]
    [ProducesResponseType(typeof(ApiResponse<RegistrationDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegisterParticipant(
        Guid eventId,
        [FromBody] RegisterParticipantCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Registering participant for event: {EventId}", eventId);

        command.EventId = eventId;

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Participant registered: {RegistrationId}", result.Value!.Id);

        return CreatedAtAction(
            null,
            new { eventId, version = "1.0" },
            ApiResponse<RegistrationDto>.SuccessResult(result.Value, "Participant registered successfully."));
    }

    /// <summary>
    /// Gets registrations for an event with optional status filter and pagination.
    /// </summary>
    /// <param name="eventId">The event identifier.</param>
    /// <param name="status">Filter by registration status.</param>
    /// <param name="page">Page number.</param>
    /// <param name="pageSize">Page size.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paged list of registrations.</returns>
    [HttpGet]
    [Authorize(Roles = "Admin,Academy Admin,Event Manager")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<RegistrationDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRegistrationsByEvent(
        Guid eventId,
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching registrations for event: {EventId}", eventId);

        EventRegistrationStatus? registrationStatus = null;
        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<EventRegistrationStatus>(status, true, out var parsedStatus))
            registrationStatus = parsedStatus;

        var query = new GetRegistrationsByEventQuery
        {
            EventId = eventId,
            Status = registrationStatus,
            Page = page,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<PagedResult<RegistrationDto>>.SuccessResult(result.Value!, "Registrations retrieved successfully."));
    }

    /// <summary>
    /// Cancels a registration.
    /// </summary>
    /// <param name="eventId">The event identifier.</param>
    /// <param name="registrationId">The registration identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The cancelled registration.</returns>
    [HttpDelete("{registrationId:guid}")]
    [Authorize(Roles = "Admin,Academy Admin,Event Manager,Athlete")]
    [ProducesResponseType(typeof(ApiResponse<RegistrationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelRegistration(
        Guid eventId,
        Guid registrationId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Cancelling registration: {RegistrationId} for event: {EventId}", registrationId, eventId);

        var result = await _mediator.Send(new CancelRegistrationCommand { RegistrationId = registrationId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Registration cancelled: {RegistrationId}", registrationId);

        return Ok(ApiResponse<RegistrationDto>.SuccessResult(result.Value!, "Registration cancelled successfully."));
    }

    /// <summary>
    /// Approves a pending registration.
    /// </summary>
    /// <param name="eventId">The event identifier.</param>
    /// <param name="registrationId">The registration identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The approved registration.</returns>
    [HttpPost("{registrationId:guid}/approve")]
    [Authorize(Roles = "Admin,Academy Admin,Event Manager")]
    [ProducesResponseType(typeof(ApiResponse<RegistrationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ApproveRegistration(
        Guid eventId,
        Guid registrationId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Approving registration: {RegistrationId} for event: {EventId}", registrationId, eventId);

        var result = await _mediator.Send(new ApproveRegistrationCommand { RegistrationId = registrationId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Registration approved: {RegistrationId}", registrationId);

        return Ok(ApiResponse<RegistrationDto>.SuccessResult(result.Value!, "Registration approved successfully."));
    }

    /// <summary>
    /// Rejects a pending registration.
    /// </summary>
    /// <param name="eventId">The event identifier.</param>
    /// <param name="registrationId">The registration identifier.</param>
    /// <param name="request">Optional rejection reason.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The rejected registration.</returns>
    [HttpPost("{registrationId:guid}/reject")]
    [Authorize(Roles = "Admin,Academy Admin,Event Manager")]
    [ProducesResponseType(typeof(ApiResponse<RegistrationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RejectRegistration(
        Guid eventId,
        Guid registrationId,
        [FromBody] RejectEventRegistrationRequest? request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Rejecting registration: {RegistrationId} for event: {EventId}", registrationId, eventId);

        var command = new RejectRegistrationCommand
        {
            RegistrationId = registrationId,
            Reason = request?.Reason
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Registration rejected: {RegistrationId}", registrationId);

        return Ok(ApiResponse<RegistrationDto>.SuccessResult(result.Value!, "Registration rejected successfully."));
    }

    /// <summary>
    /// Promotes a waitlisted registration to approved.
    /// </summary>
    /// <param name="eventId">The event identifier.</param>
    /// <param name="registrationId">The registration identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The promoted registration.</returns>
    [HttpPost("{registrationId:guid}/waitlist/promote")]
    [Authorize(Roles = "Admin,Academy Admin,Event Manager")]
    [ProducesResponseType(typeof(ApiResponse<RegistrationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MoveFromWaitlist(
        Guid eventId,
        Guid registrationId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Promoting from waitlist: {RegistrationId} for event: {EventId}", registrationId, eventId);

        var result = await _mediator.Send(new MoveFromWaitlistCommand { RegistrationId = registrationId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Registration promoted from waitlist: {RegistrationId}", registrationId);

        return Ok(ApiResponse<RegistrationDto>.SuccessResult(result.Value!, "Registration promoted from waitlist successfully."));
    }

    /// <summary>
    /// Opens registration for an event.
    /// </summary>
    /// <param name="eventId">The event identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated event.</returns>
    [HttpPost("open")]
    [Authorize(Roles = "Admin,Academy Admin,Event Manager")]
    [ProducesResponseType(typeof(ApiResponse<EventDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> OpenRegistration(
        Guid eventId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Opening registration for event: {EventId}", eventId);

        var result = await _mediator.Send(new OpenRegistrationCommand { EventId = eventId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Registration opened for event: {EventId}", eventId);

        return Ok(ApiResponse<EventDto>.SuccessResult(result.Value!, "Registration opened successfully."));
    }

    /// <summary>
    /// Closes registration for an event.
    /// </summary>
    /// <param name="eventId">The event identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated event.</returns>
    [HttpPost("close")]
    [Authorize(Roles = "Admin,Academy Admin,Event Manager")]
    [ProducesResponseType(typeof(ApiResponse<EventDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CloseRegistration(
        Guid eventId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Closing registration for event: {EventId}", eventId);

        var result = await _mediator.Send(new CloseRegistrationCommand { EventId = eventId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Registration closed for event: {EventId}", eventId);

        return Ok(ApiResponse<EventDto>.SuccessResult(result.Value!, "Registration closed successfully."));
    }

    #region Helpers

    private IActionResult HandleFailure(string error)
    {
        if (error.Contains("not found", StringComparison.OrdinalIgnoreCase))
            return NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Not Found",
                Detail = error,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4"
            });

        if (error.Contains("already exists", StringComparison.OrdinalIgnoreCase) ||
            error.Contains("already", StringComparison.OrdinalIgnoreCase) ||
            error.Contains("conflict", StringComparison.OrdinalIgnoreCase))
            return Conflict(new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Conflict",
                Detail = error,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8"
            });

        if (error.Contains("cannot", StringComparison.OrdinalIgnoreCase) ||
            error.Contains("invalid", StringComparison.OrdinalIgnoreCase) ||
            error.Contains("must", StringComparison.OrdinalIgnoreCase) ||
            error.Contains("no eligible", StringComparison.OrdinalIgnoreCase) ||
            error.Contains("not eligible", StringComparison.OrdinalIgnoreCase) ||
            error.Contains("only", StringComparison.OrdinalIgnoreCase) ||
            error.Contains("duplicate", StringComparison.OrdinalIgnoreCase) ||
            error.Contains("capacity", StringComparison.OrdinalIgnoreCase) ||
            error.Contains("registration", StringComparison.OrdinalIgnoreCase))
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Bad Request",
                Detail = error,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
            });

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

public record RejectEventRegistrationRequest(string? Reason);
