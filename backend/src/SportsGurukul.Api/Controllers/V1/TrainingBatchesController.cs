using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Batch.CreateTrainingBatch;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Batch.UpdateTrainingBatch;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Batch.StartTrainingBatch;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Batch.CompleteTrainingBatch;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Batch.CancelTrainingBatch;
using SportsGurukul.Application.Features.TrainingProgramManagement.Queries.GetTrainingBatchQuery;
using SportsGurukul.Application.Features.TrainingProgramManagement.Queries.GetBatchesByProgramQuery;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;

namespace SportsGurukul.Api.Controllers.V1;

/// <summary>
/// Manages training batch operations including creation under programs, enrollment management, and batch lifecycle.
/// </summary>
[ApiController]
[Route("api/v1/training-batches")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
[Tags("Training Batches")]
public class TrainingBatchesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TrainingBatchesController> _logger;

    public TrainingBatchesController(IMediator mediator, ILogger<TrainingBatchesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Request body for creating a training batch.
    /// </summary>
    public record CreateBatchRequest(
        Guid CoachId,
        Guid BranchId,
        DateTime StartDate,
        DateTime? EndDate,
        int MaximumSeats);

    /// <summary>
    /// Creates a new training batch under a training program.
    /// </summary>
    /// <param name="programId">The parent training program's unique identifier</param>
    /// <param name="request">Batch details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created training batch</returns>
    /// <response code="201">Training batch created successfully</response>
    /// <response code="400">Validation error</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Training program not found</response>
    /// <response code="409">Batch code already exists</response>
    [HttpPost("~/api/v1/training-programs/{programId:guid}/batches")]
    [Authorize(Roles = "System Admin,Academy Admin")]
    [ProducesResponseType(typeof(ApiResponse<TrainingBatchDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateBatch(
        Guid programId,
        [FromBody] CreateBatchRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating training batch for program: {ProgramId}", programId);

        var command = new CreateTrainingBatchCommand(programId, request.CoachId, request.BranchId, request.StartDate, request.EndDate, request.MaximumSeats);

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Training batch created: {Id}", result.Value!.Id);

        return CreatedAtAction(
            nameof(GetBatch),
            new { batchId = result.Value.Id, version = "1.0" },
            ApiResponse<TrainingBatchDto>.SuccessResult(result.Value, "Training batch created successfully."));
    }

    /// <summary>
    /// Gets all training batches for a specific training program.
    /// </summary>
    /// <param name="programId">The parent training program's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of training batches</returns>
    /// <response code="200">Training batches retrieved successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="404">Training program not found</response>
    [HttpGet("~/api/v1/training-programs/{programId:guid}/batches")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<TrainingBatchDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBatchesByProgram(
        Guid programId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetBatchesByProgramQuery { ProgramId = programId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<IReadOnlyList<TrainingBatchDto>>.SuccessResult(result.Value!, "Training batches retrieved successfully."));
    }

    /// <summary>
    /// Gets a training batch by its unique identifier.
    /// </summary>
    /// <param name="batchId">The training batch's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Training batch details</returns>
    /// <response code="200">Training batch retrieved successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="404">Training batch not found</response>
    [HttpGet("{batchId:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<TrainingBatchDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBatch(
        Guid batchId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetTrainingBatchQuery { Id = batchId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<TrainingBatchDto>.SuccessResult(result.Value!, "Training batch retrieved successfully."));
    }

    /// <summary>
    /// Updates a training batch.
    /// </summary>
    /// <param name="batchId">The training batch's unique identifier</param>
    /// <param name="request">Batch fields to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated training batch</returns>
    /// <response code="200">Training batch updated successfully</response>
    /// <response code="400">Validation error</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Training batch not found</response>
    [HttpPut("{batchId:guid}")]
    [Authorize(Roles = "System Admin,Academy Admin")]
    [ProducesResponseType(typeof(ApiResponse<TrainingBatchDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateBatch(
        Guid batchId,
        [FromBody] UpdateTrainingBatchCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating training batch: {BatchId}", batchId);

        var command = request with { Id = batchId };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Training batch updated: {BatchId}", batchId);

        return Ok(ApiResponse<TrainingBatchDto>.SuccessResult(result.Value!, "Training batch updated successfully."));
    }

    /// <summary>
    /// Starts a training batch, changing its status to Active.
    /// </summary>
    /// <param name="batchId">The training batch's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Started training batch</returns>
    /// <response code="200">Training batch started successfully</response>
    /// <response code="400">Training batch is not in a startable state</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Training batch not found</response>
    [HttpPost("{batchId:guid}/start")]
    [Authorize(Roles = "System Admin,Academy Admin")]
    [ProducesResponseType(typeof(ApiResponse<TrainingBatchDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> StartBatch(
        Guid batchId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting training batch: {BatchId}", batchId);

        var result = await _mediator.Send(new StartTrainingBatchCommand(batchId), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Training batch started: {BatchId}", batchId);

        return Ok(ApiResponse<TrainingBatchDto>.SuccessResult(result.Value!, "Training batch started successfully."));
    }

    /// <summary>
    /// Completes a training batch, changing its status to Completed.
    /// </summary>
    /// <param name="batchId">The training batch's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Completed training batch</returns>
    /// <response code="200">Training batch completed successfully</response>
    /// <response code="400">Training batch is not in a completable state</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Training batch not found</response>
    [HttpPost("{batchId:guid}/complete")]
    [Authorize(Roles = "System Admin,Academy Admin")]
    [ProducesResponseType(typeof(ApiResponse<TrainingBatchDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CompleteBatch(
        Guid batchId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Completing training batch: {BatchId}", batchId);

        var result = await _mediator.Send(new CompleteTrainingBatchCommand(batchId), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Training batch completed: {BatchId}", batchId);

        return Ok(ApiResponse<TrainingBatchDto>.SuccessResult(result.Value!, "Training batch completed successfully."));
    }

    /// <summary>
    /// Cancels a training batch, changing its status to Cancelled.
    /// </summary>
    /// <param name="batchId">The training batch's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Cancelled training batch</returns>
    /// <response code="200">Training batch cancelled successfully</response>
    /// <response code="400">Training batch is not in a cancellable state</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Training batch not found</response>
    [HttpPost("{batchId:guid}/cancel")]
    [Authorize(Roles = "System Admin,Academy Admin")]
    [ProducesResponseType(typeof(ApiResponse<TrainingBatchDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelBatch(
        Guid batchId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Cancelling training batch: {BatchId}", batchId);

        var result = await _mediator.Send(new CancelTrainingBatchCommand(batchId), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Training batch cancelled: {BatchId}", batchId);

        return Ok(ApiResponse<TrainingBatchDto>.SuccessResult(result.Value!, "Training batch cancelled successfully."));
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
