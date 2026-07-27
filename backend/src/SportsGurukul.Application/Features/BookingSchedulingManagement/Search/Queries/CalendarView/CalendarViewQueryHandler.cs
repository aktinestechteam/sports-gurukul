using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Search.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Search.Queries.CalendarView;

public class CalendarViewQueryHandler
    : IRequestHandler<CalendarViewQuery, Result<CalendarViewResultDto>>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly ILogger<CalendarViewQueryHandler> _logger;

    public CalendarViewQueryHandler(
        IBookingRepository bookingRepository,
        ILogger<CalendarViewQueryHandler> logger)
    {
        _bookingRepository = bookingRepository;
        _logger = logger;
    }

    public async Task<Result<CalendarViewResultDto>> Handle(
        CalendarViewQuery request, CancellationToken cancellationToken)
    {
        var referenceDate = request.ViewDate ?? DateTime.UtcNow.Date;

        var (startDate, endDate) = request.ViewType switch
        {
            CalendarViewType.Daily => (referenceDate.Date, referenceDate.Date.AddDays(1)),
            CalendarViewType.Weekly => (
                referenceDate.Date.AddDays(-(int)referenceDate.DayOfWeek),
                referenceDate.Date.AddDays(7 - (int)referenceDate.DayOfWeek)),
            CalendarViewType.Monthly => (
                new DateTime(referenceDate.Year, referenceDate.Month, 1),
                new DateTime(referenceDate.Year, referenceDate.Month, 1).AddMonths(1)),
            CalendarViewType.Agenda => (
                DateTime.UtcNow.Date,
                DateTime.UtcNow.Date.AddDays(30)),
            _ => (
                new DateTime(referenceDate.Year, referenceDate.Month, 1),
                new DateTime(referenceDate.Year, referenceDate.Month, 1).AddMonths(1))
        };

        _logger.LogInformation(
            "Calendar view: {ViewType} from {Start} to {End} for academy {AcademyId}",
            request.ViewType, startDate, endDate, request.AcademyId);

        var bookings = await _bookingRepository.GetByDateRangeAsync(
            request.AcademyId, startDate, endDate, cancellationToken);

        var activeBookings = bookings
            .Where(b => b.Status != BookingStatus.Cancelled && b.Status != BookingStatus.Expired)
            .AsEnumerable();

        if (request.FacilityId.HasValue)
            activeBookings = activeBookings.Where(b => b.FacilityId == request.FacilityId);

        if (request.CoachId.HasValue)
            activeBookings = activeBookings.Where(b => b.CoachId == request.CoachId);

        var sorted = activeBookings
            .OrderBy(b => b.BookingDate)
            .ThenBy(b => b.StartTime)
            .ToList();

        var events = sorted.Select(b => new BookingCalendarEventDto
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
            ResourceTypeName = b.FacilityId.HasValue ? "Facility" : b.CoachId.HasValue ? "Coach" : null
        }).ToList();

        var daySummaries = BuildDaySummaries(startDate, endDate, sorted);

        var result = new CalendarViewResultDto
        {
            ViewType = request.ViewType,
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
            var dayBookings = bookings
                .Where(b => b.BookingDate.Date == current.Date)
                .ToList();

            var totalMinutes = dayBookings.Sum(b =>
                (int)(b.EndTime - b.StartTime).TotalMinutes);

            var facilityNames = dayBookings
                .Select(b => b.Facility?.FacilityName)
                .Where(n => !string.IsNullOrEmpty(n))
                .Distinct()
                .ToList()!;

            summaries.Add(new CalendarDaySummaryDto
            {
                Date = current,
                EventCount = dayBookings.Count,
                TotalMinutesBooked = totalMinutes,
                UtilizationPercent = Math.Min(100, totalMinutes / (12.0 * 60) * 100),
                FacilityNames = facilityNames
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
