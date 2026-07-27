using System.Text;
using FluentAssertions;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Calendar.Abstractions;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Calendar.Ics;

namespace SportsGurukul.Application.Tests.Calendar;

public class IcsImporterTests
{
    private readonly IcsImporter _importer = new();

    [Fact]
    public async Task ImportAsync_ValidIcs_ParsesEvents()
    {
        var ics = """
            BEGIN:VCALENDAR
            VERSION:2.0
            PRODID:-//Test//Test//EN
            BEGIN:VEVENT
            UID:test-123
            DTSTART:20250615T090000Z
            DTEND:20250615T100000Z
            SUMMARY:Morning Training
            DESCRIPTION:A test event
            LOCATION:Court A
            STATUS:CONFIRMED
            END:VEVENT
            END:VCALENDAR
            """;

        var data = Encoding.UTF8.GetBytes(ics);

        var result = await _importer.ImportAsync(data);

        result.Should().HaveCount(1);
        result[0].Uid.Should().Be("test-123");
        result[0].Summary.Should().Be("Morning Training");
        result[0].Description.Should().Be("A test event");
        result[0].Location.Should().Be("Court A");
        result[0].Status.Should().Be("CONFIRMED");
    }

    [Fact]
    public async Task ImportAsync_MultipleEvents_ParsesAll()
    {
        var ics = """
            BEGIN:VCALENDAR
            VERSION:2.0
            BEGIN:VEVENT
            UID:uid-1
            DTSTART:20250615T090000Z
            DTEND:20250615T100000Z
            SUMMARY:Event 1
            END:VEVENT
            BEGIN:VEVENT
            UID:uid-2
            DTSTART:20250615T110000Z
            DTEND:20250615T120000Z
            SUMMARY:Event 2
            END:VEVENT
            END:VCALENDAR
            """;

        var data = Encoding.UTF8.GetBytes(ics);

        var result = await _importer.ImportAsync(data);

        result.Should().HaveCount(2);
        result[0].Summary.Should().Be("Event 1");
        result[1].Summary.Should().Be("Event 2");
    }

    [Fact]
    public async Task ImportAsync_FilterAfter_ExcludesOlderEvents()
    {
        var ics = """
            BEGIN:VCALENDAR
            VERSION:2.0
            BEGIN:VEVENT
            UID:old
            DTSTART:20250101T090000Z
            DTEND:20250101T100000Z
            SUMMARY:Old Event
            END:VEVENT
            BEGIN:VEVENT
            UID:new
            DTSTART:20250615T090000Z
            DTEND:20250615T100000Z
            SUMMARY:New Event
            END:VEVENT
            END:VCALENDAR
            """;

        var data = Encoding.UTF8.GetBytes(ics);

        var result = await _importer.ImportAsync(data, new CalendarImportOptions
        {
            FilterAfter = new DateTime(2025, 3, 1, 0, 0, 0, DateTimeKind.Utc)
        });

        result.Should().HaveCount(1);
        result[0].Summary.Should().Be("New Event");
    }

    [Fact]
    public async Task ImportAsync_MaxLimitsEvents()
    {
        var sb = new StringBuilder();
        sb.AppendLine("BEGIN:VCALENDAR");
        for (int i = 0; i < 10; i++)
        {
            sb.AppendLine("BEGIN:VEVENT");
            sb.AppendLine($"UID:uid-{i}");
            sb.AppendLine($"DTSTART:2025061{i}T090000Z");
            sb.AppendLine($"DTEND:2025061{i}T100000Z");
            sb.AppendLine($"SUMMARY:Event {i}");
            sb.AppendLine("END:VEVENT");
        }
        sb.AppendLine("END:VCALENDAR");

        var data = Encoding.UTF8.GetBytes(sb.ToString());

        var result = await _importer.ImportAsync(data, new CalendarImportOptions
        {
            MaxEvents = 3
        });

        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task ImportAsync_EmptyCalendar_ReturnsEmpty()
    {
        var ics = """
            BEGIN:VCALENDAR
            VERSION:2.0
            END:VCALENDAR
            """;

        var data = Encoding.UTF8.GetBytes(ics);

        var result = await _importer.ImportAsync(data);

        result.Should().BeEmpty();
    }

    [Fact]
    public void Format_ReturnsICS()
    {
        _importer.Format.Should().Be("ICS");
    }

    [Fact]
    public async Task ImportAsync_ContinuationLines_ParsesCorrectly()
    {
        var ics = """
            BEGIN:VCALENDAR
            VERSION:2.0
            BEGIN:VEVENT
            UID:test
            DTSTART:20250615T090000Z
            DTEND:20250615T100000Z
            SUMMARY:Very Long Event Name That
             Continues On Next Line
            END:VEVENT
            END:VCALENDAR
            """;

        var data = Encoding.UTF8.GetBytes(ics);

        var result = await _importer.ImportAsync(data);

        result.Should().HaveCount(1);
        result[0].Summary.Should().Contain("Very Long Event Name That");
        result[0].Summary.Should().Contain("Continues On Next Line");
    }
}
