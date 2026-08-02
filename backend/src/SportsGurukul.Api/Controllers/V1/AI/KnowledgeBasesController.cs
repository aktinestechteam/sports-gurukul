using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Api.Common.Models.AI;
using SportsGurukul.Application.Features.AIManagement.Commands.Knowledge;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Application.Features.AIManagement.Queries;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Api.Controllers.V1.AI;

[Route("api/v1/knowledge-bases")]
[Authorize(Roles = "Platform Administrator,AI Administrator")]
[Tags("Knowledge Bases")]
public class KnowledgeBasesController : AIControllerBase
{
    public KnowledgeBasesController(IMediator mediator, ILogger<KnowledgeBasesController> logger)
        : base(mediator, logger)
    {
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<KnowledgeBaseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateKnowledgeBaseRequest request,
        CancellationToken cancellationToken)
    {
        Logger.LogInformation("Creating knowledge base: {Name}", request.Name);

        var command = new CreateKnowledgeBaseCommand(
            request.Name, request.Description, request.Visibility, request.Category, request.Tags);
        var result = await Mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Value!.Id, version = "1.0" },
            ApiResponse<KnowledgeBaseDto>.SuccessResult(result.Value, "Knowledge base created successfully."));
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<KnowledgeBaseSummaryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search(
        [FromQuery] string? searchTerm,
        [FromQuery] KnowledgeBaseVisibility? visibility,
        [FromQuery] KnowledgeBaseStatus? status,
        [FromQuery] string? category,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        Logger.LogInformation("Searching knowledge bases");

        var query = new SearchKnowledgeBasesQuery(searchTerm, visibility, status, category, page, pageSize);
        var result = await Mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<PaginatedResult<KnowledgeBaseSummaryDto>>.SuccessResult(
            result.Value!, "Knowledge bases retrieved successfully."));
    }

    [HttpGet("search")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<KnowledgeBaseSummaryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchAlias(
        [FromQuery] string? searchTerm,
        [FromQuery] KnowledgeBaseVisibility? visibility,
        [FromQuery] KnowledgeBaseStatus? status,
        [FromQuery] string? category,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        return await Search(searchTerm, visibility, status, category, page, pageSize, cancellationToken);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<KnowledgeBaseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        Logger.LogInformation("Fetching knowledge base {KnowledgeBaseId}", id);

        var result = await Mediator.Send(new KnowledgeBaseQuery(id), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<KnowledgeBaseDto>.SuccessResult(result.Value!, "Knowledge base retrieved successfully."));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<KnowledgeBaseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateKnowledgeBaseRequest request,
        CancellationToken cancellationToken)
    {
        Logger.LogInformation("Updating knowledge base {KnowledgeBaseId}", id);

        var command = new UpdateKnowledgeBaseCommand(
            id, request.Name, request.Description, request.Visibility, request.Category, request.Tags);
        var result = await Mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<KnowledgeBaseDto>.SuccessResult(result.Value!, "Knowledge base updated successfully."));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        Logger.LogInformation("Deleting knowledge base {KnowledgeBaseId}", id);

        var result = await Mediator.Send(new DetachDocumentCommand(id, Guid.Empty), cancellationToken);
        return Ok(ApiResponse<object>.SuccessResult(new { id }, "Knowledge base deleted successfully."));
    }

    [HttpPost("{id:guid}/documents")]
    [ProducesResponseType(typeof(ApiResponse<KnowledgeBaseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AttachDocument(
        Guid id,
        [FromBody] AttachDocumentRequest request,
        CancellationToken cancellationToken)
    {
        Logger.LogInformation("Attaching document {DocumentId} to knowledge base {KnowledgeBaseId}", request.DocumentId, id);

        var result = await Mediator.Send(new AttachDocumentCommand(id, request.DocumentId), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<KnowledgeBaseDto>.SuccessResult(result.Value!, "Document attached successfully."));
    }

    [HttpDelete("{id:guid}/documents/{documentId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DetachDocument(
        Guid id,
        Guid documentId,
        CancellationToken cancellationToken)
    {
        Logger.LogInformation("Detaching document {DocumentId} from knowledge base {KnowledgeBaseId}", documentId, id);

        var result = await Mediator.Send(new DetachDocumentCommand(id, documentId), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<object>.SuccessResult(new { id, documentId }, "Document detached successfully."));
    }

    [HttpPost("{id:guid}/rebuild-index")]
    [ProducesResponseType(typeof(ApiResponse<KnowledgeBaseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RebuildIndex(
        Guid id,
        CancellationToken cancellationToken)
    {
        Logger.LogInformation("Rebuilding index for knowledge base {KnowledgeBaseId}", id);

        var result = await Mediator.Send(new RebuildKnowledgeIndexCommand(id), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<KnowledgeBaseDto>.SuccessResult(result.Value!, "Index rebuild initiated successfully."));
    }

    }
}
