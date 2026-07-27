using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Features.SharedScheduling.Models;

namespace SportsGurukul.Application.Features.SharedScheduling.Engine;

public class DefaultHolidayProvider : IHolidayProvider
{
    private readonly ILogger<DefaultHolidayProvider> _logger;

    public DefaultHolidayProvider(ILogger<DefaultHolidayProvider> logger) => _logger = logger;

    public Task<IReadOnlyList<Holiday>> GetHolidaysAsync(Guid? academyId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        var holidays = GetDefaultHolidays(startDate.Year)
            .Concat(startDate.Year != endDate.Year ? GetDefaultHolidays(endDate.Year) : [])
            .Where(h => h.Date.Date >= startDate.Date && h.Date.Date <= endDate.Date)
            .ToList();

        _logger.LogDebug("Found {Count} holidays between {Start} and {End}", holidays.Count, startDate.Date, endDate.Date);
        return Task.FromResult<IReadOnlyList<Holiday>>(holidays);
    }

    public async Task<bool> IsHolidayAsync(DateTime date, Guid? academyId = null, CancellationToken cancellationToken = default)
    {
        var holidays = await GetHolidaysAsync(academyId, date.AddDays(-1), date.AddDays(1), cancellationToken);
        return holidays.Any(h => h.Date.Date == date.Date);
    }

    public Task<IReadOnlyList<Holiday>> GetHolidaysInRangeAsync(DateTime startDate, DateTime endDate, Guid? academyId = null, CancellationToken cancellationToken = default)
    {
        return GetHolidaysAsync(academyId, startDate, endDate, cancellationToken);
    }

    private static List<Holiday> GetDefaultHolidays(int year)
    {
        return
        [
            new Holiday { Date = new DateTime(year, 1, 1), Name = "New Year's Day", IsRecurring = true },
            new Holiday { Date = new DateTime(year, 1, 26), Name = "Republic Day", IsRecurring = true },
            new Holiday { Date = new DateTime(year, 8, 15), Name = "Independence Day", IsRecurring = true },
            new Holiday { Date = new DateTime(year, 10, 2), Name = "Gandhi Jayanti", IsRecurring = true },
            new Holiday { Date = new DateTime(year, 11, 1), Name = "Diwali", IsRecurring = false },
            new Holiday { Date = new DateTime(year, 12, 25), Name = "Christmas", IsRecurring = true },
        ];
    }
}
