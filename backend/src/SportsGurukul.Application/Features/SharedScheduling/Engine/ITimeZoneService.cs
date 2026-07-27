using SportsGurukul.Application.Features.SharedScheduling.Models;

namespace SportsGurukul.Application.Features.SharedScheduling.Engine;

public interface ITimeZoneService
{
    DateTime ToUtc(DateTime localTime, string timeZoneId);
    DateTime ToLocal(DateTime utcTime, string timeZoneId);
    TimeSlot ToUtc(TimeSlot localSlot, string timeZoneId);
    TimeSlot ToLocal(TimeSlot utcSlot, string timeZoneId);
    IReadOnlyList<TimeSlot> AdjustForTimeZone(IReadOnlyList<TimeSlot> slots, string fromTimeZone, string toTimeZone);
    string GetDefaultTimeZone(Guid? academyId = null);
}
