using System.Diagnostics;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Features.SharedScheduling.Engine;
using SportsGurukul.Application.Features.SharedScheduling.Models;

namespace SportsGurukul.Application.Tests.Performance;

public class SchedulingEnginePerformanceTests
{
    private readonly Mock<ILogger<TimeSlotGenerator>> _slotLoggerMock = new();
    private readonly Mock<ILogger<RecurrenceEngine>> _recurrenceLoggerMock = new();
    private readonly TimeSlotGenerator _slotGenerator;
    private readonly RecurrenceEngine _recurrenceEngine;

    public SchedulingEnginePerformanceTests()
    {
        _slotGenerator = new TimeSlotGenerator(_slotLoggerMock.Object);
        _recurrenceEngine = new RecurrenceEngine(_recurrenceLoggerMock.Object);
    }

    [Fact]
    public void SlotGeneration_LargeDateRange_CompletesWithin100ms()
    {
        var startDate = new DateTime(2026, 1, 1);
        var endDate = new DateTime(2026, 12, 31);

        var sw = Stopwatch.StartNew();
        var slots = _slotGenerator.GenerateSlotsForDateRange(
            startDate, endDate, new TimeOnly(6, 0), new TimeOnly(22, 0), TimeSpan.FromMinutes(30));
        sw.Stop();

        slots.Should().NotBeEmpty();
        sw.ElapsedMilliseconds.Should().BeLessThan(100,
            $"Slot generation for a full year should complete within 100ms, took {sw.ElapsedMilliseconds}ms");
    }

    [Fact]
    public void SlotSubtraction_LargeDataset_CompletesWithin100ms()
    {
        var date = new DateTime(2026, 1, 15);
        var available = _slotGenerator.GenerateDailySlots(
            date, new TimeOnly(6, 0), new TimeOnly(22, 0), TimeSpan.FromMinutes(30));

        var blocked = new List<TimeSlot>();
        for (int i = 0; i < 5; i++)
        {
            var startHour = 6 + i * 4;
            blocked.Add(TimeSlot.Create(date, TimeSpan.FromHours(startHour), TimeSpan.FromHours(startHour + 1)));
        }

        var sw = Stopwatch.StartNew();
        var result = _slotGenerator.SubtractSlots(available, blocked);
        sw.Stop();

        result.Should().NotBeEmpty();
        sw.ElapsedMilliseconds.Should().BeLessThan(100,
            $"Slot subtraction should complete within 100ms, took {sw.ElapsedMilliseconds}ms");
    }

    [Fact]
    public void RecurrenceGeneration_LargePattern_CompletesWithin100ms()
    {
        var pattern = new RecurrencePattern
        {
            Frequency = RecurrenceFrequency.Daily,
            Interval = 1,
            MaxOccurrences = 365
        };

        var sw = Stopwatch.StartNew();
        var dates = _recurrenceEngine.GenerateOccurrences(pattern, new DateTime(2026, 1, 1));
        sw.Stop();

        dates.Should().HaveCount(365);
        sw.ElapsedMilliseconds.Should().BeLessThan(100,
            $"Recurrence generation for 365 daily occurrences should complete within 100ms, took {sw.ElapsedMilliseconds}ms");
    }

    [Fact]
    public void SlotMerge_LargeDataset_CompletesWithin100ms()
    {
        var date = new DateTime(2026, 1, 15);
        var slots = new List<TimeSlot>();
        for (int i = 0; i < 100; i++)
        {
            var start = TimeSpan.FromHours(6 + (i * 0.24));
            var end = start + TimeSpan.FromMinutes(30);
            slots.Add(TimeSlot.Create(date, start, end));
        }

        var sw = Stopwatch.StartNew();
        var merged = _slotGenerator.MergeSlots(slots);
        sw.Stop();

        merged.Should().NotBeEmpty();
        sw.ElapsedMilliseconds.Should().BeLessThan(100,
            $"Slot merge for 100 slots should complete within 100ms, took {sw.ElapsedMilliseconds}ms");
    }
}
