using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.Commands.Campaign;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;
using SportsGurukul.Application.Features.NotificationManagement.Queries;

namespace SportsGurukul.Api.Controllers.V1;

[ApiController]
[Route("api/v1/campaigns")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
[EnableRateLimiting("default")]
[Tags("Campaigns")]
public class CampaignsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CampaignsController> _logger;

    public CampaignsController(IMediator mediator, ILogger<CampaignsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    [Authorize(Roles = "Admin,SuperAdmin,Communication Admin")]
    [ProducesResponseType(typeof(ApiResponse<CampaignDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create(
        [FromBody] CreateCampaignCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating campaign: {CampaignName}", command.Name);

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Value!.Id, version = "1.0" },
            ApiResponse<CampaignDto>.SuccessResult(result.Value, "Campaign created successfully."));
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Listing all campaigns");
        await Task.CompletedTask;
        return Ok(ApiResponse<object>.SuccessResult(new { }, "Campaigns endpoint available. Use GET /api/v1/campaigns/{id} for detail."));
    }

    [HttpGet("search")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Searching campaigns");
        await Task.CompletedTask;
        return Ok(ApiResponse<object>.SuccessResult(new { }, "Campaign search endpoint available. Use GET /api/v1/campaigns/{id} for detail."));
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<CampaignDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching campaign: {CampaignId}", id);

        var result = await _mediator.Send(new CampaignQuery(id), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<CampaignDto>.SuccessResult(result.Value!, "Campaign retrieved successfully."));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,SuperAdmin,Communication Admin")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePlaceholder(
        Guid id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Update campaign endpoint: {CampaignId}", id);
        await Task.CompletedTask;
        return Ok(ApiResponse<object>.SuccessResult(new { id }, "Campaign update will be available in a future sprint."));
    }

    [HttpPost("{id:guid}/schedule")]
    [Authorize(Roles = "Admin,SuperAdmin,Communication Admin")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Schedule(
        Guid id,
        [FromBody] CampaignScheduleRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Scheduling campaign: {CampaignId} at {ScheduledAt}", id, request.ScheduledAt);

        var result = await _mediator.Send(new ScheduleCampaignCommand(id, request.ScheduledAt), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<object>.SuccessResult(new { id, request.ScheduledAt }, "Campaign scheduled successfully."));
    }

    [HttpPost("{id:guid}/pause")]
    [Authorize(Roles = "Admin,SuperAdmin,Communication Admin")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Pause(
        Guid id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Pausing campaign: {CampaignId}", id);

        var result = await _mediator.Send(new PauseCampaignCommand(id), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<object>.SuccessResult(new { id }, "Campaign paused successfully."));
    }

    [HttpPost("{id:guid}/resume")]
    [Authorize(Roles = "Admin,SuperAdmin,Communication Admin")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Resume(
        Guid id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Resuming campaign: {CampaignId}", id);

        var result = await _mediator.Send(new ResumeCampaignCommand(id), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<object>.SuccessResult(new { id }, "Campaign resumed successfully."));
    }

    [HttpPost("{id:guid}/cancel")]
    [Authorize(Roles = "Admin,SuperAdmin,Communication Admin")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel(
        Guid id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Cancelling campaign: {CampaignId}", id);

        var result = await _mediator.Send(new CancelCampaignCommand(id), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<object>.SuccessResult(new { id }, "Campaign cancelled successfully."));
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

public record CampaignScheduleRequest(DateTime ScheduledAt);
