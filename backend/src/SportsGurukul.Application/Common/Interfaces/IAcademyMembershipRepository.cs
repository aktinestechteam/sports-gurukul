using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Common.Interfaces;

public interface IAcademyMembershipRepository : IRepository<AcademyMembership>
{
    Task<IReadOnlyList<AcademyMembership>> GetByAcademyIdAsync(Guid academyId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AcademyMembership>> GetActiveByAcademyIdAsync(Guid academyId, CancellationToken cancellationToken = default);
}
