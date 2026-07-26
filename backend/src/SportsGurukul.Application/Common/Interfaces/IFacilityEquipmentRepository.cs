using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Common.Interfaces;

public interface IFacilityEquipmentRepository : IRepository<FacilityEquipment>
{
    Task<IReadOnlyList<FacilityEquipment>> GetByFacilityIdAsync(Guid facilityId, CancellationToken cancellationToken = default);
    Task<FacilityEquipment?> GetWithMaintenanceAsync(Guid equipmentId, CancellationToken cancellationToken = default);
}
