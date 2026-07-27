using SportsGurukul.Platform.Competition.Models;

namespace SportsGurukul.Platform.Competition.Interfaces;

public interface ILiveUpdateSubscriber
{
    Task SubscribeToMatchAsync(Guid matchId, Func<LiveMatch, CancellationToken, Task> handler, CancellationToken cancellationToken = default);
    Task SubscribeToTournamentAsync(Guid tournamentId, Func<string, object, CancellationToken, Task> handler, CancellationToken cancellationToken = default);
    Task UnsubscribeFromMatchAsync(Guid matchId, CancellationToken cancellationToken = default);
    Task UnsubscribeFromTournamentAsync(Guid tournamentId, CancellationToken cancellationToken = default);
}
