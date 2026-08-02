using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Api.Common.Models.AI;
using SportsGurukul.Application.Features.AIManagement.Commands.Conversation;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Application.Features.AIManagement.Queries;

namespace SportsGurukul.Api.Controllers.V1.AI;

[Route("api/v1/conversations/{conversationId:guid}/messages")]
[Authorize]
[Tags("Messages")]
public class MessagesController : AIControllerBase
{
    public MessagesController(IMediator mediator, ILogger<MessagesController> logger)
        : base(mediator, logger)
    {
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ConversationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Add(
        Guid conversationId,
        [FromBody] AddMessageRequest request,
        CancellationToken cancellationToken)
    {
        Logger.LogInformation("Adding message to conversation {ConversationId}", conversationId);

        var command = new AddMessageCommand(conversationId, request.Role, request.Content, request.Metadata);
        var result = await Mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<ConversationDto>.SuccessResult(result.Value!, "Message added successfully."));
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<MessageDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetHistory(
        Guid conversationId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        Logger.LogInformation("Fetching messages for conversation {ConversationId}", conversationId);

        var result = await Mediator.Send(new ConversationHistoryQuery(conversationId, page, pageSize), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<PaginatedResult<MessageDto>>.SuccessResult(
            result.Value!, "Messages retrieved successfully."));
    }

    }
}
