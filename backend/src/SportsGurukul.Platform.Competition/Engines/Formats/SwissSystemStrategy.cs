using SportsGurukul.Platform.Competition.Models;
using SportsGurukul.Platform.Competition.Models.Enums;

namespace SportsGurukul.Platform.Competition.Engines.Formats;

/// <summary>
/// Implements the Swiss-system tournament format.
/// Participants are paired based on similar win-loss records each round.
/// Avoids repeat pairings and supports Buchholz tiebreaker calculations.
/// The number of rounds is configurable (typically log2(n) rounded up).
/// </summary>
public class SwissSystemStrategy : IFormatStrategy
{
    /// <inheritdoc />
    public CompetitionFormat Format => CompetitionFormat.SwissSystem;

    /// <inheritdoc />
    public async Task<IReadOnlyList<CompetitionMatch>> GenerateMatchesAsync(
        IReadOnlyList<Participant> participants,
        CompetitionConfig config,
        CancellationToken cancellationToken = default)
    {
        if (participants.Count < 2)
            return Array.Empty<CompetitionMatch>();

        int totalRounds = config.RoundsCount ?? CalculateSwissRounds(participants.Count);
        var seedOrder = GetSeedOrder(participants, config.SeedingStrategy);
        var matches = new List<CompetitionMatch>();
        int matchNumber = 1;

        var playerRecords = seedOrder.ToDictionary(p => p.Id, _ => new SwissRecord());

        var firstRoundPairings = GenerateFirstRoundPairings(seedOrder);
        int roundNumber = 1;

        foreach (var (home, away) in firstRoundPairings)
        {
            matches.Add(new CompetitionMatch
            {
                Id = Guid.NewGuid(),
                MatchNumber = matchNumber++,
                RoundNumber = roundNumber,
                RoundType = RoundType.SwissRound,
                BracketType = BracketType.Main,
                HomeParticipantId = home.Id,
                HomeParticipantName = home.Name,
                AwayParticipantId = away.Id,
                AwayParticipantName = away.Name,
                Status = MatchStatus.Scheduled
            });
        }

        for (int round = 2; round <= totalRounds; round++)
        {
            int matchesThisRound = participants.Count / 2;
            for (int i = 0; i < matchesThisRound; i++)
            {
                matches.Add(new CompetitionMatch
                {
                    Id = Guid.NewGuid(),
                    MatchNumber = matchNumber++,
                    RoundNumber = round,
                    RoundType = RoundType.SwissRound,
                    BracketType = BracketType.Main,
                    Status = MatchStatus.Scheduled
                });
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

        int totalRounds = config.RoundsCount ?? CalculateSwissRounds(
            existingMatches.SelectMany(m => new[] { m.HomeParticipantId, m.AwayParticipantId })
                .Where(id => id.HasValue).Select(id => id!.Value).Distinct().Count());

        if (currentRound >= totalRounds)
            return Array.Empty<CompetitionMatch>();

        var records = BuildPlayerRecords(existingMatches);
        var scheduledMatches = existingMatches.Where(m => m.RoundNumber == currentRound + 1).ToList();
        var pendingMatches = scheduledMatches.Where(m => m.Status == MatchStatus.Scheduled).ToList();
        var playedPairings = GetPlayedPairings(existingMatches);

        int nextRound = currentRound + 1;
        var availablePlayers = records.Keys.ToList();

        var grouped = availablePlayers
            .GroupBy(id => records[id].Wins)
            .OrderByDescending(g => g.Key)
            .ToList();

        var paired = new HashSet<Guid>();
        var scheduledMap = pendingMatches.ToDictionary(m => m.Id);
        int matchIdx = 0;

        var flatList = grouped.SelectMany(g =>
            g.OrderByDescending(id => CalculateBuchholz(id, records, existingMatches))).ToList();

        for (int i = 0; i < flatList.Count; i++)
        {
            if (paired.Contains(flatList[i])) continue;

            for (int j = i + 1; j < flatList.Count; j++)
            {
                if (paired.Contains(flatList[j])) continue;

                var p1 = flatList[i];
                var p2 = flatList[j];

                if (!playedPairings.Contains(GetPairingKey(p1, p2)))
                {
                    if (matchIdx < pendingMatches.Count)
                    {
                        var match = pendingMatches[matchIdx++];
                        match.HomeParticipantId = p1;
                        match.HomeParticipantName = records[p1].Name;
                        match.AwayParticipantId = p2;
                        match.AwayParticipantName = records[p2].Name;
                        match.RoundNumber = nextRound;
                        match.RoundType = RoundType.SwissRound;

                        paired.Add(p1);
                        paired.Add(p2);
                    }
                    break;
                }
            }
        }

        var unpaired = flatList.Where(id => !paired.Contains(id)).ToList();
        for (int i = 0; i + 1 < unpaired.Count; i += 2)
        {
            if (matchIdx < pendingMatches.Count)
            {
                var match = pendingMatches[matchIdx++];
                match.HomeParticipantId = unpaired[i];
                match.HomeParticipantName = records[unpaired[i]].Name;
                match.AwayParticipantId = unpaired[i + 1];
                match.AwayParticipantName = records[unpaired[i + 1]].Name;
                match.RoundNumber = nextRound;
                match.RoundType = RoundType.SwissRound;
            }
        }

        return await Task.FromResult<IReadOnlyList<CompetitionMatch>>(new List<CompetitionMatch>());
    }

    /// <inheritdoc />
    public bool IsComplete(IReadOnlyList<CompetitionMatch> matches)
    {
        if (matches.Count == 0) return false;
        return matches.All(m => m.IsCompleted || m.IsBye);
    }

    /// <summary>
    /// Calculates the recommended number of Swiss rounds for a given participant count.
    /// Typically ceil(log2(n)).
    /// </summary>
    private static int CalculateSwissRounds(int participantCount)
    {
        if (participantCount <= 2) return 1;
        return (int)Math.Ceiling(Math.Log2(participantCount));
    }

    /// <summary>
    /// Orders participants by seed for the first round.
    /// </summary>
    private static List<Participant> GetSeedOrder(IReadOnlyList<Participant> participants, SeedingStrategy strategy)
    {
        return strategy switch
        {
            SeedingStrategy.RankingBased => participants
                .OrderBy(p => p.Ranking ?? int.MaxValue)
                .ThenBy(p => p.Name)
                .ToList(),
            SeedingStrategy.Random => participants
                .OrderBy(_ => Random.Shared.Next())
                .ToList(),
            _ => participants
                .OrderBy(p => p.SeedNumber)
                .ThenBy(p => p.Name)
                .ToList()
        };
    }

    /// <summary>
    /// Generates first-round pairings using seeds: 1v2, 3v4, 5v6, etc.
    /// This ensures top seeds face each other early in a balanced way.
    /// </summary>
    private static List<(Participant Home, Participant Away)> GenerateFirstRoundPairings(List<Participant> seeded)
    {
        var pairings = new List<(Participant, Participant)>();

        for (int i = 0; i + 1 < seeded.Count; i += 2)
        {
            pairings.Add((seeded[i], seeded[i + 1]));
        }

        return pairings;
    }

    /// <summary>
    /// Calculates the Buchholz score for a player (sum of opponents' win counts).
    /// Used as a tiebreaker in Swiss tournaments.
    /// </summary>
    private static int CalculateBuchholz(Guid playerId, Dictionary<Guid, SwissRecord> records, IReadOnlyList<CompetitionMatch> matches)
    {
        int buchholz = 0;
        var opponents = matches
            .Where(m => m.IsCompleted && (m.HomeParticipantId == playerId || m.AwayParticipantId == playerId))
            .Select(m => m.HomeParticipantId == playerId ? m.AwayParticipantId : m.HomeParticipantId)
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .Distinct()
            .ToList();

        foreach (var oppId in opponents)
        {
            if (records.TryGetValue(oppId, out var oppRecord))
                buchholz += oppRecord.Wins;
        }

        return buchholz;
    }

    /// <summary>
    /// Builds player records from existing matches.
    /// </summary>
    private static Dictionary<Guid, SwissRecord> BuildPlayerRecords(IReadOnlyList<CompetitionMatch> matches)
    {
        var records = new Dictionary<Guid, SwissRecord>();

        foreach (var match in matches.Where(m => m.IsCompleted && !m.IsBye))
        {
            if (match.HomeParticipantId.HasValue && match.HomeParticipantName != null)
            {
                if (!records.ContainsKey(match.HomeParticipantId.Value))
                    records[match.HomeParticipantId.Value] = new SwissRecord { Name = match.HomeParticipantName };
            }
            if (match.AwayParticipantId.HasValue && match.AwayParticipantName != null)
            {
                if (!records.ContainsKey(match.AwayParticipantId.Value))
                    records[match.AwayParticipantId.Value] = new SwissRecord { Name = match.AwayParticipantName };
            }

            if (match.WinnerId.HasValue && match.HomeParticipantId.HasValue && match.AwayParticipantId.HasValue)
            {
                records[match.HomeParticipantId.Value].MatchesPlayed++;
                records[match.AwayParticipantId.Value].MatchesPlayed++;

                if (match.WinnerId.Value == match.HomeParticipantId.Value)
                {
                    records[match.HomeParticipantId.Value].Wins++;
                    records[match.AwayParticipantId.Value].Losses++;
                }
                else
                {
                    records[match.AwayParticipantId.Value].Wins++;
                    records[match.HomeParticipantId.Value].Losses++;
                }
            }
        }

        return records;
    }

    /// <summary>
    /// Collects all unique pairings that have already been played.
    /// </summary>
    private static HashSet<string> GetPlayedPairings(IReadOnlyList<CompetitionMatch> matches)
    {
        var pairings = new HashSet<string>();
        foreach (var match in matches.Where(m => m.IsCompleted && !m.IsBye))
        {
            if (match.HomeParticipantId.HasValue && match.AwayParticipantId.HasValue)
                pairings.Add(GetPairingKey(match.HomeParticipantId.Value, match.AwayParticipantId.Value));
        }
        return pairings;
    }

    /// <summary>
    /// Creates a canonical pairing key to avoid duplicate matches regardless of home/away order.
    /// </summary>
    private static string GetPairingKey(Guid p1, Guid p2)
    {
        return string.Compare(p1.ToString(), p2.ToString(), StringComparison.Ordinal) < 0
            ? $"{p1}-{p2}"
            : $"{p2}-{p1}";
    }

    /// <summary>
    /// Internal record tracker for a Swiss-system participant.
    /// </summary>
    private sealed class SwissRecord
    {
        public string Name { get; set; } = string.Empty;
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int MatchesPlayed { get; set; }
    }
}
