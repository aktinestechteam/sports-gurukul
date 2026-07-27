using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Services;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Tests.Services;

public class ConflictDetectionServiceExpandedTests
{
    private readonly Mock<IBookingRepository> _bookingRepoMock = TestMocks.CreateBookingRepository();
    private readonly Mock<IConflictRepository> _conflictRepoMock = TestMocks.CreateConflictRepository();
    private readonly Mock<ILogger<ConflictDetectionService>> _loggerMock = TestMocks.CreateLogger<ConflictDetectionService>();
    private readonly ConflictDetectionService _service;

    public ConflictDetectionServiceExpandedTests()
    {
        _service = new ConflictDetectionService(
            _bookingRepoMock.Object,
            _conflictRepoMock.Object,
            _loggerMock.Object);
    }

    #region DetectConflictsAsync - Coach Overlap

    [Fact]
    public async Task DetectConflictsAsync_CoachOverlap_DetectsConflict()
    {
        var coachId = Guid.NewGuid();
        var booking = BookingTestDataBuilder.CreateBooking(coachId: coachId);
        _bookingRepoMock.Setup(r => r.GetByFacilityIdAsync(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>());
        _bookingRepoMock.Setup(r => r.GetByCoachIdAsync(coachId, booking.BookingDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>
            {
                BookingTestDataBuilder.CreateBooking(
                    coachId: coachId,
                    status: BookingStatus.Confirmed,
                    bookingDate: booking.BookingDate,
                    startTime: booking.StartTime.Add(TimeSpan.FromMinutes(30)),
                    endTime: booking.EndTime.Add(TimeSpan.FromMinutes(30)))
            });

        var result = await _service.DetectConflictsAsync(booking);

        result.Should().Contain(c => c.ConflictType == BookingConflictType.CoachOverlap);
    }

    [Fact]
    public async Task DetectConflictsAsync_CoachOverlap_ExcludesRejectedBookings()
    {
        var coachId = Guid.NewGuid();
        var booking = BookingTestDataBuilder.CreateBooking(coachId: coachId);
        _bookingRepoMock.Setup(r => r.GetByFacilityIdAsync(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>());
        _bookingRepoMock.Setup(r => r.GetByCoachIdAsync(coachId, booking.BookingDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>
            {
                BookingTestDataBuilder.CreateBooking(
                    coachId: coachId,
                    status: BookingStatus.Rejected,
                    bookingDate: booking.BookingDate,
                    startTime: booking.StartTime.Add(TimeSpan.FromMinutes(30)),
                    endTime: booking.EndTime.Add(TimeSpan.FromMinutes(30)))
            });

        var result = await _service.DetectConflictsAsync(booking);

        result.Should().BeEmpty();
    }

    #endregion

    #region DetectConflictsAsync - Athlete Overlap

    [Fact]
    public async Task DetectConflictsAsync_AthleteOverlap_DetectsConflict()
    {
        var athleteId = Guid.NewGuid();
        var booking = BookingTestDataBuilder.CreateBooking(athleteId: athleteId);
        _bookingRepoMock.Setup(r => r.GetByFacilityIdAsync(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>());
        _bookingRepoMock.Setup(r => r.GetByCoachIdAsync(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>());
        _bookingRepoMock.Setup(r => r.GetByAthleteIdAsync(athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>
            {
                BookingTestDataBuilder.CreateBooking(
                    athleteId: athleteId,
                    status: BookingStatus.Confirmed,
                    bookingDate: booking.BookingDate,
                    startTime: booking.StartTime.Add(TimeSpan.FromMinutes(30)),
                    endTime: booking.EndTime.Add(TimeSpan.FromMinutes(30)))
            });

        var result = await _service.DetectConflictsAsync(booking);

        result.Should().Contain(c => c.ConflictType == BookingConflictType.AthleteOverlap);
    }

    [Fact]
    public async Task DetectConflictsAsync_AthleteOverlap_DifferentDate_NoConflict()
    {
        var athleteId = Guid.NewGuid();
        var booking = BookingTestDataBuilder.CreateBooking(athleteId: athleteId);
        var otherDate = booking.BookingDate.AddDays(5);
        _bookingRepoMock.Setup(r => r.GetByFacilityIdAsync(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>());
        _bookingRepoMock.Setup(r => r.GetByCoachIdAsync(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>());
        _bookingRepoMock.Setup(r => r.GetByAthleteIdAsync(athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>
            {
                BookingTestDataBuilder.CreateBooking(
                    athleteId: athleteId,
                    status: BookingStatus.Confirmed,
                    bookingDate: otherDate,
                    startTime: booking.StartTime,
                    endTime: booking.EndTime)
            });

        var result = await _service.DetectConflictsAsync(booking);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task DetectConflictsAsync_AthleteOverlap_ExcludesCancelled()
    {
        var athleteId = Guid.NewGuid();
        var booking = BookingTestDataBuilder.CreateBooking(athleteId: athleteId);
        _bookingRepoMock.Setup(r => r.GetByFacilityIdAsync(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>());
        _bookingRepoMock.Setup(r => r.GetByCoachIdAsync(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>());
        _bookingRepoMock.Setup(r => r.GetByAthleteIdAsync(athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>
            {
                BookingTestDataBuilder.CreateBooking(
                    athleteId: athleteId,
                    status: BookingStatus.Cancelled,
                    bookingDate: booking.BookingDate,
                    startTime: booking.StartTime,
                    endTime: booking.EndTime)
            });

        var result = await _service.DetectConflictsAsync(booking);

        result.Should().BeEmpty();
    }

    #endregion

    #region DetectConflictsAsync - Multiple conflicts

    [Fact]
    public async Task DetectConflictsAsync_MultipleFacilityConflicts_ReturnsAll()
    {
        var facilityId = Guid.NewGuid();
        var booking = BookingTestDataBuilder.CreateBooking(facilityId: facilityId);
        _bookingRepoMock.Setup(r => r.GetByFacilityIdAsync(facilityId, booking.BookingDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>
            {
                BookingTestDataBuilder.CreateBooking(
                    facilityId: facilityId,
                    status: BookingStatus.Confirmed,
                    bookingDate: booking.BookingDate,
                    startTime: booking.StartTime.Add(TimeSpan.FromMinutes(15)),
                    endTime: booking.EndTime.Add(TimeSpan.FromMinutes(15))),
                BookingTestDataBuilder.CreateBooking(
                    facilityId: facilityId,
                    status: BookingStatus.Confirmed,
                    bookingDate: booking.BookingDate,
                    startTime: booking.StartTime.Add(TimeSpan.FromMinutes(30)),
                    endTime: booking.EndTime.Add(TimeSpan.FromMinutes(30)))
            });

        var result = await _service.DetectConflictsAsync(booking);

        result.Should().HaveCount(2);
        result.Should().OnlyContain(c => c.ConflictType == BookingConflictType.FacilityOverlap);
    }

    [Fact]
    public async Task DetectConflictsAsync_BothFacilityAndCoachOverlaps_ReturnsBothTypes()
    {
        var facilityId = Guid.NewGuid();
        var coachId = Guid.NewGuid();
        var booking = BookingTestDataBuilder.CreateBooking(facilityId: facilityId, coachId: coachId);
        var overlap1Start = booking.StartTime.Add(TimeSpan.FromMinutes(15));
        var overlap1End = booking.EndTime.Add(TimeSpan.FromMinutes(15));
        var overlap2Start = booking.StartTime.Add(TimeSpan.FromMinutes(45));
        var overlap2End = booking.EndTime.Add(TimeSpan.FromMinutes(45));

        _bookingRepoMock.Setup(r => r.GetByFacilityIdAsync(facilityId, booking.BookingDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>
            {
                BookingTestDataBuilder.CreateBooking(
                    facilityId: facilityId,
                    status: BookingStatus.Confirmed,
                    bookingDate: booking.BookingDate,
                    startTime: overlap1Start,
                    endTime: overlap1End)
            });
        _bookingRepoMock.Setup(r => r.GetByCoachIdAsync(coachId, booking.BookingDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>
            {
                BookingTestDataBuilder.CreateBooking(
                    coachId: coachId,
                    status: BookingStatus.Confirmed,
                    bookingDate: booking.BookingDate,
                    startTime: overlap1Start,
                    endTime: overlap1End)
            });

        var result = await _service.DetectConflictsAsync(booking);

        result.Should().Contain(c => c.ConflictType == BookingConflictType.FacilityOverlap);
        result.Should().Contain(c => c.ConflictType == BookingConflictType.CoachOverlap);
    }

    [Fact]
    public async Task DetectConflictsAsync_ExcludesSelfBooking()
    {
        var facilityId = Guid.NewGuid();
        var booking = BookingTestDataBuilder.CreateBooking(facilityId: facilityId);
        _bookingRepoMock.Setup(r => r.GetByFacilityIdAsync(facilityId, booking.BookingDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking> { booking });

        var result = await _service.DetectConflictsAsync(booking);

        result.Should().BeEmpty();
    }

    #endregion

    #region DetectConflictsForUpdateAsync

    [Fact]
    public async Task DetectConflictsForUpdateAsync_CreatesUpdatedBookingAndDetects()
    {
        var facilityId = Guid.NewGuid();
        var booking = BookingTestDataBuilder.CreateBooking(facilityId: facilityId);
        var newDate = booking.BookingDate.AddDays(1);
        var newStart = TimeSpan.FromHours(14);
        var newEnd = TimeSpan.FromHours(16);

        _bookingRepoMock.Setup(r => r.GetByFacilityIdAsync(facilityId, newDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>
            {
                BookingTestDataBuilder.CreateBooking(
                    facilityId: facilityId,
                    status: BookingStatus.Confirmed,
                    bookingDate: newDate,
                    startTime: TimeSpan.FromHours(15),
                    endTime: TimeSpan.FromHours(17))
            });

        var result = await _service.DetectConflictsForUpdateAsync(
            booking, newDate, newStart, newEnd);

        result.Should().HaveCount(1);
        result[0].ConflictType.Should().Be(BookingConflictType.FacilityOverlap);
    }

    [Fact]
    public async Task DetectConflictsForUpdateAsync_NoOverlap_ReturnsEmpty()
    {
        var facilityId = Guid.NewGuid();
        var booking = BookingTestDataBuilder.CreateBooking(facilityId: facilityId);
        var newDate = booking.BookingDate.AddDays(1);

        _bookingRepoMock.Setup(r => r.GetByFacilityIdAsync(facilityId, newDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>());

        var result = await _service.DetectConflictsForUpdateAsync(
            booking, newDate, TimeSpan.FromHours(9), TimeSpan.FromHours(10));

        result.Should().BeEmpty();
    }

    #endregion

    #region Adjacent time slots (no conflict)

    [Fact]
    public async Task DetectConflictsAsync_AdjacentTimeSlots_NoConflict()
    {
        var facilityId = Guid.NewGuid();
        var booking = BookingTestDataBuilder.CreateBooking(
            facilityId: facilityId,
            startTime: TimeSpan.FromHours(9),
            endTime: TimeSpan.FromHours(10));
        _bookingRepoMock.Setup(r => r.GetByFacilityIdAsync(facilityId, booking.BookingDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>
            {
                BookingTestDataBuilder.CreateBooking(
                    facilityId: facilityId,
                    status: BookingStatus.Confirmed,
                    bookingDate: booking.BookingDate,
                    startTime: TimeSpan.FromHours(10),
                    endTime: TimeSpan.FromHours(11))
            });

        var result = await _service.DetectConflictsAsync(booking);

        result.Should().BeEmpty();
    }

    #endregion
}
