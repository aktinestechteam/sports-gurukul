using SportsGurukul.Platform.Competition.Models;
using SportsGurukul.Platform.Competition.Models.Enums;

namespace SportsGurukul.Platform.Competition.Engines.Formats;

/// <summary>
/// Implements a hybrid tournament format combining a round-robin group stage with knockout finals.
/// Participants are divided into groups and play round-robin within each group.
/// The top N participants from each group advance to a knockout bracket.
/// The knockout bracket starts from quarterfinals or earlier depending on the number of qualifiers.
/// </summary>
public class HybridTournamentStrategy : IFormatStrategy
{
    /// <inheritdoc />
    public CompetitionFormat Format => CompetitionFormat.HybridTournament;

    /// <inheritdoc />
    public async Task<IReadOnlyList<CompetitionMatch>> GenerateMatchesAsync(
        IReadOnlyList<Participant> participants,
        CompetitionConfig config,
        CancellationToken cancellationToken = default)
    {
        if (participants.Count < 2)
            return Array.Empty<CompetitionMatch>();

        int groupsCount = config.GroupsCount ?? CalculateOptimalGroups(participants.Count);
        int advancementPerGroup = config.AdvancementPerGroup ?? 2;

        var seeded = SeedParticipants(participants, config.SeedingStrategy);
        var groups = AssignGroupsSerpentine(seeded, groupsCount);

        var matches = new List<CompetitionMatch>();
        int matchNumber = 1;

        for (int g = 0; g < groups.Count; g++)
        {
            var group = groups[g];
            int n = group.Count;
            bool isOdd = n % 2 != 0;
            var groupParticipants = new List<Participant>(group);

            if (isOdd)
            {
                groupParticipants.Add(new Participant
                {
                    Id = Guid.NewGuid(),
                    Name = $"BYE-Group{g + 1}",
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
                        home = groupParticipants[0];
                        away = groupParticipants[n - 1 - round];
                    }
                    else
                    {
                        int homeIdx = round - i;
                        if (homeIdx < 1) homeIdx += n - 2;
                        int awayIdx = round + i;
                        if (awayIdx >= n - 1) awayIdx -= n - 2;

                        home = groupParticipants[homeIdx];
                        away = groupParticipants[awayIdx];
                    }

                    if (home.IsBye || away.IsBye)
                        continue;

                    var match = new CompetitionMatch
                    {
                        Id = Guid.NewGuid(),
                        MatchNumber = matchNumber++,
                        RoundNumber = round + 1,
                        RoundType = RoundType.Group,
                        BracketType = BracketType.Main,
                        HomeParticipantId = home.Id,
                        HomeParticipantName = home.Name,
                        AwayParticipantId = away.Id,
                        AwayParticipantName = away.Name,
                        Status = MatchStatus.Scheduled,
                        Notes = $"Group {g + 1}"
                    };

                    matches.Add(match);
                }
            }
        }

        int totalQualifiers = advancementPerGroup * groupsCount;
        if (totalQualifiers >= 2)
        {
            int knockoutSize = FindKnockoutSize(totalQualifiers);
            int knockoutRounds = (int)Math.Log2(knockoutSize);
            int maxGroupRound = matches.Any() ? matches.Max(m => m.RoundNumber) : 0;

            for (int round = 1; round <= knockoutRounds; round++)
            {
                int matchesInRound = knockoutSize >> round;
                var roundType = round == knockoutRounds ? RoundType.Final :
                                round == knockoutRounds - 1 ? RoundType.SemiFinal :
                                round == knockoutRounds - 2 && knockoutSize >= 8 ? RoundType.KnockoutRound :
                                RoundType.KnockoutRound;

                for (int i = 0; i < matchesInRound; i++)
                {
                    matches.Add(new CompetitionMatch
                    {
                        Id = Guid.NewGuid(),
                        MatchNumber = matchNumber++,
                        RoundNumber = maxGroupRound + round,
                        RoundType = roundType,
                        BracketType = BracketType.Main,
                        Status = MatchStatus.Scheduled,
                        Notes = "Knockout"
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

        var knockoutMatches = existingMatches.Where(m => m.Notes?.Contains("Knockout") == true).ToList();
        bool isInKnockoutPhase = currentRoundMatches.Any(m => m.Notes?.Contains("Knockout") == true);

        if (isInKnockoutPhase)
        {
            var maxKnockoutRound = knockoutMatches.Max(m => m.RoundNumber);
            if (currentRound >= maxKnockoutRound)
                return Array.Empty<CompetitionMatch>();

            var nextRoundMatches = existingMatches.Where(m => m.RoundNumber == currentRound + 1).ToList();
            var winners = currentRoundMatches
                .Where(m => m.WinnerId.HasValue)
                .OrderBy(m => m.MatchNumber)
                .ToList();

            int matchIdx = 0;
            for (int i = 0; i < nextRoundMatches.Count && matchIdx + 1 < winners.Count; i++)
            {
                nextRoundMatches[i].HomeParticipantId = winners[matchIdx].WinnerId;
                nextRoundMatches[i].HomeParticipantName = winners[matchIdx].WinnerName;
                nextRoundMatches[i].AwayParticipantId = winners[matchIdx + 1].WinnerId;
                nextRoundMatches[i].AwayParticipantName = winners[matchIdx + 1].WinnerName;
                matchIdx += 2;
            }

            return await Task.FromResult<IReadOnlyList<CompetitionMatch>>(nextRoundMatches);
        }

        bool allGroupStagesComplete = existingMatches
            .Where(m => m.Notes?.StartsWith("Group") == true)
            .All(m => m.IsCompleted);

        if (allGroupStagesComplete)
        {
            int maxGroupRound = existingMatches
                .Where(m => m.Notes?.StartsWith("Group") == true)
                .Max(m => m.RoundNumber);

            var knockoutMatchesPending = existingMatches
                .Where(m => m.Notes?.Contains("Knockout") == true && m.RoundNumber == maxGroupRound + 1)
                .ToList();

            return await Task.FromResult<IReadOnlyList<CompetitionMatch>>(knockoutMatchesPending);
        }

        return Array.Empty<CompetitionMatch>();
    }

    /// <inheritdoc />
    public bool IsComplete(IReadOnlyList<CompetitionMatch> matches)
    {
        if (matches.Count == 0) return false;
        return matches.All(m => m.IsCompleted || m.IsBye);
    }

    /// <summary>
    /// Calculates the optimal number of groups based on participant count.
    /// Targets 4-6 participants per group.
    /// </summary>
    private static int CalculateOptimalGroups(int participantCount)
    {
        if (participantCount <= 4) return 1;
        if (participantCount <= 8) return 2;
        if (participantCount <= 12) return 3;
        if (participantCount <= 16) return 4;
        if (participantCount <= 24) return 6;
        return Math.Max(4, participantCount / 5);
    }

    /// <summary>
    /// Assigns participants to groups using a serpentine (snake) draft for balanced groups.
    /// </summary>
    private static List<List<Participant>> AssignGroupsSerpentine(List<Participant> seeded, int groupsCount)
    {
        var groups = new List<List<Participant>>();
        for (int i = 0; i < groupsCount; i++)
            groups.Add(new List<Participant>());

        for (int i = 0; i < seeded.Count; i++)
        {
            int cycle = i / groupsCount;
            int position = i % groupsCount;

            int targetIdx = cycle % 2 == 0 ? position : groupsCount - 1 - position;
            groups[targetIdx].Add(seeded[i]);
        }

        return groups;
    }

    /// <summary>
    /// Finds the smallest power of 2 greater than or equal to the given value.
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
