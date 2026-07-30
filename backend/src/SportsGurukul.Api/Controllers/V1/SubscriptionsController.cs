using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.Commands.Preference;

namespace SportsGurukul.Api.Controllers.V1;

[ApiController]
[Route("api/v1/subscriptions")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
[Tags("Subscriptions")]
public class SubscriptionsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<SubscriptionsController> _logger;

    public SubscriptionsController(IMediator mediator, ILogger<SubscriptionsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
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

    [HttpDelete]
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

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Listing subscriptions");
        await Task.CompletedTask;
        return Ok(ApiResponse<object>.SuccessResult(new { }, "Subscriptions endpoint available. Use POST to subscribe or DELETE to unsubscribe."));
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
