using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Api.Common.Models.AI;
using SportsGurukul.Application.Features.AIManagement.Commands.Conversation;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Application.Features.AIManagement.Queries;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Api.Controllers.V1.AI;

[Route("api/v1/conversations")]
[Authorize]
[Tags("Conversations")]
public class ConversationsController : AIControllerBase
{
    public ConversationsController(IMediator mediator, ILogger<ConversationsController> logger)
        : base(mediator, logger)
    {
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ConversationDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateConversationRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        Logger.LogInformation("Creating conversation");

        var command = new CreateConversationCommand(request.Title, request.AssistantId, userId);
        var result = await Mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Value!.Id, version = "1.0" },
            ApiResponse<ConversationDto>.SuccessResult(result.Value, "Conversation created successfully."));
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<ConversationSummaryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search(
        [FromQuery] string? searchTerm,
        [FromQuery] Guid? assistantId,
        [FromQuery] ConversationStatus? status,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        Logger.LogInformation("Searching conversations");

        var query = new SearchConversationsQuery(searchTerm, assistantId, GetUserId(), status, fromDate, toDate, page, pageSize);
        var result = await Mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<PaginatedResult<ConversationSummaryDto>>.SuccessResult(
            result.Value!, "Conversations retrieved successfully."));
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<ConversationSummaryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchAlias(
        [FromQuery] string? searchTerm,
        [FromQuery] Guid? assistantId,
        [FromQuery] ConversationStatus? status,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        return await Search(searchTerm, assistantId, status, fromDate, toDate, page, pageSize, cancellationToken);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ConversationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        Logger.LogInformation("Fetching conversation {ConversationId}", id);

        var result = await Mediator.Send(new GetConversationQuery(id), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<ConversationDto>.SuccessResult(result.Value!, "Conversation retrieved successfully."));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ConversationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Rename(
        Guid id,
        [FromBody] RenameConversationRequest request,
        CancellationToken cancellationToken)
    {
        Logger.LogInformation("Renaming conversation {ConversationId}", id);

        var result = await Mediator.Send(new RenameConversationCommand(id, request.Title), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<ConversationDto>.SuccessResult(result.Value!, "Conversation renamed successfully."));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        Logger.LogInformation("Deleting conversation {ConversationId}", id);

        var result = await Mediator.Send(new DeleteConversationCommand(id), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<object>.SuccessResult(new { id }, "Conversation deleted successfully."));
    }

    [HttpPost("{id:guid}/summarize")]
    [ProducesResponseType(typeof(ApiResponse<ConversationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Summarize(
        Guid id,
        CancellationToken cancellationToken)
    {
        Logger.LogInformation("Summarizing conversation {ConversationId}", id);

        var result = await Mediator.Send(new SummarizeConversationCommand(id), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<ConversationDto>.SuccessResult(result.Value!, "Conversation summarized successfully."));
    }

    [HttpPost("{id:guid}/regenerate")]
    [ProducesResponseType(typeof(ApiResponse<ConversationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Regenerate(
        Guid id,
        [FromBody] RegenerateRequest request,
        CancellationToken cancellationToken)
    {
        Logger.LogInformation("Regenerating response in conversation {ConversationId}", id);

        var result = await Mediator.Send(new RegenerateResponseCommand(id, request.MessageId), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<ConversationDto>.SuccessResult(result.Value!, "Response regenerated successfully."));
    }

    [HttpDelete("{id:guid}/memory")]
    [ProducesResponseType(typeof(ApiResponse<ConversationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ClearMemory(
        Guid id,
        CancellationToken cancellationToken)
    {
        Logger.LogInformation("Clearing memory for conversation {ConversationId}", id);

        var result = await Mediator.Send(new ClearConversationMemoryCommand(id), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<ConversationDto>.SuccessResult(result.Value!, "Conversation memory cleared successfully."));
    }

    }
}
