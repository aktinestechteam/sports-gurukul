using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.AIManagement.Commands.Knowledge;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Application.Features.AIManagement.Queries;

namespace SportsGurukul.Api.Controllers.V1;

/// <summary>
/// Manages documents attached to a knowledge base.
/// </summary>
[ApiController]
[Route("api/v1/ai/knowledge-bases/{knowledgeBaseId:guid}/documents")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize(Roles = "AI Administrator,System Admin")]
[Tags("AI Knowledge Documents")]
public class KnowledgeDocumentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<KnowledgeDocumentsController> _logger;

    public KnowledgeDocumentsController(IMediator mediator, ILogger<KnowledgeDocumentsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Lists all documents attached to a knowledge base.
    /// </summary>
    /// <param name="knowledgeBaseId">The knowledge base's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of knowledge documents</returns>
    /// <response code="200">Documents retrieved successfully</response>
    /// <response code="404">Knowledge base not found</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<KnowledgeDocumentDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDocuments(
        Guid knowledgeBaseId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching documents for knowledge base: {KnowledgeBaseId}", knowledgeBaseId);

        var result = await _mediator.Send(new GetKnowledgeBaseDocumentsQuery(knowledgeBaseId), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<IReadOnlyList<KnowledgeDocumentDto>>.SuccessResult(
            result.Value!, "Documents retrieved successfully."));
    }

    /// <summary>
    /// Attaches a document to a knowledge base.
    /// </summary>
    /// <param name="knowledgeBaseId">The knowledge base's unique identifier</param>
    /// <param name="command">Document details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created knowledge document</returns>
    /// <response code="200">Document attached successfully</response>
    /// <response code="400">Validation error</response>
    /// <response code="404">Knowledge base not found</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<KnowledgeDocumentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AttachDocument(
        Guid knowledgeBaseId,
        [FromBody] AttachDocumentCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Attaching document to knowledge base: {KnowledgeBaseId}", knowledgeBaseId);

        var result = await _mediator.Send(new AttachDocumentCommand(
            knowledgeBaseId,
            command.Title,
            command.DocumentType,
            command.Content,
            command.ExternalId,
            command.StoragePath,
            command.MimeType,
            command.MetadataJson), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<KnowledgeDocumentDto>.SuccessResult(result.Value!, "Document attached successfully."));
    }

    /// <summary>
    /// Detaches a document from a knowledge base.
    /// </summary>
    /// <param name="knowledgeBaseId">The knowledge base's unique identifier</param>
    /// <param name="documentId">The document's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Whether the document was detached</returns>
    /// <response code="200">Document detached successfully</response>
    /// <response code="404">Knowledge base or document not found</response>
    [HttpDelete("{documentId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DetachDocument(
        Guid knowledgeBaseId,
        Guid documentId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Detaching document: {DocumentId} from knowledge base: {KnowledgeBaseId}",
            documentId, knowledgeBaseId);

        var result = await _mediator.Send(
            new DetachDocumentCommand(knowledgeBaseId, documentId), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<bool>.SuccessResult(result.Value, "Document detached successfully."));
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
