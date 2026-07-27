using SportsGurukul.Platform.Competition.Models;

namespace SportsGurukul.Platform.Competition.Interfaces;

public interface IMatchAssignmentService
{
    Task<IReadOnlyList<CompetitionMatch>> AssignVenuesAsync(
        IReadOnlyList<CompetitionMatch> matches,
        IReadOnlyList<Guid> availableVenueIds,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CompetitionMatch>> AssignOfficialsAsync(
        IReadOnlyList<CompetitionMatch> matches,
        IReadOnlyList<Guid> availableOfficialIds,
        CancellationToken cancellationToken = default);
}
