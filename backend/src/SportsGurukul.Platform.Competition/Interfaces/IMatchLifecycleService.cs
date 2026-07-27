using SportsGurukul.Platform.Competition.Models;
using SportsGurukul.Platform.Competition.Models.Enums;

namespace SportsGurukul.Platform.Competition.Interfaces;

public interface IMatchLifecycleService
{
    Task<LiveMatch> TransitionToCheckInAsync(Guid matchId, CancellationToken cancellationToken = default);
    Task<LiveMatch> TransitionToWarmUpAsync(Guid matchId, CancellationToken cancellationToken = default);
    Task<LiveMatch> TransitionToLiveAsync(Guid matchId, CancellationToken cancellationToken = default);
    Task<LiveMatch> TransitionToPausedAsync(Guid matchId, CancellationToken cancellationToken = default);
    Task<LiveMatch> TransitionToCompletedAsync(Guid matchId, Guid? winnerId, string? winnerName, CancellationToken cancellationToken = default);
    Task<LiveMatch> RecordWalkoverAsync(Guid matchId, Guid winnerId, string winnerName, CancellationToken cancellationToken = default);
    Task<LiveMatch> RecordForfeitAsync(Guid matchId, Guid winnerId, string winnerName, CancellationToken cancellationToken = default);
    Task<LiveMatch> TransitionToCancelledAsync(Guid matchId, CancellationToken cancellationToken = default);
    Task<LiveMatch> TransitionToAbandonedAsync(Guid matchId, CancellationToken cancellationToken = default);
    Task<bool> IsValidTransitionAsync(LiveMatchStatus current, LiveMatchStatus target, CancellationToken cancellationToken = default);
}
