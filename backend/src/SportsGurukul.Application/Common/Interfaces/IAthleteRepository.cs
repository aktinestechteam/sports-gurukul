using SportsGurukul.Application.Features.AthleteManagement.DTOs;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Common.Interfaces;

public interface IAthleteRepository : IRepository<Athlete>
{
    Task<Athlete?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Athlete?> GetByAthleteCodeAsync(string athleteCode, CancellationToken cancellationToken = default);
    Task<Athlete?> GetByUserIdWithDetailsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Athlete>> GetAllWithUserAsync(CancellationToken cancellationToken = default);
    Task<Athlete?> GetDeletedByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<AthleteSummaryDto> Athletes, int TotalCount)> SearchAthletesAsync(AthleteSearchRequest request, CancellationToken cancellationToken = default);
    Task<Athlete?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AthleteSport>> GetAthleteSportsAsync(Guid athleteId, CancellationToken cancellationToken = default);
}
