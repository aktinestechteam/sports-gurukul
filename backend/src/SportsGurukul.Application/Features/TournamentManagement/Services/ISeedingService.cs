using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.TournamentManagement.Services;

public interface ISeedingService
{
    Task<IReadOnlyList<TournamentSeed>> GenerateSeedsAsync(
        Tournament tournament,
        IReadOnlyList<TournamentParticipant> participants,
        CancellationToken cancellationToken = default);
}
