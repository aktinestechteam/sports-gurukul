using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Features.SharedScheduling.Engine;
using SportsGurukul.Application.Features.SharedScheduling.Models;

namespace SportsGurukul.Application.Tests.SharedScheduling.Engine;

public class ConflictDetectionEngineTests
{
    private readonly Mock<IAvailabilityEngine> _availabilityEngineMock = new();
    private readonly Mock<ILogger<ConflictDetectionEngine>> _loggerMock = new();
    private readonly ConflictDetectionEngine _engine;

    public ConflictDetectionEngineTests()
    {
        _engine = new ConflictDetectionEngine(_loggerMock.Object);
    }

    [Fact]
    public async Task DetectConflictsAsync_BusinessHoursViolation_DetectsConflict()
    {
        var slot = TimeSlot.Create(new DateTime(2026, 1, 15), new TimeSpan(4, 0, 0), new TimeSpan(5, 0, 0));
        var context = new SchedulingContext
        {
            DayStartTime = new TimeOnly(6, 0),
            DayEndTime = new TimeOnly(22, 0),
            CheckBusinessHours = true
        };

        var conflicts = await _engine.DetectConflictsAsync(slot, [], context);

        conflicts.Should().HaveCount(1);
        conflicts[0].Type.Should().Be(ConflictType.BusinessHoursViolation);
    }

    [Fact]
    public async Task DetectConflictsAsync_HolidayConflict_DetectsConflict()
    {
        var slot = TimeSlot.Create(new DateTime(2026, 1, 26), TimeSpan.FromHours(9), TimeSpan.FromHours(10));
        var context = new SchedulingContext
        {
            Holidays = [new Holiday { Date = new DateTime(2026, 1, 26), Name = "Republic Day" }],
            CheckHolidays = true
        };

        var conflicts = await _engine.DetectConflictsAsync(slot, [], context);

        conflicts.Should().HaveCount(1);
        conflicts[0].Type.Should().Be(ConflictType.HolidayConflict);
    }

    [Fact]
    public async Task DetectConflictsAsync_BlockedSlot_DetectsConflict()
    {
        var resourceId = Guid.NewGuid();
        var slot = TimeSlot.Create(new DateTime(2026, 1, 15), TimeSpan.FromHours(9), TimeSpan.FromHours(10));
        var context = new SchedulingContext
        {
            BlockedSlots =
            [
                new BlockedTimeSlot
                {
                    ResourceId = resourceId,
                    ResourceType = "Facility",
                    Slot = slot,
                    Reason = BlockedReason.Maintenance
                }
            ]
        };
        var resources = new List<ResourceRequirement>
        {
            new() { ResourceId = resourceId, ResourceType = "Facility" }
        };

        var conflicts = await _engine.DetectConflictsAsync(slot, resources, context);

        conflicts.Should().HaveCount(1);
        conflicts[0].Type.Should().Be(ConflictType.MaintenanceWindow);
    }

    [Fact]
    public async Task DetectConflictsAsync_NoConflicts_ReturnsEmpty()
    {
        var slot = TimeSlot.Create(new DateTime(2026, 1, 15), TimeSpan.FromHours(9), TimeSpan.FromHours(10));
        var context = new SchedulingContext();

        var conflicts = await _engine.DetectConflictsAsync(slot, [], context);

        conflicts.Should().BeEmpty();
    }

    [Fact]
    public async Task HasConflictAsync_WithConflict_ReturnsTrue()
    {
        var slot = TimeSlot.Create(new DateTime(2026, 1, 26), TimeSpan.FromHours(9), TimeSpan.FromHours(10));
        var context = new SchedulingContext
        {
            Holidays = [new Holiday { Date = new DateTime(2026, 1, 26), Name = "Republic Day" }]
        };

        var hasConflict = await _engine.HasConflictAsync(slot, [], context);

        hasConflict.Should().BeTrue();
    }

    [Fact]
    public async Task HasConflictAsync_NoConflict_ReturnsFalse()
    {
        var slot = TimeSlot.Create(new DateTime(2026, 1, 15), TimeSpan.FromHours(9), TimeSpan.FromHours(10));
        var context = new SchedulingContext();

        var hasConflict = await _engine.HasConflictAsync(slot, [], context);

        hasConflict.Should().BeFalse();
    }

    [Fact]
    public async Task DetectConflictsForMultipleSlots_DetectsAllConflicts()
    {
        var slots = new List<TimeSlot>
        {
            TimeSlot.Create(new DateTime(2026, 1, 26), TimeSpan.FromHours(9), TimeSpan.FromHours(10)),
            TimeSlot.Create(new DateTime(2026, 1, 15), TimeSpan.FromHours(9), TimeSpan.FromHours(10))
        };
        var context = new SchedulingContext
        {
            Holidays = [new Holiday { Date = new DateTime(2026, 1, 26), Name = "Republic Day" }]
        };

        var conflicts = await _engine.DetectConflictsForMultipleSlotsAsync(slots, [], context);

        conflicts.Should().HaveCount(1);
    }

    [Fact]
    public async Task ResolveConflictAsync_ExistingConflict_ReturnsTrue()
    {
        var resourceId = Guid.NewGuid();
        var slot = TimeSlot.Create(new DateTime(2026, 1, 15), TimeSpan.FromHours(9), TimeSpan.FromHours(10));
        var context = new SchedulingContext
        {
            BlockedSlots =
            [
                new BlockedTimeSlot
                {
                    ResourceId = resourceId,
                    ResourceType = "Facility",
                    Slot = slot,
                    Reason = BlockedReason.Maintenance
                }
            ]
        };

        var conflicts = await _engine.DetectConflictsAsync(slot,
            [new ResourceRequirement { ResourceId = resourceId, ResourceType = "Facility" }], context);

        var conflictId = conflicts[0].ConflictId;
        var resolved = await _engine.ResolveConflictAsync(conflictId, "Resolved by admin");

        resolved.Should().BeTrue();
    }

    [Fact]
    public async Task ResolveConflictAsync_NonExistentConflict_ReturnsFalse()
    {
        var resolved = await _engine.ResolveConflictAsync(Guid.NewGuid(), "notes");

        resolved.Should().BeFalse();
    }

    [Fact]
    public async Task GetUnresolvedConflictsAsync_ReturnsStoredConflicts()
    {
        var resourceId = Guid.NewGuid();
        var conflicts = await _engine.GetUnresolvedConflictsAsync(resourceId, "Facility");

        conflicts.Should().BeEmpty();
    }
}
