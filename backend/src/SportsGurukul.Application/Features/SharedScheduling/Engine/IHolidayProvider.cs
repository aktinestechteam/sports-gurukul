using SportsGurukul.Application.Features.SharedScheduling.Models;

namespace SportsGurukul.Application.Features.SharedScheduling.Engine;

public interface IHolidayProvider
{
    Task<IReadOnlyList<Holiday>> GetHolidaysAsync(Guid? academyId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<bool> IsHolidayAsync(DateTime date, Guid? academyId = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Holiday>> GetHolidaysInRangeAsync(DateTime startDate, DateTime endDate, Guid? academyId = null, CancellationToken cancellationToken = default);
}
