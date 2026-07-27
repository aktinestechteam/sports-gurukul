using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Services;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace SportsGurukul.Application.Tests.Services;

public class RecurrenceServiceTests
{
    private readonly RecurrenceService _service = new();

    [Fact]
    public void GenerateOccurrences_Daily_ReturnsCorrectDates()
    {
        var startDate = new DateTime(2026, 1, 1);
        var startTime = TimeSpan.FromHours(9);
        var endTime = TimeSpan.FromHours(10);

        var result = _service.GenerateOccurrences(
            RecurrenceType.Daily, startDate, startTime, endTime,
            occurrenceCount: 5, endDate: null);

        result.Should().HaveCount(5);
        result[0].Should().Be(new DateTime(2026, 1, 1));
        result[1].Should().Be(new DateTime(2026, 1, 2));
        result[2].Should().Be(new DateTime(2026, 1, 3));
        result[3].Should().Be(new DateTime(2026, 1, 4));
        result[4].Should().Be(new DateTime(2026, 1, 5));
    }

    [Fact]
    public void GenerateOccurrences_Weekly_ReturnsCorrectDates()
    {
        var startDate = new DateTime(2026, 1, 5);
        var startTime = TimeSpan.FromHours(9);
        var endTime = TimeSpan.FromHours(10);

        var result = _service.GenerateOccurrences(
            RecurrenceType.Weekly, startDate, startTime, endTime,
            occurrenceCount: 4, endDate: null);

        result.Should().HaveCount(4);
        result[0].Should().Be(new DateTime(2026, 1, 5));
        result[1].Should().Be(new DateTime(2026, 1, 12));
        result[2].Should().Be(new DateTime(2026, 1, 19));
        result[3].Should().Be(new DateTime(2026, 1, 26));
    }

    [Fact]
    public void GenerateOccurrences_Monthly_ReturnsCorrectDates()
    {
        var startDate = new DateTime(2026, 1, 15);
        var startTime = TimeSpan.FromHours(9);
        var endTime = TimeSpan.FromHours(10);

        var result = _service.GenerateOccurrences(
            RecurrenceType.Monthly, startDate, startTime, endTime,
            occurrenceCount: 3, endDate: null);

        result.Should().HaveCount(3);
        result[0].Should().Be(new DateTime(2026, 1, 15));
        result[1].Should().Be(new DateTime(2026, 2, 15));
        result[2].Should().Be(new DateTime(2026, 3, 15));
    }

    [Fact]
    public void GenerateOccurrences_WithEndDate_StopsAtEndDate()
    {
        var startDate = new DateTime(2026, 1, 1);
        var endDate = new DateTime(2026, 1, 4);
        var startTime = TimeSpan.FromHours(9);
        var endTime = TimeSpan.FromHours(10);

        var result = _service.GenerateOccurrences(
            RecurrenceType.Daily, startDate, startTime, endTime,
            occurrenceCount: 100, endDate: endDate);

        result.Should().HaveCount(4);
    }

    [Fact]
    public void GenerateOccurrences_WithExceptions_ExcludesExceptionDates()
    {
        var startDate = new DateTime(2026, 1, 1);
        var startTime = TimeSpan.FromHours(9);
        var endTime = TimeSpan.FromHours(10);
        var exceptions = "2026-01-03,2026-01-05";

        var result = _service.GenerateOccurrences(
            RecurrenceType.Daily, startDate, startTime, endTime,
            occurrenceCount: 7, endDate: null, exceptions: exceptions);

        result.Should().HaveCount(5);
        result.Should().NotContain(d => d.Day == 3);
        result.Should().NotContain(d => d.Day == 5);
    }
}
