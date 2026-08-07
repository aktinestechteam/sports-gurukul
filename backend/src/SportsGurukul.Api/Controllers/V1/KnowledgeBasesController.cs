using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.AIManagement.Commands.Knowledge;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Application.Features.AIManagement.Queries;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Api.Controllers.V1;

/// <summary>
/// Manages AI knowledge bases and their index lifecycle.
/// </summary>
[ApiController]
[Route("api/v1/ai/knowledge-bases")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize(Roles = "AI Administrator,System Admin")]
[Tags("AI Knowledge Bases")]
public class KnowledgeBasesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<KnowledgeBasesController> _logger;

    public KnowledgeBasesController(IMediator mediator, ILogger<KnowledgeBasesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new knowledge base.
    /// </summary>
    /// <param name="command">Knowledge base creation details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created knowledge base</returns>
    /// <response code="200">Knowledge base created successfully</response>
    /// <response code="400">Validation error</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<KnowledgeBaseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateKnowledgeBase(
        [FromBody] CreateKnowledgeBaseCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating knowledge base: {KnowledgeBaseName}", command.Name);

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<KnowledgeBaseDto>.SuccessResult(result.Value!, "Knowledge base created successfully."));
    }

    /// <summary>
    /// Searches knowledge bases with optional filters and pagination.
    /// </summary>
    /// <param name="searchTerm">Optional search term</param>
    /// <param name="knowledgeBaseType">Filter by knowledge base type</param>
    /// <param name="ownerUserId">Filter by owner user</param>
    /// <param name="isActive">Filter by active status</param>
    /// <param name="page">Page number (1-based, default 1)</param>
    /// <param name="pageSize">Items per page (default 20, max 100)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of knowledge bases</returns>
    /// <response code="200">Knowledge bases retrieved successfully</response>
    /// <response code="400">Validation error</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<KnowledgeBaseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SearchKnowledgeBases(
        [FromQuery] string? searchTerm = null,
        [FromQuery] AIKnowledgeBaseType? knowledgeBaseType = null,
        [FromQuery] Guid? ownerUserId = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Knowledge base search: Page={Page}, PageSize={PageSize}", page, pageSize);

        var result = await _mediator.Send(
            new SearchKnowledgeBasesQuery(searchTerm, knowledgeBaseType, ownerUserId, isActive, page, pageSize),
            cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<IReadOnlyList<KnowledgeBaseDto>>.SuccessResult(
            result.Value!, "Knowledge bases retrieved successfully."));
    }

    /// <summary>
    /// Gets a knowledge base by its unique identifier.
    /// </summary>
    /// <param name="knowledgeBaseId">The knowledge base's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The knowledge base details</returns>
    /// <response code="200">Knowledge base retrieved successfully</response>
    /// <response code="404">Knowledge base not found</response>
    [HttpGet("{knowledgeBaseId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<KnowledgeBaseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetKnowledgeBaseById(
        Guid knowledgeBaseId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching knowledge base: {KnowledgeBaseId}", knowledgeBaseId);

        var result = await _mediator.Send(new GetKnowledgeBaseByIdQuery(knowledgeBaseId), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<KnowledgeBaseDto>.SuccessResult(result.Value!, "Knowledge base retrieved successfully."));
    }

    /// <summary>
    /// Updates an existing knowledge base.
    /// </summary>
    /// <param name="knowledgeBaseId">The knowledge base's unique identifier</param>
    /// <param name="command">Update details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated knowledge base</returns>
    /// <response code="200">Knowledge base updated successfully</response>
    /// <response code="404">Knowledge base not found</response>
    [HttpPatch("{knowledgeBaseId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<KnowledgeBaseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateKnowledgeBase(
        Guid knowledgeBaseId,
        [FromBody] UpdateKnowledgeBaseCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating knowledge base: {KnowledgeBaseId}", knowledgeBaseId);

        var result = await _mediator.Send(new UpdateKnowledgeBaseCommand(
            knowledgeBaseId,
            command.Name,
            command.Description,
            command.KnowledgeBaseType,
            command.EmbeddingModelId,
            command.VectorIndexId,
            command.ChunkingStrategy,
            command.ChunkSize,
            command.ChunkOverlap,
            command.MetadataSchemaJson,
            command.IsActive,
            command.ExpectedRowVersion), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<KnowledgeBaseDto>.SuccessResult(result.Value!, "Knowledge base updated successfully."));
    }

    /// <summary>
    /// Triggers a rebuild of the knowledge base's vector index.
    /// </summary>
    /// <param name="knowledgeBaseId">The knowledge base's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The knowledge base with its refreshed index status</returns>
    /// <response code="200">Index rebuild triggered successfully</response>
    /// <response code="404">Knowledge base not found</response>
    [HttpPost("{knowledgeBaseId:guid}/rebuild-index")]
    [ProducesResponseType(typeof(ApiResponse<KnowledgeBaseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RebuildKnowledgeIndex(
        Guid knowledgeBaseId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Rebuilding index for knowledge base: {KnowledgeBaseId}", knowledgeBaseId);

        var result = await _mediator.Send(new RebuildKnowledgeIndexCommand(knowledgeBaseId), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<KnowledgeBaseDto>.SuccessResult(result.Value!, "Index rebuild triggered successfully."));
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
