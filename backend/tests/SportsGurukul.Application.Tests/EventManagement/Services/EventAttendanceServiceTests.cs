using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.EventManagement.Services;
using SportsGurukul.Domain.Enums;
using SportsGurukul.Application.Tests.EventManagement.Fixtures;
using SportsGurukul.Application.Tests.EventManagement.Mocks;

namespace SportsGurukul.Application.Tests.EventManagement.Services;

public class EventAttendanceServiceTests
{
    private readonly Mock<IEventAttendanceRepository> _attendanceRepo;
    private readonly Mock<ILogger<EventAttendanceService>> _logger;
    private readonly EventAttendanceService _service;

    public EventAttendanceServiceTests()
    {
        _attendanceRepo = EventMockFactory.CreateAttendanceRepository();
        _logger = EventMockFactory.CreateLogger<EventAttendanceService>();
        _service = new EventAttendanceService(_attendanceRepo.Object, _logger.Object);
    }

    [Fact]
    public async Task CanCheckInAsync_Registered_ReturnsTrue()
    {
        var participant = EventDataFixture.CreateParticipant(status: EventAttendanceStatus.Registered);
        var result = await _service.CanCheckInAsync(participant);
        result.Should().BeTrue();
    }

    [Fact]
    public async Task CanCheckInAsync_Late_ReturnsTrue()
    {
        var participant = EventDataFixture.CreateParticipant(status: EventAttendanceStatus.Late);
        var result = await _service.CanCheckInAsync(participant);
        result.Should().BeTrue();
    }

    [Fact]
    public async Task CanCheckInAsync_AlreadyCheckedIn_ReturnsFalse()
    {
        var participant = EventDataFixture.CreateParticipant(status: EventAttendanceStatus.CheckedIn);
        var result = await _service.CanCheckInAsync(participant);
        result.Should().BeFalse();
    }

    [Fact]
    public async Task CanCheckOutAsync_HasCheckInNoCheckOut_ReturnsTrue()
    {
        var participant = EventDataFixture.CreateParticipant();
        var attendance = EventDataFixture.CreateAttendance();
        attendance.CheckInTime = DateTime.UtcNow;
        attendance.CheckOutTime = null;

        var result = await _service.CanCheckOutAsync(participant, attendance);
        result.Should().BeTrue();
    }

    [Fact]
    public async Task CanCheckOutAsync_NoCheckIn_ReturnsFalse()
    {
        var participant = EventDataFixture.CreateParticipant();
        var attendance = EventDataFixture.CreateAttendance();
        attendance.CheckInTime = null;

        var result = await _service.CanCheckOutAsync(participant, attendance);
        result.Should().BeFalse();
    }

    [Fact]
    public async Task CanCheckOutAsync_AlreadyCheckedOut_ReturnsFalse()
    {
        var participant = EventDataFixture.CreateParticipant();
        var attendance = EventDataFixture.CreateAttendance();
        attendance.CheckInTime = DateTime.UtcNow;
        attendance.CheckOutTime = DateTime.UtcNow;

        var result = await _service.CanCheckOutAsync(participant, attendance);
        result.Should().BeFalse();
    }

    [Fact]
    public async Task CalculateAttendanceRateAsync_NoParticipants_ReturnsZero()
    {
        _attendanceRepo.Setup(x => x.GetAttendeeCountAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(0);

        var result = await _service.CalculateAttendanceRateAsync(Guid.NewGuid());
        result.Should().Be(0);
    }
}
