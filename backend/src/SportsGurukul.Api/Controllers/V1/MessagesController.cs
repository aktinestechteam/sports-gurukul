using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.AIManagement.Commands.Conversation;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Application.Features.AIManagement.Queries;

namespace SportsGurukul.Api.Controllers.V1;

/// <summary>
/// Manages messages within an AI conversation.
/// </summary>
[ApiController]
[Route("api/v1/ai/conversations/{conversationId:guid}/messages")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize(Roles = "Coach,Athlete,Academy Admin,AI Administrator,System Admin")]
[Tags("AI Messages")]
public class MessagesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<MessagesController> _logger;

    public MessagesController(IMediator mediator, ILogger<MessagesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Gets the full message history of a conversation, ordered oldest first.
    /// </summary>
    /// <param name="conversationId">The conversation's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of messages</returns>
    /// <response code="200">Messages retrieved successfully</response>
    /// <response code="404">Conversation not found</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<MessageDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetConversationHistory(
        Guid conversationId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching message history for conversation: {ConversationId}", conversationId);

        var result = await _mediator.Send(new GetConversationHistoryQuery(conversationId), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<IReadOnlyList<MessageDto>>.SuccessResult(
            result.Value!, "Messages retrieved successfully."));
    }

    /// <summary>
    /// Adds a message to a conversation.
    /// </summary>
    /// <param name="conversationId">The conversation's unique identifier</param>
    /// <param name="command">Message details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created message</returns>
    /// <response code="200">Message added successfully</response>
    /// <response code="400">Validation error</response>
    /// <response code="404">Conversation not found</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<MessageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddMessage(
        Guid conversationId,
        [FromBody] AddMessageCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Adding message to conversation: {ConversationId}", conversationId);

        var result = await _mediator.Send(new AddMessageCommand(
            conversationId,
            command.Role,
            command.ContentType,
            command.Content,
            command.ModelName,
            command.PromptVersionUsed,
            command.InputTokenCount,
            command.OutputTokenCount,
            command.LatencyMs,
            command.ToolCallsJson,
            command.ToolResultsJson), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<MessageDto>.SuccessResult(result.Value!, "Message added successfully."));
    }

    /// <summary>
    /// Regenerates the latest assistant response for a conversation.
    /// </summary>
    /// <param name="conversationId">The conversation's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The regenerated assistant message</returns>
    /// <response code="200">Response regenerated successfully</response>
    /// <response code="404">Conversation not found</response>
    [HttpPost("regenerate")]
    [ProducesResponseType(typeof(ApiResponse<MessageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RegenerateResponse(
        Guid conversationId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Regenerating response for conversation: {ConversationId}", conversationId);

        var result = await _mediator.Send(new RegenerateResponseCommand(conversationId), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<MessageDto>.SuccessResult(result.Value!, "Response regenerated successfully."));
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
