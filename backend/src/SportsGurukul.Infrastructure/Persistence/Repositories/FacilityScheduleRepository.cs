using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Repositories;

public class FacilityScheduleRepository : Repository<FacilitySchedule>, IFacilityScheduleRepository
{
    public FacilityScheduleRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<FacilitySchedule>> GetByFacilityIdAsync(
        Guid facilityId, CancellationToken cancellationToken = default)
    {
        return await Context.FacilitySchedules
            .AsNoTracking()
            .Where(s => s.FacilityId == facilityId && !s.IsDeleted)
            .OrderBy(s => s.DayOfWeek)
            .ToListAsync(cancellationToken);
    }
}
