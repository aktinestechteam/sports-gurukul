using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Enrollment.EnrollAthlete;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Enrollment.TransferEnrollment;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Enrollment.CompleteEnrollment;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Enrollment.CancelEnrollment;
using SportsGurukul.Application.Features.TrainingProgramManagement.Queries.GetEnrollmentsByBatchQuery;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;

namespace SportsGurukul.Api.Controllers.V1;

/// <summary>
/// Manages athlete enrollment in training batches, transfers, and completion.
/// </summary>
[ApiController]
[Route("api/v1/enrollments")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
[Tags("Enrollments")]
public class EnrollmentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<EnrollmentsController> _logger;

    public EnrollmentsController(IMediator mediator, ILogger<EnrollmentsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Request body for enrolling an athlete in a training batch.
    /// </summary>
    public record EnrollAthleteRequest(Guid AthleteId);

    /// <summary>
    /// Request body for transferring an enrollment to another batch.
    /// </summary>
    public record TransferEnrollmentRequest(Guid SourceBatchId, Guid TargetBatchId);

    /// <summary>
    /// Enrolls an athlete in a training batch.
    /// </summary>
    /// <param name="batchId">The training batch's unique identifier</param>
    /// <param name="request">Enrollment details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created enrollment</returns>
    /// <response code="201">Athlete enrolled successfully</response>
    /// <response code="400">Validation error or batch not active/at capacity</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Batch or athlete not found</response>
    /// <response code="409">Athlete is already enrolled in this batch</response>
    [HttpPost("~/api/v1/training-batches/{batchId:guid}/enrollments")]
    [Authorize(Roles = "Coach,System Admin,Academy Admin")]
    [ProducesResponseType(typeof(ApiResponse<EnrollmentDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> EnrollAthlete(
        Guid batchId,
        [FromBody] EnrollAthleteRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Enrolling athlete {AthleteId} in batch: {BatchId}", request.AthleteId, batchId);

        var command = new EnrollAthleteCommand
        {
            BatchId = batchId,
            AthleteId = request.AthleteId
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Athlete enrolled: {EnrollmentId}", result.Value!.Id);

        return CreatedAtAction(
            nameof(CancelEnrollment),
            new { enrollmentId = result.Value.Id, version = "1.0" },
            ApiResponse<EnrollmentDto>.SuccessResult(result.Value, "Athlete enrolled successfully."));
    }

    /// <summary>
    /// Gets all enrollments for a specific training batch.
    /// </summary>
    /// <param name="batchId">The training batch's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of enrollments</returns>
    /// <response code="200">Enrollments retrieved successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Training batch not found</response>
    [HttpGet("~/api/v1/training-batches/{batchId:guid}/enrollments")]
    [Authorize(Roles = "Coach,System Admin,Academy Admin")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<EnrollmentDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEnrollmentsByBatch(
        Guid batchId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetEnrollmentsByBatchQuery { BatchId = batchId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<IReadOnlyList<EnrollmentDto>>.SuccessResult(result.Value!, "Enrollments retrieved successfully."));
    }

    /// <summary>
    /// Cancels an enrollment, removing the athlete from the batch.
    /// </summary>
    /// <param name="enrollmentId">The enrollment's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Cancelled enrollment details</returns>
    /// <response code="200">Enrollment cancelled successfully</response>
    /// <response code="400">Enrollment is not in a cancellable state</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Enrollment not found</response>
    [HttpDelete("{enrollmentId:guid}")]
    [Authorize(Roles = "Coach,System Admin,Academy Admin")]
    [ProducesResponseType(typeof(ApiResponse<EnrollmentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelEnrollment(
        Guid enrollmentId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Cancelling enrollment: {EnrollmentId}", enrollmentId);

        var result = await _mediator.Send(new CancelEnrollmentCommand { EnrollmentId = enrollmentId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Enrollment cancelled: {EnrollmentId}", enrollmentId);

        return Ok(ApiResponse<EnrollmentDto>.SuccessResult(result.Value!, "Enrollment cancelled successfully."));
    }

    /// <summary>
    /// Transfers an enrollment from one batch to another.
    /// </summary>
    /// <param name="enrollmentId">The enrollment's unique identifier</param>
    /// <param name="request">Transfer details including source and target batch IDs</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Transferred enrollment details</returns>
    /// <response code="200">Enrollment transferred successfully</response>
    /// <response code="400">Validation error or target batch not active/at capacity</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Enrollment or batch not found</response>
    /// <response code="409">Athlete is already enrolled in the target batch</response>
    [HttpPost("{enrollmentId:guid}/transfer")]
    [Authorize(Roles = "Coach,System Admin,Academy Admin")]
    [ProducesResponseType(typeof(ApiResponse<EnrollmentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> TransferEnrollment(
        Guid enrollmentId,
        [FromBody] TransferEnrollmentRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Transferring enrollment: {EnrollmentId}", enrollmentId);

        var command = new TransferEnrollmentCommand
        {
            EnrollmentId = enrollmentId,
            SourceBatchId = request.SourceBatchId,
            TargetBatchId = request.TargetBatchId
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Enrollment transferred: {EnrollmentId}", enrollmentId);

        return Ok(ApiResponse<EnrollmentDto>.SuccessResult(result.Value!, "Enrollment transferred successfully."));
    }

    /// <summary>
    /// Marks an enrollment as completed.
    /// </summary>
    /// <param name="enrollmentId">The enrollment's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Completed enrollment details</returns>
    /// <response code="200">Enrollment completed successfully</response>
    /// <response code="400">Enrollment is not in a completable state</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Enrollment not found</response>
    [HttpPost("{enrollmentId:guid}/complete")]
    [Authorize(Roles = "Coach,System Admin,Academy Admin")]
    [ProducesResponseType(typeof(ApiResponse<EnrollmentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CompleteEnrollment(
        Guid enrollmentId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Completing enrollment: {EnrollmentId}", enrollmentId);

        var result = await _mediator.Send(new CompleteEnrollmentCommand { EnrollmentId = enrollmentId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Enrollment completed: {EnrollmentId}", enrollmentId);

        return Ok(ApiResponse<EnrollmentDto>.SuccessResult(result.Value!, "Enrollment completed successfully."));
    }

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
