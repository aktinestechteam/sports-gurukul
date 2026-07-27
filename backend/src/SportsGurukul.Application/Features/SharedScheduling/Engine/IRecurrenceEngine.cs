using SportsGurukul.Application.Features.SharedScheduling.Models;

namespace SportsGurukul.Application.Features.SharedScheduling.Engine;

public interface IRecurrenceEngine
{
    IReadOnlyList<DateTime> GenerateOccurrences(RecurrencePattern pattern, DateTime startDate);
    IReadOnlyList<DateTime> FilterOccurrences(IReadOnlyList<DateTime> occurrences, SchedulingContext context);
    IReadOnlyList<DateTime> ParseRRule(string rRule, DateTime startDate, int maxOccurrences = 365);
    RecurrencePattern? TryParseRRule(string rRule);
    string ToRRule(RecurrencePattern pattern);
}
