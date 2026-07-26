using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Common.Interfaces;

public interface IAcademyRepository : IRepository<Academy>
{
    Task<Academy?> GetByAcademyCodeAsync(string academyCode, CancellationToken cancellationToken = default);
    Task<Academy?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Academy?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Academy?> GetByAcademyCodeWithDetailsAsync(string academyCode, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AcademyBranch>> GetBranchesAsync(Guid academyId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AcademySport>> GetAcademySportsAsync(Guid academyId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AcademyFacility>> GetFacilitiesAsync(Guid academyId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AcademyMembership>> GetMembershipsAsync(Guid academyId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AcademyDocument>> GetDocumentsAsync(Guid academyId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AcademyGallery>> GetGalleryImagesAsync(Guid academyId, CancellationToken cancellationToken = default);
}
