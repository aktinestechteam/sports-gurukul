using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Features.SharedScheduling.Engine;
using SportsGurukul.Application.Features.SharedScheduling.Models;

namespace SportsGurukul.Application.Tests.SharedScheduling.Engine;

public class AvailabilityEngineTests
{
    private readonly Mock<ITimeSlotGenerator> _slotGeneratorMock = new();
    private readonly Mock<IBusinessHoursProvider> _businessHoursMock = new();
    private readonly Mock<IConflictDetectionEngine> _conflictEngineMock = new();
    private readonly Mock<ILogger<AvailabilityEngine>> _loggerMock = new();
    private readonly AvailabilityEngine _engine;

    public AvailabilityEngineTests()
    {
        _engine = new AvailabilityEngine(
            _slotGeneratorMock.Object,
            _businessHoursMock.Object,
            _conflictEngineMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task GetAvailableSlotsAsync_ReturnsSlotsExcludingBlocked()
    {
        var resourceId = Guid.NewGuid();
        var date = new DateTime(2026, 1, 15);
        var context = new SchedulingContext();

        var allSlots = new List<TimeSlot>
        {
            TimeSlot.Create(date, TimeSpan.FromHours(9), TimeSpan.FromHours(10)),
            TimeSlot.Create(date, TimeSpan.FromHours(10), TimeSpan.FromHours(11)),
            TimeSlot.Create(date, TimeSpan.FromHours(11), TimeSpan.FromHours(12))
        };
        var blockedSlots = new List<TimeSlot>
        {
            TimeSlot.Create(date, TimeSpan.FromHours(10), TimeSpan.FromHours(11))
        };
        var expectedSlots = new List<TimeSlot>
        {
            TimeSlot.Create(date, TimeSpan.FromHours(9), TimeSpan.FromHours(10)),
            TimeSlot.Create(date, TimeSpan.FromHours(11), TimeSpan.FromHours(12))
        };

        _businessHoursMock.Setup(b => b.GetBusinessHourSlotsAsync(resourceId, "Facility", date, It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(allSlots);
        _slotGeneratorMock.Setup(s => s.SubtractSlots(It.IsAny<IReadOnlyList<TimeSlot>>(), It.IsAny<IReadOnlyList<TimeSlot>>()))
            .Returns(expectedSlots);

        var result = await _engine.GetAvailableSlotsAsync(resourceId, "Facility", date, context);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetNextAvailableSlotAsync_WithAvailableSlot_ReturnsFirstSlot()
    {
        var resourceId = Guid.NewGuid();
        var date = new DateTime(2026, 1, 15);
        var context = new SchedulingContext();
        var expected = TimeSlot.Create(date, TimeSpan.FromHours(9), TimeSpan.FromHours(10));

        _businessHoursMock.Setup(b => b.GetBusinessHourSlotsAsync(resourceId, "Facility", date, It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TimeSlot> { expected });
        _slotGeneratorMock.Setup(s => s.SubtractSlots(It.IsAny<IReadOnlyList<TimeSlot>>(), It.IsAny<IReadOnlyList<TimeSlot>>()))
            .Returns(new List<TimeSlot> { expected });

        var result = await _engine.GetNextAvailableSlotAsync(resourceId, "Facility", date, context);

        result.Should().NotBeNull();
        result!.StartTime.Should().Be(TimeSpan.FromHours(9));
    }

    [Fact]
    public async Task GetNextAvailableSlotAsync_NoAvailableSlots_ReturnsNull()
    {
        var resourceId = Guid.NewGuid();
        var context = new SchedulingContext();

        _businessHoursMock.Setup(b => b.GetBusinessHourSlotsAsync(resourceId, "Facility", It.IsAny<DateTime>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TimeSlot>());
        _slotGeneratorMock.Setup(s => s.SubtractSlots(It.IsAny<IReadOnlyList<TimeSlot>>(), It.IsAny<IReadOnlyList<TimeSlot>>()))
            .Returns(new List<TimeSlot>());

        var result = await _engine.GetNextAvailableSlotAsync(resourceId, "Facility", new DateTime(2026, 1, 15), context);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAvailabilityWindowAsync_ReturnsWindowWithCorrectCounts()
    {
        var resourceId = Guid.NewGuid();
        var date = new DateTime(2026, 1, 15);
        var context = new SchedulingContext
        {
            BlockedSlots =
            [
                new BlockedTimeSlot
                {
                    ResourceId = resourceId,
                    ResourceType = "Facility",
                    Slot = TimeSlot.Create(date, TimeSpan.FromHours(10), TimeSpan.FromHours(11)),
                    Reason = BlockedReason.Maintenance
                }
            ]
        };

        var availableSlots = new List<TimeSlot>
        {
            TimeSlot.Create(date, TimeSpan.FromHours(9), TimeSpan.FromHours(10)),
            TimeSlot.Create(date, TimeSpan.FromHours(11), TimeSpan.FromHours(12))
        };

        _businessHoursMock.Setup(b => b.GetBusinessHourSlotsAsync(resourceId, "Facility", date, It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TimeSlot>
            {
                TimeSlot.Create(date, TimeSpan.FromHours(9), TimeSpan.FromHours(10)),
                TimeSlot.Create(date, TimeSpan.FromHours(10), TimeSpan.FromHours(11)),
                TimeSlot.Create(date, TimeSpan.FromHours(11), TimeSpan.FromHours(12))
            });
        _slotGeneratorMock.Setup(s => s.SubtractSlots(It.IsAny<IReadOnlyList<TimeSlot>>(), It.IsAny<IReadOnlyList<TimeSlot>>()))
            .Returns(availableSlots);

        var window = await _engine.GetAvailabilityWindowAsync(resourceId, "Facility", date, context);

        window.ResourceId.Should().Be(resourceId);
        window.AvailableSlots.Should().HaveCount(2);
        window.BlockedSlots.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetResourceUtilizationAsync_ReturnsMetricsForEachResource()
    {
        var resourceIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var startDate = new DateTime(2026, 1, 15);
        var endDate = new DateTime(2026, 1, 16);
        var context = new SchedulingContext();

        _businessHoursMock.Setup(b => b.GetBusinessHourSlotsAsync(It.IsAny<Guid>(), "Facility", It.IsAny<DateTime>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TimeSlot>());
        _slotGeneratorMock.Setup(s => s.SubtractSlots(It.IsAny<IReadOnlyList<TimeSlot>>(), It.IsAny<IReadOnlyList<TimeSlot>>()))
            .Returns(new List<TimeSlot>());

        var metrics = await _engine.GetResourceUtilizationAsync("Facility", resourceIds, startDate, endDate, context);

        metrics.Should().HaveCount(2);
    }
}
