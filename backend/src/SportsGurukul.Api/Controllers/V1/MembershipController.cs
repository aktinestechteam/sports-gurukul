using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Api.Common.Models.SwaggerExamples;
using SportsGurukul.Application.Features.AcademyManagement.Commands.ActivateMembershipPlan;
using SportsGurukul.Application.Features.AcademyManagement.Commands.CreateMembershipPlan;
using SportsGurukul.Application.Features.AcademyManagement.Commands.DeactivateMembershipPlan;
using SportsGurukul.Application.Features.AcademyManagement.Commands.DeleteMembershipPlan;
using SportsGurukul.Application.Features.AcademyManagement.Commands.UpdateMembershipPlan;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;
using SportsGurukul.Application.Features.AcademyManagement.Queries.GetMembershipPlans;
using SportsGurukul.Application.Features.Authentication.DTOs.Responses;
using Swashbuckle.AspNetCore.Filters;

namespace SportsGurukul.Api.Controllers.V1;

/// <summary>
/// Manages academy membership plans — creation, retrieval, update, activation, deactivation, and deletion.
/// </summary>
[ApiController]
[Route("api/v1/academies/{academyId:guid}/memberships")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
[Tags("Academy Memberships")]
public class MembershipController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<MembershipController> _logger;

    public MembershipController(IMediator mediator, ILogger<MembershipController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new membership plan under an academy.
    /// </summary>
    /// <param name="academyId">The academy's unique identifier</param>
    /// <param name="request">Membership plan details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created membership plan</returns>
    /// <response code="201">Membership plan created successfully</response>
    /// <response code="400">Validation error</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Academy not found</response>
    [HttpPost]
    [Authorize(Roles = "Academy Admin,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<MembershipPlanDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [SwaggerRequestExample(typeof(CreateMembershipPlanRequest), typeof(CreateMembershipPlanRequestExample))]
    public async Task<IActionResult> CreateMembershipPlan(
        Guid academyId,
        [FromBody] CreateMembershipPlanRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating membership plan for academy: {AcademyId}", academyId);

        var command = new CreateMembershipPlanCommand
        {
            AcademyId = academyId,
            MembershipName = request.MembershipName,
            Description = request.Description,
            Price = request.Price,
            Duration = request.Duration,
            Benefits = request.Benefits
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Membership plan created: {MembershipId}", result.Value!.Id);

        return CreatedAtAction(
            nameof(GetMembershipPlanById),
            new { academyId, membershipId = result.Value.Id, version = "1.0" },
            ApiResponse<MembershipPlanDto>.SuccessResult(result.Value, "Membership plan created successfully."));
    }

    /// <summary>
    /// Gets all membership plans for an academy.
    /// </summary>
    /// <param name="academyId">The academy's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of membership plans</returns>
    /// <response code="200">Membership plans retrieved successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="404">Academy not found</response>
    [HttpGet]
    [Authorize(Roles = "Academy Admin,Coach,Athlete,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<MembershipPlanDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMembershipPlans(
        Guid academyId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching membership plans for academy: {AcademyId}", academyId);

        var result = await _mediator.Send(new GetMembershipPlansQuery { AcademyId = academyId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<IReadOnlyList<MembershipPlanDto>>.SuccessResult(result.Value!, "Membership plans retrieved successfully."));
    }

    /// <summary>
    /// Gets a specific membership plan by its unique identifier.
    /// </summary>
    /// <param name="academyId">The academy's unique identifier</param>
    /// <param name="membershipId">The membership plan's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Membership plan details</returns>
    /// <response code="200">Membership plan retrieved successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="404">Membership plan not found</response>
    [HttpGet("{membershipId:guid}")]
    [Authorize(Roles = "Academy Admin,Coach,Athlete,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<MembershipPlanDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(MembershipPlanDtoExample))]
    public async Task<IActionResult> GetMembershipPlanById(
        Guid academyId,
        Guid membershipId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching membership plan: {MembershipId} for academy: {AcademyId}", membershipId, academyId);

        var result = await _mediator.Send(new GetMembershipPlansQuery { AcademyId = academyId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        var plan = result.Value!.FirstOrDefault(m => m.Id == membershipId);

        if (plan is null)
        {
            return NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Not Found",
                Detail = $"Membership plan with ID {membershipId} not found.",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4"
            });
        }

        return Ok(ApiResponse<MembershipPlanDto>.SuccessResult(plan, "Membership plan retrieved successfully."));
    }

    /// <summary>
    /// Updates a membership plan. All fields are optional — only supplied fields are applied.
    /// </summary>
    /// <param name="academyId">The academy's unique identifier</param>
    /// <param name="membershipId">The membership plan's unique identifier</param>
    /// <param name="request">Fields to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated membership plan</returns>
    /// <response code="200">Membership plan updated successfully</response>
    /// <response code="400">Validation error</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Membership plan not found</response>
    [HttpPut("{membershipId:guid}")]
    [Authorize(Roles = "Academy Admin,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<MembershipPlanDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [SwaggerRequestExample(typeof(UpdateMembershipPlanRequest), typeof(UpdateMembershipPlanRequestExample))]
    public async Task<IActionResult> UpdateMembershipPlan(
        Guid academyId,
        Guid membershipId,
        [FromBody] UpdateMembershipPlanRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating membership plan: {MembershipId} for academy: {AcademyId}", membershipId, academyId);

        var command = new UpdateMembershipPlanCommand
        {
            MembershipId = membershipId,
            AcademyId = academyId,
            MembershipName = request.MembershipName,
            Description = request.Description,
            Price = request.Price,
            Duration = request.Duration,
            Benefits = request.Benefits
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Membership plan updated: {MembershipId}", membershipId);

        return Ok(ApiResponse<MembershipPlanDto>.SuccessResult(result.Value!, "Membership plan updated successfully."));
    }

    /// <summary>
    /// Deletes a membership plan.
    /// </summary>
    /// <param name="academyId">The academy's unique identifier</param>
    /// <param name="membershipId">The membership plan's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content</returns>
    /// <response code="204">Membership plan deleted successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Membership plan not found</response>
    [HttpDelete("{membershipId:guid}")]
    [Authorize(Roles = "Academy Admin,System Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMembershipPlan(
        Guid academyId,
        Guid membershipId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting membership plan: {MembershipId} from academy: {AcademyId}", membershipId, academyId);

        var result = await _mediator.Send(new DeleteMembershipPlanCommand { MembershipId = membershipId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Membership plan deleted: {MembershipId}", membershipId);

        return NoContent();
    }

    /// <summary>
    /// Activates a membership plan. Only inactive plans can be activated.
    /// </summary>
    /// <param name="academyId">The academy's unique identifier</param>
    /// <param name="membershipId">The membership plan's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Activated membership plan</returns>
    /// <response code="200">Membership plan activated successfully</response>
    /// <response code="400">Plan is already active or cannot be activated</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Membership plan not found</response>
    [HttpPost("{membershipId:guid}/activate")]
    [Authorize(Roles = "Academy Admin,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<MembershipPlanDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActivateMembershipPlan(
        Guid academyId,
        Guid membershipId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Activating membership plan: {MembershipId} for academy: {AcademyId}", membershipId, academyId);

        var result = await _mediator.Send(new ActivateMembershipPlanCommand { MembershipId = membershipId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Membership plan activated: {MembershipId}", membershipId);

        return Ok(ApiResponse<MembershipPlanDto>.SuccessResult(result.Value!, "Membership plan activated successfully."));
    }

    /// <summary>
    /// Deactivates a membership plan. Only active plans can be deactivated.
    /// </summary>
    /// <param name="academyId">The academy's unique identifier</param>
    /// <param name="membershipId">The membership plan's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Deactivated membership plan</returns>
    /// <response code="200">Membership plan deactivated successfully</response>
    /// <response code="400">Plan is already inactive or cannot be deactivated</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Membership plan not found</response>
    [HttpPost("{membershipId:guid}/deactivate")]
    [Authorize(Roles = "Academy Admin,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<MembershipPlanDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateMembershipPlan(
        Guid academyId,
        Guid membershipId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deactivating membership plan: {MembershipId} for academy: {AcademyId}", membershipId, academyId);

        var result = await _mediator.Send(new DeactivateMembershipPlanCommand { MembershipId = membershipId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Membership plan deactivated: {MembershipId}", membershipId);

        return Ok(ApiResponse<MembershipPlanDto>.SuccessResult(result.Value!, "Membership plan deactivated successfully."));
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

        if ((error.Contains("already active", StringComparison.OrdinalIgnoreCase) ||
             error.Contains("already inactive", StringComparison.OrdinalIgnoreCase) ||
             error.Contains("is active", StringComparison.OrdinalIgnoreCase) ||
             error.Contains("is inactive", StringComparison.OrdinalIgnoreCase)) &&
            (error.Contains("activate", StringComparison.OrdinalIgnoreCase) ||
             error.Contains("deactivate", StringComparison.OrdinalIgnoreCase)))
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
