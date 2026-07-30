using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;
using SportsGurukul.Application.Features.NotificationManagement.Queries;

namespace SportsGurukul.Api.Controllers.V1;

[ApiController]
[Route("api/v1/statistics")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
[Tags("Statistics")]
public class StatisticsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<StatisticsController> _logger;

    public StatisticsController(IMediator mediator, ILogger<StatisticsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("notifications")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<NotificationStatisticsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetNotificationStatistics(
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] Guid? channelId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching notification statistics");

        var result = await _mediator.Send(
            new NotificationStatisticsQuery(fromDate, toDate, channelId), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<NotificationStatisticsDto>.SuccessResult(
            result.Value!, "Notification statistics retrieved successfully."));
    }

    [HttpGet("delivery")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<NotificationStatisticsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetDeliveryStatistics(
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
