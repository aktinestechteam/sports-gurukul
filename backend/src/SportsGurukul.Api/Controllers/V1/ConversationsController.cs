using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.AIManagement.Commands.Conversation;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Application.Features.AIManagement.Queries;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Api.Controllers.V1;

/// <summary>
/// Manages AI conversations, including lifecycle, memory, and summarization.
/// </summary>
[ApiController]
[Route("api/v1/ai/conversations")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize(Roles = "Coach,Athlete,Academy Admin,AI Administrator,System Admin")]
[Tags("AI Conversations")]
public class ConversationsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ConversationsController> _logger;

    public ConversationsController(IMediator mediator, ILogger<ConversationsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new AI conversation.
    /// </summary>
    /// <param name="command">Conversation creation details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created conversation</returns>
    /// <response code="200">Conversation created successfully</response>
    /// <response code="400">Validation error</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ConversationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateConversation(
        [FromBody] CreateConversationCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating conversation for assistant: {AssistantId}", command.AssistantId);

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<ConversationDto>.SuccessResult(result.Value!, "Conversation created successfully."));
    }

    /// <summary>
    /// Searches conversations with optional filters and pagination.
    /// </summary>
    /// <param name="searchTerm">Optional search term</param>
    /// <param name="assistantId">Filter by assistant</param>
    /// <param name="participantUserId">Filter by participant user</param>
    /// <param name="status">Filter by conversation status</param>
    /// <param name="page">Page number (1-based, default 1)</param>
    /// <param name="pageSize">Items per page (default 20, max 100)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of conversation summaries</returns>
    /// <response code="200">Conversations retrieved successfully</response>
    /// <response code="400">Validation error</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ConversationSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SearchConversations(
        [FromQuery] string? searchTerm = null,
        [FromQuery] Guid? assistantId = null,
        [FromQuery] Guid? participantUserId = null,
        [FromQuery] AIConversationStatus? status = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Conversation search: Page={Page}, PageSize={PageSize}", page, pageSize);

        var result = await _mediator.Send(
            new SearchConversationsQuery(searchTerm, assistantId, participantUserId, status, page, pageSize),
            cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<IReadOnlyList<ConversationSummaryDto>>.SuccessResult(
            result.Value!, "Conversations retrieved successfully."));
    }

    /// <summary>
    /// Gets a conversation by its unique identifier.
    /// </summary>
    /// <param name="conversationId">The conversation's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The conversation details</returns>
    /// <response code="200">Conversation retrieved successfully</response>
    /// <response code="404">Conversation not found</response>
    [HttpGet("{conversationId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ConversationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetConversationById(
        Guid conversationId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching conversation: {ConversationId}", conversationId);

        var result = await _mediator.Send(new GetConversationByIdQuery(conversationId), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<ConversationDto>.SuccessResult(result.Value!, "Conversation retrieved successfully."));
    }

    /// <summary>
    /// Renames a conversation.
    /// </summary>
    /// <param name="conversationId">The conversation's unique identifier</param>
    /// <param name="command">Rename details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated conversation</returns>
    /// <response code="200">Conversation renamed successfully</response>
    /// <response code="404">Conversation not found</response>
    [HttpPatch("{conversationId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ConversationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RenameConversation(
        Guid conversationId,
        [FromBody] RenameConversationCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Renaming conversation: {ConversationId}", conversationId);

        var result = await _mediator.Send(
            new RenameConversationCommand(conversationId, command.Title), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<ConversationDto>.SuccessResult(result.Value!, "Conversation renamed successfully."));
    }

    /// <summary>
    /// Archives a conversation.
    /// </summary>
    /// <param name="conversationId">The conversation's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The archived conversation</returns>
    /// <response code="200">Conversation archived successfully</response>
    /// <response code="404">Conversation not found</response>
    [HttpPost("{conversationId:guid}/archive")]
    [ProducesResponseType(typeof(ApiResponse<ConversationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ArchiveConversation(
        Guid conversationId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Archiving conversation: {ConversationId}", conversationId);

        var result = await _mediator.Send(new ArchiveConversationCommand(conversationId), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<ConversationDto>.SuccessResult(result.Value!, "Conversation archived successfully."));
    }

    /// <summary>
    /// Permanently deletes a conversation.
    /// </summary>
    /// <param name="conversationId">The conversation's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Whether the conversation was deleted</returns>
    /// <response code="200">Conversation deleted successfully</response>
    /// <response code="404">Conversation not found</response>
    [HttpDelete("{conversationId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteConversation(
        Guid conversationId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting conversation: {ConversationId}", conversationId);

        var result = await _mediator.Send(new DeleteConversationCommand(conversationId), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<bool>.SuccessResult(result.Value, "Conversation deleted successfully."));
    }

    /// <summary>
    /// Stores a generated summary on a conversation.
    /// </summary>
    /// <param name="conversationId">The conversation's unique identifier</param>
    /// <param name="command">Summary details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated conversation</returns>
    /// <response code="200">Conversation summarized successfully</response>
    /// <response code="404">Conversation not found</response>
    [HttpPost("{conversationId:guid}/summarize")]
    [ProducesResponseType(typeof(ApiResponse<ConversationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SummarizeConversation(
        Guid conversationId,
        [FromBody] SummarizeConversationCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Summarizing conversation: {ConversationId}", conversationId);

        var result = await _mediator.Send(
            new SummarizeConversationCommand(conversationId, command.Summary), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<ConversationDto>.SuccessResult(result.Value!, "Conversation summary stored successfully."));
    }

    /// <summary>
    /// Clears the persisted memory of a conversation.
    /// </summary>
    /// <param name="conversationId">The conversation's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Whether the memory was cleared</returns>
    /// <response code="200">Conversation memory cleared successfully</response>
    /// <response code="404">Conversation not found</response>
    [HttpDelete("{conversationId:guid}/memory")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ClearConversationMemory(
        Guid conversationId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Clearing memory for conversation: {ConversationId}", conversationId);

        var result = await _mediator.Send(new ClearConversationMemoryCommand(conversationId), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<bool>.SuccessResult(result.Value, "Conversation memory cleared successfully."));
    }

    /// <summary>
    /// Gets the persisted memory entries of a conversation.
    /// </summary>
    /// <param name="conversationId">The conversation's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of conversation memory entries</returns>
    /// <response code="200">Conversation memory retrieved successfully</response>
    /// <response code="404">Conversation not found</response>
    [HttpGet("{conversationId:guid}/memory")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ConversationMemoryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetConversationMemory(
        Guid conversationId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching memory for conversation: {ConversationId}", conversationId);

        var result = await _mediator.Send(new GetConversationMemoryQuery(conversationId), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<IReadOnlyList<ConversationMemoryDto>>.SuccessResult(
            result.Value!, "Conversation memory retrieved successfully."));
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
