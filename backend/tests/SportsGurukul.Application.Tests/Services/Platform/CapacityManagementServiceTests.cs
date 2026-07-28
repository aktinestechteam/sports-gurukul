using FluentAssertions;
using Moq;
using SportsGurukul.Application.Features.RegistrationAttendancePlatform.Engines;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Tests.Services.Platform;

public class CapacityManagementServiceTests
{
    private readonly Mock<ILogger<CapacityManagementService>> _loggerMock = new();
    private readonly CapacityManagementService _service;

    public CapacityManagementServiceTests()
    {
        _service = new CapacityManagementService(_loggerMock.Object);
    }

    [Fact]
    public async Task HasAvailableCapacityAsync_WithinLimit_ReturnsTrue()
    {
        var result = await _service.HasAvailableCapacityAsync(50, 100);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task HasAvailableCapacityAsync_AtLimit_ReturnsFalse()
    {
        var result = await _service.HasAvailableCapacityAsync(100, 100);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task HasAvailableCapacityAsync_UnlimitedCapacity_ReturnsTrue()
    {
        var result = await _service.HasAvailableCapacityAsync(1000, null);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task GetAvailableSlotsAsync_WithCapacity_ReturnsDifference()
    {
        var result = await _service.GetAvailableSlotsAsync(30, 100);

        result.Should().Be(70);
    }

    [Fact]
    public async Task GetAvailableSlotsAsync_AtCapacity_ReturnsZero()
    {
        var result = await _service.GetAvailableSlotsAsync(100, 100);

        result.Should().Be(0);
    }

    [Fact]
    public async Task GetAvailableSlotsAsync_OverCapacity_ReturnsZero()
    {
        var result = await _service.GetAvailableSlotsAsync(110, 100);

        result.Should().Be(0);
    }

    [Fact]
    public async Task GetAvailableSlotsAsync_UnlimitedCapacity_ReturnsMaxValue()
    {
        var result = await _service.GetAvailableSlotsAsync(50, null);

        result.Should().Be(int.MaxValue);
    }

    [Fact]
    public async Task IsAtCapacityAsync_AtLimit_ReturnsTrue()
    {
        var result = await _service.IsAtCapacityAsync(100, 100);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsAtCapacityAsync_BelowLimit_ReturnsFalse()
    {
        var result = await _service.IsAtCapacityAsync(50, 100);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsAtCapacityAsync_UnlimitedCapacity_ReturnsFalse()
    {
        var result = await _service.IsAtCapacityAsync(10000, null);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task CalculateNextWaitlistPositionAsync_EmptyWaitlist_ReturnsOne()
    {
        var result = await _service.CalculateNextWaitlistPositionAsync(0);

        result.Should().Be(1);
    }

    [Fact]
    public async Task CalculateNextWaitlistPositionAsync_ExistingWaitlist_ReturnsIncremented()
    {
        var result = await _service.CalculateNextWaitlistPositionAsync(5);

        result.Should().Be(6);
    }

    [Fact]
    public async Task ShouldAutoApproveAsync_FreeWithCapacity_ReturnsTrue()
    {
        var result = await _service.ShouldAutoApproveAsync(ProgramType.Event, EventRegistrationType.Free, true);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task ShouldAutoApproveAsync_PaidWithCapacity_ReturnsFalse()
    {
        var result = await _service.ShouldAutoApproveAsync(ProgramType.Event, EventRegistrationType.Paid, true);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task ShouldAutoApproveAsync_FreeWithoutCapacity_ReturnsFalse()
    {
        var result = await _service.ShouldAutoApproveAsync(ProgramType.Event, EventRegistrationType.Free, false);

        result.Should().BeFalse();
    }
}
