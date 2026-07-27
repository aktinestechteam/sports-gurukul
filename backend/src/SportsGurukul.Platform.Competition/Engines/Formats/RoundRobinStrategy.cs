using SportsGurukul.Platform.Competition.Models;
using SportsGurukul.Platform.Competition.Models.Enums;

namespace SportsGurukul.Platform.Competition.Engines.Formats;

/// <summary>
/// Implements a round-robin tournament format where every participant plays every other participant.
/// Uses the circle method (Berger table) for round generation.
/// Supports single round-robin or double round-robin (home/away).
/// </summary>
public class RoundRobinStrategy : IFormatStrategy
{
    /// <inheritdoc />
    public CompetitionFormat Format => CompetitionFormat.RoundRobin;

    /// <inheritdoc />
    public async Task<IReadOnlyList<CompetitionMatch>> GenerateMatchesAsync(
        IReadOnlyList<Participant> participants,
        CompetitionConfig config,
        CancellationToken cancellationToken = default)
    {
        if (participants.Count < 2)
            return Array.Empty<CompetitionMatch>();

        var ordered = participants.OrderBy(p => p.Id.ToString()).ToList();
        int n = ordered.Count;
        bool isOdd = n % 2 != 0;

        if (isOdd)
        {
            ordered.Add(new Participant
            {
                Id = Guid.NewGuid(),
                Name = "BYE",
                IsBye = true
            });
            n++;
        }

        int rounds = n - 1;
        int matchesPerRound = n / 2;
        var matches = new List<CompetitionMatch>();
        int matchNumber = 1;

        var fixedPlayer = ordered[0];
        var rotatingPlayers = ordered.Skip(1).ToList();

        for (int round = 0; round < rounds; round++)
        {
            for (int i = 0; i < matchesPerRound; i++)
            {
                Participant home, away;

                if (i == 0)
                {
                    home = rotatingPlayers.Count > 0 ? rotatingPlayers[round % rotatingPlayers.Count] : fixedPlayer;
                    away = fixedPlayer;
                }
                else
                {
                    int rightIdx = (round + i) % rotatingPlayers.Count;
                    int leftIdx = (round - i + rotatingPlayers.Count) % rotatingPlayers.Count;
                    home = rotatingPlayers[leftIdx];
                    away = rotatingPlayers[rightIdx];
                }

                if (home.IsBye || away.IsBye)
                    continue;

                var match = new CompetitionMatch
                {
                    Id = Guid.NewGuid(),
                    MatchNumber = matchNumber++,
                    RoundNumber = round + 1,
                    RoundType = RoundType.RoundRobin,
                    BracketType = BracketType.Main,
                    HomeParticipantId = home.Id,
                    HomeParticipantName = home.Name,
                    AwayParticipantId = away.Id,
                    AwayParticipantName = away.Name,
                    Status = MatchStatus.Scheduled
                };

                matches.Add(match);
            }
        }

        if (config.UseHomeAway)
        {
            int firstLegCount = matches.Count;
            for (int i = 0; i < firstLegCount; i++)
            {
                var original = matches[i];
                var returnMatch = new CompetitionMatch
                {
                    Id = Guid.NewGuid(),
                    MatchNumber = matchNumber++,
                    RoundNumber = rounds + original.RoundNumber,
                    RoundType = RoundType.RoundRobin,
                    BracketType = BracketType.Main,
                    HomeParticipantId = original.AwayParticipantId,
                    HomeParticipantName = original.AwayParticipantName,
                    AwayParticipantId = original.HomeParticipantId,
                    AwayParticipantName = original.HomeParticipantName,
                    Status = MatchStatus.Scheduled
                };
                matches.Add(returnMatch);
            }
        }

        return await Task.FromResult<IReadOnlyList<CompetitionMatch>>(matches);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<CompetitionMatch>> GenerateNextRoundAsync(
        IReadOnlyList<CompetitionMatch> existingMatches,
        CompetitionConfig config,
        CancellationToken cancellationToken = default)
    {
        if (existingMatches.Count == 0)
            return Array.Empty<CompetitionMatch>();

        int currentRound = existingMatches.Max(m => m.RoundNumber);
        var currentRoundMatches = existingMatches.Where(m => m.RoundNumber == currentRound).ToList();

        bool currentRoundComplete = currentRoundMatches.All(m => m.IsCompleted);
        if (!currentRoundComplete)
            return Array.Empty<CompetitionMatch>();

        var nextRoundMatches = existingMatches.Where(m => m.RoundNumber == currentRound + 1).ToList();
        if (nextRoundMatches.Count == 0)
            return Array.Empty<CompetitionMatch>();

        return await Task.FromResult<IReadOnlyList<CompetitionMatch>>(nextRoundMatches);
    }

    /// <inheritdoc />
    public bool IsComplete(IReadOnlyList<CompetitionMatch> matches)
    {
        if (matches.Count == 0) return false;
        return matches.All(m => m.IsCompleted || m.IsBye);
    }
}
