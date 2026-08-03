using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.AIManagement.Commands.Assistant;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Application.Features.AIManagement.Queries;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Api.Controllers.V1;

/// <summary>
/// Manages AI assistants and their assigned knowledge bases and tools.
/// </summary>
[ApiController]
[Route("api/v1/ai/assistants")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize(Roles = "AI Administrator,System Admin")]
[Tags("AI Assistants")]
public class AssistantsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AssistantsController> _logger;

    public AssistantsController(IMediator mediator, ILogger<AssistantsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new AI assistant.
    /// </summary>
    /// <param name="command">Assistant creation details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created assistant</returns>
    /// <response code="200">Assistant created successfully</response>
    /// <response code="400">Validation error</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<AssistantDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAssistant(
        [FromBody] CreateAssistantCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating assistant: {AssistantName}", command.Name);

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<AssistantDto>.SuccessResult(result.Value!, "Assistant created successfully."));
    }

    /// <summary>
    /// Searches assistants with optional filters and pagination.
    /// </summary>
    /// <param name="searchTerm">Optional search term</param>
    /// <param name="assistantType">Filter by assistant type</param>
    /// <param name="ownerUserId">Filter by owner user</param>
    /// <param name="isActive">Filter by active status</param>
    /// <param name="page">Page number (1-based, default 1)</param>
    /// <param name="pageSize">Items per page (default 20, max 100)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of assistants</returns>
    /// <response code="200">Assistants retrieved successfully</response>
    /// <response code="400">Validation error</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<AssistantDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SearchAssistants(
        [FromQuery] string? searchTerm = null,
        [FromQuery] AIAssistantType? assistantType = null,
        [FromQuery] Guid? ownerUserId = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Assistant search: Page={Page}, PageSize={PageSize}", page, pageSize);

        var result = await _mediator.Send(
            new SearchAssistantsQuery(searchTerm, assistantType, ownerUserId, isActive, page, pageSize),
            cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<IReadOnlyList<AssistantDto>>.SuccessResult(
            result.Value!, "Assistants retrieved successfully."));
    }

    /// <summary>
    /// Gets an assistant by its unique identifier.
    /// </summary>
    /// <param name="assistantId">The assistant's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The assistant details</returns>
    /// <response code="200">Assistant retrieved successfully</response>
    /// <response code="404">Assistant not found</response>
    [HttpGet("{assistantId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<AssistantDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAssistantById(
        Guid assistantId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching assistant: {AssistantId}", assistantId);

        var result = await _mediator.Send(new GetAssistantByIdQuery(assistantId), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<AssistantDto>.SuccessResult(result.Value!, "Assistant retrieved successfully."));
    }

    /// <summary>
    /// Updates an existing assistant.
    /// </summary>
    /// <param name="assistantId">The assistant's unique identifier</param>
    /// <param name="command">Update details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated assistant</returns>
    /// <response code="200">Assistant updated successfully</response>
    /// <response code="404">Assistant not found</response>
    [HttpPatch("{assistantId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<AssistantDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAssistant(
        Guid assistantId,
        [FromBody] UpdateAssistantCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating assistant: {AssistantId}", assistantId);

        var result = await _mediator.Send(new UpdateAssistantCommand(
            assistantId,
            command.Name,
            command.DisplayName,
            command.Description,
            command.AssistantType,
            command.SystemPrompt,
            command.ModelId,
            command.Temperature,
            command.TopP,
            command.MaxTokens,
            command.MemoryEnabled,
            command.StreamingEnabled,
            command.AvatarUrl,
            command.GuardrailsJson,
            command.ExpectedRowVersion), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<AssistantDto>.SuccessResult(result.Value!, "Assistant updated successfully."));
    }

    /// <summary>
    /// Publishes an assistant, making it available for use.
    /// </summary>
    /// <param name="assistantId">The assistant's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The published assistant</returns>
    /// <response code="200">Assistant published successfully</response>
    /// <response code="404">Assistant not found</response>
    [HttpPost("{assistantId:guid}/publish")]
    [ProducesResponseType(typeof(ApiResponse<AssistantDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PublishAssistant(
        Guid assistantId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Publishing assistant: {AssistantId}", assistantId);

        var result = await _mediator.Send(new PublishAssistantCommand(assistantId), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<AssistantDto>.SuccessResult(result.Value!, "Assistant published successfully."));
    }

    /// <summary>
    /// Archives an assistant.
    /// </summary>
    /// <param name="assistantId">The assistant's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The archived assistant</returns>
    /// <response code="200">Assistant archived successfully</response>
    /// <response code="404">Assistant not found</response>
    [HttpPost("{assistantId:guid}/archive")]
    [ProducesResponseType(typeof(ApiResponse<AssistantDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ArchiveAssistant(
        Guid assistantId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Archiving assistant: {AssistantId}", assistantId);

        var result = await _mediator.Send(new ArchiveAssistantCommand(assistantId), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<AssistantDto>.SuccessResult(result.Value!, "Assistant archived successfully."));
    }

    /// <summary>
    /// Assigns knowledge bases to an assistant.
    /// </summary>
    /// <param name="assistantId">The assistant's unique identifier</param>
    /// <param name="command">Knowledge base assignment details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated assistant</returns>
    /// <response code="200">Knowledge bases assigned successfully</response>
    /// <response code="404">Assistant not found</response>
    [HttpPut("{assistantId:guid}/knowledge-bases")]
    [ProducesResponseType(typeof(ApiResponse<AssistantDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignKnowledgeBases(
        Guid assistantId,
        [FromBody] AssignKnowledgeBaseCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Assigning knowledge bases to assistant: {AssistantId}", assistantId);

        var result = await _mediator.Send(
            new AssignKnowledgeBaseCommand(assistantId, command.KnowledgeBaseIds, command.ClearExisting),
            cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<AssistantDto>.SuccessResult(result.Value!, "Knowledge bases assigned successfully."));
    }

    /// <summary>
    /// Assigns tools to an assistant.
    /// </summary>
    /// <param name="assistantId">The assistant's unique identifier</param>
    /// <param name="command">Tool assignment details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated assistant</returns>
    /// <response code="200">Tools assigned successfully</response>
    /// <response code="404">Assistant not found</response>
    [HttpPut("{assistantId:guid}/tools")]
    [ProducesResponseType(typeof(ApiResponse<AssistantDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignTools(
        Guid assistantId,
        [FromBody] AssignToolsCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Assigning tools to assistant: {AssistantId}", assistantId);

        var result = await _mediator.Send(
            new AssignToolsCommand(assistantId, command.ToolDefinitionIds, command.ClearExisting),
            cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<AssistantDto>.SuccessResult(result.Value!, "Tools assigned successfully."));
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
