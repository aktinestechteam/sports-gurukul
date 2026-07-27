using System.Net.Mime;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Api.Common.Models.SwaggerExamples;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.ApproveBooking;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.RejectBookingApproval;
using SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;
using Swashbuckle.AspNetCore.Filters;

namespace SportsGurukul.Api.Controllers.V1;

/// <summary>
/// Manages the booking approval workflow — approve or reject bookings pending authorization.
/// </summary>
[ApiController]
[Route("api/v1/bookings/{bookingId:guid}/approval")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
[Tags("Booking Approvals")]
public class BookingApprovalsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<BookingApprovalsController> _logger;

    public BookingApprovalsController(IMediator mediator, ILogger<BookingApprovalsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Approves a pending booking.
    /// </summary>
    /// <param name="bookingId">The booking's unique identifier</param>
    /// <param name="request">Approval details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Approved booking</returns>
    /// <response code="200">Booking approved successfully</response>
    /// <response code="400">Booking not in approvable state</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Booking not found</response>
    [HttpPost("approve")]
    [Authorize(Roles = "Academy Admin,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<BookingDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [SwaggerRequestExample(typeof(ApprovalActionRequest), typeof(ApprovalActionRequestExample))]
    public async Task<IActionResult> ApproveBooking(
        Guid bookingId,
        [FromBody] ApprovalActionRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Approving booking: {BookingId}", bookingId);

        var approverUserId = GetUserId();
        if (approverUserId is null)
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Bad Request",
                Detail = "Unable to identify the approver from the current user context.",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
            });
        }

        var command = new ApproveBookingCommand
        {
            BookingId = bookingId,
            ApproverUserId = approverUserId.Value,
            Comments = request.Comments
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Booking approved: {BookingId}", bookingId);

        return Ok(ApiResponse<BookingDto>.SuccessResult(result.Value!, "Booking approved successfully."));
    }

    /// <summary>
    /// Rejects a pending booking approval.
    /// </summary>
    /// <param name="bookingId">The booking's unique identifier</param>
    /// <param name="request">Rejection details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Rejected booking</returns>
    /// <response code="200">Booking approval rejected successfully</response>
    /// <response code="400">Booking not in rejectable state</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Booking not found</response>
    [HttpPost("reject")]
    [Authorize(Roles = "Academy Admin,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<BookingDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [SwaggerRequestExample(typeof(ApprovalActionRequest), typeof(ApprovalActionRequestExample))]
    public async Task<IActionResult> RejectBookingApproval(
        Guid bookingId,
        [FromBody] ApprovalActionRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Rejecting booking approval: {BookingId}", bookingId);

        var approverUserId = GetUserId();
        if (approverUserId is null)
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Bad Request",
                Detail = "Unable to identify the approver from the current user context.",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
            });
        }

        var command = new RejectBookingApprovalCommand
        {
            BookingId = bookingId,
            ApproverUserId = approverUserId.Value,
            Comments = request.Comments
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Booking approval rejected: {BookingId}", bookingId);

        return Ok(ApiResponse<BookingDto>.SuccessResult(result.Value!, "Booking approval rejected successfully."));
    }

    #region Helpers

    private Guid? GetUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return null;
        return userId;
    }

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
