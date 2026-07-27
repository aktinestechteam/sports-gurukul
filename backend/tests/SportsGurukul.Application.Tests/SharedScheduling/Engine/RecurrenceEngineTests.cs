using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Features.SharedScheduling.Engine;
using SportsGurukul.Application.Features.SharedScheduling.Models;

namespace SportsGurukul.Application.Tests.SharedScheduling.Engine;

public class RecurrenceEngineTests
{
    private readonly Mock<ILogger<RecurrenceEngine>> _loggerMock = new();
    private readonly RecurrenceEngine _engine;

    public RecurrenceEngineTests()
    {
        _engine = new RecurrenceEngine(_loggerMock.Object);
    }

    [Fact]
    public void GenerateOccurrences_Daily_ReturnsCorrectDates()
    {
        var pattern = new RecurrencePattern
        {
            Frequency = RecurrenceFrequency.Daily,
            Interval = 1,
            MaxOccurrences = 5
        };
        var startDate = new DateTime(2026, 1, 1);

        var dates = _engine.GenerateOccurrences(pattern, startDate);

        dates.Should().HaveCount(5);
        dates[0].Should().Be(new DateTime(2026, 1, 1));
        dates[4].Should().Be(new DateTime(2026, 1, 5));
    }

    [Fact]
    public void GenerateOccurrences_Weekly_ReturnsCorrectDates()
    {
        var pattern = new RecurrencePattern
        {
            Frequency = RecurrenceFrequency.Weekly,
            Interval = 1,
            MaxOccurrences = 4
        };
        var startDate = new DateTime(2026, 1, 5);

        var dates = _engine.GenerateOccurrences(pattern, startDate);

        dates.Should().HaveCount(4);
        dates[1].Should().Be(new DateTime(2026, 1, 12));
    }

    [Fact]
    public void GenerateOccurrences_WeeklyWithDaysOfWeek_FiltersDays()
    {
        var pattern = new RecurrencePattern
        {
            Frequency = RecurrenceFrequency.Weekly,
            Interval = 1,
            DaysOfWeek = [DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday],
            MaxOccurrences = 6
        };
        var startDate = new DateTime(2026, 1, 5);

        var dates = _engine.GenerateOccurrences(pattern, startDate);

        dates.Should().HaveCount(6);
        dates.All(d => d.DayOfWeek is DayOfWeek.Monday or DayOfWeek.Wednesday or DayOfWeek.Friday).Should().BeTrue();
    }

    [Fact]
    public void GenerateOccurrences_Monthly_ReturnsCorrectDates()
    {
        var pattern = new RecurrencePattern
        {
            Frequency = RecurrenceFrequency.Monthly,
            Interval = 1,
            MaxOccurrences = 3
        };
        var startDate = new DateTime(2026, 1, 15);

        var dates = _engine.GenerateOccurrences(pattern, startDate);

        dates.Should().HaveCount(3);
        dates[0].Should().Be(new DateTime(2026, 1, 15));
        dates[1].Should().Be(new DateTime(2026, 2, 15));
    }

    [Fact]
    public void GenerateOccurrences_WithExceptionDates_SkipsExceptions()
    {
        var pattern = new RecurrencePattern
        {
            Frequency = RecurrenceFrequency.Daily,
            Interval = 1,
            MaxOccurrences = 5,
            ExceptionDates = [new DateTime(2026, 1, 3)]
        };
        var startDate = new DateTime(2026, 1, 1);

        var dates = _engine.GenerateOccurrences(pattern, startDate);

        dates.Should().HaveCount(4);
        dates.Should().NotContain(d => d.Date == new DateTime(2026, 1, 3));
    }

    [Fact]
    public void GenerateOccurrences_EndDate_StopsAtEnd()
    {
        var pattern = new RecurrencePattern
        {
            Frequency = RecurrenceFrequency.Daily,
            Interval = 1,
            EndDate = new DateTime(2026, 1, 3)
        };
        var startDate = new DateTime(2026, 1, 1);

        var dates = _engine.GenerateOccurrences(pattern, startDate);

        dates.Should().HaveCount(3);
    }

    [Fact]
    public void FilterOccurrences_RemovesHolidays()
    {
        var occurrences = new List<DateTime>
        {
            new(2026, 1, 1),
            new(2026, 1, 2),
            new(2026, 1, 3)
        };
        var context = new SchedulingContext
        {
            Holidays =
            [
                new Holiday { Date = new DateTime(2026, 1, 2), Name = "Holiday", IsRecurring = true }
            ]
        };

        var filtered = _engine.FilterOccurrences(occurrences, context);

        filtered.Should().HaveCount(2);
        filtered.Should().NotContain(d => d.Date == new DateTime(2026, 1, 2));
    }

    [Fact]
    public void ToRRule_DailyPattern_GeneratesCorrectRRule()
    {
        var pattern = new RecurrencePattern
        {
            Frequency = RecurrenceFrequency.Daily,
            Interval = 1,
            MaxOccurrences = 10
        };

        var rrule = _engine.ToRRule(pattern);

        rrule.Should().Contain("FREQ=DAILY");
        rrule.Should().Contain("COUNT=10");
    }

    [Fact]
    public void ToRRule_WeeklyPatternWithDays_GeneratesCorrectRRule()
    {
        var pattern = new RecurrencePattern
        {
            Frequency = RecurrenceFrequency.Weekly,
            Interval = 2,
            DaysOfWeek = [DayOfWeek.Monday, DayOfWeek.Friday]
        };

        var rrule = _engine.ToRRule(pattern);

        rrule.Should().Contain("FREQ=WEEKLY");
        rrule.Should().Contain("INTERVAL=2");
        rrule.Should().Contain("BYDAY=MO,FR");
    }

    [Fact]
    public void TryParseRRule_ValidRRule_ParsesCorrectly()
    {
        var rrule = "FREQ=WEEKLY;INTERVAL=2;BYDAY=MO,WE,FR;COUNT=10";

        var pattern = _engine.TryParseRRule(rrule);

        pattern.Should().NotBeNull();
        pattern!.Frequency.Should().Be(RecurrenceFrequency.Weekly);
        pattern.Interval.Should().Be(2);
        pattern.DaysOfWeek.Should().Contain(new[] { DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday });
    }

    [Fact]
    public void ParseRRule_ValidRRule_GeneratesDates()
    {
        var rrule = "FREQ=DAILY;INTERVAL=1;COUNT=5";
        var startDate = new DateTime(2026, 1, 1);

        var dates = _engine.ParseRRule(rrule, startDate);

        dates.Should().HaveCount(5);
    }
}
