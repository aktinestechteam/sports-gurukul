using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Features.SharedScheduling.Models;

namespace SportsGurukul.Application.Features.SharedScheduling.Engine;

public class TimeZoneService : ITimeZoneService
{
    private readonly ILogger<TimeZoneService> _logger;

    public TimeZoneService(ILogger<TimeZoneService> logger) => _logger = logger;

    public DateTime ToUtc(DateTime localTime, string timeZoneId)
    {
        var tz = TimeZoneInfo.FindSystemTimeZoneById(ResolveTimeZoneId(timeZoneId));
        return TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(localTime, DateTimeKind.Unspecified), tz);
    }

    public DateTime ToLocal(DateTime utcTime, string timeZoneId)
    {
        var tz = TimeZoneInfo.FindSystemTimeZoneById(ResolveTimeZoneId(timeZoneId));
        return TimeZoneInfo.ConvertTimeFromUtc(utcTime, tz);
    }

    public TimeSlot ToUtc(TimeSlot localSlot, string timeZoneId)
    {
        var utcDate = ToUtc(localSlot.Date.Date + localSlot.StartTime, timeZoneId);
        var utcEnd = ToUtc(localSlot.Date.Date + localSlot.EndTime, timeZoneId);
        return new TimeSlot
        {
            Date = utcDate.Date,
            StartTime = utcDate.TimeOfDay,
            EndTime = utcEnd.TimeOfDay
        };
    }

    public TimeSlot ToLocal(TimeSlot utcSlot, string timeZoneId)
    {
        var localDate = ToLocal(utcSlot.Date.Date + utcSlot.StartTime, timeZoneId);
        var localEnd = ToLocal(utcSlot.Date.Date + utcSlot.EndTime, timeZoneId);
        return new TimeSlot
        {
            Date = localDate.Date,
            StartTime = localDate.TimeOfDay,
            EndTime = localEnd.TimeOfDay
        };
    }

    public IReadOnlyList<TimeSlot> AdjustForTimeZone(IReadOnlyList<TimeSlot> slots, string fromTimeZone, string toTimeZone)
    {
        return slots.Select(s =>
        {
            var utc = ToUtc(s, fromTimeZone);
            return ToLocal(utc, toTimeZone);
        }).ToList();
    }

    public string GetDefaultTimeZone(Guid? academyId = null) => "Asia/Kolkata";

    private static string ResolveTimeZoneId(string timeZoneId)
    {
        try
        {
            _ = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return timeZoneId;
        }
        catch (TimeZoneNotFoundException)
        {
            return timeZoneId switch
            {
                "IST" or "India" => "Asia/Kolkata",
                "EST" => "America/New_York",
                "PST" => "America/Los_Angeles",
                "GMT" or "UTC" => "UTC",
                _ => "UTC"
            };
        }
    }
}
