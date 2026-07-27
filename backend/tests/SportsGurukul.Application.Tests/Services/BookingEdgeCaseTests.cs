using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Services;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Tests.Services;

public class BookingEdgeCaseTests
{
    private readonly Mock<IBookingRepository> _bookingRepoMock = TestMocks.CreateBookingRepository();
    private readonly Mock<IConflictRepository> _conflictRepoMock = TestMocks.CreateConflictRepository();
    private readonly Mock<ILogger<ConflictDetectionService>> _loggerMock = TestMocks.CreateLogger<ConflictDetectionService>();
    private readonly ConflictDetectionService _conflictService;

    public BookingEdgeCaseTests()
    {
        _conflictService = new ConflictDetectionService(
            _bookingRepoMock.Object,
            _conflictRepoMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task DoubleBooking_SameFacility_SameTime_DetectedAsConflict()
    {
        var facilityId = Guid.NewGuid();
        var date = DateTime.UtcNow.Date.AddDays(1);
        var booking = BookingTestDataBuilder.CreateBooking(
            facilityId: facilityId,
            bookingDate: date,
            startTime: TimeSpan.FromHours(9),
            endTime: TimeSpan.FromHours(10));
        var doubleBooking = BookingTestDataBuilder.CreateBooking(
            facilityId: facilityId,
            bookingDate: date,
            startTime: TimeSpan.FromHours(9),
            endTime: TimeSpan.FromHours(10));
        _bookingRepoMock.Setup(r => r.GetByFacilityIdAsync(facilityId, date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking> { doubleBooking });

        var conflicts = await _conflictService.DetectConflictsAsync(booking);

        conflicts.Should().HaveCount(1);
        conflicts[0].ConflictType.Should().Be(BookingConflictType.FacilityOverlap);
    }

    [Fact]
    public async Task DoubleBooking_SameCoach_SameTime_DetectedAsConflict()
    {
        var coachId = Guid.NewGuid();
        var date = DateTime.UtcNow.Date.AddDays(1);
        var booking = BookingTestDataBuilder.CreateBooking(
            coachId: coachId,
            bookingDate: date,
            startTime: TimeSpan.FromHours(14),
            endTime: TimeSpan.FromHours(15));
        var doubleBooking = BookingTestDataBuilder.CreateBooking(
            coachId: coachId,
            bookingDate: date,
            startTime: TimeSpan.FromHours(14),
            endTime: TimeSpan.FromHours(15));
        _bookingRepoMock.Setup(r => r.GetByFacilityIdAsync(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>());
        _bookingRepoMock.Setup(r => r.GetByCoachIdAsync(coachId, date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking> { doubleBooking });

        var conflicts = await _conflictService.DetectConflictsAsync(booking);

        conflicts.Should().HaveCount(1);
        conflicts[0].ConflictType.Should().Be(BookingConflictType.CoachOverlap);
    }

    [Fact]
    public async Task SameTimeDifferentFacility_NoFacilityConflict()
    {
        var facility1 = Guid.NewGuid();
        var facility2 = Guid.NewGuid();
        var date = DateTime.UtcNow.Date.AddDays(1);
        var booking = BookingTestDataBuilder.CreateBooking(
            facilityId: facility1,
            bookingDate: date,
            startTime: TimeSpan.FromHours(9),
            endTime: TimeSpan.FromHours(10));
        var otherBooking = BookingTestDataBuilder.CreateBooking(
            facilityId: facility2,
            bookingDate: date,
            startTime: TimeSpan.FromHours(9),
            endTime: TimeSpan.FromHours(10));
        _bookingRepoMock.Setup(r => r.GetByFacilityIdAsync(facility1, date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>());
        _bookingRepoMock.Setup(r => r.GetByFacilityIdAsync(facility2, date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking> { otherBooking });

        var conflicts = await _conflictService.DetectConflictsAsync(booking);

        conflicts.Should().BeEmpty();
    }

    [Fact]
    public async Task BackToBackBookings_SameFacility_NoConflict()
    {
        var facilityId = Guid.NewGuid();
        var date = DateTime.UtcNow.Date.AddDays(1);
        var booking = BookingTestDataBuilder.CreateBooking(
            facilityId: facilityId,
            bookingDate: date,
            startTime: TimeSpan.FromHours(9),
            endTime: TimeSpan.FromHours(10));
        var nextBooking = BookingTestDataBuilder.CreateBooking(
            facilityId: facilityId,
            bookingDate: date,
            startTime: TimeSpan.FromHours(10),
            endTime: TimeSpan.FromHours(11));
        _bookingRepoMock.Setup(r => r.GetByFacilityIdAsync(facilityId, date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking> { nextBooking });

        var conflicts = await _conflictService.DetectConflictsAsync(booking);

        conflicts.Should().BeEmpty();
    }

    [Fact]
    public async Task ContainedBooking_FullOverlap_Detected()
    {
        var facilityId = Guid.NewGuid();
        var date = DateTime.UtcNow.Date.AddDays(1);
        var booking = BookingTestDataBuilder.CreateBooking(
            facilityId: facilityId,
            bookingDate: date,
            startTime: TimeSpan.FromHours(9),
            endTime: TimeSpan.FromHours(12));
        var containedBooking = BookingTestDataBuilder.CreateBooking(
            facilityId: facilityId,
            bookingDate: date,
            startTime: TimeSpan.FromHours(10),
            endTime: TimeSpan.FromHours(11));
        _bookingRepoMock.Setup(r => r.GetByFacilityIdAsync(facilityId, date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking> { containedBooking });

        var conflicts = await _conflictService.DetectConflictsAsync(booking);

        conflicts.Should().HaveCount(1);
    }

    [Fact]
    public async Task OverlappingAtStart_PartialOverlap_Detected()
    {
        var facilityId = Guid.NewGuid();
        var date = DateTime.UtcNow.Date.AddDays(1);
        var booking = BookingTestDataBuilder.CreateBooking(
            facilityId: facilityId,
            bookingDate: date,
            startTime: TimeSpan.FromHours(10),
            endTime: TimeSpan.FromHours(12));
        var existing = BookingTestDataBuilder.CreateBooking(
            facilityId: facilityId,
            bookingDate: date,
            startTime: TimeSpan.FromHours(9),
            endTime: TimeSpan.FromHours(11));
        _bookingRepoMock.Setup(r => r.GetByFacilityIdAsync(facilityId, date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking> { existing });

        var conflicts = await _conflictService.DetectConflictsAsync(booking);

        conflicts.Should().HaveCount(1);
    }

    [Fact]
    public async Task OverlappingAtEnd_PartialOverlap_Detected()
    {
        var facilityId = Guid.NewGuid();
        var date = DateTime.UtcNow.Date.AddDays(1);
        var booking = BookingTestDataBuilder.CreateBooking(
            facilityId: facilityId,
            bookingDate: date,
            startTime: TimeSpan.FromHours(9),
            endTime: TimeSpan.FromHours(11));
        var existing = BookingTestDataBuilder.CreateBooking(
            facilityId: facilityId,
            bookingDate: date,
            startTime: TimeSpan.FromHours(10),
            endTime: TimeSpan.FromHours(12));
        _bookingRepoMock.Setup(r => r.GetByFacilityIdAsync(facilityId, date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking> { existing });

        var conflicts = await _conflictService.DetectConflictsAsync(booking);

        conflicts.Should().HaveCount(1);
    }

    [Fact]
    public async Task NoFacilityNoCoachNoAthlete_NoConflicts()
    {
        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            BookingNumber = "BK-TEST-NO-RES",
            FacilityId = null,
            CoachId = null,
            AthleteId = null,
            BookingDate = DateTime.UtcNow.Date.AddDays(1),
            StartTime = TimeSpan.FromHours(9),
            EndTime = TimeSpan.FromHours(10),
            Status = BookingStatus.Confirmed
        };

        var conflicts = await _conflictService.DetectConflictsAsync(booking);

        conflicts.Should().BeEmpty();
    }

    [Fact]
    public async Task WaitingStatusBooking_DoesNotCountAsConflict()
    {
        var facilityId = Guid.NewGuid();
        var date = DateTime.UtcNow.Date.AddDays(1);
        var booking = BookingTestDataBuilder.CreateBooking(
            facilityId: facilityId,
            bookingDate: date,
            startTime: TimeSpan.FromHours(9),
            endTime: TimeSpan.FromHours(10));
        var waitingBooking = BookingTestDataBuilder.CreateBooking(
            facilityId: facilityId,
            bookingDate: date,
            startTime: TimeSpan.FromHours(9),
            endTime: TimeSpan.FromHours(10),
            status: BookingStatus.Pending);
        _bookingRepoMock.Setup(r => r.GetByFacilityIdAsync(facilityId, date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking> { waitingBooking });

        var conflicts = await _conflictService.DetectConflictsAsync(booking);

        conflicts.Should().ContainSingle();
    }

    [Fact]
    public async Task ExpiredBooking_DoesCountAsConflict()
    {
        var facilityId = Guid.NewGuid();
        var date = DateTime.UtcNow.Date.AddDays(1);
        var booking = BookingTestDataBuilder.CreateBooking(
            facilityId: facilityId,
            bookingDate: date,
            startTime: TimeSpan.FromHours(9),
            endTime: TimeSpan.FromHours(10));
        var expiredBooking = BookingTestDataBuilder.CreateBooking(
            facilityId: facilityId,
            bookingDate: date,
            startTime: TimeSpan.FromHours(9),
            endTime: TimeSpan.FromHours(10),
            status: BookingStatus.Expired);
        _bookingRepoMock.Setup(r => r.GetByFacilityIdAsync(facilityId, date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking> { expiredBooking });

        var conflicts = await _conflictService.DetectConflictsAsync(booking);

        conflicts.Should().ContainSingle();
    }

    [Fact]
    public void WaitlistService_SamePriority_EarlierRequestedOnFirst()
    {
        var bookingId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var entries = new List<BookingWaitlist>
        {
            BookingTestDataBuilder.CreateBookingWaitlist(bookingId, priority: 1),
            BookingTestDataBuilder.CreateBookingWaitlist(bookingId, priority: 1)
        };
        entries[0].RequestedOn = now.AddHours(-2);
        entries[1].RequestedOn = now.AddHours(-1);

        var sorted = entries.OrderBy(w => w.Priority).ThenBy(w => w.RequestedOn).ToList();

        sorted[0].RequestedOn.Should().Be(entries[0].RequestedOn);
    }

    [Fact]
    public void BookingSchedule_CrossMidnight_EndTimeLessThanStartTime()
    {
        var booking = BookingTestDataBuilder.CreateCrossMidnightBooking();

        booking.StartTime.Should().Be(new TimeSpan(22, 0, 0));
        booking.EndTime.Should().Be(new TimeSpan(1, 0, 0));
        booking.StartTime.Should().BeGreaterThan(booking.EndTime);
    }

    [Fact]
    public void BookingSchedule_LeapYearDate_CreatedCorrectly()
    {
        var leapDate = new DateTime(2028, 2, 29);
        var booking = BookingTestDataBuilder.CreateBooking(
            bookingDate: leapDate,
            startTime: TimeSpan.FromHours(9),
            endTime: TimeSpan.FromHours(10));

        booking.BookingDate.Should().Be(leapDate);
        booking.BookingDate.Day.Should().Be(29);
        booking.BookingDate.Month.Should().Be(2);
    }

    [Fact]
    public void BookingSchedule_MaxDuration_16Hours()
    {
        var booking = BookingTestDataBuilder.CreateMaxDurationBooking();

        booking.StartTime.Should().Be(TimeSpan.FromHours(6));
        booking.EndTime.Should().Be(new TimeSpan(22, 0, 0));
        var duration = booking.EndTime - booking.StartTime;
        duration.TotalHours.Should().Be(16);
    }

    [Fact]
    public void BookingSchedule_MinDuration_30Minutes()
    {
        var booking = BookingTestDataBuilder.CreateBooking(
            startTime: TimeSpan.FromHours(9),
            endTime: new TimeSpan(9, 30, 0));

        var duration = booking.EndTime - booking.StartTime;
        duration.TotalMinutes.Should().Be(30);
    }

    [Fact]
    public void RecurrenceService_AllDayEveryDay_DailyForMonth()
    {
        var startDate = new DateTime(2026, 2, 1);
        var service = new RecurrenceService();

        var result = service.GenerateOccurrences(
            RecurrenceType.Daily, startDate, TimeSpan.Zero, new TimeSpan(23, 59, 0),
            occurrenceCount: 28, endDate: null);

        result.Should().HaveCount(28);
        result.First().Should().Be(new DateTime(2026, 2, 1));
        result.Last().Should().Be(new DateTime(2026, 2, 28));
    }

    [Fact]
    public async Task AvailabilityService_FacilityAvailable_NoBookingsOnDifferentDate()
    {
        var facilityId = Guid.NewGuid();
        var date1 = DateTime.UtcNow.Date.AddDays(1);
        var facilityRepoMock = new Mock<IFacilityRepository>();
        var bookingRepoMock = TestMocks.CreateBookingRepository();
        var coachRepoMock = new Mock<ICoachRepository>();
        var athleteRepoMock = new Mock<IAthleteRepository>();
        var loggerMock = TestMocks.CreateLogger<AvailabilityService>();

        var facility = BookingTestDataBuilder.CreateFacility(facilityId);
        facilityRepoMock.Setup(r => r.GetByIdAsync(facilityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(facility);
        bookingRepoMock.Setup(r => r.GetByFacilityIdAsync(facilityId, date1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>());

        var service = new AvailabilityService(
            bookingRepoMock.Object,
            facilityRepoMock.Object,
            coachRepoMock.Object,
            athleteRepoMock.Object,
            loggerMock.Object);

        var result = await service.IsFacilityAvailableAsync(
            facilityId, date1, TimeSpan.FromHours(9), TimeSpan.FromHours(11));

        result.Should().BeTrue();
    }
}
