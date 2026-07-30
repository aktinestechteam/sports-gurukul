using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.Commands.Template;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;
using SportsGurukul.Application.Features.NotificationManagement.Queries;

namespace SportsGurukul.Api.Controllers.V1;

[ApiController]
[Route("api/v1/templates")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
[EnableRateLimiting("default")]
[Tags("Templates")]
public class TemplatesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TemplatesController> _logger;

    public TemplatesController(IMediator mediator, ILogger<TemplatesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    [Authorize(Roles = "Admin,SuperAdmin,Communication Admin")]
    [ProducesResponseType(typeof(ApiResponse<TemplateDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create(
        [FromBody] CreateTemplateCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating template: {TemplateName}", command.Name);

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Value!.Id, version = "1.0" },
            ApiResponse<TemplateDto>.SuccessResult(result.Value, "Template created successfully."));
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Listing all templates");
        await Task.CompletedTask;
        return Ok(ApiResponse<object>.SuccessResult(new { }, "Templates endpoint available. Use GET /api/v1/templates/{id} for detail."));
    }

    [HttpGet("search")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Searching templates");
        await Task.CompletedTask;
        return Ok(ApiResponse<object>.SuccessResult(new { }, "Template search endpoint available. Use GET /api/v1/templates/{id} for detail."));
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<TemplateDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching template: {TemplateId}", id);

        var result = await _mediator.Send(new TemplateQuery(id), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<TemplateDto>.SuccessResult(result.Value!, "Template retrieved successfully."));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,SuperAdmin,Communication Admin")]
    [ProducesResponseType(typeof(ApiResponse<TemplateDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateTemplateCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating template: {TemplateId}", id);

        var cmd = command with { Id = id };
        var result = await _mediator.Send(cmd, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<TemplateDto>.SuccessResult(result.Value!, "Template updated successfully."));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Archive(
        Guid id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Archiving template: {TemplateId}", id);

        var result = await _mediator.Send(new ArchiveTemplateCommand(id), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<object>.SuccessResult(new { id }, "Template archived successfully."));
    }

    [HttpPost("{id:guid}/publish")]
    [Authorize(Roles = "Admin,SuperAdmin,Communication Admin")]
    [ProducesResponseType(typeof(ApiResponse<TemplateDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Publish(
        Guid id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Publishing template: {TemplateId}", id);

        var result = await _mediator.Send(new PublishTemplateCommand(id), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<TemplateDto>.SuccessResult(result.Value!, "Template published successfully."));
    }

    [HttpGet("{id:guid}/versions")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<List<TemplateVersionDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetVersions(
        Guid id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching versions for template: {TemplateId}", id);

        var result = await _mediator.Send(new TemplateVersionsQuery(id), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<List<TemplateVersionDto>>.SuccessResult(result.Value!, "Template versions retrieved successfully."));
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
