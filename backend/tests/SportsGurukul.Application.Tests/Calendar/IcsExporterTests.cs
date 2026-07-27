using System.Text;
using FluentAssertions;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Calendar.Abstractions;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Calendar.Ics;

namespace SportsGurukul.Application.Tests.Calendar;

public class IcsExporterTests
{
    private readonly IcsExporter _exporter = new();

    [Fact]
    public async Task ExportSingleAsync_ProducesValidVCALENDAR()
    {
        var calendarEvent = new CalendarEvent
        {
            Id = Guid.NewGuid(),
            Uid = "test-uid-123",
            Summary = "Morning Training",
            Description = "Regular morning session",
            Location = "Court A",
            StartDateTime = new DateTime(2025, 6, 15, 9, 0, 0, DateTimeKind.Utc),
            EndDateTime = new DateTime(2025, 6, 15, 10, 0, 0, DateTimeKind.Utc),
            Status = "CONFIRMED"
        };

        var result = await _exporter.ExportSingleAsync(calendarEvent);
        var content = Encoding.UTF8.GetString(result);

        content.Should().Contain("BEGIN:VCALENDAR");
        content.Should().Contain("END:VCALENDAR");
        content.Should().Contain("BEGIN:VEVENT");
        content.Should().Contain("END:VEVENT");
        content.Should().Contain("UID:test-uid-123");
        content.Should().Contain("SUMMARY:Morning Training");
        content.Should().Contain("DESCRIPTION:Regular morning session");
        content.Should().Contain("LOCATION:Court A");
        content.Should().Contain("STATUS:CONFIRMED");
    }

    [Fact]
    public async Task ExportAsync_MultipleEvents_AllIncluded()
    {
        var events = new List<CalendarEvent>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Uid = "uid-1",
                Summary = "Event 1",
                StartDateTime = new DateTime(2025, 6, 15, 9, 0, 0, DateTimeKind.Utc),
                EndDateTime = new DateTime(2025, 6, 15, 10, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.NewGuid(),
                Uid = "uid-2",
                Summary = "Event 2",
                StartDateTime = new DateTime(2025, 6, 15, 11, 0, 0, DateTimeKind.Utc),
                EndDateTime = new DateTime(2025, 6, 15, 12, 0, 0, DateTimeKind.Utc)
            }
        };

        var result = await _exporter.ExportAsync(events);
        var content = Encoding.UTF8.GetString(result);

        content.Should().Contain("UID:uid-1");
        content.Should().Contain("UID:uid-2");
        content.Should().Contain("SUMMARY:Event 1");
        content.Should().Contain("SUMMARY:Event 2");
    }

    [Fact]
    public async Task ExportAsync_WithReminder_IncludesVALARM()
    {
        var events = new List<CalendarEvent>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Uid = "uid-1",
                Summary = "Event",
                StartDateTime = new DateTime(2025, 6, 15, 9, 0, 0, DateTimeKind.Utc),
                EndDateTime = new DateTime(2025, 6, 15, 10, 0, 0, DateTimeKind.Utc)
            }
        };

        var result = await _exporter.ExportAsync(events, new CalendarExportOptions
        {
            ReminderMinutesBefore = 30
        });
        var content = Encoding.UTF8.GetString(result);

        content.Should().Contain("BEGIN:VALARM");
        content.Should().Contain("END:VALARM");
        content.Should().Contain("TRIGGER:-PT30M");
    }

    [Fact]
    public async Task ExportAsync_EscapesSpecialCharacters()
    {
        var events = new List<CalendarEvent>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Uid = "uid-1",
                Summary = "Event; with, special\nchars",
                StartDateTime = new DateTime(2025, 6, 15, 9, 0, 0, DateTimeKind.Utc),
                EndDateTime = new DateTime(2025, 6, 15, 10, 0, 0, DateTimeKind.Utc)
            }
        };

        var result = await _exporter.ExportAsync(events);
        var content = Encoding.UTF8.GetString(result);

        content.Should().Contain("SUMMARY:Event\\; with\\, special\\nchars");
    }

    [Fact]
    public async Task ExportAsync_AllDayEvent_UsesDateValue()
    {
        var events = new List<CalendarEvent>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Uid = "uid-1",
                Summary = "All Day Event",
                IsAllDay = true,
                StartDateTime = new DateTime(2025, 6, 15),
                EndDateTime = new DateTime(2025, 6, 15)
            }
        };

        var result = await _exporter.ExportAsync(events);
        var content = Encoding.UTF8.GetString(result);

        content.Should().Contain("DTSTART;VALUE=DATE:20250615");
    }

    [Fact]
    public void Format_ReturnsICS()
    {
        _exporter.Format.Should().Be("ICS");
    }
}
