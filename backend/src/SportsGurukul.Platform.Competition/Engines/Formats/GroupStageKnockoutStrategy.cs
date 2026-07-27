using SportsGurukul.Platform.Competition.Models;
using SportsGurukul.Platform.Competition.Models.Enums;

namespace SportsGurukul.Platform.Competition.Engines.Formats;

/// <summary>
/// Implements a group stage followed by a seeded knockout bracket.
/// Provides more control over group sizes and advancement compared to the hybrid format.
/// Group winners are seeded into the knockout bracket with cross-group pairing
/// (e.g., Group A 1st vs Group B 2nd).
/// Supports configurable group sizes and advancement counts.
/// </summary>
public class GroupStageKnockoutStrategy : IFormatStrategy
{
    /// <inheritdoc />
    public CompetitionFormat Format => CompetitionFormat.GroupStageKnockout;

    /// <inheritdoc />
    public async Task<IReadOnlyList<CompetitionMatch>> GenerateMatchesAsync(
        IReadOnlyList<Participant> participants,
        CompetitionConfig config,
        CancellationToken cancellationToken = default)
    {
        if (participants.Count < 2)
            return Array.Empty<CompetitionMatch>();

        int groupsCount = config.GroupsCount ?? CalculateGroups(participants.Count);
        int advancementPerGroup = config.AdvancementPerGroup ?? CalculateAdvancementPerGroup(participants.Count, groupsCount);

        var seeded = SeedParticipants(participants, config.SeedingStrategy);
        var groups = AssignGroups(seeded, groupsCount);

        var matches = new List<CompetitionMatch>();
        int matchNumber = 1;

        for (int g = 0; g < groups.Count; g++)
        {
            var group = groups[g];
            int n = group.Count;

            if (n < 2) continue;

            bool isOdd = n % 2 != 0;
            var roundRobinParticipants = new List<Participant>(group);

            if (isOdd)
            {
                roundRobinParticipants.Add(new Participant
                {
                    Id = Guid.NewGuid(),
                    Name = $"BYE-G{g + 1}",
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
                        home = roundRobinParticipants[0];
                        away = roundRobinParticipants[n - 1 - round];
                    }
                    else
                    {
                        int homeIdx = round - i;
                        if (homeIdx < 1) homeIdx += n - 2;
                        int awayIdx = round + i;
                        if (awayIdx >= n - 1) awayIdx -= n - 2;

                        home = roundRobinParticipants[homeIdx];
                        away = roundRobinParticipants[awayIdx];
                    }

                    if (home.IsBye || away.IsBye)
                        continue;

                    matches.Add(new CompetitionMatch
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
                    });
                }
            }
        }

        int totalQualifiers = advancementPerGroup * groupsCount;
        if (totalQualifiers < 2)
            return await Task.FromResult<IReadOnlyList<CompetitionMatch>>(matches);

        int knockoutSize = FindKnockoutSize(totalQualifiers);
        int knockoutRounds = (int)Math.Log2(knockoutSize);
        int maxGroupRound = matches.Any() ? matches.Max(m => m.RoundNumber) : 0;

        var knockoutBracket = GenerateKnockoutBracketSeeded(
            groupsCount, advancementPerGroup, knockoutSize,
            knockoutRounds, maxGroupRound, ref matchNumber);

        matches.AddRange(knockoutBracket);

        if (config.HasThirdPlaceMatch && knockoutSize >= 4)
        {
            matches.Add(new CompetitionMatch
            {
                Id = Guid.NewGuid(),
                MatchNumber = matchNumber++,
                RoundNumber = maxGroupRound + knockoutRounds,
                RoundType = RoundType.ThirdPlace,
                BracketType = BracketType.ThirdPlace,
                Status = MatchStatus.Scheduled,
                Notes = "Third Place"
            });
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

        bool isGroupRound = currentRoundMatches.Any(m => m.RoundType == RoundType.Group);
        bool isKnockoutRound = currentRoundMatches.Any(m => m.RoundType == RoundType.KnockoutRound ||
                                                             m.RoundType == RoundType.SemiFinal ||
                                                             m.RoundType == RoundType.Final);

        if (isKnockoutRound)
        {
            var maxKnockoutRound = existingMatches
                .Where(m => m.RoundType != RoundType.Group)
                .Max(m => m.RoundNumber);

            if (currentRound >= maxKnockoutRound)
            {
                bool hasThirdPlace = existingMatches.Any(m => m.BracketType == BracketType.ThirdPlace);
                if (hasThirdPlace && currentRoundMatches.Count(m => m.RoundType == RoundType.Final) == 1)
                {
                    var thirdPlaceMatch = existingMatches.FirstOrDefault(m => m.BracketType == BracketType.ThirdPlace);
                    if (thirdPlaceMatch != null && thirdPlaceMatch.Status == MatchStatus.Scheduled && !thirdPlaceMatch.HomeParticipantId.HasValue)
                    {
                        var semiFinalMatches = existingMatches
                            .Where(m => m.RoundType == RoundType.SemiFinal && m.IsCompleted)
                            .ToList();

                        var losers = GetSemiFinalLosers(semiFinalMatches);
                        if (losers.Count == 2)
                        {
                            thirdPlaceMatch.HomeParticipantId = losers[0].Id;
                            thirdPlaceMatch.HomeParticipantName = losers[0].Name;
                            thirdPlaceMatch.AwayParticipantId = losers[1].Id;
                            thirdPlaceMatch.AwayParticipantName = losers[1].Name;
                            return await Task.FromResult<IReadOnlyList<CompetitionMatch>>(new List<CompetitionMatch> { thirdPlaceMatch });
                        }
                    }
                }
                return Array.Empty<CompetitionMatch>();
            }

            var winners = currentRoundMatches
                .Where(m => m.WinnerId.HasValue)
                .OrderBy(m => m.MatchNumber)
                .ToList();

            var nextRoundMatches = existingMatches.Where(m => m.RoundNumber == currentRound + 1).ToList();
            int idx = 0;
            foreach (var match in nextRoundMatches.Where(m => !m.HomeParticipantId.HasValue))
            {
                if (idx < winners.Count)
                {
                    match.HomeParticipantId = winners[idx].WinnerId;
                    match.HomeParticipantName = winners[idx].WinnerName;
                }
                if (idx + 1 < winners.Count)
                {
                    match.AwayParticipantId = winners[idx + 1].WinnerId;
                    match.AwayParticipantName = winners[idx + 1].WinnerName;
                }
                idx += 2;
            }

            return await Task.FromResult<IReadOnlyList<CompetitionMatch>>(nextRoundMatches.Where(m => m.HomeParticipantId.HasValue).ToList());
        }

        if (isGroupRound)
        {
            bool allGroupsComplete = existingMatches
                .Where(m => m.RoundType == RoundType.Group)
                .All(m => m.IsCompleted || m.IsBye);

            if (!allGroupsComplete)
                return Array.Empty<CompetitionMatch>();

            var knockoutMatches = existingMatches
                .Where(m => m.RoundType != RoundType.Group && m.RoundType != RoundType.ThirdPlace)
                .Where(m => m.RoundNumber == currentRound + 1 || m.RoundNumber == existingMatches.Where(x => x.RoundType != RoundType.Group && x.RoundType != RoundType.ThirdPlace).Min(x => x.RoundNumber))
                .ToList();

            return await Task.FromResult<IReadOnlyList<CompetitionMatch>>(knockoutMatches);
        }

        return Array.Empty<CompetitionMatch>();
    }

    /// <inheritdoc />
    public bool IsComplete(IReadOnlyList<CompetitionMatch> matches)
    {
        if (matches.Count == 0) return false;

        bool hasThirdPlace = matches.Any(m => m.BracketType == BracketType.ThirdPlace);
        if (hasThirdPlace)
            return matches.All(m => m.IsCompleted || m.IsBye);

        var knockoutMatches = matches.Where(m => m.RoundType != RoundType.Group).ToList();
        if (knockoutMatches.Count == 0)
            return matches.All(m => m.IsCompleted || m.IsBye);

        var finalMatch = knockoutMatches
            .Where(m => m.RoundType == RoundType.Final)
            .OrderByDescending(m => m.RoundNumber)
            .FirstOrDefault();

        return finalMatch?.IsCompleted == true;
    }

    /// <summary>
    /// Generates the knockout bracket with seeded cross-group pairings.
    /// Group winners are paired against runners-up from other groups.
    /// </summary>
    private static List<CompetitionMatch> GenerateKnockoutBracketSeeded(
        int groupsCount, int advancementPerGroup, int knockoutSize,
        int knockoutRounds, int baseRound, ref int matchNumber)
    {
        var bracket = new List<CompetitionMatch>();
        var slotCount = knockoutSize;

        for (int round = 1; round <= knockoutRounds; round++)
        {
            int matchesInRound = slotCount >> round;
            var roundType = round == knockoutRounds ? RoundType.Final :
                            round == knockoutRounds - 1 ? RoundType.SemiFinal :
                            RoundType.KnockoutRound;

            for (int i = 0; i < matchesInRound; i++)
            {
                bracket.Add(new CompetitionMatch
                {
                    Id = Guid.NewGuid(),
                    MatchNumber = matchNumber++,
                    RoundNumber = baseRound + round,
                    RoundType = roundType,
                    BracketType = BracketType.Main,
                    Status = MatchStatus.Scheduled,
                    Notes = $"Knockout R{round}"
                });
            }
        }

        return bracket;
    }

    /// <summary>
    /// Gets the losers from semi-final matches.
    /// </summary>
    private static List<Participant> GetSemiFinalLosers(List<CompetitionMatch> semiFinalMatches)
    {
        var losers = new List<Participant>();

        foreach (var match in semiFinalMatches)
        {
            if (match.WinnerId.HasValue && match.HomeParticipantId.HasValue && match.AwayParticipantId.HasValue)
            {
                bool homeWon = match.WinnerId.Value == match.HomeParticipantId.Value;
                losers.Add(new Participant
                {
                    Id = homeWon ? match.AwayParticipantId.Value : match.HomeParticipantId.Value,
                    Name = homeWon ? match.AwayParticipantName ?? "Unknown" : match.HomeParticipantName ?? "Unknown"
                });
            }
        }

        return losers;
    }

    /// <summary>
    /// Calculates the number of groups. Targets 4-6 participants per group.
    /// </summary>
    private static int CalculateGroups(int participantCount)
    {
        if (participantCount <= 4) return 1;
        if (participantCount <= 8) return 2;
        if (participantCount <= 12) return 3;
        if (participantCount <= 16) return 4;
        if (participantCount <= 24) return 6;
        return Math.Max(4, participantCount / 5);
    }

    /// <summary>
    /// Calculates how many participants advance per group.
    /// Default: top 2 from each group.
    /// </summary>
    private static int CalculateAdvancementPerGroup(int participantCount, int groupsCount)
    {
        int perGroup = participantCount / groupsCount;
        if (perGroup <= 3) return 1;
        if (perGroup <= 6) return 2;
        return 3;
    }

    /// <summary>
    /// Assigns participants to groups. Uses geographic or seed-based balancing when possible.
    /// Falls back to serpentine draft.
    /// </summary>
    private static List<List<Participant>> AssignGroups(List<Participant> seeded, int groupsCount)
    {
        if (groupsCount == 1)
            return new List<List<Participant>> { new(seeded) };

        var regions = seeded
            .Where(p => !string.IsNullOrEmpty(p.Region))
            .GroupBy(p => p.Region)
            .ToList();

        if (regions.Count >= groupsCount)
        {
            var regionGroups = new List<List<Participant>>();
            for (int i = 0; i < groupsCount; i++)
                regionGroups.Add(new List<Participant>());

            var regionList = regions.OrderByDescending(r => r.Count()).ToList();
            for (int i = 0; i < regionList.Count; i++)
            {
                int targetGroup = i % groupsCount;
                regionGroups[targetGroup].AddRange(regionList[i]);
            }

            bool allNonEmpty = regionGroups.All(g => g.Count > 0);
            if (allNonEmpty)
                return regionGroups;
        }

        return AssignGroupsSerpentine(seeded, groupsCount);
    }

    /// <summary>
    /// Assigns participants to groups using serpentine (snake) draft.
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
    /// Finds the smallest power of 2 greater than or equal to the value.
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
