using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Services;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Tests.Services;

public class SchedulingEngineTests
{
    private readonly Mock<IBookingRepository> _bookingRepoMock = TestMocks.CreateBookingRepository();
    private readonly Mock<IBookingScheduleRepository> _scheduleRepoMock = new();
    private readonly Mock<IConflictDetectionService> _conflictServiceMock = new();
    private readonly Mock<ILogger<SchedulingEngine>> _loggerMock = TestMocks.CreateLogger<SchedulingEngine>();
    private readonly SchedulingEngine _engine;

    public SchedulingEngineTests()
    {
        _engine = new SchedulingEngine(
            _bookingRepoMock.Object,
            _scheduleRepoMock.Object,
            _conflictServiceMock.Object,
            _loggerMock.Object);
    }

    #region GenerateBookingNumberAsync

    [Fact]
    public async Task GenerateBookingNumberAsync_ReturnsValidFormat()
    {
        _bookingRepoMock.Setup(r => r.IsBookingNumberUniqueAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _engine.GenerateBookingNumberAsync();

        result.Should().StartWith("BK-");
        result.Should().HaveLength(16);
    }

    [Fact]
    public async Task GenerateBookingNumberAsync_CallsRepositoryToCheckUniqueness()
    {
        _bookingRepoMock.Setup(r => r.IsBookingNumberUniqueAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _engine.GenerateBookingNumberAsync();

        _bookingRepoMock.Verify(r => r.IsBookingNumberUniqueAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GenerateBookingNumberAsync_RetriesWhenNotUnique()
    {
        _bookingRepoMock.SetupSequence(r => r.IsBookingNumberUniqueAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false)
            .ReturnsAsync(false)
            .ReturnsAsync(true);

        var result = await _engine.GenerateBookingNumberAsync();

        result.Should().StartWith("BK-");
        _bookingRepoMock.Verify(r => r.IsBookingNumberUniqueAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
    }

    [Fact]
    public async Task GenerateBookingNumberAsync_GeneratesUniqueNumbersOnEachCall()
    {
        _bookingRepoMock.Setup(r => r.IsBookingNumberUniqueAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var results = new HashSet<string>();
        for (int i = 0; i < 10; i++)
        {
            results.Add(await _engine.GenerateBookingNumberAsync());
        }

        results.Should().HaveCount(10, "each booking number should be unique");
    }

    #endregion

    #region GenerateScheduleInstancesAsync

    [Fact]
    public async Task GenerateScheduleInstancesAsync_SingleOccurrence_ReturnsOneSchedule()
    {
        var booking = BookingTestDataBuilder.CreateBooking();
        var startDate = DateTime.UtcNow.Date.AddDays(1);

        var result = await _engine.GenerateScheduleInstancesAsync(
            booking, startDate, TimeSpan.FromHours(9), TimeSpan.FromHours(10),
            occurrenceCount: 1, endDate: null);

        result.Should().HaveCount(1);
        result[0].ScheduledDate.Date.Should().Be(startDate.Date);
        result[0].StartTime.Should().Be(TimeSpan.FromHours(9));
        result[0].EndTime.Should().Be(TimeSpan.FromHours(10));
        result[0].IsCancelled.Should().BeFalse();
    }

    [Fact]
    public async Task GenerateScheduleInstancesAsync_MultipleOccurrences_ReturnsCorrectCount()
    {
        var booking = BookingTestDataBuilder.CreateBooking();
        var startDate = DateTime.UtcNow.Date.AddDays(1);

        var result = await _engine.GenerateScheduleInstancesAsync(
            booking, startDate, TimeSpan.FromHours(9), TimeSpan.FromHours(10),
            occurrenceCount: 5, endDate: null);

        result.Should().HaveCount(5);
        for (int i = 0; i < 5; i++)
        {
            result[i].ScheduledDate.Date.Should().Be(startDate.AddDays(i).Date);
            result[i].BookingId.Should().Be(booking.Id);
        }
    }

    [Fact]
    public async Task GenerateScheduleInstancesAsync_WithEndDate_StopsAtEndDate()
    {
        var booking = BookingTestDataBuilder.CreateBooking();
        var startDate = DateTime.UtcNow.Date.AddDays(1);
        var endDate = startDate.AddDays(3);

        var result = await _engine.GenerateScheduleInstancesAsync(
            booking, startDate, TimeSpan.FromHours(9), TimeSpan.FromHours(10),
            occurrenceCount: 100, endDate: endDate);

        result.Should().HaveCount(4);
    }

    [Fact]
    public async Task GenerateScheduleInstancesAsync_DailySchedule_CreatesOnePerDay()
    {
        var booking = BookingTestDataBuilder.CreateBooking();
        var startDate = DateTime.UtcNow.Date.AddDays(1);

        var result = await _engine.GenerateScheduleInstancesAsync(
            booking, startDate, TimeSpan.FromHours(9), TimeSpan.FromHours(10),
            occurrenceCount: 7, endDate: null);

        result.Should().HaveCount(7);
        for (int i = 0; i < 7; i++)
        {
            result[i].ScheduledDate.Date.Should().Be(startDate.AddDays(i).Date);
        }
    }

    [Fact]
    public async Task GenerateScheduleInstancesAsync_CrossMidnightBooking_CreatesCorrectSchedule()
    {
        var booking = BookingTestDataBuilder.CreateBooking();
        var startDate = DateTime.UtcNow.Date.AddDays(1);
        var startTime = new TimeSpan(22, 0, 0);
        var endTime = new TimeSpan(1, 0, 0);

        var result = await _engine.GenerateScheduleInstancesAsync(
            booking, startDate, startTime, endTime,
            occurrenceCount: 3, endDate: null);

        result.Should().HaveCount(3);
        result[0].StartTime.Should().Be(startTime);
        result[0].EndTime.Should().Be(endTime);
    }

    [Fact]
    public async Task GenerateScheduleInstancesAsync_SetsCreatedAtAndUpdatedAt()
    {
        var booking = BookingTestDataBuilder.CreateBooking();
        var before = DateTime.UtcNow;

        var result = await _engine.GenerateScheduleInstancesAsync(
            booking, DateTime.UtcNow.Date.AddDays(1), TimeSpan.FromHours(9), TimeSpan.FromHours(10),
            occurrenceCount: 1, endDate: null);

        result[0].CreatedAt.Should().BeOnOrAfter(before);
        result[0].UpdatedAt.Should().BeOnOrAfter(before);
    }

    #endregion

    #region IsSlotAvailableAsync

    [Fact]
    public async Task IsSlotAvailableAsync_NoFacilityNoCoach_ReturnsTrue()
    {
        var result = await _engine.IsSlotAvailableAsync(
            Guid.NewGuid(), facilityId: null, coachId: null,
            DateTime.UtcNow.Date.AddDays(1), TimeSpan.FromHours(9), TimeSpan.FromHours(10));

        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsSlotAvailableAsync_FacilityAvailable_ReturnsTrue()
    {
        var facilityId = Guid.NewGuid();
        var date = DateTime.UtcNow.Date.AddDays(1);
        _bookingRepoMock.Setup(r => r.GetByFacilityIdAsync(facilityId, date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>());

        var result = await _engine.IsSlotAvailableAsync(
            Guid.NewGuid(), facilityId, coachId: null,
            date, TimeSpan.FromHours(9), TimeSpan.FromHours(10));

        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsSlotAvailableAsync_FacilityOverlap_ReturnsFalse()
    {
        var facilityId = Guid.NewGuid();
        var date = DateTime.UtcNow.Date.AddDays(1);
        _bookingRepoMock.Setup(r => r.GetByFacilityIdAsync(facilityId, date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>
            {
                BookingTestDataBuilder.CreateBooking(
                    facilityId: facilityId,
                    status: BookingStatus.Confirmed,
                    bookingDate: date,
                    startTime: TimeSpan.FromHours(10),
                    endTime: TimeSpan.FromHours(12))
            });

        var result = await _engine.IsSlotAvailableAsync(
            Guid.NewGuid(), facilityId, coachId: null,
            date, TimeSpan.FromHours(9), TimeSpan.FromHours(11));

        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsSlotAvailableAsync_CoachOverlap_ReturnsFalse()
    {
        var coachId = Guid.NewGuid();
        var date = DateTime.UtcNow.Date.AddDays(1);
        _bookingRepoMock.Setup(r => r.GetByCoachIdAsync(coachId, date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>
            {
                BookingTestDataBuilder.CreateBooking(
                    coachId: coachId,
                    status: BookingStatus.Confirmed,
                    bookingDate: date,
                    startTime: TimeSpan.FromHours(10),
                    endTime: TimeSpan.FromHours(12))
            });

        var result = await _engine.IsSlotAvailableAsync(
            Guid.NewGuid(), facilityId: null, coachId,
            date, TimeSpan.FromHours(9), TimeSpan.FromHours(11));

        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsSlotAvailableAsync_ExcludesCancelledBookings_ReturnsTrue()
    {
        var facilityId = Guid.NewGuid();
        var date = DateTime.UtcNow.Date.AddDays(1);
        _bookingRepoMock.Setup(r => r.GetByFacilityIdAsync(facilityId, date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>
            {
                BookingTestDataBuilder.CreateBooking(
                    facilityId: facilityId,
                    status: BookingStatus.Cancelled,
                    bookingDate: date,
                    startTime: TimeSpan.FromHours(9),
                    endTime: TimeSpan.FromHours(11))
            });

        var result = await _engine.IsSlotAvailableAsync(
            Guid.NewGuid(), facilityId, coachId: null,
            date, TimeSpan.FromHours(9), TimeSpan.FromHours(11));

        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsSlotAvailableAsync_ExcludesRejectedBookings_ReturnsTrue()
    {
        var facilityId = Guid.NewGuid();
        var date = DateTime.UtcNow.Date.AddDays(1);
        _bookingRepoMock.Setup(r => r.GetByFacilityIdAsync(facilityId, date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>
            {
                BookingTestDataBuilder.CreateBooking(
                    facilityId: facilityId,
                    status: BookingStatus.Rejected,
                    bookingDate: date,
                    startTime: TimeSpan.FromHours(9),
                    endTime: TimeSpan.FromHours(11))
            });

        var result = await _engine.IsSlotAvailableAsync(
            Guid.NewGuid(), facilityId, coachId: null,
            date, TimeSpan.FromHours(9), TimeSpan.FromHours(11));

        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsSlotAvailableAsync_ExcludeBookingId_ReturnsTrue()
    {
        var facilityId = Guid.NewGuid();
        var date = DateTime.UtcNow.Date.AddDays(1);
        var bookingId = Guid.NewGuid();
        _bookingRepoMock.Setup(r => r.GetByFacilityIdAsync(facilityId, date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>
            {
                BookingTestDataBuilder.CreateBooking(
                    id: bookingId,
                    facilityId: facilityId,
                    status: BookingStatus.Confirmed,
                    bookingDate: date,
                    startTime: TimeSpan.FromHours(9),
                    endTime: TimeSpan.FromHours(11))
            });

        var result = await _engine.IsSlotAvailableAsync(
            Guid.NewGuid(), facilityId, coachId: null,
            date, TimeSpan.FromHours(9), TimeSpan.FromHours(11),
            excludeBookingId: bookingId);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsSlotAvailableAsync_DifferentDate_ReturnsTrue()
    {
        var facilityId = Guid.NewGuid();
        var date = DateTime.UtcNow.Date.AddDays(1);
        var otherDate = DateTime.UtcNow.Date.AddDays(2);
        _bookingRepoMock.Setup(r => r.GetByFacilityIdAsync(facilityId, date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>
            {
                BookingTestDataBuilder.CreateBooking(
                    facilityId: facilityId,
                    status: BookingStatus.Confirmed,
                    bookingDate: otherDate,
                    startTime: TimeSpan.FromHours(9),
                    endTime: TimeSpan.FromHours(11))
            });

        var result = await _engine.IsSlotAvailableAsync(
            Guid.NewGuid(), facilityId, coachId: null,
            date, TimeSpan.FromHours(9), TimeSpan.FromHours(11));

        result.Should().BeTrue();
    }

    #endregion
}
