using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.Competition.Interfaces;
using SportsGurukul.Platform.Competition.Models;

namespace SportsGurukul.Platform.Competition.Services;

public class MatchAssignmentService : IMatchAssignmentService
{
    private readonly ILogger<MatchAssignmentService> _logger;

    public MatchAssignmentService(ILogger<MatchAssignmentService> logger)
    {
        _logger = logger;
    }

    public Task<IReadOnlyList<CompetitionMatch>> AssignVenuesAsync(
        IReadOnlyList<CompetitionMatch> matches,
        IReadOnlyList<Guid> availableVenueIds,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Assigning venues to {MatchCount} matches", matches.Count);

        if (availableVenueIds.Count == 0)
            return Task.FromResult(matches);

        var result = matches.ToList();
        for (int i = 0; i < result.Count; i++)
        {
            result[i].VenueId = availableVenueIds[i % availableVenueIds.Count];
        }

        return Task.FromResult<IReadOnlyList<CompetitionMatch>>(result);
    }

    public Task<IReadOnlyList<CompetitionMatch>> AssignOfficialsAsync(
        IReadOnlyList<CompetitionMatch> matches,
        IReadOnlyList<Guid> availableOfficialIds,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Assigning officials to {MatchCount} matches", matches.Count);

        if (availableOfficialIds.Count == 0)
            return Task.FromResult(matches);

        var result = matches.ToList();
        for (int i = 0; i < result.Count; i++)
        {
            result[i].OfficialId = availableOfficialIds[i % availableOfficialIds.Count];
        }

        return Task.FromResult<IReadOnlyList<CompetitionMatch>>(result);
    }
}
