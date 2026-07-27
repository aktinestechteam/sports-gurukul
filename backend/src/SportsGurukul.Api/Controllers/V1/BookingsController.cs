using System.Net.Mime;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Api.Common.Models.SwaggerExamples;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.CancelBooking;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.CompleteBooking;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.ConfirmBooking;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.CreateBooking;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.CreateRecurringBooking;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.ExpireBooking;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.RescheduleBooking;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.RejectBooking;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.UpdateBooking;
using SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Queries.GetAthleteBookings;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Queries.GetBookingHistory;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Queries.GetBookingById;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Queries.GetCoachBookings;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Queries.GetFacilityBookings;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Queries.GetUpcomingBookings;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Queries.SearchBookings;
using Swashbuckle.AspNetCore.Filters;

namespace SportsGurukul.Api.Controllers.V1;

/// <summary>
/// Manages bookings — creation, retrieval, update, cancellation, rescheduling,
/// state transitions, recurring bookings, and search.
/// </summary>
[ApiController]
[Route("api/v1/bookings")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
[Tags("Bookings")]
public class BookingsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<BookingsController> _logger;

    public BookingsController(IMediator mediator, ILogger<BookingsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new booking.
    /// </summary>
    /// <param name="request">Booking details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created booking</returns>
    /// <response code="201">Booking created successfully</response>
    /// <response code="400">Validation error</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Related resource not found</response>
    /// <response code="409">Booking conflict detected</response>
    [HttpPost]
    [Authorize(Roles = "Academy Admin,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<BookingDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [SwaggerRequestExample(typeof(CreateBookingApiRequest), typeof(CreateBookingApiRequestExample))]
    public async Task<IActionResult> CreateBooking(
        [FromBody] CreateBookingApiRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating booking: {Title} for academy: {AcademyId}", request.Title, request.AcademyId);

        var command = new CreateBookingCommand
        {
            BookingType = request.BookingType.ToString(),
            Title = request.Title,
            Description = request.Description,
            AcademyId = request.AcademyId,
            BranchId = request.BranchId,
            FacilityId = request.FacilityId,
            CoachId = request.CoachId,
            AthleteId = request.AthleteId,
            TrainingSessionId = request.TrainingSessionId,
            BookingDate = request.BookingDate,
            StartTime = request.StartTime,
            EndTime = request.EndTime
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Booking created: {BookingId}", result.Value!.Id);

        return CreatedAtAction(
            nameof(GetBookingById),
            new { bookingId = result.Value.Id, version = "1.0" },
            ApiResponse<BookingDto>.SuccessResult(result.Value, "Booking created successfully."));
    }

    /// <summary>
    /// Creates a recurring booking series.
    /// </summary>
    /// <param name="request">Recurring booking details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created recurring booking (first occurrence)</returns>
    /// <response code="201">Recurring booking created successfully</response>
    /// <response code="400">Validation error</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Related resource not found</response>
    /// <response code="409">Booking conflict detected</response>
    [HttpPost("recurring")]
    [Authorize(Roles = "Academy Admin,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<BookingDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [SwaggerRequestExample(typeof(CreateRecurringBookingApiRequest), typeof(CreateRecurringBookingApiRequestExample))]
    public async Task<IActionResult> CreateRecurringBooking(
        [FromBody] CreateRecurringBookingApiRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating recurring booking: {Title}", request.Title);

        var command = new CreateRecurringBookingCommand
        {
            BookingType = request.BookingType.ToString(),
            Title = request.Title,
            Description = request.Description,
            AcademyId = request.AcademyId,
            BranchId = request.BranchId,
            FacilityId = request.FacilityId,
            CoachId = request.CoachId,
            AthleteId = request.AthleteId,
            TrainingSessionId = request.TrainingSessionId,
            StartDate = request.StartDate,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            RecurrenceType = request.RecurrenceType.ToString(),
            OccurrenceCount = request.OccurrenceCount,
            EndDate = request.EndDate,
            RRule = request.RRule,
            Exceptions = request.Exceptions
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Recurring booking created: {BookingId}", result.Value!.Id);

        return CreatedAtAction(
            nameof(GetBookingById),
            new { bookingId = result.Value.Id, version = "1.0" },
            ApiResponse<BookingDto>.SuccessResult(result.Value, "Recurring booking created successfully."));
    }

    /// <summary>
    /// Searches and lists bookings with filtering and pagination.
    /// </summary>
    /// <param name="academyId">Optional academy identifier filter</param>
    /// <param name="branchId">Optional branch identifier filter</param>
    /// <param name="bookingType">Optional booking type filter</param>
    /// <param name="status">Optional status filter</param>
    /// <param name="searchTerm">Optional search term</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 20)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of booking summaries</returns>
    /// <response code="200">Bookings retrieved successfully</response>
    /// <response code="400">Validation error</response>
    /// <response code="401">Not authenticated</response>
    [HttpGet]
    [Authorize(Roles = "Academy Admin,Coach,Athlete,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<SearchBookingsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SearchBookings(
        [FromQuery] Guid? academyId,
        [FromQuery] Guid? branchId,
        [FromQuery] string? bookingType,
        [FromQuery] string? status,
        [FromQuery] string? searchTerm,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Searching bookings - Page: {Page}, PageSize: {PageSize}", page, pageSize);

        var query = new SearchBookingsQuery
        {
            AcademyId = academyId,
            BranchId = branchId,
            BookingType = bookingType,
            Status = status,
            SearchTerm = searchTerm,
            Page = page,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        var response = new SearchBookingsResponse
        {
            Items = result.Value!.Items,
            TotalCount = result.Value.TotalCount,
            Page = page,
            PageSize = pageSize
        };

        return Ok(ApiResponse<SearchBookingsResponse>.SuccessResult(response, "Bookings retrieved successfully."));
    }

    /// <summary>
    /// Gets a specific booking by its unique identifier.
    /// </summary>
    /// <param name="bookingId">The booking's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Booking details</returns>
    /// <response code="200">Booking retrieved successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="404">Booking not found</response>
    [HttpGet("{bookingId:guid}")]
    [Authorize(Roles = "Academy Admin,Coach,Athlete,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<BookingDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBookingById(
        Guid bookingId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching booking: {BookingId}", bookingId);

        var result = await _mediator.Send(new GetBookingByIdQuery { BookingId = bookingId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<BookingDto>.SuccessResult(result.Value!, "Booking retrieved successfully."));
    }

    /// <summary>
    /// Updates a booking. All fields are optional — only supplied fields are applied.
    /// </summary>
    /// <param name="bookingId">The booking's unique identifier</param>
    /// <param name="request">Fields to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated booking</returns>
    /// <response code="200">Booking updated successfully</response>
    /// <response code="400">Validation error</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Booking not found</response>
    /// <response code="409">Booking conflict detected</response>
    [HttpPut("{bookingId:guid}")]
    [Authorize(Roles = "Academy Admin,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<BookingDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [SwaggerRequestExample(typeof(UpdateBookingApiRequest), typeof(UpdateBookingApiRequestExample))]
    public async Task<IActionResult> UpdateBooking(
        Guid bookingId,
        [FromBody] UpdateBookingApiRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating booking: {BookingId}", bookingId);

        var command = new UpdateBookingCommand
        {
            BookingId = bookingId,
            Title = request.Title,
            Description = request.Description,
            BookingDate = request.BookingDate,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            FacilityId = request.FacilityId,
            CoachId = request.CoachId,
            AthleteId = request.AthleteId
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Booking updated: {BookingId}", bookingId);

        return Ok(ApiResponse<BookingDto>.SuccessResult(result.Value!, "Booking updated successfully."));
    }

    /// <summary>
    /// Cancels an existing booking.
    /// </summary>
    /// <param name="bookingId">The booking's unique identifier</param>
    /// <param name="request">Cancellation details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Cancelled booking</returns>
    /// <response code="200">Booking cancelled successfully</response>
    /// <response code="400">Validation error or booking not in cancellable state</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Booking not found</response>
    [HttpPost("{bookingId:guid}/cancel")]
    [Authorize(Roles = "Academy Admin,Coach,Athlete,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<BookingDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [SwaggerRequestExample(typeof(CancelBookingApiRequest), typeof(CancelBookingApiRequestExample))]
    public async Task<IActionResult> CancelBooking(
        Guid bookingId,
        [FromBody] CancelBookingApiRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Cancelling booking: {BookingId}", bookingId);

        var command = new CancelBookingCommand
        {
            BookingId = bookingId,
            Reason = request.Reason,
            Notes = request.Notes
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Booking cancelled: {BookingId}", bookingId);

        return Ok(ApiResponse<BookingDto>.SuccessResult(result.Value!, "Booking cancelled successfully."));
    }

    /// <summary>
    /// Confirms a pending booking.
    /// </summary>
    /// <param name="bookingId">The booking's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Confirmed booking</returns>
    /// <response code="200">Booking confirmed successfully</response>
    /// <response code="400">Booking not in confirmable state</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Booking not found</response>
    [HttpPost("{bookingId:guid}/confirm")]
    [Authorize(Roles = "Academy Admin,Coach,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<BookingDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConfirmBooking(
        Guid bookingId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Confirming booking: {BookingId}", bookingId);

        var result = await _mediator.Send(new ConfirmBookingCommand { BookingId = bookingId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Booking confirmed: {BookingId}", bookingId);

        return Ok(ApiResponse<BookingDto>.SuccessResult(result.Value!, "Booking confirmed successfully."));
    }

    /// <summary>
    /// Marks a booking as completed.
    /// </summary>
    /// <param name="bookingId">The booking's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Completed booking</returns>
    /// <response code="200">Booking completed successfully</response>
    /// <response code="400">Booking not in completable state</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Booking not found</response>
    [HttpPost("{bookingId:guid}/complete")]
    [Authorize(Roles = "Academy Admin,Coach,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<BookingDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CompleteBooking(
        Guid bookingId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Completing booking: {BookingId}", bookingId);

        var result = await _mediator.Send(new CompleteBookingCommand { BookingId = bookingId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Booking completed: {BookingId}", bookingId);

        return Ok(ApiResponse<BookingDto>.SuccessResult(result.Value!, "Booking completed successfully."));
    }

    /// <summary>
    /// Expires a booking that has passed its scheduled time without confirmation.
    /// </summary>
    /// <param name="bookingId">The booking's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Expired booking</returns>
    /// <response code="200">Booking expired successfully</response>
    /// <response code="400">Booking not in expirable state</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Booking not found</response>
    [HttpPost("{bookingId:guid}/expire")]
    [Authorize(Roles = "System Admin")]
    [ProducesResponseType(typeof(ApiResponse<BookingDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ExpireBooking(
        Guid bookingId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Expiring booking: {BookingId}", bookingId);

        var result = await _mediator.Send(new ExpireBookingCommand { BookingId = bookingId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Booking expired: {BookingId}", bookingId);

        return Ok(ApiResponse<BookingDto>.SuccessResult(result.Value!, "Booking expired successfully."));
    }

    /// <summary>
    /// Reschedules a booking to a new date and time.
    /// </summary>
    /// <param name="bookingId">The booking's unique identifier</param>
    /// <param name="request">New schedule details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Rescheduled booking</returns>
    /// <response code="200">Booking rescheduled successfully</response>
    /// <response code="400">Validation error or booking not in reschedulable state</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Booking not found</response>
    /// <response code="409">Conflict with new schedule</response>
    [HttpPost("{bookingId:guid}/reschedule")]
    [Authorize(Roles = "Academy Admin,Coach")]
    [ProducesResponseType(typeof(ApiResponse<BookingDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [SwaggerRequestExample(typeof(RescheduleBookingApiRequest), typeof(RescheduleBookingApiRequestExample))]
    public async Task<IActionResult> RescheduleBooking(
        Guid bookingId,
        [FromBody] RescheduleBookingApiRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Rescheduling booking: {BookingId}", bookingId);

        var command = new RescheduleBookingCommand
        {
            BookingId = bookingId,
            NewDate = request.NewDate,
            NewStartTime = request.NewStartTime,
            NewEndTime = request.NewEndTime,
            Reason = request.Reason,
            Notes = request.Notes
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Booking rescheduled: {BookingId}", bookingId);

        return Ok(ApiResponse<BookingDto>.SuccessResult(result.Value!, "Booking rescheduled successfully."));
    }

    /// <summary>
    /// Rejects a booking.
    /// </summary>
    /// <param name="bookingId">The booking's unique identifier</param>
    /// <param name="request">Rejection details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Rejected booking</returns>
    /// <response code="200">Booking rejected successfully</response>
    /// <response code="400">Booking not in rejectable state</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Booking not found</response>
    [HttpPost("{bookingId:guid}/reject")]
    [Authorize(Roles = "Academy Admin,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<BookingDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [SwaggerRequestExample(typeof(RejectBookingApiRequest), typeof(RejectBookingApiRequestExample))]
    public async Task<IActionResult> RejectBooking(
        Guid bookingId,
        [FromBody] RejectBookingApiRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Rejecting booking: {BookingId}", bookingId);

        var command = new RejectBookingCommand
        {
            BookingId = bookingId,
            Reason = request.Reason
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Booking rejected: {BookingId}", bookingId);

        return Ok(ApiResponse<BookingDto>.SuccessResult(result.Value!, "Booking rejected successfully."));
    }

    /// <summary>
    /// Gets all bookings for a specific athlete.
    /// </summary>
    /// <param name="athleteId">The athlete's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of booking summaries</returns>
    /// <response code="200">Bookings retrieved successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="404">Athlete not found</response>
    [HttpGet("athlete/{athleteId:guid}")]
    [Authorize(Roles = "Academy Admin,Coach,Athlete,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<BookingSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAthleteBookings(
        Guid athleteId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching bookings for athlete: {AthleteId}", athleteId);

        var result = await _mediator.Send(new GetAthleteBookingsQuery { AthleteId = athleteId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<IReadOnlyList<BookingSummaryDto>>.SuccessResult(result.Value!, "Bookings retrieved successfully."));
    }

    /// <summary>
    /// Gets all bookings for a specific coach on a given date.
    /// </summary>
    /// <param name="coachId">The coach's unique identifier</param>
    /// <param name="date">Date to filter bookings</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of booking summaries</returns>
    /// <response code="200">Bookings retrieved successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="404">Coach not found</response>
    [HttpGet("coach/{coachId:guid}")]
    [Authorize(Roles = "Academy Admin,Coach,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<BookingSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCoachBookings(
        Guid coachId,
        [FromQuery] DateTime date,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching bookings for coach: {CoachId} on {Date}", coachId, date);

        var result = await _mediator.Send(new GetCoachBookingsQuery { CoachId = coachId, Date = date }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<IReadOnlyList<BookingSummaryDto>>.SuccessResult(result.Value!, "Bookings retrieved successfully."));
    }

    /// <summary>
    /// Gets all bookings for a specific facility on a given date.
    /// </summary>
    /// <param name="facilityId">The facility's unique identifier</param>
    /// <param name="date">Date to filter bookings</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of booking summaries</returns>
    /// <response code="200">Bookings retrieved successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="404">Facility not found</response>
    [HttpGet("facility/{facilityId:guid}")]
    [Authorize(Roles = "Academy Admin,Coach,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<BookingSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFacilityBookings(
        Guid facilityId,
        [FromQuery] DateTime date,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching bookings for facility: {FacilityId} on {Date}", facilityId, date);

        var result = await _mediator.Send(new GetFacilityBookingsQuery { FacilityId = facilityId, Date = date }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<IReadOnlyList<BookingSummaryDto>>.SuccessResult(result.Value!, "Bookings retrieved successfully."));
    }

    /// <summary>
    /// Gets upcoming bookings for an academy within the specified number of days.
    /// </summary>
    /// <param name="academyId">The academy's unique identifier</param>
    /// <param name="daysAhead">Number of days to look ahead (default: 7)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of upcoming booking summaries</returns>
    /// <response code="200">Upcoming bookings retrieved successfully</response>
    /// <response code="401">Not authenticated</response>
    [HttpGet("upcoming")]
    [Authorize(Roles = "Academy Admin,Coach,Athlete,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<BookingSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUpcomingBookings(
        [FromQuery] Guid academyId,
        [FromQuery] int daysAhead = 7,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching upcoming bookings for academy: {AcademyId}, days ahead: {DaysAhead}", academyId, daysAhead);

        var result = await _mediator.Send(new GetUpcomingBookingsQuery { AcademyId = academyId, DaysAhead = daysAhead }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<IReadOnlyList<BookingSummaryDto>>.SuccessResult(result.Value!, "Upcoming bookings retrieved successfully."));
    }

    /// <summary>
    /// Gets the change history for a specific booking.
    /// </summary>
    /// <param name="bookingId">The booking's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of history entries</returns>
    /// <response code="200">Booking history retrieved successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Booking not found</response>
    [HttpGet("{bookingId:guid}/history")]
    [Authorize(Roles = "Academy Admin,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<BookingHistoryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBookingHistory(
        Guid bookingId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching history for booking: {BookingId}", bookingId);

        var result = await _mediator.Send(new GetBookingHistoryQuery { BookingId = bookingId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<IReadOnlyList<BookingHistoryDto>>.SuccessResult(result.Value!, "Booking history retrieved successfully."));
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
            error.Contains("already associated", StringComparison.OrdinalIgnoreCase) ||
            error.Contains("conflict", StringComparison.OrdinalIgnoreCase))
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

public class SearchBookingsResponse
{
    public IReadOnlyList<BookingSummaryDto> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
