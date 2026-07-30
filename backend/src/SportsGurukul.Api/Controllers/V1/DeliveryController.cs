using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;
using SportsGurukul.Application.Features.NotificationManagement.Queries;

namespace SportsGurukul.Api.Controllers.V1;

[ApiController]
[Route("api/v1/delivery")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
[EnableRateLimiting("default")]
[Tags("Delivery")]
public class DeliveryController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<DeliveryController> _logger;

    public DeliveryController(IMediator mediator, ILogger<DeliveryController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,SuperAdmin,Communication Admin")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? notificationId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching delivery records for notification: {NotificationId}", notificationId);

        if (!notificationId.HasValue)
        {
            await Task.CompletedTask;
            return Ok(ApiResponse<object>.SuccessResult(new { },
                "Use GET /api/v1/delivery?notificationId={id} to fetch delivery status for a specific notification."));
        }

        var result = await _mediator.Send(new DeliveryStatusQuery(notificationId.Value), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<List<DeliveryDto>>.SuccessResult(result.Value!, "Delivery records retrieved successfully."));
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin,SuperAdmin,Communication Admin")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching delivery record: {DeliveryId}", id);
        await Task.CompletedTask;
        return Ok(ApiResponse<object>.SuccessResult(new { id }, "Delivery detail endpoint available. Use GET /api/v1/delivery?notificationId={id} for delivery list."));
    }

    [HttpGet("statistics")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<NotificationStatisticsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetStatistics(
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] Guid? channelId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching delivery statistics");

        var result = await _mediator.Send(
            new NotificationStatisticsQuery(fromDate, toDate, channelId), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<NotificationStatisticsDto>.SuccessResult(
            result.Value!, "Delivery statistics retrieved successfully."));
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
