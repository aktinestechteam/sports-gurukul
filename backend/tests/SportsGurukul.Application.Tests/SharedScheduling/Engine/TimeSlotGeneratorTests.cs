using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Features.SharedScheduling.Engine;
using SportsGurukul.Application.Features.SharedScheduling.Models;

namespace SportsGurukul.Application.Tests.SharedScheduling.Engine;

public class TimeSlotGeneratorTests
{
    private readonly Mock<ILogger<TimeSlotGenerator>> _loggerMock = new();
    private readonly TimeSlotGenerator _generator;

    public TimeSlotGeneratorTests()
    {
        _generator = new TimeSlotGenerator(_loggerMock.Object);
    }

    [Fact]
    public void GenerateDailySlots_StandardDay_ReturnsCorrectSlots()
    {
        var date = new DateTime(2026, 1, 15);
        var start = new TimeOnly(9, 0);
        var end = new TimeOnly(12, 0);
        var duration = TimeSpan.FromMinutes(60);

        var slots = _generator.GenerateDailySlots(date, start, end, duration);

        slots.Should().HaveCount(3);
        slots[0].StartTime.Should().Be(TimeSpan.FromHours(9));
        slots[0].EndTime.Should().Be(TimeSpan.FromHours(10));
        slots[1].StartTime.Should().Be(TimeSpan.FromHours(10));
        slots[1].EndTime.Should().Be(TimeSpan.FromHours(11));
        slots[2].StartTime.Should().Be(TimeSpan.FromHours(11));
        slots[2].EndTime.Should().Be(TimeSpan.FromHours(12));
    }

    [Fact]
    public void GenerateDailySlots_WithBuffer_RespectsBufferTime()
    {
        var date = new DateTime(2026, 1, 15);
        var start = new TimeOnly(9, 0);
        var end = new TimeOnly(12, 0);
        var duration = TimeSpan.FromMinutes(60);
        var buffer = TimeSpan.FromMinutes(15);

        var slots = _generator.GenerateDailySlots(date, start, end, duration, buffer);

        slots.Should().HaveCount(2);
        slots[0].StartTime.Should().Be(TimeSpan.FromHours(9));
        slots[0].EndTime.Should().Be(TimeSpan.FromHours(10));
        slots[1].StartTime.Should().Be(new TimeSpan(10, 15, 0));
        slots[1].EndTime.Should().Be(new TimeSpan(11, 15, 0));
    }

    [Fact]
    public void GenerateSlotsForDateRange_MultipleDays_ReturnsAllSlots()
    {
        var start = new DateTime(2026, 1, 15);
        var end = new DateTime(2026, 1, 17);
        var startTime = new TimeOnly(9, 0);
        var endTime = new TimeOnly(11, 0);
        var duration = TimeSpan.FromMinutes(60);

        var slots = _generator.GenerateSlotsForDateRange(start, end, startTime, endTime, duration);

        slots.Should().HaveCount(6);
        slots.Count(s => s.Date.Date == start.Date).Should().Be(2);
        slots.Count(s => s.Date.Date == end.Date).Should().Be(2);
    }

    [Fact]
    public void GenerateSlotsExcluding_RemovesExistingSlots()
    {
        var date = new DateTime(2026, 1, 15);
        var start = new TimeOnly(9, 0);
        var end = new TimeOnly(12, 0);
        var duration = TimeSpan.FromMinutes(60);
        var existing = new List<TimeSlot>
        {
            TimeSlot.Create(date, TimeSpan.FromHours(10), TimeSpan.FromHours(11))
        };

        var slots = _generator.GenerateSlotsExcluding(date, start, end, duration, existing);

        slots.Should().HaveCount(2);
        slots.Should().NotContain(s => s.StartTime == TimeSpan.FromHours(10));
    }

    [Fact]
    public void MergeSlots_OverlappingSlots_Merges()
    {
        var date = new DateTime(2026, 1, 15);
        var slots = new List<TimeSlot>
        {
            TimeSlot.Create(date, TimeSpan.FromHours(9), TimeSpan.FromHours(10)),
            TimeSlot.Create(date, TimeSpan.FromHours(10), TimeSpan.FromHours(11)),
            TimeSlot.Create(date, TimeSpan.FromHours(13), TimeSpan.FromHours(14))
        };

        var merged = _generator.MergeSlots(slots);

        merged.Should().HaveCount(2);
        merged[0].StartTime.Should().Be(TimeSpan.FromHours(9));
        merged[0].EndTime.Should().Be(TimeSpan.FromHours(11));
    }

    [Fact]
    public void SubtractSlots_RemovesBlockedTime()
    {
        var date = new DateTime(2026, 1, 15);
        var available = new List<TimeSlot>
        {
            TimeSlot.Create(date, TimeSpan.FromHours(9), TimeSpan.FromHours(12))
        };
        var blocked = new List<TimeSlot>
        {
            TimeSlot.Create(date, TimeSpan.FromHours(10), TimeSpan.FromHours(11))
        };

        var result = _generator.SubtractSlots(available, blocked);

        result.Should().HaveCount(2);
        result[0].StartTime.Should().Be(TimeSpan.FromHours(9));
        result[0].EndTime.Should().Be(TimeSpan.FromHours(10));
        result[1].StartTime.Should().Be(TimeSpan.FromHours(11));
        result[1].EndTime.Should().Be(TimeSpan.FromHours(12));
    }

    [Fact]
    public void SubtractSlots_NoOverlap_ReturnsAllAvailable()
    {
        var date = new DateTime(2026, 1, 15);
        var available = new List<TimeSlot>
        {
            TimeSlot.Create(date, TimeSpan.FromHours(9), TimeSpan.FromHours(10))
        };
        var blocked = new List<TimeSlot>
        {
            TimeSlot.Create(date, TimeSpan.FromHours(14), TimeSpan.FromHours(15))
        };

        var result = _generator.SubtractSlots(available, blocked);

        result.Should().HaveCount(1);
    }
}
