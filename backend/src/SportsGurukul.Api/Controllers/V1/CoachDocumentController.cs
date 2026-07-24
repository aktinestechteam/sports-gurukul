using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Api.Common.Models.SwaggerExamples;
using SportsGurukul.Application.Features.CoachManagement.Commands.DeleteCoachDocument;
using SportsGurukul.Application.Features.CoachManagement.Commands.RejectCoachDocument;
using SportsGurukul.Application.Features.CoachManagement.Commands.RestoreCoachDocument;
using SportsGurukul.Application.Features.CoachManagement.Commands.UpdateCoachDocumentMetadata;
using SportsGurukul.Application.Features.CoachManagement.Commands.VerifyCoachDocument;
using SportsGurukul.Application.Features.CoachManagement.DTOs;
using SportsGurukul.Application.Features.CoachManagement.Queries.DownloadCoachDocument;
using SportsGurukul.Application.Features.CoachManagement.Queries.GetCoachDocumentById;
using Swashbuckle.AspNetCore.Filters;

namespace SportsGurukul.Api.Controllers.V1;

/// <summary>
/// Manages individual coach documents by document ID.
/// </summary>
[ApiController]
[Route("api/v1/coach-documents")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
[Tags("Coach Documents")]
public class CoachDocumentController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CoachDocumentController> _logger;

    public CoachDocumentController(IMediator mediator, ILogger<CoachDocumentController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Gets metadata for a specific coach document.
    /// </summary>
    /// <param name="documentId">The document's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Document metadata including versions and audit trail</returns>
    /// <response code="200">Document retrieved successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="404">Document not found</response>
    [HttpGet("{documentId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CoachDocumentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDocumentById(
        Guid documentId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching coach document: {DocumentId}", documentId);

        var result = await _mediator.Send(
            new GetCoachDocumentByIdQuery { DocumentId = documentId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<CoachDocumentDto>.SuccessResult(
            result.Value!, "Document retrieved successfully."));
    }

    /// <summary>
    /// Downloads the file content of a coach document.
    /// </summary>
    /// <remarks>
    /// Returns the raw file content with the appropriate content type.
    /// A download audit entry is recorded.
    /// </remarks>
    /// <param name="documentId">The document's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>File content stream</returns>
    /// <response code="200">File downloaded successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Document not found</response>
    [HttpGet("{documentId:guid}/download")]
    [Authorize(Roles = "Coach,Academy Admin,System Admin")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DownloadDocument(
        Guid documentId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Downloading coach document: {DocumentId}", documentId);

        var result = await _mediator.Send(
            new DownloadCoachDocumentQuery { DocumentId = documentId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        var download = result.Value!;
        return File(download.Content, download.ContentType, download.FileName);
    }

    /// <summary>
    /// Updates document metadata (title, description, category, expiry, visibility).
    /// Only supplied fields are applied.
    /// </summary>
    /// <param name="documentId">The document's unique identifier</param>
    /// <param name="request">Fields to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated document metadata</returns>
    /// <response code="200">Document metadata updated successfully</response>
    /// <response code="400">Validation error</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Document not found</response>
    [HttpPut("{documentId:guid}")]
    [Authorize(Roles = "Coach,Academy Admin,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<CoachDocumentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [SwaggerRequestExample(typeof(UpdateCoachDocumentMetadataRequest), typeof(UpdateCoachDocumentMetadataRequestExample))]
    public async Task<IActionResult> UpdateDocumentMetadata(
        Guid documentId,
        [FromBody] UpdateCoachDocumentMetadataRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating coach document metadata: {DocumentId}", documentId);

        var command = new UpdateCoachDocumentMetadataCommand
        {
            DocumentId = documentId,
            Title = request.Title,
            Description = request.Description,
            Category = request.Category,
            ExpiryDate = request.ExpiryDate,
            Remarks = request.Remarks,
            IsPublic = request.IsPublic
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Coach document metadata updated: {DocumentId}", documentId);

        return Ok(ApiResponse<CoachDocumentDto>.SuccessResult(
            result.Value!, "Document metadata updated successfully."));
    }

    /// <summary>
    /// Soft-deletes a coach document. Requires Academy Admin or System Admin role.
    /// </summary>
    /// <param name="documentId">The document's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content</returns>
    /// <response code="204">Document deleted successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Document not found</response>
    [HttpDelete("{documentId:guid}")]
    [Authorize(Roles = "Academy Admin,System Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteDocument(
        Guid documentId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting coach document: {DocumentId}", documentId);

        var result = await _mediator.Send(
            new DeleteCoachDocumentCommand { DocumentId = documentId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Coach document deleted: {DocumentId}", documentId);

        return NoContent();
    }

    /// <summary>
    /// Restores a previously soft-deleted coach document. Requires Academy Admin or System Admin role.
    /// </summary>
    /// <param name="documentId">The document's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Restored document metadata</returns>
    /// <response code="200">Document restored successfully</response>
    /// <response code="400">Document is not deleted or cannot be restored</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">No deleted document found with this ID</response>
    [HttpPost("{documentId:guid}/restore")]
    [Authorize(Roles = "Academy Admin,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<CoachDocumentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RestoreDocument(
        Guid documentId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Restoring coach document: {DocumentId}", documentId);

        var result = await _mediator.Send(
            new RestoreCoachDocumentCommand { DocumentId = documentId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Coach document restored: {DocumentId}", documentId);

        return Ok(ApiResponse<CoachDocumentDto>.SuccessResult(
            result.Value!, "Document restored successfully."));
    }

    /// <summary>
    /// Verifies a coach document. Requires Academy Admin or System Admin role.
    /// </summary>
    /// <remarks>
    /// Marks the document as verified with the verifier's identity and timestamp.
    /// An audit entry is recorded.
    /// </remarks>
    /// <param name="documentId">The document's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Verified document metadata</returns>
    /// <response code="200">Document verified successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Document not found</response>
    [HttpPost("{documentId:guid}/verify")]
    [Authorize(Roles = "Academy Admin,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<CoachDocumentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> VerifyDocument(
        Guid documentId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Verifying coach document: {DocumentId}", documentId);

        var result = await _mediator.Send(
            new VerifyCoachDocumentCommand { DocumentId = documentId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Coach document verified: {DocumentId}", documentId);

        return Ok(ApiResponse<CoachDocumentDto>.SuccessResult(
            result.Value!, "Document verified successfully."));
    }

    /// <summary>
    /// Rejects a coach document. Requires Academy Admin or System Admin role.
    /// </summary>
    /// <remarks>
    /// Marks the document as rejected with a reason. The coach can re-upload after addressing the issue.
    /// An audit entry is recorded.
    /// </remarks>
    /// <param name="documentId">The document's unique identifier</param>
    /// <param name="request">Rejection details including reason</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Rejected document metadata</returns>
    /// <response code="200">Document rejected successfully</response>
    /// <response code="400">Validation error</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Document not found</response>
    [HttpPost("{documentId:guid}/reject")]
    [Authorize(Roles = "Academy Admin,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<CoachDocumentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [SwaggerRequestExample(typeof(RejectCoachDocumentRequest), typeof(RejectCoachDocumentRequestExample))]
    public async Task<IActionResult> RejectDocument(
        Guid documentId,
        [FromBody] RejectCoachDocumentRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Rejecting coach document: {DocumentId}", documentId);

        var command = new RejectCoachDocumentCommand
        {
            DocumentId = documentId,
            Reason = request.Reason
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Coach document rejected: {DocumentId}", documentId);

        return Ok(ApiResponse<CoachDocumentDto>.SuccessResult(
            result.Value!, "Document rejected successfully."));
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
