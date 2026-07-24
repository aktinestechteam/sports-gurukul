using System.Net.Mime;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Api.Common.Models.SwaggerExamples;
using SportsGurukul.Application.Features.CoachManagement.Commands.UploadCoachDocument;
using SportsGurukul.Application.Features.CoachManagement.DTOs;
using SportsGurukul.Application.Features.CoachManagement.Queries.GetCoachDocuments;
using Swashbuckle.AspNetCore.Filters;

namespace SportsGurukul.Api.Controllers.V1;

/// <summary>
/// Manages coach documents scoped to a specific coach.
/// </summary>
[ApiController]
[Route("api/v1/coaches/{coachId:guid}/documents")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
[Tags("Coach Documents")]
public class CoachDocumentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CoachDocumentsController> _logger;

    public CoachDocumentsController(IMediator mediator, ILogger<CoachDocumentsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Uploads a new document for a coach.
    /// </summary>
    /// <remarks>
    /// Accepts multipart/form-data. Supported file types: PDF, JPG, JPEG, PNG, WEBP.
    /// Maximum file size: 20 MB.
    /// </remarks>
    /// <param name="coachId">The coach's unique identifier</param>
    /// <param name="request">Document upload details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created document metadata</returns>
    /// <response code="201">Document uploaded successfully</response>
    /// <response code="400">Validation error or unsupported file type</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Coach not found</response>
    [HttpPost]
    [Consumes("multipart/form-data")]
    [Authorize(Roles = "Coach,Academy Admin,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<CoachDocumentDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [SwaggerRequestExample(typeof(UploadCoachDocumentRequest), typeof(UploadCoachDocumentRequestExample))]
    public async Task<IActionResult> UploadDocument(
        Guid coachId,
        [FromForm] UploadCoachDocumentRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Uploading document for coach: {CoachId}", coachId);

        var command = new UploadCoachDocumentCommand
        {
            CoachId = coachId,
            File = request.File,
            Category = request.Category,
            Title = request.Title,
            Description = request.Description,
            ExpiryDate = request.ExpiryDate,
            Remarks = request.Remarks,
            IsPublic = request.IsPublic
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Document uploaded: {DocumentId} for coach: {CoachId}",
            result.Value!.Id, coachId);

        return CreatedAtAction(
            "GetDocumentById",
            "CoachDocument",
            new { documentId = result.Value.Id, version = "1.0" },
            ApiResponse<CoachDocumentDto>.SuccessResult(result.Value, "Document uploaded successfully."));
    }

    /// <summary>
    /// Lists all documents for a coach, ordered by upload date descending.
    /// </summary>
    /// <param name="coachId">The coach's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of document metadata</returns>
    /// <response code="200">Documents retrieved successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Coach not found</response>
    [HttpGet]
    [Authorize(Roles = "Coach,Academy Admin,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<CoachDocumentDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDocuments(
        Guid coachId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching documents for coach: {CoachId}", coachId);

        var result = await _mediator.Send(
            new GetCoachDocumentsQuery { CoachId = coachId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<IReadOnlyList<CoachDocumentDto>>.SuccessResult(
            result.Value!, "Documents retrieved successfully."));
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
