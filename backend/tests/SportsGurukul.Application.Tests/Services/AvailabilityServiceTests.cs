using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Services;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Tests.Services;

public class AvailabilityServiceTests
{
    private readonly Mock<IBookingRepository> _bookingRepoMock = TestMocks.CreateBookingRepository();
    private readonly Mock<IFacilityRepository> _facilityRepoMock = new();
    private readonly Mock<ICoachRepository> _coachRepoMock = new();
    private readonly Mock<IAthleteRepository> _athleteRepoMock = new();
    private readonly Mock<ILogger<AvailabilityService>> _loggerMock = TestMocks.CreateLogger<AvailabilityService>();
    private readonly AvailabilityService _service;

    public AvailabilityServiceTests()
    {
        _service = new AvailabilityService(
            _bookingRepoMock.Object,
            _facilityRepoMock.Object,
            _coachRepoMock.Object,
            _athleteRepoMock.Object,
            _loggerMock.Object);
    }

    #region IsFacilityAvailableAsync

    [Fact]
    public async Task IsFacilityAvailableAsync_FacilityNotFound_ReturnsFalse()
    {
        _facilityRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Facility?)null);

        var result = await _service.IsFacilityAvailableAsync(
            Guid.NewGuid(), DateTime.UtcNow.Date, TimeSpan.FromHours(9), TimeSpan.FromHours(10));

        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsFacilityAvailableAsync_FacilityDeleted_ReturnsFalse()
    {
        var facility = BookingTestDataBuilder.CreateFacility(isDeleted: true);
        _facilityRepoMock.Setup(r => r.GetByIdAsync(facility.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(facility);

        var result = await _service.IsFacilityAvailableAsync(
            facility.Id, DateTime.UtcNow.Date, TimeSpan.FromHours(9), TimeSpan.FromHours(10));

        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsFacilityAvailableAsync_FacilityInactive_ReturnsFalse()
    {
        var facility = BookingTestDataBuilder.CreateFacility(status: FacilityStatus.Inactive);
        _facilityRepoMock.Setup(r => r.GetByIdAsync(facility.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(facility);

        var result = await _service.IsFacilityAvailableAsync(
            facility.Id, DateTime.UtcNow.Date, TimeSpan.FromHours(9), TimeSpan.FromHours(10));

        result.Should().BeFalse();
    }

    [Theory]
    [InlineData(FacilityStatus.UnderMaintenance)]
    [InlineData(FacilityStatus.Closed)]
    [InlineData(FacilityStatus.PendingApproval)]
    public async Task IsFacilityAvailableAsync_NonActiveStatus_ReturnsFalse(FacilityStatus status)
    {
        var facility = BookingTestDataBuilder.CreateFacility(status: status);
        _facilityRepoMock.Setup(r => r.GetByIdAsync(facility.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(facility);

        var result = await _service.IsFacilityAvailableAsync(
            facility.Id, DateTime.UtcNow.Date, TimeSpan.FromHours(9), TimeSpan.FromHours(10));

        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsFacilityAvailableAsync_NoExistingBookings_ReturnsTrue()
    {
        var facility = BookingTestDataBuilder.CreateFacility();
        _facilityRepoMock.Setup(r => r.GetByIdAsync(facility.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(facility);
        _bookingRepoMock.Setup(r => r.GetByFacilityIdAsync(facility.Id, It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>());

        var result = await _service.IsFacilityAvailableAsync(
            facility.Id, DateTime.UtcNow.Date, TimeSpan.FromHours(9), TimeSpan.FromHours(10));

        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsFacilityAvailableAsync_ExistingBookingOverlaps_ReturnsFalse()
    {
        var facility = BookingTestDataBuilder.CreateFacility();
        var date = DateTime.UtcNow.Date.AddDays(1);
        _facilityRepoMock.Setup(r => r.GetByIdAsync(facility.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(facility);
        _bookingRepoMock.Setup(r => r.GetByFacilityIdAsync(facility.Id, date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>
            {
                BookingTestDataBuilder.CreateBooking(
                    facilityId: facility.Id,
                    status: BookingStatus.Confirmed,
                    bookingDate: date,
                    startTime: TimeSpan.FromHours(10),
                    endTime: TimeSpan.FromHours(12))
            });

        var result = await _service.IsFacilityAvailableAsync(
            facility.Id, date, TimeSpan.FromHours(9), TimeSpan.FromHours(11));

        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsFacilityAvailableAsync_ExistingBookingDoesNotOverlap_ReturnsTrue()
    {
        var facility = BookingTestDataBuilder.CreateFacility();
        var date = DateTime.UtcNow.Date.AddDays(1);
        _facilityRepoMock.Setup(r => r.GetByIdAsync(facility.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(facility);
        _bookingRepoMock.Setup(r => r.GetByFacilityIdAsync(facility.Id, date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>
            {
                BookingTestDataBuilder.CreateBooking(
                    facilityId: facility.Id,
                    status: BookingStatus.Confirmed,
                    bookingDate: date,
                    startTime: TimeSpan.FromHours(12),
                    endTime: TimeSpan.FromHours(14))
            });

        var result = await _service.IsFacilityAvailableAsync(
            facility.Id, date, TimeSpan.FromHours(9), TimeSpan.FromHours(11));

        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsFacilityAvailableAsync_ExcludesCancelledBookings_ReturnsTrue()
    {
        var facility = BookingTestDataBuilder.CreateFacility();
        var date = DateTime.UtcNow.Date.AddDays(1);
        _facilityRepoMock.Setup(r => r.GetByIdAsync(facility.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(facility);
        _bookingRepoMock.Setup(r => r.GetByFacilityIdAsync(facility.Id, date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>
            {
                BookingTestDataBuilder.CreateBooking(
                    facilityId: facility.Id,
                    status: BookingStatus.Cancelled,
                    bookingDate: date,
                    startTime: TimeSpan.FromHours(9),
                    endTime: TimeSpan.FromHours(11))
            });

        var result = await _service.IsFacilityAvailableAsync(
            facility.Id, date, TimeSpan.FromHours(9), TimeSpan.FromHours(11));

        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsFacilityAvailableAsync_ExcludesRejectedBookings_ReturnsTrue()
    {
        var facility = BookingTestDataBuilder.CreateFacility();
        var date = DateTime.UtcNow.Date.AddDays(1);
        _facilityRepoMock.Setup(r => r.GetByIdAsync(facility.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(facility);
        _bookingRepoMock.Setup(r => r.GetByFacilityIdAsync(facility.Id, date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>
            {
                BookingTestDataBuilder.CreateBooking(
                    facilityId: facility.Id,
                    status: BookingStatus.Rejected,
                    bookingDate: date,
                    startTime: TimeSpan.FromHours(9),
                    endTime: TimeSpan.FromHours(11))
            });

        var result = await _service.IsFacilityAvailableAsync(
            facility.Id, date, TimeSpan.FromHours(9), TimeSpan.FromHours(11));

        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsFacilityAvailableAsync_ExcludeBookingId_ReturnsTrue()
    {
        var facility = BookingTestDataBuilder.CreateFacility();
        var date = DateTime.UtcNow.Date.AddDays(1);
        var bookingId = Guid.NewGuid();
        _facilityRepoMock.Setup(r => r.GetByIdAsync(facility.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(facility);
        _bookingRepoMock.Setup(r => r.GetByFacilityIdAsync(facility.Id, date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>
            {
                BookingTestDataBuilder.CreateBooking(
                    id: bookingId,
                    facilityId: facility.Id,
                    status: BookingStatus.Confirmed,
                    bookingDate: date,
                    startTime: TimeSpan.FromHours(9),
                    endTime: TimeSpan.FromHours(11))
            });

        var result = await _service.IsFacilityAvailableAsync(
            facility.Id, date, TimeSpan.FromHours(9), TimeSpan.FromHours(11),
            excludeBookingId: bookingId);

        result.Should().BeTrue();
    }

    #endregion

    #region IsCoachAvailableAsync

    [Fact]
    public async Task IsCoachAvailableAsync_CoachNotFound_ReturnsFalse()
    {
        _coachRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Coach?)null);

        var result = await _service.IsCoachAvailableAsync(
            Guid.NewGuid(), DateTime.UtcNow.Date, TimeSpan.FromHours(9), TimeSpan.FromHours(10));

        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsCoachAvailableAsync_CoachDeleted_ReturnsFalse()
    {
        var coach = TestDataBuilder.CreateCoach(isDeleted: true);
        _coachRepoMock.Setup(r => r.GetByIdAsync(coach.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coach);

        var result = await _service.IsCoachAvailableAsync(
            coach.Id, DateTime.UtcNow.Date, TimeSpan.FromHours(9), TimeSpan.FromHours(10));

        result.Should().BeFalse();
    }

    [Theory]
    [InlineData(CoachStatus.Inactive)]
    [InlineData(CoachStatus.Suspended)]
    [InlineData(CoachStatus.Pending)]
    [InlineData(CoachStatus.Rejected)]
    public async Task IsCoachAvailableAsync_NonActiveStatus_ReturnsFalse(CoachStatus status)
    {
        var coach = TestDataBuilder.CreateCoach();
        coach.Status = status;
        _coachRepoMock.Setup(r => r.GetByIdAsync(coach.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coach);

        var result = await _service.IsCoachAvailableAsync(
            coach.Id, DateTime.UtcNow.Date, TimeSpan.FromHours(9), TimeSpan.FromHours(10));

        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsCoachAvailableAsync_NoExistingBookings_ReturnsTrue()
    {
        var coach = TestDataBuilder.CreateCoach();
        _coachRepoMock.Setup(r => r.GetByIdAsync(coach.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coach);
        _bookingRepoMock.Setup(r => r.GetByCoachIdAsync(coach.Id, It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>());

        var result = await _service.IsCoachAvailableAsync(
            coach.Id, DateTime.UtcNow.Date, TimeSpan.FromHours(9), TimeSpan.FromHours(10));

        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsCoachAvailableAsync_ExistingBookingOverlaps_ReturnsFalse()
    {
        var coach = TestDataBuilder.CreateCoach();
        var date = DateTime.UtcNow.Date.AddDays(1);
        _coachRepoMock.Setup(r => r.GetByIdAsync(coach.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coach);
        _bookingRepoMock.Setup(r => r.GetByCoachIdAsync(coach.Id, date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>
            {
                BookingTestDataBuilder.CreateBooking(
                    coachId: coach.Id,
                    status: BookingStatus.Confirmed,
                    bookingDate: date,
                    startTime: TimeSpan.FromHours(10),
                    endTime: TimeSpan.FromHours(12))
            });

        var result = await _service.IsCoachAvailableAsync(
            coach.Id, date, TimeSpan.FromHours(9), TimeSpan.FromHours(11));

        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsCoachAvailableAsync_ExcludeBookingId_ReturnsTrue()
    {
        var coach = TestDataBuilder.CreateCoach();
        var date = DateTime.UtcNow.Date.AddDays(1);
        var bookingId = Guid.NewGuid();
        _coachRepoMock.Setup(r => r.GetByIdAsync(coach.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coach);
        _bookingRepoMock.Setup(r => r.GetByCoachIdAsync(coach.Id, date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>
            {
                BookingTestDataBuilder.CreateBooking(
                    id: bookingId,
                    coachId: coach.Id,
                    status: BookingStatus.Confirmed,
                    bookingDate: date,
                    startTime: TimeSpan.FromHours(9),
                    endTime: TimeSpan.FromHours(11))
            });

        var result = await _service.IsCoachAvailableAsync(
            coach.Id, date, TimeSpan.FromHours(9), TimeSpan.FromHours(11),
            excludeBookingId: bookingId);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsCoachAvailableAsync_ExactTimeMatch_ReturnsFalse()
    {
        var coach = TestDataBuilder.CreateCoach();
        var date = DateTime.UtcNow.Date.AddDays(1);
        _coachRepoMock.Setup(r => r.GetByIdAsync(coach.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coach);
        _bookingRepoMock.Setup(r => r.GetByCoachIdAsync(coach.Id, date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>
            {
                BookingTestDataBuilder.CreateBooking(
                    coachId: coach.Id,
                    status: BookingStatus.Confirmed,
                    bookingDate: date,
                    startTime: TimeSpan.FromHours(9),
                    endTime: TimeSpan.FromHours(10))
            });

        var result = await _service.IsCoachAvailableAsync(
            coach.Id, date, TimeSpan.FromHours(9), TimeSpan.FromHours(10));

        result.Should().BeFalse();
    }

    #endregion

    #region IsAthleteAvailableAsync

    [Fact]
    public async Task IsAthleteAvailableAsync_AthleteNotFound_ReturnsFalse()
    {
        _athleteRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Athlete?)null);

        var result = await _service.IsAthleteAvailableAsync(
            Guid.NewGuid(), DateTime.UtcNow.Date, TimeSpan.FromHours(9), TimeSpan.FromHours(10));

        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsAthleteAvailableAsync_AthleteDeleted_ReturnsFalse()
    {
        var athlete = TestDataBuilder.CreateAthlete(isDeleted: true);
        _athleteRepoMock.Setup(r => r.GetByIdAsync(athlete.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(athlete);

        var result = await _service.IsAthleteAvailableAsync(
            athlete.Id, DateTime.UtcNow.Date, TimeSpan.FromHours(9), TimeSpan.FromHours(10));

        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsAthleteAvailableAsync_NoExistingBookings_ReturnsTrue()
    {
        var athlete = TestDataBuilder.CreateAthlete();
        _athleteRepoMock.Setup(r => r.GetByIdAsync(athlete.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(athlete);
        _bookingRepoMock.Setup(r => r.GetByAthleteIdAsync(athlete.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>());

        var result = await _service.IsAthleteAvailableAsync(
            athlete.Id, DateTime.UtcNow.Date, TimeSpan.FromHours(9), TimeSpan.FromHours(10));

        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsAthleteAvailableAsync_SameDateOverlap_ReturnsFalse()
    {
        var athlete = TestDataBuilder.CreateAthlete();
        var date = DateTime.UtcNow.Date.AddDays(1);
        _athleteRepoMock.Setup(r => r.GetByIdAsync(athlete.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(athlete);
        _bookingRepoMock.Setup(r => r.GetByAthleteIdAsync(athlete.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>
            {
                BookingTestDataBuilder.CreateBooking(
                    athleteId: athlete.Id,
                    status: BookingStatus.Confirmed,
                    bookingDate: date,
                    startTime: TimeSpan.FromHours(10),
                    endTime: TimeSpan.FromHours(12))
            });

        var result = await _service.IsAthleteAvailableAsync(
            athlete.Id, date, TimeSpan.FromHours(9), TimeSpan.FromHours(11));

        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsAthleteAvailableAsync_DifferentDate_ReturnsTrue()
    {
        var athlete = TestDataBuilder.CreateAthlete();
        var date = DateTime.UtcNow.Date.AddDays(1);
        var otherDate = DateTime.UtcNow.Date.AddDays(2);
        _athleteRepoMock.Setup(r => r.GetByIdAsync(athlete.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(athlete);
        _bookingRepoMock.Setup(r => r.GetByAthleteIdAsync(athlete.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>
            {
                BookingTestDataBuilder.CreateBooking(
                    athleteId: athlete.Id,
                    status: BookingStatus.Confirmed,
                    bookingDate: otherDate,
                    startTime: TimeSpan.FromHours(10),
                    endTime: TimeSpan.FromHours(12))
            });

        var result = await _service.IsAthleteAvailableAsync(
            athlete.Id, date, TimeSpan.FromHours(10), TimeSpan.FromHours(12));

        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsAthleteAvailableAsync_ExcludesCancelledBookings_ReturnsTrue()
    {
        var athlete = TestDataBuilder.CreateAthlete();
        var date = DateTime.UtcNow.Date.AddDays(1);
        _athleteRepoMock.Setup(r => r.GetByIdAsync(athlete.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(athlete);
        _bookingRepoMock.Setup(r => r.GetByAthleteIdAsync(athlete.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>
            {
                BookingTestDataBuilder.CreateBooking(
                    athleteId: athlete.Id,
                    status: BookingStatus.Cancelled,
                    bookingDate: date,
                    startTime: TimeSpan.FromHours(9),
                    endTime: TimeSpan.FromHours(11))
            });

        var result = await _service.IsAthleteAvailableAsync(
            athlete.Id, date, TimeSpan.FromHours(9), TimeSpan.FromHours(11));

        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsAthleteAvailableAsync_ExcludeBookingId_ReturnsTrue()
    {
        var athlete = TestDataBuilder.CreateAthlete();
        var date = DateTime.UtcNow.Date.AddDays(1);
        var bookingId = Guid.NewGuid();
        _athleteRepoMock.Setup(r => r.GetByIdAsync(athlete.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(athlete);
        _bookingRepoMock.Setup(r => r.GetByAthleteIdAsync(athlete.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>
            {
                BookingTestDataBuilder.CreateBooking(
                    id: bookingId,
                    athleteId: athlete.Id,
                    status: BookingStatus.Confirmed,
                    bookingDate: date,
                    startTime: TimeSpan.FromHours(9),
                    endTime: TimeSpan.FromHours(11))
            });

        var result = await _service.IsAthleteAvailableAsync(
            athlete.Id, date, TimeSpan.FromHours(9), TimeSpan.FromHours(11),
            excludeBookingId: bookingId);

        result.Should().BeTrue();
    }

    #endregion
}
