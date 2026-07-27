using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Assessment.CreateAssessment;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Assessment.SubmitAssessmentResult;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Assessment.PublishAssessmentResults;
using SportsGurukul.Application.Features.TrainingProgramManagement.Queries.GetAssessmentsBySessionQuery;
using SportsGurukul.Application.Features.TrainingProgramManagement.Queries.GetAssessmentResultsQuery;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Api.Controllers.V1;

/// <summary>
/// Manages training assessments, result submission, and assessment publishing.
/// </summary>
[ApiController]
[Route("api/v1/assessments")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
[Tags("Assessments")]
public class AssessmentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AssessmentsController> _logger;

    public AssessmentsController(IMediator mediator, ILogger<AssessmentsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    #region Request Types

    public record CreateAssessmentRequest
    {
        public string AssessmentType { get; init; } = string.Empty;
        public string AssessmentName { get; init; } = string.Empty;
        public decimal MaximumScore { get; init; }
        public decimal PassingScore { get; init; }
    }

    public record SubmitResultRequest
    {
        public Guid AthleteId { get; init; }
        public decimal Score { get; init; }
        public string? Remarks { get; init; }
    }

    #endregion

    #region Assessments

    /// <summary>
    /// Creates a new assessment for a training session.
    /// </summary>
    /// <param name="sessionId">The training session's unique identifier</param>
    /// <param name="request">Assessment details including type, name, and scoring</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created assessment</returns>
    /// <response code="201">Assessment created successfully</response>
    /// <response code="400">Validation error</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="409">Assessment already exists for this session</response>
    [HttpPost("~/api/v1/training-sessions/{sessionId:guid}/assessments")]
    [Authorize(Roles = "Coach,System Admin,Academy Admin")]
    [ProducesResponseType(typeof(ApiResponse<AssessmentDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateAssessment(
        Guid sessionId,
        [FromBody] CreateAssessmentRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating assessment for session {SessionId}: {AssessmentName}", sessionId, request.AssessmentName);

        var command = new CreateAssessmentCommand
        {
            SessionId = sessionId,
            AssessmentType = request.AssessmentType,
            AssessmentName = request.AssessmentName,
            MaximumScore = request.MaximumScore,
            PassingScore = request.PassingScore
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Assessment created: {AssessmentId}", result.Value!.Id);

        return CreatedAtAction(
            nameof(GetSessionAssessments),
            new { sessionId = result.Value.SessionId, version = "1.0" },
            ApiResponse<AssessmentDto>.SuccessResult(result.Value, "Assessment created successfully."));
    }

    /// <summary>
    /// Gets all assessments for a specific training session.
    /// </summary>
    /// <param name="sessionId">The training session's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of assessments for the session</returns>
    /// <response code="200">Assessments retrieved successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Session not found</response>
    [HttpGet("~/api/v1/training-sessions/{sessionId:guid}/assessments")]
    [Authorize(Roles = "Coach,System Admin,Academy Admin")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<AssessmentDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSessionAssessments(
        Guid sessionId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching assessments for session {SessionId}", sessionId);

        var result = await _mediator.Send(new GetAssessmentsBySessionQuery { SessionId = sessionId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<IReadOnlyList<AssessmentDto>>.SuccessResult(result.Value!, "Assessments retrieved successfully."));
    }

    /// <summary>
    /// Publishes assessment results for an assessment, making them visible to athletes.
    /// </summary>
    /// <param name="assessmentId">The assessment's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if results were published successfully</returns>
    /// <response code="200">Results published successfully</response>
    /// <response code="400">Validation error</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Assessment not found</response>
    [HttpPost("{assessmentId:guid}/publish")]
    [Authorize(Roles = "Coach,System Admin,Academy Admin")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PublishResults(
        Guid assessmentId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Publishing results for assessment {AssessmentId}", assessmentId);

        var result = await _mediator.Send(new PublishAssessmentResultsCommand { AssessmentId = assessmentId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Results published for assessment {AssessmentId}", assessmentId);

        return Ok(ApiResponse<bool>.SuccessResult(result.Value!, "Results published successfully."));
    }

    /// <summary>
    /// Submits an assessment result for an athlete.
    /// </summary>
    /// <param name="assessmentId">The assessment's unique identifier</param>
    /// <param name="request">Result details including athlete, score, and remarks</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created assessment result</returns>
    /// <response code="201">Result submitted successfully</response>
    /// <response code="400">Validation error</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Assessment not found</response>
    /// <response code="409">Result already submitted for this athlete</response>
    [HttpPost("{assessmentId:guid}/results")]
    [Authorize(Roles = "Coach,System Admin,Academy Admin")]
    [ProducesResponseType(typeof(ApiResponse<AssessmentResultDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> SubmitResult(
        Guid assessmentId,
        [FromBody] SubmitResultRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Submitting result for athlete {AthleteId} in assessment {AssessmentId}", request.AthleteId, assessmentId);

        var command = new SubmitAssessmentResultCommand
        {
            AssessmentId = assessmentId,
            AthleteId = request.AthleteId,
            Score = request.Score,
            Remarks = request.Remarks
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Result submitted: {ResultId}", result.Value!.Id);

        return CreatedAtAction(
            nameof(GetAssessmentResults),
            new { assessmentId = result.Value.AssessmentId, version = "1.0" },
            ApiResponse<AssessmentResultDto>.SuccessResult(result.Value, "Result submitted successfully."));
    }

    /// <summary>
    /// Gets all results for a specific assessment.
    /// </summary>
    /// <param name="assessmentId">The assessment's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of assessment results</returns>
    /// <response code="200">Results retrieved successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Assessment not found</response>
    [HttpGet("{assessmentId:guid}/results")]
    [Authorize(Roles = "Coach,System Admin,Academy Admin")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<AssessmentResultDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAssessmentResults(
        Guid assessmentId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching results for assessment {AssessmentId}", assessmentId);

        var result = await _mediator.Send(new GetAssessmentResultsQuery { AssessmentId = assessmentId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<IReadOnlyList<AssessmentResultDto>>.SuccessResult(result.Value!, "Results retrieved successfully."));
    }

    #endregion

    #region Helpers

    private IActionResult HandleFailure(string error)
    {
        if (error.Contains("not found", StringComparison.OrdinalIgnoreCase))
            return NotFound(new ProblemDetails { Status = StatusCodes.Status404NotFound, Title = "Not Found", Detail = error, Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4" });
        if (error.Contains("already exists", StringComparison.OrdinalIgnoreCase) || error.Contains("already", StringComparison.OrdinalIgnoreCase))
            return Conflict(new ProblemDetails { Status = StatusCodes.Status409Conflict, Title = "Conflict", Detail = error, Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8" });
        return BadRequest(new ProblemDetails { Status = StatusCodes.Status400BadRequest, Title = "Bad Request", Detail = error, Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1" });
    }

    #endregion
}
