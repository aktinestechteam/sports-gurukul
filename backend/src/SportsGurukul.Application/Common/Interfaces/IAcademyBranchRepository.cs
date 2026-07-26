using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Common.Interfaces;

public interface IAcademyBranchRepository : IRepository<AcademyBranch>
{
    Task<IReadOnlyList<AcademyBranch>> GetByAcademyIdAsync(Guid academyId, CancellationToken cancellationToken = default);
    Task<AcademyBranch?> GetByAcademyIdAndNameAsync(Guid academyId, string branchName, CancellationToken cancellationToken = default);
}
