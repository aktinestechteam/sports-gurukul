using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.TournamentManagement.Services;

public interface IBracketGenerationService
{
    Task<IReadOnlyList<TournamentBracket>> GenerateBracketsAsync(
        Tournament tournament,
        IReadOnlyList<TournamentParticipant> participants,
        IReadOnlyList<TournamentCategory> categories,
        CancellationToken cancellationToken = default);
}
