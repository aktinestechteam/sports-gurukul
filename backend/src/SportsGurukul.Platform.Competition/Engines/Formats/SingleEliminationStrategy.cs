using SportsGurukul.Platform.Competition.Models;
using SportsGurukul.Platform.Competition.Models.Enums;

namespace SportsGurukul.Platform.Competition.Engines.Formats;

/// <summary>
/// Implements a standard single-elimination bracket format.
/// Participants are seeded into a bracket padded to the next power of 2 with BYEs.
/// Winners advance each round until a champion is determined.
/// Supports an optional third-place match between semi-final losers.
/// </summary>
public class SingleEliminationStrategy : IFormatStrategy
{
    /// <inheritdoc />
    public CompetitionFormat Format => CompetitionFormat.SingleElimination;

    /// <inheritdoc />
    public async Task<IReadOnlyList<CompetitionMatch>> GenerateMatchesAsync(
        IReadOnlyList<Participant> participants,
        CompetitionConfig config,
        CancellationToken cancellationToken = default)
    {
        if (participants.Count == 0)
            return Array.Empty<CompetitionMatch>();

        var seededParticipants = SeedParticipants(participants, config.SeedingStrategy);
        var bracketSize = NextPowerOfTwo(seededParticipants.Count);

        var bracket = BuildSeededBracket(seededParticipants, bracketSize);
        var matches = new List<CompetitionMatch>();
        int matchNumber = 1;

        int totalRounds = (int)Math.Log2(bracketSize);

        for (int round = 1; round <= totalRounds; round++)
        {
            int matchesInRound = bracketSize >> round;
            var roundType = GetRoundType(round, totalRounds, config.HasThirdPlaceMatch);

            for (int i = 0; i < matchesInRound; i++)
            {
                int homePos = i * (1 << round);
                int awayPos = homePos + (1 << (round - 1));

                var seed1 = bracket[homePos];
                var seed2 = bracket[awayPos];

                if (seed1 == null && seed2 == null)
                    continue;

                var match = new CompetitionMatch
                {
                    Id = Guid.NewGuid(),
                    MatchNumber = matchNumber++,
                    RoundNumber = round,
                    RoundType = roundType,
                    BracketType = BracketType.Main,
                    Status = MatchStatus.Scheduled
                };

                if (seed1 != null)
                {
                    match.HomeParticipantId = seed1.Id;
                    match.HomeParticipantName = seed1.Name;
                }

                if (seed2 != null)
                {
                    match.AwayParticipantId = seed2.Id;
                    match.AwayParticipantName = seed2.Name;
                }

                if (match.IsBye)
                {
                    var advancing = seed1 ?? seed2!;
                    match.WinnerId = advancing.Id;
                    match.WinnerName = advancing.Name;
                    match.Status = MatchStatus.Completed;
                    match.WinnerAdvancementReason = AdvancementReason.Bye;
                    match.ScoreDetails = "BYE";
                }

                matches.Add(match);
            }
        }

        if (config.HasThirdPlaceMatch && seededParticipants.Count >= 3)
        {
            matches.Add(new CompetitionMatch
            {
                Id = Guid.NewGuid(),
                MatchNumber = matchNumber,
                RoundNumber = totalRounds,
                RoundType = RoundType.ThirdPlace,
                BracketType = BracketType.ThirdPlace,
                Status = MatchStatus.Scheduled
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

        var currentRound = existingMatches.Max(m => m.RoundNumber);
        var currentRoundMatches = existingMatches
            .Where(m => m.RoundNumber == currentRound && m.BracketType == BracketType.Main)
            .ToList();

        bool currentRoundComplete = currentRoundMatches.All(m => m.IsCompleted);
        if (!currentRoundComplete)
            return Array.Empty<CompetitionMatch>();

        var maxBracketRounds = existingMatches
            .Where(m => m.BracketType == BracketType.Main)
            .Max(m => m.RoundNumber);

        if (currentRound >= maxBracketRounds)
        {
            bool hasThirdPlace = existingMatches.Any(m => m.BracketType == BracketType.ThirdPlace);
            if (hasThirdPlace && currentRoundMatches.Count == 1)
            {
                var thirdPlaceMatch = existingMatches.First(m => m.BracketType == BracketType.ThirdPlace);
                if (thirdPlaceMatch.Status == MatchStatus.Scheduled &&
                    thirdPlaceMatch.HomeParticipantId == null)
                {
                    return await GenerateThirdPlaceMatch(currentRoundMatches, existingMatches, config);
                }
            }

            return Array.Empty<CompetitionMatch>();
        }

        var winners = currentRoundMatches
            .Where(m => m.WinnerId.HasValue)
            .Select(m => new { m.WinnerId, m.WinnerName, m.MatchNumber })
            .ToList();

        int nextRound = currentRound + 1;
        int matchNumber = existingMatches.Max(m => m.MatchNumber) + 1;
        var newMatches = new List<CompetitionMatch>();

        for (int i = 0; i < winners.Count; i += 2)
        {
            var roundType = GetRoundType(nextRound, maxBracketRounds, config.HasThirdPlaceMatch);
            var match = new CompetitionMatch
            {
                Id = Guid.NewGuid(),
                MatchNumber = matchNumber++,
                RoundNumber = nextRound,
                RoundType = roundType,
                BracketType = BracketType.Main,
                Status = MatchStatus.Scheduled
            };

            if (i < winners.Count)
            {
                match.HomeParticipantId = winners[i].WinnerId;
                match.HomeParticipantName = winners[i].WinnerName;
            }

            if (i + 1 < winners.Count)
            {
                match.AwayParticipantId = winners[i + 1].WinnerId;
                match.AwayParticipantName = winners[i + 1].WinnerName;
            }

            newMatches.Add(match);
        }

        if (config.HasThirdPlaceMatch && currentRound == maxBracketRounds - 1)
        {
            var sfLosers = GetSemiFinalLosers(currentRoundMatches, existingMatches);
            if (sfLosers.Count == 2)
            {
                var tpMatch = existingMatches.FirstOrDefault(m => m.BracketType == BracketType.ThirdPlace);
                if (tpMatch != null)
                {
                    tpMatch.HomeParticipantId = sfLosers[0].Id;
                    tpMatch.HomeParticipantName = sfLosers[0].Name;
                    tpMatch.AwayParticipantId = sfLosers[1].Id;
                    tpMatch.AwayParticipantName = sfLosers[1].Name;
                }
            }
        }

        return await Task.FromResult<IReadOnlyList<CompetitionMatch>>(newMatches);
    }

    /// <inheritdoc />
    public bool IsComplete(IReadOnlyList<CompetitionMatch> matches)
    {
        if (matches.Count == 0)
            return false;

        var mainMatches = matches.Where(m => m.BracketType == BracketType.Main).ToList();
        if (mainMatches.Count == 0)
            return false;

        var finalMatch = mainMatches
            .OrderByDescending(m => m.RoundNumber)
            .ThenByDescending(m => m.MatchNumber)
            .FirstOrDefault();

        if (finalMatch == null || !finalMatch.IsCompleted)
            return false;

        bool hasThirdPlace = matches.Any(m => m.BracketType == BracketType.ThirdPlace);
        if (hasThirdPlace)
        {
            var thirdPlace = matches.First(m => m.BracketType == BracketType.ThirdPlace);
            return thirdPlace.IsCompleted;
        }

        return true;
    }

    private async Task<IReadOnlyList<CompetitionMatch>> GenerateThirdPlaceMatch(
        List<CompetitionMatch> finalRoundMatches,
        IReadOnlyList<CompetitionMatch> allMatches,
        CompetitionConfig config)
    {
        var losers = GetSemiFinalLosers(finalRoundMatches, allMatches);
        if (losers.Count != 2)
            return Array.Empty<CompetitionMatch>();

        var tpMatch = allMatches.FirstOrDefault(m => m.BracketType == BracketType.ThirdPlace);
        if (tpMatch == null)
            return Array.Empty<CompetitionMatch>();

        tpMatch.HomeParticipantId = losers[0].Id;
        tpMatch.HomeParticipantName = losers[0].Name;
        tpMatch.AwayParticipantId = losers[1].Id;
        tpMatch.AwayParticipantName = losers[1].Name;

        return await Task.FromResult<IReadOnlyList<CompetitionMatch>>(new List<CompetitionMatch> { tpMatch });
    }

    private List<Participant> GetSemiFinalLosers(
        List<CompetitionMatch> finalRoundMatches,
        IReadOnlyList<CompetitionMatch> allMatches)
    {
        var losers = new List<Participant>();
        int semiFinalRound = finalRoundMatches.First().RoundNumber - 1;

        var semiFinalMatches = allMatches
            .Where(m => m.RoundNumber == semiFinalRound && m.BracketType == BracketType.Main && m.IsCompleted)
            .ToList();

        foreach (var match in semiFinalMatches)
        {
            if (match.WinnerId.HasValue && match.HomeParticipantId.HasValue && match.AwayParticipantId.HasValue)
            {
                Guid loserId = match.WinnerId.Value == match.HomeParticipantId.Value
                    ? match.AwayParticipantId.Value
                    : match.HomeParticipantId.Value;
                string loserName = match.WinnerId.Value == match.HomeParticipantId.Value
                    ? match.AwayParticipantName ?? "Unknown"
                    : match.HomeParticipantName ?? "Unknown";

                losers.Add(new Participant { Id = loserId, Name = loserName });
            }
        }

        return losers;
    }

    private static RoundType GetRoundType(int round, int totalRounds, bool hasThirdPlace)
    {
        if (round == totalRounds) return RoundType.Final;
        if (round == totalRounds - 1) return RoundType.SemiFinal;
        return RoundType.KnockoutRound;
    }

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

    private static List<Participant?> BuildSeededBracket(List<Participant> seeded, int bracketSize)
    {
        var bracket = new Participant?[bracketSize];

        var positions = new int[bracketSize];
        positions[0] = 0;

        for (int size = 2; size <= bracketSize; size *= 2)
        {
            int half = size / 2;
            for (int i = 0; i < half; i++)
            {
                positions[half + i] = size - 1 - positions[i];
            }
        }

        for (int i = 0; i < seeded.Count; i++)
        {
            bracket[positions[i]] = seeded[i];
        }

        return bracket.ToList();
    }

    private static int NextPowerOfTwo(int value)
    {
        if (value <= 1) return 1;
        int power = 1;
        while (power < value)
            power *= 2;
        return power;
    }
}
