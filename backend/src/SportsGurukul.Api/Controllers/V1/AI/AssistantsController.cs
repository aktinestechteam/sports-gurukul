using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Api.Common.Models.AI;
using SportsGurukul.Application.Features.AIManagement.Commands.Assistant;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Application.Features.AIManagement.Queries;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Api.Controllers.V1.AI;

[Route("api/v1/assistants")]
[Authorize]
[Tags("Assistants")]
public class AssistantsController : AIControllerBase
{
    public AssistantsController(IMediator mediator, ILogger<AssistantsController> logger)
        : base(mediator, logger)
    {
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<AssistantDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateAssistantRequest request,
        CancellationToken cancellationToken)
    {
        Logger.LogInformation("Creating assistant: {Name}", request.Name);

        var command = new CreateAssistantCommand(
            request.Name, request.Description, request.AssistantType,
            request.Personality, request.SystemPrompt, request.GreetingMessage, request.IsPublic);
        var result = await Mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Value!.Id, version = "1.0" },
            ApiResponse<AssistantDto>.SuccessResult(result.Value, "Assistant created successfully."));
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<AssistantSummaryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search(
        [FromQuery] string? searchTerm,
        [FromQuery] AIAssistantType? assistantType,
        [FromQuery] bool? isActive,
        [FromQuery] bool? isPublic,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        Logger.LogInformation("Searching assistants");

        var query = new SearchAssistantsQuery(searchTerm, assistantType, isActive, isPublic, page, pageSize);
        var result = await Mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<PaginatedResult<AssistantSummaryDto>>.SuccessResult(
            result.Value!, "Assistants retrieved successfully."));
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<AssistantSummaryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchAlias(
        [FromQuery] string? searchTerm,
        [FromQuery] AIAssistantType? assistantType,
        [FromQuery] bool? isActive,
        [FromQuery] bool? isPublic,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        return await Search(searchTerm, assistantType, isActive, isPublic, page, pageSize, cancellationToken);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<AssistantDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        Logger.LogInformation("Fetching assistant {AssistantId}", id);

        var result = await Mediator.Send(new AssistantQuery(id), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<AssistantDto>.SuccessResult(result.Value!, "Assistant retrieved successfully."));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<AssistantDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateAssistantRequest request,
        CancellationToken cancellationToken)
    {
        Logger.LogInformation("Updating assistant {AssistantId}", id);

        var command = new UpdateAssistantCommand(
            id, request.Name, request.Description, request.AssistantType,
            request.Personality, request.SystemPrompt, request.GreetingMessage, request.IsPublic);
        var result = await Mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<AssistantDto>.SuccessResult(result.Value!, "Assistant updated successfully."));
    }

    [HttpPost("{id:guid}/publish")]
    [ProducesResponseType(typeof(ApiResponse<AssistantDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Publish(
        Guid id,
        CancellationToken cancellationToken)
    {
        Logger.LogInformation("Publishing assistant {AssistantId}", id);

        var result = await Mediator.Send(new PublishAssistantCommand(id), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<AssistantDto>.SuccessResult(result.Value!, "Assistant published successfully."));
    }

    [HttpPost("{id:guid}/knowledge")]
    [ProducesResponseType(typeof(ApiResponse<AssistantDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AttachKnowledge(
        Guid id,
        [FromBody] AttachKnowledgeRequest request,
        CancellationToken cancellationToken)
    {
        Logger.LogInformation("Attaching knowledge base {KnowledgeBaseId} to assistant {AssistantId}", request.KnowledgeBaseId, id);

        var result = await Mediator.Send(new AssignKnowledgeBaseCommand(id, request.KnowledgeBaseId), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<AssistantDto>.SuccessResult(result.Value!, "Knowledge base attached successfully."));
    }

    [HttpPost("{id:guid}/tools")]
    [ProducesResponseType(typeof(ApiResponse<AssistantDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignTools(
        Guid id,
        [FromBody] AssignToolsRequest request,
        CancellationToken cancellationToken)
    {
        Logger.LogInformation("Assigning {ToolCount} tools to assistant {AssistantId}", request.ToolIds.Count, id);

        var result = await Mediator.Send(new AssignToolsCommand(id, request.ToolIds), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<AssistantDto>.SuccessResult(result.Value!, "Tools assigned successfully."));
    }

    }
}
