using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Api.Common.Models.SwaggerExamples;
using SportsGurukul.Application.Features.AcademyManagement.Commands.CreateBranch;
using SportsGurukul.Application.Features.AcademyManagement.Commands.DeleteBranch;
using SportsGurukul.Application.Features.AcademyManagement.Commands.RestoreBranch;
using SportsGurukul.Application.Features.AcademyManagement.Commands.UpdateBranch;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;
using SportsGurukul.Application.Features.AcademyManagement.Queries.GetBranches;
using SportsGurukul.Application.Features.Authentication.DTOs.Responses;
using Swashbuckle.AspNetCore.Filters;

namespace SportsGurukul.Api.Controllers.V1;

/// <summary>
/// Manages academy branches — creation, retrieval, update, deletion, and restoration.
/// </summary>
[ApiController]
[Route("api/v1/academies/{academyId:guid}/branches")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
[Tags("Academy Branches")]
public class BranchController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<BranchController> _logger;

    public BranchController(IMediator mediator, ILogger<BranchController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new branch under an academy.
    /// </summary>
    /// <param name="academyId">The academy's unique identifier</param>
    /// <param name="request">Branch details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created branch</returns>
    /// <response code="201">Branch created successfully</response>
    /// <response code="400">Validation error</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Academy not found</response>
    [HttpPost]
    [Authorize(Roles = "Academy Admin,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<BranchDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [SwaggerRequestExample(typeof(CreateBranchRequest), typeof(CreateBranchRequestExample))]
    public async Task<IActionResult> CreateBranch(
        Guid academyId,
        [FromBody] CreateBranchRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating branch for academy: {AcademyId}", academyId);

        var command = new CreateBranchCommand
        {
            AcademyId = academyId,
            BranchName = request.BranchName,
            Address = request.Address,
            Country = request.Country,
            State = request.State,
            City = request.City,
            District = request.District,
            PostalCode = request.PostalCode,
            Latitude = request.Latitude,
            Longitude = request.Longitude
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Branch created: {BranchId}", result.Value!.Id);

        return CreatedAtAction(
            nameof(GetBranchById),
            new { academyId, branchId = result.Value.Id, version = "1.0" },
            ApiResponse<BranchDto>.SuccessResult(result.Value, "Branch created successfully."));
    }

    /// <summary>
    /// Gets all branches for an academy.
    /// </summary>
    /// <param name="academyId">The academy's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of branches</returns>
    /// <response code="200">Branches retrieved successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="404">Academy not found</response>
    [HttpGet]
    [Authorize(Roles = "Academy Admin,Coach,Athlete,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<BranchDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBranches(
        Guid academyId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching branches for academy: {AcademyId}", academyId);

        var result = await _mediator.Send(new GetBranchesQuery { AcademyId = academyId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<IReadOnlyList<BranchDto>>.SuccessResult(result.Value!, "Branches retrieved successfully."));
    }

    /// <summary>
    /// Gets a specific branch by its unique identifier.
    /// </summary>
    /// <param name="academyId">The academy's unique identifier</param>
    /// <param name="branchId">The branch's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Branch details</returns>
    /// <response code="200">Branch retrieved successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="404">Branch not found</response>
    [HttpGet("{branchId:guid}")]
    [Authorize(Roles = "Academy Admin,Coach,Athlete,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<BranchDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(BranchDtoExample))]
    public async Task<IActionResult> GetBranchById(
        Guid academyId,
        Guid branchId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching branch: {BranchId} for academy: {AcademyId}", branchId, academyId);

        var result = await _mediator.Send(new GetBranchesQuery { AcademyId = academyId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        var branch = result.Value!.FirstOrDefault(b => b.Id == branchId);

        if (branch is null)
        {
            return NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Not Found",
                Detail = $"Branch with ID {branchId} not found.",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4"
            });
        }

        return Ok(ApiResponse<BranchDto>.SuccessResult(branch, "Branch retrieved successfully."));
    }

    /// <summary>
    /// Updates a branch. All fields are optional — only supplied fields are applied.
    /// </summary>
    /// <param name="academyId">The academy's unique identifier</param>
    /// <param name="branchId">The branch's unique identifier</param>
    /// <param name="request">Fields to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated branch</returns>
    /// <response code="200">Branch updated successfully</response>
    /// <response code="400">Validation error</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Branch not found</response>
    [HttpPut("{branchId:guid}")]
    [Authorize(Roles = "Academy Admin,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<BranchDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [SwaggerRequestExample(typeof(UpdateBranchRequest), typeof(UpdateBranchRequestExample))]
    public async Task<IActionResult> UpdateBranch(
        Guid academyId,
        Guid branchId,
        [FromBody] UpdateBranchRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating branch: {BranchId} for academy: {AcademyId}", branchId, academyId);

        var command = new UpdateBranchCommand
        {
            BranchId = branchId,
            AcademyId = academyId,
            BranchName = request.BranchName,
            Address = request.Address,
            Country = request.Country,
            State = request.State,
            City = request.City,
            District = request.District,
            PostalCode = request.PostalCode,
            Latitude = request.Latitude,
            Longitude = request.Longitude
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Branch updated: {BranchId}", branchId);

        return Ok(ApiResponse<BranchDto>.SuccessResult(result.Value!, "Branch updated successfully."));
    }

    /// <summary>
    /// Soft-deletes a branch.
    /// </summary>
    /// <param name="academyId">The academy's unique identifier</param>
    /// <param name="branchId">The branch's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content</returns>
    /// <response code="204">Branch deleted successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Branch not found</response>
    [HttpDelete("{branchId:guid}")]
    [Authorize(Roles = "Academy Admin,System Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteBranch(
        Guid academyId,
        Guid branchId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting branch: {BranchId} from academy: {AcademyId}", branchId, academyId);

        var result = await _mediator.Send(new DeleteBranchCommand { BranchId = branchId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Branch deleted: {BranchId}", branchId);

        return NoContent();
    }

    /// <summary>
    /// Restores a previously soft-deleted branch.
    /// </summary>
    /// <param name="academyId">The academy's unique identifier</param>
    /// <param name="branchId">The branch's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Restored branch</returns>
    /// <response code="200">Branch restored successfully</response>
    /// <response code="400">Branch is not deleted or cannot be restored</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">No deleted branch found with this ID</response>
    [HttpPost("{branchId:guid}/restore")]
    [Authorize(Roles = "Academy Admin,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<BranchDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RestoreBranch(
        Guid academyId,
        Guid branchId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Restoring branch: {BranchId} for academy: {AcademyId}", branchId, academyId);

        var result = await _mediator.Send(new RestoreBranchCommand { BranchId = branchId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Branch restored: {BranchId}", branchId);

        return Ok(ApiResponse<BranchDto>.SuccessResult(result.Value!, "Branch restored successfully."));
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

        if (error.Contains("already exists", StringComparison.OrdinalIgnoreCase) ||
            error.Contains("already associated", StringComparison.OrdinalIgnoreCase))
        {
            return Conflict(new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Conflict",
                Detail = error,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8"
            });
        }

        if (error.Contains("deleted", StringComparison.OrdinalIgnoreCase) &&
            error.Contains("restore", StringComparison.OrdinalIgnoreCase))
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
