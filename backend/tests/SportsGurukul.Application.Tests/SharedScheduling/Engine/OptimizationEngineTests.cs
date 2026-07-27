using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Features.SharedScheduling.Engine;
using SportsGurukul.Application.Features.SharedScheduling.Models;

namespace SportsGurukul.Application.Tests.SharedScheduling.Engine;

public class OptimizationEngineTests
{
    private readonly Mock<IAvailabilityEngine> _availabilityMock = new();
    private readonly Mock<ILogger<OptimizationEngine>> _loggerMock = new();
    private readonly OptimizationEngine _engine;

    public OptimizationEngineTests()
    {
        _engine = new OptimizationEngine(_availabilityMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task FindBestAvailableSlotAsync_AvailableSlot_ReturnsSlot()
    {
        var resourceId = Guid.NewGuid();
        var date = new DateTime(2026, 1, 15);
        var context = new SchedulingContext();
        var expected = TimeSlot.Create(date, TimeSpan.FromHours(9), TimeSpan.FromHours(10));

        _availabilityMock.Setup(a => a.GetAvailableSlotsAsync(resourceId, "Facility", date, context, It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TimeSlot> { expected });

        var result = await _engine.FindBestAvailableSlotAsync("Facility", [resourceId], date, TimeSpan.FromHours(1), context);

        result.Should().NotBeNull();
        result!.StartTime.Should().Be(TimeSpan.FromHours(9));
    }

    [Fact]
    public async Task FindBestAvailableSlotAsync_NoSlots_ReturnsNull()
    {
        var resourceId = Guid.NewGuid();
        var context = new SchedulingContext();

        _availabilityMock.Setup(a => a.GetAvailableSlotsAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<SchedulingContext>(), It.IsAny<TimeSpan?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TimeSlot>());

        var result = await _engine.FindBestAvailableSlotAsync("Facility", [resourceId], new DateTime(2026, 1, 15), TimeSpan.FromHours(1), context);

        result.Should().BeNull();
    }

    [Fact]
    public async Task FindLeastBusyResourceAsync_ReturnsLeastUtilized()
    {
        var resourceIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var context = new SchedulingContext();

        _availabilityMock.Setup(a => a.GetResourceUtilizationAsync("Facility", resourceIds, It.IsAny<DateTime>(), It.IsAny<DateTime>(), context, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UtilizationMetric>
            {
                new() { ResourceId = resourceIds[0], ResourceType = "Facility", TotalSlots = 10, BookedSlots = 8 },
                new() { ResourceId = resourceIds[1], ResourceType = "Facility", TotalSlots = 10, BookedSlots = 3 }
            });

        var result = await _engine.FindLeastBusyResourceAsync("Facility", resourceIds, new DateTime(2026, 1, 15), new DateTime(2026, 1, 22), context);

        result.Should().Be(resourceIds[1]);
    }

    [Fact]
    public async Task BalanceCoachLoadAsync_EmptyInputs_ReturnsEmpty()
    {
        var context = new SchedulingContext();

        var result = await _engine.BalanceCoachLoadAsync([], [], context);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task BalanceCoachLoadAsync_WithCoachesAndSlots_Balances()
    {
        var coach1 = Guid.NewGuid();
        var coach2 = Guid.NewGuid();
        var date = new DateTime(2026, 1, 15);
        var context = new SchedulingContext();
        var slots = new List<TimeSlot>
        {
            TimeSlot.Create(date, TimeSpan.FromHours(9), TimeSpan.FromHours(10)),
            TimeSlot.Create(date, TimeSpan.FromHours(10), TimeSpan.FromHours(11))
        };

        _availabilityMock.Setup(a => a.GetAvailableSlotsAsync(It.IsAny<Guid>(), "Coach", date, context, It.IsAny<TimeSpan?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TimeSlot>
            {
                TimeSlot.Create(date, TimeSpan.FromHours(9), TimeSpan.FromHours(12))
            });

        var result = await _engine.BalanceCoachLoadAsync([coach1, coach2], slots, context);

        result.Should().NotBeEmpty();
    }
}
