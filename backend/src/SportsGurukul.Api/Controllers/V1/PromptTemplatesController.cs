using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.AIManagement.Commands.Prompt;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Application.Features.AIManagement.Queries;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Api.Controllers.V1;

/// <summary>
/// Manages AI prompt templates and their versioned lifecycle.
/// </summary>
[ApiController]
[Route("api/v1/ai/prompt-templates")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize(Roles = "AI Administrator,System Admin")]
[Tags("AI Prompt Templates")]
public class PromptTemplatesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<PromptTemplatesController> _logger;

    public PromptTemplatesController(IMediator mediator, ILogger<PromptTemplatesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new prompt template.
    /// </summary>
    /// <param name="command">Prompt template creation details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created prompt template</returns>
    /// <response code="200">Prompt template created successfully</response>
    /// <response code="400">Validation error</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<PromptTemplateDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePromptTemplate(
        [FromBody] CreatePromptTemplateCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating prompt template: {PromptTemplateName}", command.Name);

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<PromptTemplateDto>.SuccessResult(result.Value!, "Prompt template created successfully."));
    }

    /// <summary>
    /// Searches prompt templates with optional filters and pagination.
    /// </summary>
    /// <param name="searchTerm">Optional search term</param>
    /// <param name="assistantId">Filter by assistant</param>
    /// <param name="promptType">Filter by prompt type</param>
    /// <param name="isActive">Filter by active status</param>
    /// <param name="page">Page number (1-based, default 1)</param>
    /// <param name="pageSize">Items per page (default 20, max 100)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of prompt templates</returns>
    /// <response code="200">Prompt templates retrieved successfully</response>
    /// <response code="400">Validation error</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<PromptTemplateDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SearchPromptTemplates(
        [FromQuery] string? searchTerm = null,
        [FromQuery] Guid? assistantId = null,
        [FromQuery] AIPromptType? promptType = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Prompt template search: Page={Page}, PageSize={PageSize}", page, pageSize);

        var result = await _mediator.Send(
            new SearchPromptTemplatesQuery(searchTerm, assistantId, promptType, isActive, page, pageSize),
            cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<IReadOnlyList<PromptTemplateDto>>.SuccessResult(
            result.Value!, "Prompt templates retrieved successfully."));
    }

    /// <summary>
    /// Gets a prompt template by its unique identifier.
    /// </summary>
    /// <param name="promptTemplateId">The prompt template's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The prompt template details</returns>
    /// <response code="200">Prompt template retrieved successfully</response>
    /// <response code="404">Prompt template not found</response>
    [HttpGet("{promptTemplateId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PromptTemplateDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPromptTemplateById(
        Guid promptTemplateId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching prompt template: {PromptTemplateId}", promptTemplateId);

        var result = await _mediator.Send(new GetPromptTemplateByIdQuery(promptTemplateId), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<PromptTemplateDto>.SuccessResult(result.Value!, "Prompt template retrieved successfully."));
    }

    /// <summary>
    /// Updates an existing prompt template.
    /// </summary>
    /// <param name="promptTemplateId">The prompt template's unique identifier</param>
    /// <param name="command">Update details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated prompt template</returns>
    /// <response code="200">Prompt template updated successfully</response>
    /// <response code="404">Prompt template not found</response>
    [HttpPatch("{promptTemplateId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PromptTemplateDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePromptTemplate(
        Guid promptTemplateId,
        [FromBody] UpdatePromptTemplateCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating prompt template: {PromptTemplateId}", promptTemplateId);

        var result = await _mediator.Send(new UpdatePromptTemplateCommand(
            promptTemplateId,
            command.Name,
            command.Description,
            command.TemplateText,
            command.InputSchemaJson,
            command.OutputSchemaJson,
            command.VariablesJson,
            command.IsActive,
            command.ExpectedRowVersion), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<PromptTemplateDto>.SuccessResult(result.Value!, "Prompt template updated successfully."));
    }

    /// <summary>
    /// Publishes the current draft of a prompt template as a new version.
    /// </summary>
    /// <param name="promptTemplateId">The prompt template's unique identifier</param>
    /// <param name="command">Publish details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The published prompt template</returns>
    /// <response code="200">Prompt template published successfully</response>
    /// <response code="404">Prompt template not found</response>
    [HttpPost("{promptTemplateId:guid}/publish")]
    [ProducesResponseType(typeof(ApiResponse<PromptTemplateDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PublishPromptTemplate(
        Guid promptTemplateId,
        [FromBody] PublishPromptTemplateCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Publishing prompt template: {PromptTemplateId}", promptTemplateId);

        var result = await _mediator.Send(
            new PublishPromptTemplateCommand(promptTemplateId, command.ChangeSummary, command.Notes),
            cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<PromptTemplateDto>.SuccessResult(result.Value!, "Prompt template published successfully."));
    }

    /// <summary>
    /// Rolls a prompt template back to a previously published version.
    /// </summary>
    /// <param name="promptTemplateId">The prompt template's unique identifier</param>
    /// <param name="command">Rollback details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The rolled back prompt template</returns>
    /// <response code="200">Prompt template rolled back successfully</response>
    /// <response code="404">Prompt template or version not found</response>
    [HttpPost("{promptTemplateId:guid}/rollback")]
    [ProducesResponseType(typeof(ApiResponse<PromptTemplateDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RollbackPromptVersion(
        Guid promptTemplateId,
        [FromBody] RollbackPromptVersionCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Rolling back prompt template: {PromptTemplateId} to version {Version}",
            promptTemplateId, command.VersionNumber);

        var result = await _mediator.Send(
            new RollbackPromptVersionCommand(promptTemplateId, command.VersionNumber), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<PromptTemplateDto>.SuccessResult(result.Value!, "Prompt template rolled back successfully."));
    }

    /// <summary>
    /// Clones a prompt template, optionally for a different assistant.
    /// </summary>
    /// <param name="promptTemplateId">The source prompt template's unique identifier</param>
    /// <param name="command">Clone details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The cloned prompt template</returns>
    /// <response code="200">Prompt template cloned successfully</response>
    /// <response code="404">Source prompt template not found</response>
    [HttpPost("{promptTemplateId:guid}/clone")]
    [ProducesResponseType(typeof(ApiResponse<PromptTemplateDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ClonePromptTemplate(
        Guid promptTemplateId,
        [FromBody] ClonePromptCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Cloning prompt template: {PromptTemplateId}", promptTemplateId);

        var result = await _mediator.Send(
            new ClonePromptCommand(promptTemplateId, command.TargetAssistantId, command.NewName), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<PromptTemplateDto>.SuccessResult(result.Value!, "Prompt template cloned successfully."));
    }

    #region Helpers

    private IActionResult HandleFailure(string error)
    {
        if (error.Contains("not found", StringComparison.OrdinalIgnoreCase))
        {
            return NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Not Found",
                Detail = error,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4"
            });
        }

        if (error.Contains("already exists", StringComparison.OrdinalIgnoreCase))
        {
            return Conflict(new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Conflict",
                Detail = error,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8"
            });
        }

        if (error.Contains("insufficient permissions", StringComparison.OrdinalIgnoreCase) ||
            error.Contains("unauthorized", StringComparison.OrdinalIgnoreCase))
        {
            return StatusCode(StatusCodes.Status403Forbidden, new ProblemDetails
            {
                Status = StatusCodes.Status403Forbidden,
                Title = "Forbidden",
                Detail = error,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3"
            });
        }

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
