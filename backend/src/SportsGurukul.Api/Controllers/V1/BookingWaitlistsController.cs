using System.Net.Mime;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Api.Common.Models.SwaggerExamples;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.JoinWaitlist;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.PromoteWaitlistedBooking;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.RemoveFromWaitlist;
using SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;
using Swashbuckle.AspNetCore.Filters;

namespace SportsGurukul.Api.Controllers.V1;

/// <summary>
/// Manages waitlist entries for fully booked time slots — joining, leaving, and promotion.
/// </summary>
[ApiController]
[Route("api/v1/bookings/{bookingId:guid}/waitlist")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
[Tags("Booking Waitlists")]
public class BookingWaitlistsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<BookingWaitlistsController> _logger;

    public BookingWaitlistsController(IMediator mediator, ILogger<BookingWaitlistsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Adds a user to the waitlist for a fully booked time slot.
    /// </summary>
    /// <param name="bookingId">The booking's unique identifier</param>
    /// <param name="request">Waitlist entry details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created waitlist entry</returns>
    /// <response code="201">Added to waitlist successfully</response>
    /// <response code="400">Validation error or booking is not full</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Booking not found</response>
    /// <response code="409">User already on waitlist</response>
    [HttpPost]
    [Authorize(Roles = "Academy Admin,Coach,Athlete,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<WaitlistDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [SwaggerRequestExample(typeof(JoinWaitlistApiRequest), typeof(JoinWaitlistApiRequestExample))]
    public async Task<IActionResult> JoinWaitlist(
        Guid bookingId,
        [FromBody] JoinWaitlistApiRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Adding user {UserId} to waitlist for booking: {BookingId}", request.WaitlistUserId, bookingId);

        var command = new JoinWaitlistCommand
        {
            BookingId = bookingId,
            WaitlistUserId = request.WaitlistUserId,
            Notes = request.Notes
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("User added to waitlist for booking: {BookingId}", bookingId);

        return CreatedAtAction(
            nameof(JoinWaitlist),
            new { bookingId, version = "1.0" },
            ApiResponse<WaitlistDto>.SuccessResult(result.Value!, "Added to waitlist successfully."));
    }

    /// <summary>
    /// Removes a user from the waitlist.
    /// </summary>
    /// <param name="bookingId">The booking's unique identifier</param>
    /// <param name="waitlistEntryId">The waitlist entry's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content</returns>
    /// <response code="204">Removed from waitlist successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Waitlist entry not found</response>
    [HttpDelete("{waitlistEntryId:guid}")]
    [Authorize(Roles = "Academy Admin,Coach,Athlete,System Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveFromWaitlist(
        Guid bookingId,
        Guid waitlistEntryId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Removing waitlist entry {WaitlistEntryId} from booking: {BookingId}", waitlistEntryId, bookingId);

        var result = await _mediator.Send(new RemoveFromWaitlistCommand { WaitlistEntryId = waitlistEntryId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Waitlist entry removed: {WaitlistEntryId}", waitlistEntryId);

        return NoContent();
    }

    /// <summary>
    /// Promotes the next waitlisted user to a confirmed booking.
    /// </summary>
    /// <param name="bookingId">The booking's unique identifier</param>
    /// <param name="waitlistEntryId">The waitlist entry's unique identifier to promote</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Promoted booking</returns>
    /// <response code="200">Waitlisted booking promoted successfully</response>
    /// <response code="400">Entry not in promotable state</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Waitlist entry not found</response>
    [HttpPost("{waitlistEntryId:guid}/promote")]
    [Authorize(Roles = "Academy Admin,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<BookingDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PromoteWaitlistedBooking(
        Guid bookingId,
        Guid waitlistEntryId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Promoting waitlist entry {WaitlistEntryId} for booking: {BookingId}", waitlistEntryId, bookingId);

        var result = await _mediator.Send(new PromoteWaitlistedBookingCommand { WaitlistEntryId = waitlistEntryId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Waitlist entry promoted: {WaitlistEntryId}", waitlistEntryId);

        return Ok(ApiResponse<BookingDto>.SuccessResult(result.Value!, "Waitlisted booking promoted successfully."));
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

        if (error.Contains("already exists", StringComparison.OrdinalIgnoreCase) ||
            error.Contains("already", StringComparison.OrdinalIgnoreCase))
        {
            return Conflict(new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Conflict",
                Detail = error,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8"
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
