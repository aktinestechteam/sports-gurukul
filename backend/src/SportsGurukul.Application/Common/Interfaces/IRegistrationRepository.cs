using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Common.Interfaces;

public interface IRegistrationRepository : IRepository<TournamentRegistration>
{
    Task<TournamentRegistration?> GetWithDetailsAsync(Guid registrationId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TournamentRegistration>> GetByTournamentIdAsync(Guid tournamentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TournamentRegistration>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TournamentRegistration>> GetByStatusAsync(Guid tournamentId, TournamentRegistrationStatus status, CancellationToken cancellationToken = default);
    Task<bool> IsAlreadyRegisteredAsync(Guid tournamentId, Guid? athleteId, Guid? teamId, CancellationToken cancellationToken = default);
    Task<int> GetRegistrationCountAsync(Guid tournamentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TournamentRegistration>> SearchAsync(
        Guid? tournamentId,
        TournamentRegistrationStatus? status,
        string? searchTerm,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task<int> CountSearchAsync(
        Guid? tournamentId,
        TournamentRegistrationStatus? status,
        string? searchTerm,
        CancellationToken cancellationToken = default);
}
