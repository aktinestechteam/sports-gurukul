using SportsGurukul.Platform.Competition.Models;
using SportsGurukul.Platform.Competition.Models.Enums;

namespace SportsGurukul.Platform.Competition.Engines.Formats;

/// <summary>
/// Implements a division-based league format.
/// Participants are divided into groups/divisions using a serpentine (snake) draft.
/// Round-robin matches are played within each division.
/// The top N participants from each division advance to a cross-division knockout stage.
/// </summary>
public class LeagueStrategy : IFormatStrategy
{
    /// <inheritdoc />
    public CompetitionFormat Format => CompetitionFormat.League;

    /// <inheritdoc />
    public async Task<IReadOnlyList<CompetitionMatch>> GenerateMatchesAsync(
        IReadOnlyList<Participant> participants,
        CompetitionConfig config,
        CancellationToken cancellationToken = default)
    {
        if (participants.Count < 2)
            return Array.Empty<CompetitionMatch>();

        int divisionsCount = config.GroupsCount ?? CalculateOptimalDivisions(participants.Count);
        var seeded = SeedParticipants(participants, config.SeedingStrategy);
        var divisions = AssignDivisionsSerpentine(seeded, divisionsCount);

        var matches = new List<CompetitionMatch>();
        int matchNumber = 1;

        for (int div = 0; div < divisions.Count; div++)
        {
            var division = divisions[div];
            int n = division.Count;
            bool isOdd = n % 2 != 0;
            var participantsList = new List<Participant>(division);

            if (isOdd)
            {
                participantsList.Add(new Participant
                {
                    Id = Guid.NewGuid(),
                    Name = "BYE",
                    IsBye = true
                });
                n++;
            }

            int rounds = n - 1;
            int matchesPerRound = n / 2;

            for (int round = 0; round < rounds; round++)
            {
                for (int i = 0; i < matchesPerRound; i++)
                {
                    Participant home, away;

                    if (i == 0)
                    {
                        home = participantsList[0];
                        away = participantsList[n - 1 - round];
                    }
                    else
                    {
                        int homeIdx = round - i;
                        if (homeIdx < 1) homeIdx += n - 2;
                        int awayIdx = round + i;
                        if (awayIdx >= n - 1) awayIdx -= n - 2;

                        home = participantsList[homeIdx];
                        away = participantsList[awayIdx];
                    }

                    if (home.IsBye || away.IsBye)
                        continue;

                    var match = new CompetitionMatch
                    {
                        Id = Guid.NewGuid(),
                        MatchNumber = matchNumber++,
                        RoundNumber = round + 1,
                        RoundType = RoundType.LeagueMatchday,
                        BracketType = BracketType.Main,
                        HomeParticipantId = home.Id,
                        HomeParticipantName = home.Name,
                        AwayParticipantId = away.Id,
                        AwayParticipantName = away.Name,
                        Status = MatchStatus.Scheduled,
                        Notes = $"Division {div + 1}"
                    };

                    matches.Add(match);
                }
            }
        }

        int advancementPerGroup = config.AdvancementPerGroup ?? CalculateAdvancement(participants.Count, divisionsCount);
        int totalAdvancees = advancementPerGroup * divisionsCount;

        if (totalAdvancees >= 4)
        {
            int knockoutSize = FindKnockoutSize(totalAdvancees);
            int knockoutRounds = (int)Math.Log2(knockoutSize);
            int totalLeagueRounds = matches.Count > 0 ? matches.Max(m => m.RoundNumber) : 0;

            for (int round = 1; round <= knockoutRounds; round++)
            {
                int matchesInRound = knockoutSize >> round;
                var roundType = round == knockoutRounds ? RoundType.Final :
                                round == knockoutRounds - 1 ? RoundType.SemiFinal :
                                RoundType.KnockoutRound;

                for (int i = 0; i < matchesInRound; i++)
                {
                    matches.Add(new CompetitionMatch
                    {
                        Id = Guid.NewGuid(),
                        MatchNumber = matchNumber++,
                        RoundNumber = totalLeagueRounds + round,
                        RoundType = roundType,
                        BracketType = BracketType.Main,
                        Status = MatchStatus.Scheduled,
                        Notes = "Cross-division knockout"
                    });
                }
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

        bool currentRoundComplete = currentRoundMatches.All(m => m.IsCompleted || m.IsBye);
        if (!currentRoundComplete)
            return Array.Empty<CompetitionMatch>();

        bool hasKnockout = existingMatches.Any(m => m.Notes?.Contains("Cross-division knockout") == true);
        if (hasKnockout)
        {
            var currentKnockoutMatches = existingMatches.Where(m => m.RoundNumber == currentRound && m.Notes?.Contains("Cross-division knockout") == true).ToList();
            bool isKnockoutRound = currentKnockoutMatches.Count > 0;

            if (isKnockoutRound && currentRoundMatches.All(m => m.IsCompleted))
            {
                var winners = currentKnockoutMatches.Where(m => m.WinnerId.HasValue).ToList();
                int maxKnockoutRound = existingMatches
                    .Where(m => m.Notes?.Contains("Cross-division knockout") == true)
                    .Max(m => m.RoundNumber);

                if (currentRound < maxKnockoutRound)
                {
                    var nextRoundMatches = existingMatches.Where(m => m.RoundNumber == currentRound + 1).ToList();
                    int matchIdx = 0;

                    for (int i = 0; i < winners.Count && matchIdx < nextRoundMatches.Count; i += 2)
                    {
                        var match = nextRoundMatches[matchIdx++];
                        match.HomeParticipantId = winners[i].WinnerId;
                        match.HomeParticipantName = winners[i].WinnerName;

                        if (i + 1 < winners.Count)
                        {
                            match.AwayParticipantId = winners[i + 1].WinnerId;
                            match.AwayParticipantName = winners[i + 1].WinnerName;
                        }
                    }

                    return await Task.FromResult<IReadOnlyList<CompetitionMatch>>(nextRoundMatches.Where(m => m.HomeParticipantId.HasValue).ToList());
                }
            }
        }

        var nextRoundAll = existingMatches.Where(m => m.RoundNumber == currentRound + 1).ToList();
        return await Task.FromResult<IReadOnlyList<CompetitionMatch>>(nextRoundAll);
    }

    /// <inheritdoc />
    public bool IsComplete(IReadOnlyList<CompetitionMatch> matches)
    {
        if (matches.Count == 0) return false;
        return matches.All(m => m.IsCompleted || m.IsBye);
    }

    /// <summary>
    /// Calculates the optimal number of divisions based on participant count.
    /// Aims for 4-8 participants per division.
    /// </summary>
    private static int CalculateOptimalDivisions(int participantCount)
    {
        if (participantCount <= 4) return 1;
        if (participantCount <= 8) return 2;
        if (participantCount <= 16) return Math.Min(4, participantCount / 4);
        return Math.Min(8, participantCount / 4);
    }

    /// <summary>
    /// Calculates how many participants advance from each division.
    /// Default is top 2 from each division.
    /// </summary>
    private static int CalculateAdvancement(int participantCount, int divisionsCount)
    {
        int perDivision = participantCount / divisionsCount;
        if (perDivision <= 4) return 1;
        if (perDivision <= 8) return 2;
        return Math.Min(4, perDivision / 2);
    }

    /// <summary>
    /// Assigns participants to divisions using a serpentine (snake) draft.
    /// This ensures balanced divisions by distributing top seeds evenly.
    /// </summary>
    private static List<List<Participant>> AssignDivisionsSerpentine(List<Participant> seeded, int divisionsCount)
    {
        var divisions = new List<List<Participant>>();
        for (int i = 0; i < divisionsCount; i++)
            divisions.Add(new List<Participant>());

        for (int i = 0; i < seeded.Count; i++)
        {
            int cycle = i / divisionsCount;
            int position = i % divisionsCount;

            int targetIdx;
            if (cycle % 2 == 0)
                targetIdx = position;
            else
                targetIdx = divisionsCount - 1 - position;

            divisions[targetIdx].Add(seeded[i]);
        }

        return divisions;
    }

    /// <summary>
    /// Finds the smallest power of 2 greater than or equal to the given value for the knockout bracket.
    /// </summary>
    private static int FindKnockoutSize(int value)
    {
        if (value <= 2) return 2;
        int power = 2;
        while (power < value)
            power *= 2;
        return power;
    }

    /// <summary>
    /// Seeds participants based on the configured strategy.
    /// </summary>
    private static List<Participant> SeedParticipants(IReadOnlyList<Participant> participants, SeedingStrategy strategy)
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
}
