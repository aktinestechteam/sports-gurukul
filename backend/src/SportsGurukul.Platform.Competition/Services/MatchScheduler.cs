using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.Competition.Interfaces;
using SportsGurukul.Platform.Competition.Interfaces.Scheduling;
using SportsGurukul.Platform.Competition.Models;

namespace SportsGurukul.Platform.Competition.Services;

public class MatchScheduler : IMatchScheduler
{
    private readonly ILogger<MatchScheduler> _logger;

    public MatchScheduler(ILogger<MatchScheduler> logger)
    {
        _logger = logger;
    }

    public async Task<IReadOnlyList<CompetitionMatch>> ScheduleMatchesAsync(
        IReadOnlyList<CompetitionMatch> matches,
        IAvailabilityService availabilityService,
        IConflictDetectionService conflictDetection,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Scheduling {MatchCount} matches", matches.Count);

        var result = matches.ToList();
        var scheduledDates = new Dictionary<Guid, HashSet<DateTime>>();

        var currentDate = DateTime.UtcNow.Date.AddDays(1);
        var currentTime = new TimeSpan(9, 0, 0);

        foreach (var match in result.Where(m => m.Status == Models.Enums.MatchStatus.Scheduled))
        {
            if (match.IsBye)
            {
                match.Status = Models.Enums.MatchStatus.Completed;
                continue;
            }

            var homeId = match.HomeParticipantId ?? Guid.Empty;
            var awayId = match.AwayParticipantId ?? Guid.Empty;

            bool hasConflict = await conflictDetection.HasConflictAsync(homeId, currentDate, currentTime, cancellationToken) ||
                               await conflictDetection.HasConflictAsync(awayId, currentDate, currentTime, cancellationToken);

            if (hasConflict)
            {
                currentTime = currentTime.Add(TimeSpan.FromHours(1));
                if (currentTime >= new TimeSpan(21, 0, 0))
                {
                    currentDate = currentDate.AddDays(1);
                    currentTime = new TimeSpan(9, 0, 0);
                }
                continue;
            }

            match.ScheduledDate = currentDate;
            match.ScheduledTime = currentTime;

            currentTime = currentTime.Add(TimeSpan.FromHours(1));
            if (currentTime >= new TimeSpan(21, 0, 0))
            {
                currentDate = currentDate.AddDays(1);
                currentTime = new TimeSpan(9, 0, 0);
            }
        }

        return result;
    }
}
