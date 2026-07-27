using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Progress.UpdateTrainingProgress;
using SportsGurukul.Application.Features.TrainingProgramManagement.Queries.GetTrainingProgressQuery;
using SportsGurukul.Application.Features.TrainingProgramManagement.Queries.GetMilestonesByProgramQuery;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Api.Controllers.V1;

/// <summary>
/// Manages athlete training progress tracking, milestone completion, and progress updates.
/// </summary>
[ApiController]
[Route("api/v1/enrollments")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
[Tags("Training Progress")]
public class ProgressController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProgressController> _logger;

    public ProgressController(IMediator mediator, ILogger<ProgressController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    #region Request Types

    public record UpdateProgressRequest
    {
        public string CurrentLevel { get; init; } = string.Empty;
        public decimal CompletedPercentage { get; init; }
        public decimal? OverallRating { get; init; }
    }

    public record GetMilestonesRequest
    {
        public Guid ProgramId { get; init; }
    }

    #endregion

    #region Progress

    /// <summary>
    /// Gets the training progress for a specific enrollment.
    /// </summary>
    /// <param name="enrollmentId">The enrollment's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Training progress details</returns>
    /// <response code="200">Progress retrieved successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Enrollment or progress not found</response>
    [HttpGet("{enrollmentId:guid}/progress")]
    [Authorize(Roles = "Athlete,System Admin,Academy Admin")]
    [ProducesResponseType(typeof(ApiResponse<TrainingProgressDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProgress(
        Guid enrollmentId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching training progress for enrollment {EnrollmentId}", enrollmentId);

        var result = await _mediator.Send(new GetTrainingProgressQuery { EnrollmentId = enrollmentId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<TrainingProgressDto>.SuccessResult(result.Value!, "Progress retrieved successfully."));
    }

    /// <summary>
    /// Updates the training progress for a specific enrollment.
    /// </summary>
    /// <param name="enrollmentId">The enrollment's unique identifier</param>
    /// <param name="request">Progress fields to update including level, percentage, and rating</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated training progress</returns>
    /// <response code="200">Progress updated successfully</response>
    /// <response code="400">Validation error</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Enrollment or progress not found</response>
    [HttpPut("{enrollmentId:guid}/progress")]
    [Authorize(Roles = "Coach,System Admin,Academy Admin")]
    [ProducesResponseType(typeof(ApiResponse<TrainingProgressDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProgress(
        Guid enrollmentId,
        [FromBody] UpdateProgressRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating training progress for enrollment {EnrollmentId}", enrollmentId);

        var command = new UpdateTrainingProgressCommand
        {
            EnrollmentId = enrollmentId,
            CurrentLevel = request.CurrentLevel,
            CompletedPercentage = request.CompletedPercentage,
            OverallRating = request.OverallRating
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Training progress updated for enrollment {EnrollmentId}", enrollmentId);

        return Ok(ApiResponse<TrainingProgressDto>.SuccessResult(result.Value!, "Progress updated successfully."));
    }

    /// <summary>
    /// Gets all milestones for the program associated with an enrollment.
    /// </summary>
    /// <param name="enrollmentId">The enrollment's unique identifier</param>
    /// <param name="request">Query parameters including the program ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of training milestones</returns>
    /// <response code="200">Milestones retrieved successfully</response>
    /// <response code="400">Validation error</response>
    /// <response code="404">Program not found</response>
    [HttpGet("{enrollmentId:guid}/milestones")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<TrainingMilestoneDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMilestones(
        Guid enrollmentId,
        [FromQuery] GetMilestonesRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching milestones for program {ProgramId} via enrollment {EnrollmentId}", request.ProgramId, enrollmentId);

        var result = await _mediator.Send(new GetMilestonesByProgramQuery { ProgramId = request.ProgramId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<IReadOnlyList<TrainingMilestoneDto>>.SuccessResult(result.Value!, "Milestones retrieved successfully."));
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
