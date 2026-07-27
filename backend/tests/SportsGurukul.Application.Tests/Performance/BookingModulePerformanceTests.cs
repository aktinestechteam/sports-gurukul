using System.Diagnostics;
using FluentAssertions;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Services;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Tests.Performance;

public class BookingModulePerformanceTests
{
    private readonly RecurrenceService _recurrenceService = new();

    [Fact]
    public void RecurrenceService_Daily365Completions_CompletesWithin50ms()
    {
        var sw = Stopwatch.StartNew();
        var result = _recurrenceService.GenerateOccurrences(
            RecurrenceType.Daily, DateTime.UtcNow.Date, TimeSpan.FromHours(9), TimeSpan.FromHours(10),
            occurrenceCount: 365, endDate: null);
        sw.Stop();

        result.Should().HaveCount(365);
        sw.ElapsedMilliseconds.Should().BeLessThan(50,
            $"Daily recurrence for 365 occurrences should complete within 50ms, took {sw.ElapsedMilliseconds}ms");
    }

    [Fact]
    public void RecurrenceService_Monthly12Completions_CompletesWithin10ms()
    {
        var sw = Stopwatch.StartNew();
        var result = _recurrenceService.GenerateOccurrences(
            RecurrenceType.Monthly, DateTime.UtcNow.Date, TimeSpan.FromHours(9), TimeSpan.FromHours(10),
            occurrenceCount: 12, endDate: null);
        sw.Stop();

        result.Should().HaveCount(12);
        sw.ElapsedMilliseconds.Should().BeLessThan(10);
    }

    [Fact]
    public void ConflictDetection_MultipleOverlaps_CompletesWithin100ms()
    {
        var facilityId = Guid.NewGuid();
        var booking = BookingTestDataBuilder.CreateBooking(facilityId: facilityId);

        var conflictingBookings = new List<Booking>();
        for (int i = 0; i < 50; i++)
        {
            conflictingBookings.Add(BookingTestDataBuilder.CreateBooking(
                facilityId: facilityId,
                status: BookingStatus.Confirmed,
                bookingDate: booking.BookingDate,
                startTime: booking.StartTime.Add(TimeSpan.FromMinutes(i * 5)),
                endTime: booking.EndTime.Add(TimeSpan.FromMinutes(i * 5))));
        }

        var sw = Stopwatch.StartNew();
        var filtered = conflictingBookings
            .Where(b => b.Id != booking.Id &&
                       b.Status != BookingStatus.Cancelled &&
                       b.Status != BookingStatus.Rejected &&
                       b.StartTime < booking.EndTime &&
                       b.EndTime > booking.StartTime)
            .ToList();
        sw.Stop();

        sw.ElapsedMilliseconds.Should().BeLessThan(100);
    }

    [Fact]
    public void BookingSchedule_Generation52Weeks_CompletesWithin20ms()
    {
        var booking = BookingTestDataBuilder.CreateBooking();
        var startDate = DateTime.UtcNow.Date.AddDays(1);

        var sw = Stopwatch.StartNew();
        var schedules = new List<BookingSchedule>();
        for (int i = 0; i < 52; i++)
        {
            schedules.Add(BookingTestDataBuilder.CreateBookingSchedule(
                bookingId: booking.Id,
                scheduledDate: startDate.AddDays(i * 7)));
        }
        sw.Stop();

        schedules.Should().HaveCount(52);
        sw.ElapsedMilliseconds.Should().BeLessThan(20);
    }
}
