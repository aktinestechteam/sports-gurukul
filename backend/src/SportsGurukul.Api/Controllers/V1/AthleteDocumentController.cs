using System.Net.Mime;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Api.Common.Models.SwaggerExamples;
using SportsGurukul.Application.Features.DocumentManagement.Commands.DeleteAthleteDocument;
using SportsGurukul.Application.Features.DocumentManagement.Commands.RestoreAthleteDocument;
using SportsGurukul.Application.Features.DocumentManagement.Commands.UpdateDocumentMetadata;
using SportsGurukul.Application.Features.DocumentManagement.Commands.UploadAthleteDocument;
using SportsGurukul.Application.Features.DocumentManagement.Commands.VerifyDocument;
using SportsGurukul.Application.Features.DocumentManagement.DTOs;
using SportsGurukul.Application.Features.DocumentManagement.Queries.DownloadDocument;
using SportsGurukul.Application.Features.DocumentManagement.Queries.GetAthleteDocuments;
using SportsGurukul.Application.Features.DocumentManagement.Queries.GetDocumentById;
using Swashbuckle.AspNetCore.Filters;

namespace SportsGurukul.Api.Controllers.V1;

/// <summary>
/// Manages athlete documents, certificates, and file uploads.
/// </summary>
[ApiController]
[Route("api/v1/athletes/{athleteId:guid}/documents")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
[Tags("Athlete Documents")]
public class AthleteDocumentController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AthleteDocumentController> _logger;

    public AthleteDocumentController(IMediator mediator, ILogger<AthleteDocumentController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Uploads a new document for an athlete.
    /// </summary>
    /// <remarks>
    /// Accepts multipart/form-data. Supported file types: PDF, DOC, DOCX, XLS, XLSX,
    /// PPT, PPTX, JPG, JPEG, PNG, GIF, BMP, WEBP, TXT, CSV, RTF, ODT.
    /// Maximum file size: 10 MB.
    /// </remarks>
    /// <param name="athleteId">The athlete's unique identifier</param>
    /// <param name="request">Document upload details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created document metadata</returns>
    /// <response code="201">Document uploaded successfully</response>
    /// <response code="400">Validation error or unsupported file type</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="404">Athlete not found</response>
    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ApiResponse<AthleteDocumentDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [SwaggerRequestExample(typeof(UploadDocumentRequest), typeof(UploadDocumentRequestExample))]
    public async Task<IActionResult> UploadDocument(
        Guid athleteId,
        [FromForm] UploadDocumentRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Uploading document for athlete: {AthleteId}", athleteId);

        var command = new UploadAthleteDocumentCommand
        {
            AthleteId = athleteId,
            File = request.File,
            Category = request.Category,
            Title = request.Title,
            Description = request.Description,
            ExpiryDate = request.ExpiryDate,
            IsPublic = request.IsPublic
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Document uploaded: {DocumentId} for athlete: {AthleteId}",
            result.Value!.Id, athleteId);

        return CreatedAtAction(
            nameof(GetDocumentById),
            new { athleteId, documentId = result.Value.Id, version = "1.0" },
            ApiResponse<AthleteDocumentDto>.SuccessResult(result.Value, "Document uploaded successfully."));
    }

    /// <summary>
    /// Lists all documents for an athlete, ordered by upload date descending.
    /// </summary>
    /// <param name="athleteId">The athlete's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of document metadata</returns>
    /// <response code="200">Documents retrieved successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="404">Athlete not found</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<AthleteDocumentDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDocuments(
        Guid athleteId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching documents for athlete: {AthleteId}", athleteId);

        var result = await _mediator.Send(
            new GetAthleteDocumentsQuery { AthleteId = athleteId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<IReadOnlyList<AthleteDocumentDto>>.SuccessResult(
            result.Value!, "Documents retrieved successfully."));
    }

    /// <summary>
    /// Gets metadata for a specific document.
    /// </summary>
    /// <param name="athleteId">The athlete's unique identifier</param>
    /// <param name="documentId">The document's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Document metadata including versions and audit trail</returns>
    /// <response code="200">Document retrieved successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="404">Document not found</response>
    [HttpGet("{documentId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<AthleteDocumentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDocumentById(
        Guid athleteId,
        Guid documentId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching document: {DocumentId} for athlete: {AthleteId}",
            documentId, athleteId);

        var result = await _mediator.Send(
            new GetDocumentByIdQuery { DocumentId = documentId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<AthleteDocumentDto>.SuccessResult(
            result.Value!, "Document retrieved successfully."));
    }

    /// <summary>
    /// Downloads the file content of a document.
    /// </summary>
    /// <remarks>
    /// Returns the raw file content with the appropriate content type.
    /// A download audit entry is recorded.
    /// </remarks>
    /// <param name="athleteId">The athlete's unique identifier</param>
    /// <param name="documentId">The document's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>File content stream</returns>
    /// <response code="200">File downloaded successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="404">Document not found</response>
    [HttpGet("{documentId:guid}/download")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DownloadDocument(
        Guid athleteId,
        Guid documentId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Downloading document: {DocumentId} for athlete: {AthleteId}",
            documentId, athleteId);

        var result = await _mediator.Send(
            new DownloadDocumentQuery { DocumentId = documentId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        var download = result.Value!;
        return File(download.Content, download.ContentType, download.FileName);
    }

    /// <summary>
    /// Updates document metadata (title, description, category, expiry, visibility).
    /// Only supplied fields are applied.
    /// </summary>
    /// <param name="athleteId">The athlete's unique identifier</param>
    /// <param name="documentId">The document's unique identifier</param>
    /// <param name="request">Fields to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated document metadata</returns>
    /// <response code="200">Document metadata updated successfully</response>
    /// <response code="400">Validation error</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="404">Document not found</response>
    [HttpPut("{documentId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<AthleteDocumentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [SwaggerRequestExample(typeof(UpdateDocumentMetadataRequest), typeof(UpdateDocumentMetadataRequestExample))]
    public async Task<IActionResult> UpdateDocumentMetadata(
        Guid athleteId,
        Guid documentId,
        [FromBody] UpdateDocumentMetadataRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating document metadata: {DocumentId}", documentId);

        var command = new UpdateDocumentMetadataCommand
        {
            DocumentId = documentId,
            Title = request.Title,
            Description = request.Description,
            Category = request.Category,
            ExpiryDate = request.ExpiryDate,
            IsPublic = request.IsPublic
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Document metadata updated: {DocumentId}", documentId);

        return Ok(ApiResponse<AthleteDocumentDto>.SuccessResult(
            result.Value!, "Document metadata updated successfully."));
    }

    /// <summary>
    /// Soft-deletes a document. Requires <c>Admin</c> role.
    /// </summary>
    /// <param name="athleteId">The athlete's unique identifier</param>
    /// <param name="documentId">The document's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content</returns>
    /// <response code="204">Document deleted successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Document not found</response>
    [HttpDelete("{documentId:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteDocument(
        Guid athleteId,
        Guid documentId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting document: {DocumentId}", documentId);

        var result = await _mediator.Send(
            new DeleteAthleteDocumentCommand { DocumentId = documentId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Document deleted: {DocumentId}", documentId);

        return NoContent();
    }

    /// <summary>
    /// Restores a previously soft-deleted document. Requires <c>Admin</c> role.
    /// </summary>
    /// <param name="athleteId">The athlete's unique identifier</param>
    /// <param name="documentId">The document's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Restored document metadata</returns>
    /// <response code="200">Document restored successfully</response>
    /// <response code="400">Document is not deleted or cannot be restored</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">No deleted document found with this ID</response>
    [HttpPost("{documentId:guid}/restore")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<AthleteDocumentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RestoreDocument(
        Guid athleteId,
        Guid documentId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Restoring document: {DocumentId}", documentId);

        var result = await _mediator.Send(
            new RestoreAthleteDocumentCommand { DocumentId = documentId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Document restored: {DocumentId}", documentId);

        return Ok(ApiResponse<AthleteDocumentDto>.SuccessResult(
            result.Value!, "Document restored successfully."));
    }

    /// <summary>
    /// Verifies a document. Requires <c>Admin</c> or <c>Coach</c> role.
    /// </summary>
    /// <remarks>
    /// Marks the document as verified with the verifier's identity and timestamp.
    /// An audit entry is recorded.
    /// </remarks>
    /// <param name="athleteId">The athlete's unique identifier</param>
    /// <param name="documentId">The document's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Verified document metadata</returns>
    /// <response code="200">Document verified successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Document not found</response>
    [HttpPost("{documentId:guid}/verify")]
    [Authorize(Roles = "Admin,Coach")]
    [ProducesResponseType(typeof(ApiResponse<AthleteDocumentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> VerifyDocument(
        Guid athleteId,
        Guid documentId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Verifying document: {DocumentId}", documentId);

        var result = await _mediator.Send(
            new VerifyDocumentCommand { DocumentId = documentId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Document verified: {DocumentId}", documentId);

        return Ok(ApiResponse<AthleteDocumentDto>.SuccessResult(
            result.Value!, "Document verified successfully."));
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

        if (error.Contains("deleted", StringComparison.OrdinalIgnoreCase) ||
            error.Contains("not deleted", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Bad Request",
                Detail = error,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
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
