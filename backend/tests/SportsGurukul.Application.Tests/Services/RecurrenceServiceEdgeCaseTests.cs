using FluentAssertions;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Services;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Tests.Services;

public class RecurrenceServiceEdgeCaseTests
{
    private readonly RecurrenceService _service = new();

    [Fact]
    public void GenerateOccurrences_CustomRRule_DailyInterval2_ReturnsEvery2Days()
    {
        var startDate = new DateTime(2026, 1, 1);
        var rRule = "FREQ=DAILY;INTERVAL=2";

        var result = _service.GenerateOccurrences(
            RecurrenceType.Custom, startDate, TimeSpan.FromHours(9), TimeSpan.FromHours(10),
            occurrenceCount: 5, endDate: null, rRule: rRule);

        result.Should().HaveCount(5);
        result[0].Should().Be(new DateTime(2026, 1, 1));
        result[1].Should().Be(new DateTime(2026, 1, 3));
        result[2].Should().Be(new DateTime(2026, 1, 5));
        result[3].Should().Be(new DateTime(2026, 1, 7));
        result[4].Should().Be(new DateTime(2026, 1, 9));
    }

    [Fact]
    public void GenerateOccurrences_CustomRRule_DailyInterval5_ReturnsEvery5Days()
    {
        var startDate = new DateTime(2026, 1, 1);
        var rRule = "FREQ=DAILY;INTERVAL=5";

        var result = _service.GenerateOccurrences(
            RecurrenceType.Custom, startDate, TimeSpan.FromHours(9), TimeSpan.FromHours(10),
            occurrenceCount: 4, endDate: null, rRule: rRule);

        result.Should().HaveCount(4);
        result[0].Should().Be(new DateTime(2026, 1, 1));
        result[1].Should().Be(new DateTime(2026, 1, 6));
        result[2].Should().Be(new DateTime(2026, 1, 11));
        result[3].Should().Be(new DateTime(2026, 1, 16));
    }

    [Fact]
    public void GenerateOccurrences_CustomRRule_WeeklyByDayMOFR_ReturnsCorrectDays()
    {
        var startDate = new DateTime(2026, 1, 5);
        var rRule = "FREQ=WEEKLY;BYDAY=MO,FR";

        var result = _service.GenerateOccurrences(
            RecurrenceType.Custom, startDate, TimeSpan.FromHours(9), TimeSpan.FromHours(10),
            occurrenceCount: 4, endDate: null, rRule: rRule);

        result.Should().HaveCount(4);
        result.All(d => d.DayOfWeek == DayOfWeek.Monday || d.DayOfWeek == DayOfWeek.Friday)
            .Should().BeTrue();
    }

    [Fact]
    public void GenerateOccurrences_CustomRRule_EmptyRRule_FallsBackToDaily()
    {
        var startDate = new DateTime(2026, 1, 1);

        var result = _service.GenerateOccurrences(
            RecurrenceType.Custom, startDate, TimeSpan.FromHours(9), TimeSpan.FromHours(10),
            occurrenceCount: 3, endDate: null, rRule: null);

        result.Should().HaveCount(3);
        result[0].Should().Be(new DateTime(2026, 1, 1));
        result[1].Should().Be(new DateTime(2026, 1, 2));
        result[2].Should().Be(new DateTime(2026, 1, 3));
    }

    [Fact]
    public void GenerateOccurrences_CustomRRule_Unrecognized_FallsBackToWeekly()
    {
        var startDate = new DateTime(2026, 1, 1);

        var result = _service.GenerateOccurrences(
            RecurrenceType.Custom, startDate, TimeSpan.FromHours(9), TimeSpan.FromHours(10),
            occurrenceCount: 3, endDate: null, rRule: "FREQ=HOURLY;INTERVAL=1");

        result.Should().HaveCount(3);
        result[0].Should().Be(new DateTime(2026, 1, 1));
        result[1].Should().Be(new DateTime(2026, 1, 8));
        result[2].Should().Be(new DateTime(2026, 1, 15));
    }

    [Fact]
    public void GenerateOccurrences_Monthly_LeapYear_HandlesFebCorrectly()
    {
        var startDate = new DateTime(2028, 1, 31);

        var result = _service.GenerateOccurrences(
            RecurrenceType.Monthly, startDate, TimeSpan.FromHours(9), TimeSpan.FromHours(10),
            occurrenceCount: 3, endDate: null);

        result.Should().HaveCount(3);
        result[0].Should().Be(new DateTime(2028, 1, 31));
        result[1].Should().Be(new DateTime(2028, 2, 29));
        result[2].Should().Be(new DateTime(2028, 3, 29));
    }

    [Fact]
    public void GenerateOccurrences_Daily_WithMultipleExceptions_ExcludesAll()
    {
        var startDate = new DateTime(2026, 1, 1);
        var exceptions = "2026-01-02,2026-01-04,2026-01-06";

        var result = _service.GenerateOccurrences(
            RecurrenceType.Daily, startDate, TimeSpan.FromHours(9), TimeSpan.FromHours(10),
            occurrenceCount: 10, endDate: null, exceptions: exceptions);

        result.Should().HaveCount(7);
        result.Should().NotContain(d => d.Day == 2 || d.Day == 4 || d.Day == 6);
    }

    [Fact]
    public void GenerateOccurrences_Daily_EmptyExceptionsString_Ignores()
    {
        var startDate = new DateTime(2026, 1, 1);

        var result = _service.GenerateOccurrences(
            RecurrenceType.Daily, startDate, TimeSpan.FromHours(9), TimeSpan.FromHours(10),
            occurrenceCount: 5, endDate: null, exceptions: "");

        result.Should().HaveCount(5);
    }

    [Fact]
    public void GenerateOccurrences_Daily_InvalidExceptionsString_IgnoresInvalid()
    {
        var startDate = new DateTime(2026, 1, 1);
        var exceptions = "not-a-date,2026-01-03,invalid";

        var result = _service.GenerateOccurrences(
            RecurrenceType.Daily, startDate, TimeSpan.FromHours(9), TimeSpan.FromHours(10),
            occurrenceCount: 5, endDate: null, exceptions: exceptions);

        result.Should().HaveCount(4);
        result.Should().NotContain(d => d.Day == 3);
    }

    [Fact]
    public void GenerateOccurrences_EndDateAndOccurrenceCount_UsesWhicheverLimitIsSmaller()
    {
        var startDate = new DateTime(2026, 1, 1);
        var endDate = new DateTime(2026, 1, 3);

        var result = _service.GenerateOccurrences(
            RecurrenceType.Daily, startDate, TimeSpan.FromHours(9), TimeSpan.FromHours(10),
            occurrenceCount: 100, endDate: endDate);

        result.Should().HaveCount(3);
    }

    [Fact]
    public void GenerateOccurrences_OccurrenceCount1_ReturnsOnlyStartDate()
    {
        var startDate = new DateTime(2026, 6, 15);

        var result = _service.GenerateOccurrences(
            RecurrenceType.Weekly, startDate, TimeSpan.FromHours(9), TimeSpan.FromHours(10),
            occurrenceCount: 1, endDate: null);

        result.Should().HaveCount(1);
        result[0].Should().Be(startDate);
    }

    [Fact]
    public void GenerateOccurrences_Weekly_FridayToMonday_CrossesWeekend()
    {
        var startDate = new DateTime(2026, 1, 9);

        var result = _service.GenerateOccurrences(
            RecurrenceType.Weekly, startDate, TimeSpan.FromHours(9), TimeSpan.FromHours(10),
            occurrenceCount: 4, endDate: null);

        result.Should().HaveCount(4);
        result[0].Should().Be(new DateTime(2026, 1, 9));
        result[1].Should().Be(new DateTime(2026, 1, 16));
        result[2].Should().Be(new DateTime(2026, 1, 23));
        result[3].Should().Be(new DateTime(2026, 1, 30));
    }

    [Fact]
    public void GenerateOccurrences_CustomRRule_WeeklyByDaySA_PicksSaturday()
    {
        var startDate = new DateTime(2026, 1, 10); // Saturday
        var rRule = "FREQ=WEEKLY;BYDAY=SA";

        var result = _service.GenerateOccurrences(
            RecurrenceType.Custom, startDate, TimeSpan.FromHours(9), TimeSpan.FromHours(10),
            occurrenceCount: 3, endDate: null, rRule: rRule);

        result.Should().HaveCount(3);
        result.All(d => d.DayOfWeek == DayOfWeek.Saturday).Should().BeTrue();
    }

    [Theory]
    [InlineData(RecurrenceType.Daily)]
    [InlineData(RecurrenceType.Weekly)]
    [InlineData(RecurrenceType.Monthly)]
    public void GenerateOccurrences_StartDateAlwaysFirst(RecurrenceType type)
    {
        var startDate = new DateTime(2026, 3, 15);

        var result = _service.GenerateOccurrences(
            type, startDate, TimeSpan.FromHours(9), TimeSpan.FromHours(10),
            occurrenceCount: 5, endDate: null);

        result.First().Should().Be(startDate);
    }

    [Fact]
    public void GenerateOccurrences_UnknownType_FallsBackToDaily()
    {
        var startDate = new DateTime(2026, 1, 1);

        var result = _service.GenerateOccurrences(
            (RecurrenceType)99, startDate, TimeSpan.FromHours(9), TimeSpan.FromHours(10),
            occurrenceCount: 3, endDate: null);

        result.Should().HaveCount(3);
        result[1].Should().Be(new DateTime(2026, 1, 2));
    }
}
