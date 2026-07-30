using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SportsGurukul.Api.Common.Models;

namespace SportsGurukul.Api.Controllers.V1;

[ApiController]
[Route("api/v1/queue")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
[EnableRateLimiting("default")]
[Tags("Queue")]
public class QueueController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<QueueController> _logger;

    public QueueController(IMediator mediator, ILogger<QueueController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,SuperAdmin,Communication Admin")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetQueue(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching queue");
        await Task.CompletedTask;
        return Ok(ApiResponse<object>.SuccessResult(new { }, "Queue listing endpoint available. Queue depth and status will be available in a future sprint."));
    }

    [HttpGet("failed")]
    [Authorize(Roles = "Admin,SuperAdmin,Communication Admin")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetFailed(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching failed queue items");
        await Task.CompletedTask;
        return Ok(ApiResponse<object>.SuccessResult(new { }, "Failed queue listing will be available in a future sprint."));
    }

    [HttpPost("reprocess")]
    [Authorize(Roles = "Admin,SuperAdmin,Communication Admin")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Reprocess(
        [FromBody] ReprocessRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Reprocessing {Count} queue items", request.NotificationIds?.Count ?? 0);

        if (request.NotificationIds is null || request.NotificationIds.Count == 0)
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Bad Request",
                Detail = "At least one notification ID is required.",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
            });

        await Task.CompletedTask;
        return Ok(ApiResponse<object>.SuccessResult(
            new { reprocessed = request.NotificationIds.Count },
            "Reprocess initiated. Full implementation will be available in a future sprint."));
    }
}

public record ReprocessRequest(List<Guid> NotificationIds);
