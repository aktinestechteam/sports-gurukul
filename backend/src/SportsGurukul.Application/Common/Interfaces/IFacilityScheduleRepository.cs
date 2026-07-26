using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Common.Interfaces;

public interface IFacilityScheduleRepository : IRepository<FacilitySchedule>
{
    Task<IReadOnlyList<FacilitySchedule>> GetByFacilityIdAsync(Guid facilityId, CancellationToken cancellationToken = default);
}
