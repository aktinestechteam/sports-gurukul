using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Search.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Search.Queries.GetResourceCalendar;

public class GetResourceCalendarQueryHandler
    : IRequestHandler<GetResourceCalendarQuery, Result<CalendarViewResultDto>>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly ILogger<GetResourceCalendarQueryHandler> _logger;

    public GetResourceCalendarQueryHandler(
        IBookingRepository bookingRepository,
        ILogger<GetResourceCalendarQueryHandler> logger)
    {
        _bookingRepository = bookingRepository;
        _logger = logger;
    }

    public async Task<Result<CalendarViewResultDto>> Handle(
        GetResourceCalendarQuery request, CancellationToken cancellationToken)
    {
        var startDate = request.StartDate ?? DateTime.UtcNow.Date;
        var endDate = request.EndDate ?? startDate.AddDays(30);

        _logger.LogInformation(
            "Resource calendar: {ResourceType} {ResourceId} from {Start} to {End}",
            request.ResourceType, request.ResourceId, startDate, endDate);

        var bookings = await _bookingRepository.GetByDateRangeAsync(
            request.AcademyId, startDate, endDate, cancellationToken);

        var resourceBookings = request.ResourceType.ToLowerInvariant() switch
        {
            "facility" => bookings.Where(b => b.FacilityId == request.ResourceId),
            "coach" => bookings.Where(b => b.CoachId == request.ResourceId),
            _ => bookings.Where(b => b.FacilityId == request.ResourceId || b.CoachId == request.ResourceId)
        };

        var activeBookings = resourceBookings
            .Where(b => b.Status != BookingStatus.Cancelled && b.Status != BookingStatus.Expired)
            .OrderBy(b => b.BookingDate)
            .ThenBy(b => b.StartTime)
            .ToList();

        var events = activeBookings.Select(b => new BookingCalendarEventDto
        {
            BookingId = b.Id,
            BookingNumber = b.BookingNumber,
            Title = b.Title,
            Description = b.Description,
            BookingType = b.BookingType.ToString(),
            Status = b.Status.ToString(),
            FacilityName = b.Facility?.FacilityName,
            CoachName = b.Coach?.User?.FullName,
            AthleteName = b.Athlete?.User?.FullName,
            StartDateTime = b.BookingDate.Date + b.StartTime,
            EndDateTime = b.BookingDate.Date + b.EndTime,
            Color = GetEventColor(b.BookingType, b.Status),
            ResourceName = b.Facility?.FacilityName ?? b.Coach?.User?.FullName,
            ResourceTypeName = request.ResourceType
        }).ToList();

        var daySummaries = BuildDaySummaries(startDate, endDate, activeBookings);

        var result = new CalendarViewResultDto
        {
            ViewType = CalendarViewType.Agenda,
            ViewStartDate = startDate,
            ViewEndDate = endDate,
            Events = events,
            DaySummaries = daySummaries,
            TotalEvents = events.Count
        };

        return Result<CalendarViewResultDto>.Success(result);
    }

    private static List<CalendarDaySummaryDto> BuildDaySummaries(
        DateTime startDate, DateTime endDate, IReadOnlyList<Domain.Entities.Booking> bookings)
    {
        var summaries = new List<CalendarDaySummaryDto>();
        var current = startDate;

        while (current < endDate)
        {
            var dayBookings = bookings.Where(b => b.BookingDate.Date == current.Date).ToList();
            var totalMinutes = dayBookings.Sum(b => (int)(b.EndTime - b.StartTime).TotalMinutes);

            summaries.Add(new CalendarDaySummaryDto
            {
                Date = current,
                EventCount = dayBookings.Count,
                TotalMinutesBooked = totalMinutes,
                UtilizationPercent = Math.Min(100, totalMinutes / (12.0 * 60) * 100),
                FacilityNames = dayBookings
                    .Select(b => b.Facility?.FacilityName)
                    .Where(n => !string.IsNullOrEmpty(n))
                    .Distinct()
                    .ToList()!
            });

            current = current.AddDays(1);
        }

        return summaries;
    }

    private static string GetEventColor(BookingType bookingType, BookingStatus status)
    {
        if (status == BookingStatus.Cancelled) return "#9CA3AF";
        if (status == BookingStatus.Pending) return "#F59E0B";
        if (status == BookingStatus.Rejected) return "#EF4444";

        return bookingType switch
        {
            BookingType.TrainingSession => "#3B82F6",
            BookingType.FacilityReservation => "#10B981",
            BookingType.PrivateCoaching => "#8B5CF6",
            BookingType.GroupCoaching => "#06B6D4",
            BookingType.EquipmentReservation => "#F97316",
            BookingType.CourtReservation => "#14B8A6",
            BookingType.GroundReservation => "#84CC16",
            BookingType.PracticeMatch => "#EC4899",
            BookingType.TournamentSlot => "#EF4444",
            BookingType.EventReservation => "#6366F1",
            _ => "#6B7280"
        };
    }
}
