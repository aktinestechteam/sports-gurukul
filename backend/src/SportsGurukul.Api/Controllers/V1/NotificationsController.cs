using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.Commands.Notification;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;
using SportsGurukul.Application.Features.NotificationManagement.Queries;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Api.Controllers.V1;

[ApiController]
[Route("api/v1/notifications")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
[EnableRateLimiting("default")]
[Tags("Notifications")]
public class NotificationsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<NotificationsController> _logger;

    public NotificationsController(IMediator mediator, ILogger<NotificationsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    [Authorize(Roles = "Admin,SuperAdmin,Communication Admin,Academy Admin")]
    [ProducesResponseType(typeof(ApiResponse<NotificationDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create(
        [FromBody] CreateNotificationCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating notification with priority {Priority}", command.Priority);

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Value!.Id, version = "1.0" },
            ApiResponse<NotificationDto>.SuccessResult(result.Value, "Notification created successfully."));
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<NotificationSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Search(
        [FromQuery] string? searchTerm,
        [FromQuery] string? status,
        [FromQuery] string? priority,
        [FromQuery] Guid? channelId,
        [FromQuery] Guid? batchId,
        [FromQuery] Guid? campaignId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Searching notifications");

        NotificationStatus? parsedStatus = null;
        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<NotificationStatus>(status, true, out var ps))
            parsedStatus = ps;

        NotificationPriority? parsedPriority = null;
        if (!string.IsNullOrWhiteSpace(priority) && Enum.TryParse<NotificationPriority>(priority, true, out var pp))
            parsedPriority = pp;

        var query = new SearchNotificationsQuery(
            searchTerm, parsedStatus, parsedPriority, channelId, null, batchId, campaignId,
            fromDate, toDate, page, pageSize);

        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<PaginatedResult<NotificationSummaryDto>>.SuccessResult(
            result.Value!, "Notifications retrieved successfully."));
    }

    [HttpGet("search")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<NotificationSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SearchAlias(
        [FromQuery] string? searchTerm,
        [FromQuery] string? status,
        [FromQuery] string? priority,
        [FromQuery] Guid? channelId,
        [FromQuery] Guid? batchId,
        [FromQuery] Guid? campaignId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Searching notifications (alias)");

        NotificationStatus? parsedStatus = null;
        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<NotificationStatus>(status, true, out var ps))
            parsedStatus = ps;

        NotificationPriority? parsedPriority = null;
        if (!string.IsNullOrWhiteSpace(priority) && Enum.TryParse<NotificationPriority>(priority, true, out var pp))
            parsedPriority = pp;

        var query = new SearchNotificationsQuery(
            searchTerm, parsedStatus, parsedPriority, channelId, null, batchId, campaignId,
            fromDate, toDate, page, pageSize);

        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<PaginatedResult<NotificationSummaryDto>>.SuccessResult(
            result.Value!, "Notifications retrieved successfully."));
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<NotificationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching notification: {NotificationId}", id);

        var result = await _mediator.Send(new GetNotificationQuery(id), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<NotificationDto>.SuccessResult(result.Value!, "Notification retrieved successfully."));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,SuperAdmin,Communication Admin,Academy Admin")]
    [ProducesResponseType(typeof(ApiResponse<NotificationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateNotificationCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating notification: {NotificationId}", id);

        var cmd = command with { Id = id };
        var result = await _mediator.Send(cmd, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<NotificationDto>.SuccessResult(result.Value!, "Notification updated successfully."));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin,SuperAdmin,Communication Admin")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting notification: {NotificationId}", id);

        var result = await _mediator.Send(new DeleteNotificationCommand(id), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<object>.SuccessResult(new { id }, "Notification deleted successfully."));
    }

    [HttpPost("{id:guid}/queue")]
    [Authorize(Roles = "Admin,SuperAdmin,Communication Admin")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Queue(
        Guid id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Queueing notification: {NotificationId}", id);

        var result = await _mediator.Send(new QueueNotificationCommand(id), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<object>.SuccessResult(new { id }, "Notification queued successfully."));
    }

    [HttpPost("{id:guid}/send")]
    [Authorize(Roles = "Admin,SuperAdmin,Communication Admin")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Send(
        Guid id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sending notification: {NotificationId}", id);

        var result = await _mediator.Send(new SendNotificationCommand(id), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<object>.SuccessResult(new { id }, "Notification send initiated successfully."));
    }

    [HttpPost("{id:guid}/schedule")]
    [Authorize(Roles = "Admin,SuperAdmin,Communication Admin,Academy Admin")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Schedule(
        Guid id,
        [FromBody] ScheduleRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Scheduling notification: {NotificationId} at {ScheduledAt}", id, request.ScheduledAt);

        var result = await _mediator.Send(new ScheduleNotificationCommand(id, request.ScheduledAt), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<object>.SuccessResult(new { id, request.ScheduledAt }, "Notification scheduled successfully."));
    }

    [HttpPost("{id:guid}/cancel")]
    [Authorize(Roles = "Admin,SuperAdmin,Communication Admin,Academy Admin")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel(
        Guid id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Cancelling notification: {NotificationId}", id);

        var result = await _mediator.Send(new CancelNotificationCommand(id), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<object>.SuccessResult(new { id }, "Notification cancelled successfully."));
    }

    [HttpPost("{id:guid}/retry")]
    [Authorize(Roles = "Admin,SuperAdmin,Communication Admin")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Retry(
        Guid id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrying notification: {NotificationId}", id);

        var result = await _mediator.Send(new RetryNotificationCommand(id), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<object>.SuccessResult(new { id }, "Notification retry initiated successfully."));
    }

    [HttpPost("{id:guid}/read")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkRead(
        Guid id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Marking notification as read: {NotificationId}", id);

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var parsedUserId = userId is not null ? Guid.Parse(userId) : (Guid?)null;

        var result = await _mediator.Send(new MarkNotificationReadCommand(id, parsedUserId), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<object>.SuccessResult(new { id }, "Notification marked as read."));
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

public record ScheduleRequest(DateTime ScheduledAt);
