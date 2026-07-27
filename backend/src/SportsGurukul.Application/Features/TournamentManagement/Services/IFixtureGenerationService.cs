using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.TournamentManagement.Services;

public interface IFixtureGenerationService
{
    Task<IReadOnlyList<TournamentFixture>> GenerateFixturesAsync(
        Tournament tournament,
        IReadOnlyList<TournamentParticipant> participants,
        IReadOnlyList<TournamentStage> stages,
        CancellationToken cancellationToken = default);
}
