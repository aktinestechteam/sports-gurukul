using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Common.Interfaces;

public interface IFacilityRepository : IRepository<Facility>
{
    Task<IReadOnlyList<Facility>> GetByAcademyIdAsync(Guid academyId, CancellationToken cancellationToken = default);
    Task<Facility?> GetWithDetailsAsync(Guid facilityId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Facility>> SearchAsync(
        Guid? academyId,
        FacilityType? facilityType,
        string? searchTerm,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task<int> CountSearchAsync(
        Guid? academyId,
        FacilityType? facilityType,
        string? searchTerm,
        CancellationToken cancellationToken = default);
    Task<bool> IsFacilityNameUniqueInBranchAsync(
        Guid academyId, Guid? branchId, string facilityName, CancellationToken cancellationToken = default);
}
