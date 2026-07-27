using SportsGurukul.Platform.Competition.Models;
using SportsGurukul.Platform.Competition.Models.Enums;

namespace SportsGurukul.Platform.Competition.Engines.Formats;

/// <summary>
/// Implements a double-elimination tournament format.
/// Participants must lose twice to be eliminated.
/// Consists of a winners bracket, losers bracket, and grand final.
/// If the losers bracket champion beats the winners bracket champion, a bracket reset match is played.
/// </summary>
public class DoubleEliminationStrategy : IFormatStrategy
{
    /// <inheritdoc />
    public CompetitionFormat Format => CompetitionFormat.DoubleElimination;

    /// <inheritdoc />
    public async Task<IReadOnlyList<CompetitionMatch>> GenerateMatchesAsync(
        IReadOnlyList<Participant> participants,
        CompetitionConfig config,
        CancellationToken cancellationToken = default)
    {
        if (participants.Count == 0)
            return Array.Empty<CompetitionMatch>();

        var seeded = SeedParticipants(participants, config.SeedingStrategy);
        var bracketSize = NextPowerOfTwo(seeded.Count);
        var matches = new List<CompetitionMatch>();
        int matchNumber = 1;

        int winnersRounds = (int)Math.Log2(bracketSize);
        int losersRounds = (winnersRounds - 1) * 2;

        var winnersBracket = new List<CompetitionMatch>();
        var losersBracket = new List<CompetitionMatch>();

        var currentWinners = new List<Participant?>(seeded);
        while (currentWinners.Count < bracketSize)
            currentWinners.Add(null);

        for (int round = 1; round <= winnersRounds; round++)
        {
            int matchesInRound = currentWinners.Count / 2;
            var nextWinners = new List<Participant?>();

            for (int i = 0; i < matchesInRound; i++)
            {
                var p1 = currentWinners[i * 2];
                var p2 = currentWinners[i * 2 + 1];

                if (p1 == null && p2 == null)
                {
                    nextWinners.Add(null);
                    continue;
                }

                var match = new CompetitionMatch
                {
                    Id = Guid.NewGuid(),
                    MatchNumber = matchNumber++,
                    RoundNumber = round,
                    RoundType = round == winnersRounds ? RoundType.Final :
                                round == winnersRounds - 1 ? RoundType.SemiFinal :
                                RoundType.KnockoutRound,
                    BracketType = BracketType.Winners,
                    Status = MatchStatus.Scheduled
                };

                if (p1 != null)
                {
                    match.HomeParticipantId = p1.Id;
                    match.HomeParticipantName = p1.Name;
                }
                if (p2 != null)
                {
                    match.AwayParticipantId = p2.Id;
                    match.AwayParticipantName = p2.Name;
                }

                if (match.IsBye)
                {
                    var advancing = p1 ?? p2!;
                    match.WinnerId = advancing.Id;
                    match.WinnerName = advancing.Name;
                    match.Status = MatchStatus.Completed;
                    match.WinnerAdvancementReason = AdvancementReason.Bye;
                    match.ScoreDetails = "BYE";
                    nextWinners.Add(advancing);
                }
                else
                {
                    nextWinners.Add(null);
                }

                winnersBracket.Add(match);
                matches.Add(match);
            }

            currentWinners = nextWinners;
        }

        int currentLosersRound = 1;
        var droppedFromWinners = new List<Participant?>();

        for (int wr = 1; wr <= winnersRounds; wr++)
        {
            var winnersRoundMatches = winnersBracket
                .Where(m => m.RoundNumber == wr)
                .ToList();

            foreach (var wm in winnersRoundMatches)
            {
                if (wm.Status == MatchStatus.Completed && wm.HomeParticipantId.HasValue && wm.AwayParticipantId.HasValue)
                {
                    var loserId = wm.WinnerId == wm.HomeParticipantId ? wm.AwayParticipantId : wm.HomeParticipantId;
                    var loserName = wm.WinnerId == wm.HomeParticipantId ? wm.AwayParticipantName : wm.HomeParticipantName;

                    if (loserId.HasValue && wr < winnersRounds)
                    {
                        droppedFromWinners.Add(new Participant
                        {
                            Id = loserId.Value,
                            Name = loserName ?? "Unknown"
                        });
                    }
                }
            }
        }

        int losersBracketSize = bracketSize / 2;
        var losersParticipants = new List<Participant?>();

        for (int wr = 1; wr < winnersRounds; wr++)
        {
            var roundMatches = winnersBracket.Where(m => m.RoundNumber == wr && !m.IsBye).ToList();
            int losersMatchesThisRound = roundMatches.Count;

            for (int i = 0; i < losersMatchesThisRound; i++)
            {
                losersParticipants.Add(null);
            }
        }

        for (int lr = 1; lr <= losersRounds; lr++)
        {
            int matchesInLoserRound = lr <= losersRounds / 2
                ? losersBracketSize >> ((lr - 1) / 2)
                : losersBracketSize >> ((lr - 1) / 2);

            if (lr % 2 == 0 && lr > 1)
                matchesInLoserRound = Math.Max(1, losersBracketSize >> ((lr - 2) / 2));

            matchesInLoserRound = Math.Max(1, matchesInLoserRound);

            bool isEliminationRound = lr % 2 == 1;
            var roundType = isEliminationRound ? RoundType.KnockoutRound : RoundType.KnockoutRound;

            for (int i = 0; i < matchesInLoserRound; i++)
            {
                var match = new CompetitionMatch
                {
                    Id = Guid.NewGuid(),
                    MatchNumber = matchNumber++,
                    RoundNumber = lr,
                    RoundType = lr == losersRounds ? RoundType.Final : RoundType.KnockoutRound,
                    BracketType = BracketType.Losers,
                    Status = MatchStatus.Scheduled
                };

                losersBracket.Add(match);
                matches.Add(match);
            }

            currentLosersRound++;
        }

        var grandFinal = new CompetitionMatch
        {
            Id = Guid.NewGuid(),
            MatchNumber = matchNumber++,
            RoundNumber = 1,
            RoundType = RoundType.Final,
            BracketType = BracketType.GrandFinal,
            Status = MatchStatus.Scheduled
        };
        matches.Add(grandFinal);

        var bracketReset = new CompetitionMatch
        {
            Id = Guid.NewGuid(),
            MatchNumber = matchNumber++,
            RoundNumber = 2,
            RoundType = RoundType.Final,
            BracketType = BracketType.GrandFinal,
            Notes = "Bracket Reset (only if losers bracket champion wins Grand Final)",
            Status = MatchStatus.Scheduled
        };
        matches.Add(bracketReset);

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

        var newMatches = new List<CompetitionMatch>();
        int matchNumber = existingMatches.Max(m => m.MatchNumber) + 1;

        var winnersMatches = existingMatches.Where(m => m.BracketType == BracketType.Winners).ToList();
        var losersMatches = existingMatches.Where(m => m.BracketType == BracketType.Losers).ToList();
        var grandFinals = existingMatches.Where(m => m.BracketType == BracketType.GrandFinal).ToList();

        var scheduledLosers = losersMatches.Where(m => m.Status == MatchStatus.Scheduled && !m.IsBye).ToList();
        var completedLosers = losersMatches.Where(m => m.IsCompleted && !m.IsBye).ToList();

        var scheduledWinners = winnersMatches.Where(m => m.Status == MatchStatus.Scheduled && !m.IsBye).ToList();

        bool allLosersComplete = scheduledLosers.Count == 0 && completedLosers.Count > 0;

        if (!allLosersComplete && losersMatches.Count > 0)
        {
            var losersRoundGroups = losersMatches
                .GroupBy(m => m.RoundNumber)
                .OrderBy(g => g.Key)
                .ToList();

            foreach (var roundGroup in losersRoundGroups)
            {
                var roundLosers = roundGroup.ToList();
                bool roundComplete = roundLosers.All(m => m.IsCompleted || m.IsBye);

                if (!roundComplete)
                    continue;

                var winnersLosersRound = winnersMatches
                    .Where(m => m.IsCompleted && m.RoundNumber <= roundGroup.Key && !m.IsBye)
                    .ToList();

                var droppedLosers = new List<(Guid Id, string Name)>();
                foreach (var wm in winnersLosersRound.Where(m => m.RoundNumber == roundGroup.Key))
                {
                    if (wm.HomeParticipantId.HasValue && wm.AwayParticipantId.HasValue && wm.WinnerId.HasValue)
                    {
                        var loserId = wm.WinnerId.Value == wm.HomeParticipantId.Value
                            ? wm.AwayParticipantId.Value
                            : wm.HomeParticipantId.Value;
                        var loserName = wm.WinnerId.Value == wm.HomeParticipantId.Value
                            ? wm.AwayParticipantName ?? "Unknown"
                            : wm.HomeParticipantName ?? "Unknown";
                        droppedLosers.Add((loserId, loserName));
                    }
                }

                var availableLosers = roundLosers
                    .Where(m => m.WinnerId.HasValue)
                    .Select(m => (m.WinnerId!.Value, m.WinnerName ?? "Unknown"))
                    .ToList();

                availableLosers.AddRange(droppedLosers);

                var nextLoserRound = losersRoundGroups
                    .FirstOrDefault(g => g.Key > roundGroup.Key);

                if (nextLoserRound != null)
                {
                    var nextRoundMatches = nextLoserRound.ToList()
                        .Where(m => m.Status == MatchStatus.Scheduled && m.HomeParticipantId == null)
                        .ToList();

                    int idx = 0;
                    foreach (var match in nextRoundMatches)
                    {
                        if (idx < availableLosers.Count)
                        {
                            match.HomeParticipantId = availableLosers[idx].Item1;
                            match.HomeParticipantName = availableLosers[idx].Item2;
                            idx++;
                        }
                        if (idx < availableLosers.Count)
                        {
                            match.AwayParticipantId = availableLosers[idx].Item1;
                            match.AwayParticipantName = availableLosers[idx].Item2;
                            idx++;
                        }
                    }
                }
            }
        }

        var winnersBracketComplete = winnersMatches.All(m => m.IsCompleted);
        var losersBracketComplete = losersMatches.All(m => m.IsCompleted || m.IsBye);

        if (winnersBracketComplete && losersBracketComplete)
        {
            var winnersChamp = winnersMatches
                .OrderByDescending(m => m.RoundNumber)
                .FirstOrDefault(m => m.WinnerId.HasValue);

            var losersChamp = losersMatches
                .OrderByDescending(m => m.RoundNumber)
                .FirstOrDefault(m => m.WinnerId.HasValue);

            var grandFinal = grandFinals.FirstOrDefault(m => m.Status == MatchStatus.Scheduled);
            if (grandFinal != null && winnersChamp?.WinnerId.HasValue == true && losersChamp?.WinnerId.HasValue == true)
            {
                grandFinal.HomeParticipantId = winnersChamp.WinnerId;
                grandFinal.HomeParticipantName = winnersChamp.WinnerName;
                grandFinal.AwayParticipantId = losersChamp.WinnerId;
                grandFinal.AwayParticipantName = losersChamp.WinnerName;
                newMatches.Add(grandFinal);
            }
        }

        return await Task.FromResult<IReadOnlyList<CompetitionMatch>>(newMatches);
    }

    /// <inheritdoc />
    public bool IsComplete(IReadOnlyList<CompetitionMatch> matches)
    {
        if (matches.Count == 0) return false;

        var grandFinals = matches.Where(m => m.BracketType == BracketType.GrandFinal).ToList();
        if (grandFinals.Count == 0) return false;

        var firstGrandFinal = grandFinals.FirstOrDefault(m => m.RoundNumber == 1);
        if (firstGrandFinal == null || !firstGrandFinal.IsCompleted)
            return false;

        var winnersBracket = matches.Where(m => m.BracketType == BracketType.Winners).ToList();
        if (winnersBracket.Count == 0) return false;

        var winnersChampMatch = winnersBracket
            .OrderByDescending(m => m.RoundNumber)
            .FirstOrDefault();

        if (winnersChampMatch?.WinnerId == firstGrandFinal.WinnerId)
            return true;

        var bracketReset = grandFinals.FirstOrDefault(m => m.RoundNumber == 2);
        return bracketReset != null && bracketReset.IsCompleted;
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

    /// <summary>
    /// Computes the next power of 2 greater than or equal to the given value.
    /// </summary>
    private static int NextPowerOfTwo(int value)
    {
        if (value <= 1) return 1;
        int power = 1;
        while (power < value)
            power *= 2;
        return power;
    }
}
