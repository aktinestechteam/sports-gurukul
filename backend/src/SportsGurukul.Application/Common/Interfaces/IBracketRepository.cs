using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Common.Interfaces;

public interface IBracketRepository : IRepository<TournamentBracket>
{
    Task<TournamentBracket?> GetWithDetailsAsync(Guid bracketId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TournamentBracket>> GetByTournamentIdAsync(Guid tournamentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TournamentBracket>> GetByDivisionIdAsync(Guid divisionId, CancellationToken cancellationToken = default);
    Task<bool> HasBracketForDivisionAsync(Guid tournamentId, Guid divisionId, CancellationToken cancellationToken = default);
}
