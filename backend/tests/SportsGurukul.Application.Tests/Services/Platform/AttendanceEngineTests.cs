using FluentAssertions;
using Moq;
using SportsGurukul.Application.Features.RegistrationAttendancePlatform.Engines;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Tests.Services.Platform;

public class AttendanceEngineTests
{
    private readonly Mock<ILogger<AttendanceEngine>> _loggerMock = new();
    private readonly AttendanceEngine _engine;

    public AttendanceEngineTests()
    {
        _engine = new AttendanceEngine(_loggerMock.Object);
    }

    [Fact]
    public async Task CanCheckInAsync_ActiveRegistration_ReturnsTrue()
    {
        Func<Guid, CancellationToken, Task<bool>> isActiveRegistration = (_, _) => Task.FromResult(true);

        var result = await _engine.CanCheckInAsync(Guid.NewGuid(), isActiveRegistration, CancellationToken.None);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task CanCheckInAsync_InactiveRegistration_ReturnsFalse()
    {
        Func<Guid, CancellationToken, Task<bool>> isActiveRegistration = (_, _) => Task.FromResult(false);

        var result = await _engine.CanCheckInAsync(Guid.NewGuid(), isActiveRegistration, CancellationToken.None);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task CanCheckOutAsync_CheckedIn_ReturnsTrue()
    {
        Func<Guid, CancellationToken, Task<bool>> isCheckedIn = (_, _) => Task.FromResult(true);

        var result = await _engine.CanCheckOutAsync(Guid.NewGuid(), isCheckedIn, CancellationToken.None);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task CanCheckOutAsync_NotCheckedIn_ReturnsFalse()
    {
        Func<Guid, CancellationToken, Task<bool>> isCheckedIn = (_, _) => Task.FromResult(false);

        var result = await _engine.CanCheckOutAsync(Guid.NewGuid(), isCheckedIn, CancellationToken.None);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task CalculateAttendanceRateAsync_WithParticipants_ReturnsCorrectRate()
    {
        var result = await _engine.CalculateAttendanceRateAsync(Guid.NewGuid(), 100, 75);

        result.Should().Be(75.0);
    }

    [Fact]
    public async Task CalculateAttendanceRateAsync_ZeroParticipants_ReturnsZero()
    {
        var result = await _engine.CalculateAttendanceRateAsync(Guid.NewGuid(), 0, 0);

        result.Should().Be(0.0);
    }

    [Fact]
    public async Task DetermineAttendanceStatusAsync_OnTimeCheckIn_ReturnsPresent()
    {
        var scheduledStart = new DateTime(2025, 1, 15, 9, 0, 0, DateTimeKind.Utc);
        var checkInTime = new DateTime(2025, 1, 15, 8, 55, 0, DateTimeKind.Utc);
        var scheduledEnd = new DateTime(2025, 1, 15, 17, 0, 0, DateTimeKind.Utc);

        var result = await _engine.DetermineAttendanceStatusAsync(checkInTime, scheduledStart, null, scheduledEnd);

        result.Should().Be(PlatformAttendanceStatus.Present);
    }

    [Fact]
    public async Task DetermineAttendanceStatusAsync_LateCheckIn_ReturnsLate()
    {
        var scheduledStart = new DateTime(2025, 1, 15, 9, 0, 0, DateTimeKind.Utc);
        var checkInTime = new DateTime(2025, 1, 15, 9, 20, 0, DateTimeKind.Utc);
        var scheduledEnd = new DateTime(2025, 1, 15, 17, 0, 0, DateTimeKind.Utc);

        var result = await _engine.DetermineAttendanceStatusAsync(checkInTime, scheduledStart, null, scheduledEnd);

        result.Should().Be(PlatformAttendanceStatus.Late);
    }
}
