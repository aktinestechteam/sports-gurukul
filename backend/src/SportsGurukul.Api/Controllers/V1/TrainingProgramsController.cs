using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.TrainingProgram.CreateTrainingProgram;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.TrainingProgram.UpdateTrainingProgram;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.TrainingProgram.DeleteTrainingProgram;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.TrainingProgram.PublishTrainingProgram;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.TrainingProgram.ArchiveTrainingProgram;
using SportsGurukul.Application.Features.TrainingProgramManagement.Queries.GetTrainingProgramByIdQuery;
using SportsGurukul.Application.Features.TrainingProgramManagement.Queries.SearchTrainingProgramsQuery;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;

namespace SportsGurukul.Api.Controllers.V1;

/// <summary>
/// Manages training program lifecycle including creation, publishing, archiving, and search.
/// </summary>
[ApiController]
[Route("api/v1/training-programs")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
[Tags("Training Programs")]
public class TrainingProgramsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TrainingProgramsController> _logger;

    public TrainingProgramsController(IMediator mediator, ILogger<TrainingProgramsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new training program.
    /// </summary>
    /// <param name="request">Training program details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created training program</returns>
    /// <response code="201">Training program created successfully</response>
    /// <response code="400">Validation error</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="409">Training program with the same name already exists</response>
    [HttpPost]
    [Authorize(Roles = "System Admin,Academy Admin")]
    [ProducesResponseType(typeof(ApiResponse<TrainingProgramDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateProgram(
        [FromBody] CreateTrainingProgramCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating training program...");

        var result = await _mediator.Send(request, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Training program created: {Id}", result.Value!.Id);

        return CreatedAtAction(
            nameof(GetProgramById),
            new { programId = result.Value.Id, version = "1.0" },
            ApiResponse<TrainingProgramDto>.SuccessResult(result.Value, "Training program created successfully."));
    }

    /// <summary>
    /// Searches training programs with optional filters and pagination.
    /// </summary>
    /// <param name="academyId">Filter by academy ID</param>
    /// <param name="sportId">Filter by sport ID</param>
    /// <param name="status">Filter by status</param>
    /// <param name="searchTerm">Search term for program name</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated search results</returns>
    /// <response code="200">Search results returned successfully</response>
    /// <response code="401">Not authenticated</response>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<TrainingProgramSearchResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SearchPrograms(
        [FromQuery] Guid? academyId,
        [FromQuery] Guid? sportId,
        [FromQuery] string? status,
        [FromQuery] string? searchTerm,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new SearchTrainingProgramsQuery
        {
            AcademyId = academyId,
            SportId = sportId,
            Status = status,
            SearchTerm = searchTerm,
            Page = page,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<TrainingProgramSearchResponse>.SuccessResult(result.Value!, "Training programs retrieved successfully."));
    }

    /// <summary>
    /// Searches training programs with optional filters and pagination (explicit /search route).
    /// </summary>
    /// <param name="academyId">Filter by academy ID</param>
    /// <param name="sportId">Filter by sport ID</param>
    /// <param name="status">Filter by status</param>
    /// <param name="searchTerm">Search term for program name</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated search results</returns>
    /// <response code="200">Search results returned successfully</response>
    /// <response code="401">Not authenticated</response>
    [HttpGet("search")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<TrainingProgramSearchResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SearchProgramsAlias(
        [FromQuery] Guid? academyId,
        [FromQuery] Guid? sportId,
        [FromQuery] string? status,
        [FromQuery] string? searchTerm,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new SearchTrainingProgramsQuery
        {
            AcademyId = academyId,
            SportId = sportId,
            Status = status,
            SearchTerm = searchTerm,
            Page = page,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<TrainingProgramSearchResponse>.SuccessResult(result.Value!, "Training programs retrieved successfully."));
    }

    /// <summary>
    /// Gets a training program by its unique identifier.
    /// </summary>
    /// <param name="programId">The training program's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Training program details</returns>
    /// <response code="200">Training program retrieved successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="404">Training program not found</response>
    [HttpGet("{programId:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<TrainingProgramDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProgramById(
        Guid programId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetTrainingProgramByIdQuery { Id = programId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<TrainingProgramDto>.SuccessResult(result.Value!, "Training program retrieved successfully."));
    }

    /// <summary>
    /// Updates a training program. All fields are optional - only supplied fields are applied.
    /// </summary>
    /// <param name="programId">The training program's unique identifier</param>
    /// <param name="request">Fields to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated training program</returns>
    /// <response code="200">Training program updated successfully</response>
    /// <response code="400">Validation error</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Training program not found</response>
    [HttpPut("{programId:guid}")]
    [Authorize(Roles = "System Admin,Academy Admin")]
    [ProducesResponseType(typeof(ApiResponse<TrainingProgramDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProgram(
        Guid programId,
        [FromBody] UpdateTrainingProgramCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating training program: {ProgramId}", programId);

        request.Id = programId;

        var result = await _mediator.Send(request, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Training program updated: {ProgramId}", programId);

        return Ok(ApiResponse<TrainingProgramDto>.SuccessResult(result.Value!, "Training program updated successfully."));
    }

    /// <summary>
    /// Soft-deletes a training program. Requires System Admin role.
    /// </summary>
    /// <param name="programId">The training program's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Deleted training program details</returns>
    /// <response code="200">Training program deleted successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Training program not found</response>
    [HttpDelete("{programId:guid}")]
    [Authorize(Roles = "System Admin")]
    [ProducesResponseType(typeof(ApiResponse<TrainingProgramDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProgram(
        Guid programId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting training program: {ProgramId}", programId);

        var result = await _mediator.Send(new DeleteTrainingProgramCommand { Id = programId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Training program deleted: {ProgramId}", programId);

        return Ok(ApiResponse<TrainingProgramDto>.SuccessResult(result.Value!, "Training program deleted successfully."));
    }

    /// <summary>
    /// Publishes a training program, making it available for batch creation and enrollment.
    /// </summary>
    /// <param name="programId">The training program's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Published training program</returns>
    /// <response code="200">Training program published successfully</response>
    /// <response code="400">Training program is not in a publishable state</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Training program not found</response>
    [HttpPost("{programId:guid}/publish")]
    [Authorize(Roles = "System Admin,Academy Admin")]
    [ProducesResponseType(typeof(ApiResponse<TrainingProgramDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PublishProgram(
        Guid programId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Publishing training program: {ProgramId}", programId);

        var result = await _mediator.Send(new PublishTrainingProgramCommand { Id = programId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Training program published: {ProgramId}", programId);

        return Ok(ApiResponse<TrainingProgramDto>.SuccessResult(result.Value!, "Training program published successfully."));
    }

    /// <summary>
    /// Archives a training program, making it inactive.
    /// </summary>
    /// <param name="programId">The training program's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Archived training program</returns>
    /// <response code="200">Training program archived successfully</response>
    /// <response code="400">Training program is not in an archivable state</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Training program not found</response>
    [HttpPost("{programId:guid}/archive")]
    [Authorize(Roles = "System Admin,Academy Admin")]
    [ProducesResponseType(typeof(ApiResponse<TrainingProgramDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ArchiveProgram(
        Guid programId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Archiving training program: {ProgramId}", programId);

        var result = await _mediator.Send(new ArchiveTrainingProgramCommand { Id = programId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Training program archived: {ProgramId}", programId);

        return Ok(ApiResponse<TrainingProgramDto>.SuccessResult(result.Value!, "Training program archived successfully."));
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
