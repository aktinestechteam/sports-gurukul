using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Common.Interfaces;

public interface ITournamentRepository : IRepository<Tournament>
{
    Task<Tournament?> GetWithDetailsAsync(Guid tournamentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Tournament>> GetByAcademyIdAsync(Guid academyId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Tournament>> GetBySportIdAsync(Guid sportId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Tournament>> SearchAsync(
        Guid? academyId,
        TournamentStatus? status,
        TournamentType? type,
        string? searchTerm,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task<int> CountSearchAsync(
        Guid? academyId,
        TournamentStatus? status,
        TournamentType? type,
        string? searchTerm,
        CancellationToken cancellationToken = default);
    Task<bool> IsTournamentCodeUniqueAsync(string tournamentCode, CancellationToken cancellationToken = default);
}
