using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Queries.GetBookingById;

namespace SportsGurukul.Api.Controllers.V1;

/// <summary>
/// Manages schedule entries (occurrences) within a booking, including recurring series overrides.
/// </summary>
[ApiController]
[Route("api/v1/bookings/{bookingId:guid}/schedules")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
[Tags("Booking Schedules")]
public class BookingSchedulesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<BookingSchedulesController> _logger;

    public BookingSchedulesController(IMediator mediator, ILogger<BookingSchedulesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Gets all schedule entries for a specific booking.
    /// </summary>
    /// <param name="bookingId">The booking's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of booking schedule entries</returns>
    /// <response code="200">Schedules retrieved successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="404">Booking not found</response>
    [HttpGet]
    [Authorize(Roles = "Academy Admin,Coach,Athlete,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<BookingScheduleDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBookingSchedules(
        Guid bookingId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching schedules for booking: {BookingId}", bookingId);

        var result = await _mediator.Send(new GetBookingByIdQuery { BookingId = bookingId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<IReadOnlyList<BookingScheduleDto>>.SuccessResult(result.Value!.Schedules, "Booking schedules retrieved successfully."));
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
