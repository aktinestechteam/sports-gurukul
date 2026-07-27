using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Services;

public interface IRecurrenceService
{
    IReadOnlyList<DateTime> GenerateOccurrences(
        RecurrenceType recurrenceType,
        DateTime startDate,
        TimeSpan startTime,
        TimeSpan endTime,
        int? occurrenceCount,
        DateTime? endDate,
        string? rRule = null,
        string? exceptions = null);
}
