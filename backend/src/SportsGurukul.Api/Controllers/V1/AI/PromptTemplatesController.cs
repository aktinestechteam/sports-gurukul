using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Api.Common.Models.AI;
using SportsGurukul.Application.Features.AIManagement.Commands.Prompt;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Application.Features.AIManagement.Queries;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Api.Controllers.V1.AI;

[Route("api/v1/prompts")]
[Authorize(Roles = "Platform Administrator,AI Administrator")]
[Tags("Prompts")]
public class PromptTemplatesController : AIControllerBase
{
    public PromptTemplatesController(IMediator mediator, ILogger<PromptTemplatesController> logger)
        : base(mediator, logger)
    {
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<PromptTemplateDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreatePromptRequest request,
        CancellationToken cancellationToken)
    {
        Logger.LogInformation("Creating prompt template: {Name}", request.Name);

        var command = new CreatePromptTemplateCommand(
            request.Name, request.Description, request.Type, request.TemplateContent,
            request.Variables, request.Tags, request.Category);
        var result = await Mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Value!.Id, version = "1.0" },
            ApiResponse<PromptTemplateDto>.SuccessResult(result.Value, "Prompt template created successfully."));
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<PromptSummaryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search(
        [FromQuery] string? searchTerm,
        [FromQuery] PromptType? type,
        [FromQuery] PromptStatus? status,
        [FromQuery] string? category,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        Logger.LogInformation("Searching prompt templates");

        var query = new SearchPromptsQuery(searchTerm, type, status, category, page, pageSize);
        var result = await Mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<PaginatedResult<PromptSummaryDto>>.SuccessResult(
            result.Value!, "Prompt templates retrieved successfully."));
    }

    [HttpGet("search")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<PromptSummaryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchAlias(
        [FromQuery] string? searchTerm,
        [FromQuery] PromptType? type,
        [FromQuery] PromptStatus? status,
        [FromQuery] string? category,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        return await Search(searchTerm, type, status, category, page, pageSize, cancellationToken);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<PromptTemplateDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        Logger.LogInformation("Fetching prompt template {PromptId}", id);

        var result = await Mediator.Send(new PromptQuery(id), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<PromptTemplateDto>.SuccessResult(result.Value!, "Prompt template retrieved successfully."));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PromptTemplateDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdatePromptRequest request,
        CancellationToken cancellationToken)
    {
        Logger.LogInformation("Updating prompt template {PromptId}", id);

        var command = new UpdatePromptTemplateCommand(
            id, request.Name, request.Description, request.TemplateContent,
            request.Variables, request.Tags, request.Category);
        var result = await Mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<PromptTemplateDto>.SuccessResult(result.Value!, "Prompt template updated successfully."));
    }

    [HttpPost("{id:guid}/publish")]
    [ProducesResponseType(typeof(ApiResponse<PromptTemplateDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Publish(
        Guid id,
        CancellationToken cancellationToken)
    {
        Logger.LogInformation("Publishing prompt template {PromptId}", id);

        var result = await Mediator.Send(new PublishPromptTemplateCommand(id), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<PromptTemplateDto>.SuccessResult(result.Value!, "Prompt template published successfully."));
    }

    [HttpPost("{id:guid}/rollback")]
    [ProducesResponseType(typeof(ApiResponse<PromptTemplateDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Rollback(
        Guid id,
        [FromBody] RollbackPromptRequest request,
        CancellationToken cancellationToken)
    {
        Logger.LogInformation("Rolling back prompt template {PromptId} to version {Version}", id, request.VersionNumber);

        var result = await Mediator.Send(new RollbackPromptVersionCommand(id, request.VersionNumber), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<PromptTemplateDto>.SuccessResult(result.Value!, "Prompt template rolled back successfully."));
    }

    [HttpPost("{id:guid}/clone")]
    [ProducesResponseType(typeof(ApiResponse<PromptTemplateDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Clone(
        Guid id,
        [FromBody] ClonePromptRequest request,
        CancellationToken cancellationToken)
    {
        Logger.LogInformation("Cloning prompt template {PromptId} as {NewName}", id, request.NewName);

        var result = await Mediator.Send(new ClonePromptCommand(id, request.NewName), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<PromptTemplateDto>.SuccessResult(result.Value!, "Prompt template cloned successfully."));
    }

    }
}
