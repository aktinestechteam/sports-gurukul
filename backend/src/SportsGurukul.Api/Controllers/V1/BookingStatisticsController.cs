using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Api.Common.Models.SwaggerExamples;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.CancelReminder;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.ScheduleReminder;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.SendReminder;
using SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Queries.GetBookingStatistics;
using Swashbuckle.AspNetCore.Filters;

namespace SportsGurukul.Api.Controllers.V1;

/// <summary>
/// Provides booking analytics/statistics and manages booking reminders.
/// </summary>
[ApiController]
[Route("api/v1/booking-statistics")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
[Tags("Booking Statistics")]
public class BookingStatisticsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<BookingStatisticsController> _logger;

    public BookingStatisticsController(IMediator mediator, ILogger<BookingStatisticsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Gets booking statistics for an academy within an optional date range.
    /// </summary>
    /// <param name="academyId">The academy's unique identifier</param>
    /// <param name="startDate">Optional start date for the statistics period</param>
    /// <param name="endDate">Optional end date for the statistics period</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Booking statistics</returns>
    /// <response code="200">Statistics retrieved successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    [HttpGet]
    [Authorize(Roles = "Academy Admin,Coach,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<BookingStatisticsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetBookingStatistics(
        [FromQuery] Guid academyId,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching booking statistics for academy: {AcademyId}", academyId);

        var query = new GetBookingStatisticsQuery
        {
            AcademyId = academyId,
            StartDate = startDate,
            EndDate = endDate
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<BookingStatisticsDto>.SuccessResult(result.Value!, "Booking statistics retrieved successfully."));
    }

    /// <summary>
    /// Schedules a reminder for a booking.
    /// </summary>
    /// <param name="bookingId">The booking's unique identifier</param>
    /// <param name="request">Reminder details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created reminder</returns>
    /// <response code="201">Reminder scheduled successfully</response>
    /// <response code="400">Validation error</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Booking not found</response>
    [HttpPost("{bookingId:guid}/reminders")]
    [Authorize(Roles = "Academy Admin,Coach,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<ReminderDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [SwaggerRequestExample(typeof(ScheduleReminderApiRequest), typeof(ScheduleReminderApiRequestExample))]
    public async Task<IActionResult> ScheduleReminder(
        Guid bookingId,
        [FromBody] ScheduleReminderApiRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Scheduling reminder for booking: {BookingId}", bookingId);

        var command = new ScheduleReminderCommand
        {
            BookingId = bookingId,
            ReminderMinutesBefore = request.ReminderMinutesBefore,
            Channel = request.Channel,
            Notes = request.Notes
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Reminder scheduled for booking: {BookingId}", bookingId);

        return CreatedAtAction(
            nameof(ScheduleReminder),
            new { bookingId, version = "1.0" },
            ApiResponse<ReminderDto>.SuccessResult(result.Value!, "Reminder scheduled successfully."));
    }

    /// <summary>
    /// Sends a scheduled reminder immediately, optionally overriding the notification channel.
    /// </summary>
    /// <param name="reminderId">The reminder's unique identifier</param>
    /// <param name="request">Optional channel override</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Send status</returns>
    /// <response code="200">Reminder sent successfully</response>
    /// <response code="400">Reminder already sent or validation error</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Reminder not found</response>
    [HttpPost("reminders/{reminderId:guid}/send")]
    [Authorize(Roles = "System Admin")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [SwaggerRequestExample(typeof(SendReminderApiRequest), typeof(SendReminderApiRequestExample))]
    public async Task<IActionResult> SendReminder(
        Guid reminderId,
        [FromBody] SendReminderApiRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sending reminder: {ReminderId}", reminderId);

        var command = new SendReminderCommand
        {
            ReminderId = reminderId,
            OverrideChannel = request.OverrideChannel
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Reminder sent: {ReminderId}", reminderId);

        return Ok(ApiResponse<bool>.SuccessResult(result.Value!, "Reminder sent successfully."));
    }

    /// <summary>
    /// Cancels a scheduled reminder.
    /// </summary>
    /// <param name="reminderId">The reminder's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content</returns>
    /// <response code="204">Reminder cancelled successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Reminder not found</response>
    [HttpDelete("reminders/{reminderId:guid}")]
    [Authorize(Roles = "Academy Admin,Coach,System Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelReminder(
        Guid reminderId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Cancelling reminder: {ReminderId}", reminderId);

        var result = await _mediator.Send(new CancelReminderCommand { ReminderId = reminderId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Reminder cancelled: {ReminderId}", reminderId);

        return NoContent();
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
