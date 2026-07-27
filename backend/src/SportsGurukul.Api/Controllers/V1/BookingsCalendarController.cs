using System.Net.Mime;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Calendar.Abstractions;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Calendar.Ics;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Search.DTOs;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Search.Queries.CalendarView;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Search.Queries.GetResourceCalendar;

namespace SportsGurukul.Api.Controllers.V1;

/// <summary>
/// Calendar views for bookings — daily, weekly, monthly, agenda, resource-specific,
/// and ICS export.
/// </summary>
[ApiController]
[Route("api/v1/bookings/calendar")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
[Tags("Booking Calendar")]
public class BookingsCalendarController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<BookingsCalendarController> _logger;

    public BookingsCalendarController(IMediator mediator, ILogger<BookingsCalendarController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Gets a calendar view (daily, weekly, monthly, or agenda) for an academy.
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Academy Admin,Coach,Athlete,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<CalendarViewResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCalendarView(
        [FromQuery] Guid academyId,
        [FromQuery] string viewType = "Monthly",
        [FromQuery] DateTime? viewDate = null,
        [FromQuery] Guid? facilityId = null,
        [FromQuery] Guid? coachId = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Calendar view: {ViewType} for academy {AcademyId}", viewType, academyId);

        if (!Enum.TryParse<CalendarViewType>(viewType, true, out var parsedViewType))
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Bad Request",
                Detail = $"Invalid view type '{viewType}'. Valid values: Daily, Weekly, Monthly, Agenda."
            });
        }

        var query = new CalendarViewQuery
        {
            AcademyId = academyId,
            ViewType = parsedViewType,
            ViewDate = viewDate,
            FacilityId = facilityId,
            CoachId = coachId
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<CalendarViewResultDto>.SuccessResult(
            result.Value!, "Calendar view retrieved successfully."));
    }

    /// <summary>
    /// Gets bookings for a specific resource (facility or coach) within a date range.
    /// </summary>
    [HttpGet("resource")]
    [Authorize(Roles = "Academy Admin,Coach,Athlete,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<CalendarViewResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetResourceCalendar(
        [FromQuery] Guid academyId,
        [FromQuery] string resourceType,
        [FromQuery] Guid resourceId,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Resource calendar: {ResourceType} {ResourceId}", resourceType, resourceId);

        var query = new GetResourceCalendarQuery
        {
            AcademyId = academyId,
            ResourceType = resourceType,
            ResourceId = resourceId,
            StartDate = startDate,
            EndDate = endDate
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<CalendarViewResultDto>.SuccessResult(
            result.Value!, "Resource calendar retrieved successfully."));
    }

    /// <summary>
    /// Exports bookings to ICS format for import into external calendar applications.
    /// </summary>
    [HttpGet("export/ics")]
    [Authorize(Roles = "Academy Admin,Coach,Athlete,System Admin")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ExportToIcs(
        [FromQuery] Guid academyId,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] Guid? facilityId = null,
        [FromQuery] Guid? coachId = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("ICS export for academy {AcademyId}", academyId);

        var query = new CalendarViewQuery
        {
            AcademyId = academyId,
            ViewType = CalendarViewType.Agenda,
            ViewDate = startDate ?? DateTime.UtcNow.Date,
            FacilityId = facilityId,
            CoachId = coachId
        };

        var calendarResult = await _mediator.Send(query, cancellationToken);

        if (!calendarResult.IsSuccess)
            return HandleFailure(calendarResult.Error!);

        var calendarEvents = calendarResult.Value!.Events.Select(e => new CalendarEvent
        {
            Id = e.BookingId,
            Uid = e.BookingNumber,
            Summary = e.Title,
            Description = e.Description,
            Location = e.FacilityName,
            StartDateTime = e.StartDateTime,
            EndDateTime = e.EndDateTime,
            Status = e.Status,
            Color = e.Color,
            Organizer = e.CoachName,
            Metadata = new Dictionary<string, string>
            {
                ["BookingType"] = e.BookingType,
                ["BookingNumber"] = e.BookingNumber,
                ["BookingId"] = e.BookingId.ToString()
            }
        }).ToList();

        var exporter = new IcsExporter();
        var icsData = await exporter.ExportAsync(calendarEvents, new CalendarExportOptions
        {
            ProductIdentifier = "-//SportsGurukul//BookingCalendar//EN",
            IncludeDescription = true,
            IncludeLocation = true,
            ReminderMinutesBefore = 30
        }, cancellationToken);

        return File(icsData, "text/calendar; charset=utf-8", "bookings.ics");
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
