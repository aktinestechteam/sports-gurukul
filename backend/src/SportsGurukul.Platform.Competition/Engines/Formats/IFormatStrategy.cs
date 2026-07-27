using SportsGurukul.Platform.Competition.Models;
using SportsGurukul.Platform.Competition.Models.Enums;

namespace SportsGurukul.Platform.Competition.Engines.Formats;

/// <summary>
/// Defines the contract for competition format strategies that generate and manage matches.
/// </summary>
public interface IFormatStrategy
{
    /// <summary>
    /// Gets the competition format this strategy implements.
    /// </summary>
    CompetitionFormat Format { get; }

    /// <summary>
    /// Generates all initial matches for a competition based on the participants and configuration.
    /// </summary>
    /// <param name="participants">The list of participants competing.</param>
    /// <param name="config">The competition configuration.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A read-only list of generated matches.</returns>
    Task<IReadOnlyList<CompetitionMatch>> GenerateMatchesAsync(
        IReadOnlyList<Participant> participants,
        CompetitionConfig config,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates the next round of matches based on existing completed matches.
    /// </summary>
    /// <param name="existingMatches">All matches played so far.</param>
    /// <param name="config">The competition configuration.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A read-only list of newly generated matches for the next round.</returns>
    Task<IReadOnlyList<CompetitionMatch>> GenerateNextRoundAsync(
        IReadOnlyList<CompetitionMatch> existingMatches,
        CompetitionConfig config,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Determines whether the competition is complete based on the current set of matches.
    /// </summary>
    /// <param name="matches">All matches in the competition.</param>
    /// <returns>True if the competition is complete; otherwise, false.</returns>
    bool IsComplete(IReadOnlyList<CompetitionMatch> matches);
}
