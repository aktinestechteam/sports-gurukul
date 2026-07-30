using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.Commands.Preference;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;
using SportsGurukul.Application.Features.NotificationManagement.Queries;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Api.Controllers.V1;

[ApiController]
[Route("api/v1/preferences")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
[EnableRateLimiting("default")]
[Tags("Preferences")]
public class PreferencesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<PreferencesController> _logger;

    public PreferencesController(IMediator mediator, ILogger<PreferencesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<PreferenceDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPreferences(CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Unauthorized",
                Detail = "User identifier not found in token.",
                Type = "https://tools.ietf.org/html/rfc7235#section-3.1"
            });

        _logger.LogInformation("Fetching preferences for user: {UserId}", userId);

        var result = await _mediator.Send(new PreferenceQuery(userId), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<List<PreferenceDto>>.SuccessResult(result.Value!, "Preferences retrieved successfully."));
    }

    [HttpPut]
    [ProducesResponseType(typeof(ApiResponse<PreferenceDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdatePreference(
        [FromBody] UpdatePreferenceCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating preference for user: {UserId}, channel: {ChannelType}",
            command.UserId, command.ChannelType);

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<PreferenceDto>.SuccessResult(result.Value!, "Preference updated successfully."));
    }

    [HttpPost("subscribe")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Subscribe(
        [FromBody] SubscribeCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("User {UserId} subscribing to {EventType} for {EntityType}:{EntityId}",
            command.UserId, command.EventType, command.EntityType, command.EntityId);

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<object>.SuccessResult(new { }, "Subscribed successfully."));
    }

    [HttpPost("unsubscribe")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Unsubscribe(
        [FromBody] UnsubscribeCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("User {UserId} unsubscribing from {EventType} for {EntityType}:{EntityId}",
            command.UserId, command.EventType, command.EntityType, command.EntityId);

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<object>.SuccessResult(new { }, "Unsubscribed successfully."));
    }

    [HttpPost("mute")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Mute(
        [FromBody] MuteChannelCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Muting channel {ChannelType} for user {UserId}",
            command.ChannelType, command.UserId);

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<object>.SuccessResult(new { }, "Channel muted successfully."));
    }

    [HttpPost("unmute")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Unmute(
        [FromBody] UnmuteChannelCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Unmuting channel {ChannelType} for user {UserId}",
            command.ChannelType, command.UserId);

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<object>.SuccessResult(new { }, "Channel unmuted successfully."));
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
            error.Contains("only", StringComparison.OrdinalIgnoreCase) ||
            error.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
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
