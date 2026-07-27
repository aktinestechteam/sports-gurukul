using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.Competition.Interfaces;
using SportsGurukul.Platform.Competition.Models;
using SportsGurukul.Platform.Competition.Models.Enums;

namespace SportsGurukul.Platform.Competition.Services;

public class AdvancementService : IAdvancementService
{
    private readonly ILogger<AdvancementService> _logger;

    public AdvancementService(ILogger<AdvancementService> logger)
    {
        _logger = logger;
    }

    public Task<IReadOnlyList<CompetitionMatch>> AdvanceWinnerAsync(
        CompetitionMatch completedMatch,
        IReadOnlyList<CompetitionMatch> allMatches,
        CancellationToken cancellationToken = default)
    {
        if (completedMatch.WinnerId is null)
            return Task.FromResult<IReadOnlyList<CompetitionMatch>>(allMatches);

        _logger.LogInformation("Advancing winner {WinnerId} from match {MatchId}",
            completedMatch.WinnerId, completedMatch.Id);

        var nextMatchNumber = GetNextMatchNumber(completedMatch);
        var targetSlot = GetTargetSlot(completedMatch);

        var nextMatch = allMatches.FirstOrDefault(m =>
            m.MatchNumber == nextMatchNumber &&
            m.RoundNumber == completedMatch.RoundNumber + 1);

        if (nextMatch is not null)
        {
            if (targetSlot == "home")
                nextMatch.HomeParticipantId = completedMatch.WinnerId;
            else
                nextMatch.AwayParticipantId = completedMatch.WinnerId;
        }

        return Task.FromResult<IReadOnlyList<CompetitionMatch>>(allMatches);
    }

    private static int GetNextMatchNumber(CompetitionMatch match)
    {
        int matchIndex = match.MatchNumber - 1;
        int roundSize = GetRoundSize(match.RoundNumber, match.RoundType);

        for (int prevRoundSize = 1; prevRoundSize < roundSize; prevRoundSize *= 2)
        {
            if (matchIndex < roundSize) break;
            matchIndex -= roundSize;
        }

        return (matchIndex / 2) + 1;
    }

    private static string GetTargetSlot(CompetitionMatch match)
    {
        return match.MatchNumber % 2 == 1 ? "home" : "away";
    }

    private static int GetRoundSize(int roundNumber, RoundType roundType)
    {
        return roundType switch
        {
            RoundType.KnockoutRound => (int)Math.Pow(2, Math.Max(1, roundNumber)),
            _ => 16
        };
    }
}
