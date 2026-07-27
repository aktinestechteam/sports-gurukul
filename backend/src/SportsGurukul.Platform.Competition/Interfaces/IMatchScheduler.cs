using SportsGurukul.Platform.Competition.Interfaces.Scheduling;
using SportsGurukul.Platform.Competition.Models;

namespace SportsGurukul.Platform.Competition.Interfaces;

public interface IMatchScheduler
{
    Task<IReadOnlyList<CompetitionMatch>> ScheduleMatchesAsync(
        IReadOnlyList<CompetitionMatch> matches,
        IAvailabilityService availabilityService,
        IConflictDetectionService conflictDetection,
        CancellationToken cancellationToken = default);
}
