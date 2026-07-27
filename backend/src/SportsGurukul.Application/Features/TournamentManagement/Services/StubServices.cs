using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.TournamentManagement.Services;

public class StubBracketGenerationService : IBracketGenerationService
{
    public Task<IReadOnlyList<TournamentBracket>> GenerateBracketsAsync(
        Tournament tournament,
        IReadOnlyList<TournamentParticipant> participants,
        IReadOnlyList<TournamentCategory> categories,
        CancellationToken cancellationToken = default)
    {
        var brackets = new List<TournamentBracket>();
        return Task.FromResult<IReadOnlyList<TournamentBracket>>(brackets);
    }
}

public class StubFixtureGenerationService : IFixtureGenerationService
{
    public Task<IReadOnlyList<TournamentFixture>> GenerateFixturesAsync(
        Tournament tournament,
        IReadOnlyList<TournamentParticipant> participants,
        IReadOnlyList<TournamentStage> stages,
        CancellationToken cancellationToken = default)
    {
        var fixtures = new List<TournamentFixture>();
        return Task.FromResult<IReadOnlyList<TournamentFixture>>(fixtures);
    }
}

public class StubSeedingService : ISeedingService
{
    public Task<IReadOnlyList<TournamentSeed>> GenerateSeedsAsync(
        Tournament tournament,
        IReadOnlyList<TournamentParticipant> participants,
        CancellationToken cancellationToken = default)
    {
        var seeds = new List<TournamentSeed>();
        return Task.FromResult<IReadOnlyList<TournamentSeed>>(seeds);
    }
}

public class StubRankingCalculationService : IRankingCalculationService
{
    public Task<IReadOnlyList<TournamentRanking>> CalculateRankingsAsync(
        Tournament tournament,
        IReadOnlyList<TournamentMatch> completedMatches,
        IReadOnlyList<TournamentParticipant> participants,
        CancellationToken cancellationToken = default)
    {
        var rankings = new List<TournamentRanking>();
        return Task.FromResult<IReadOnlyList<TournamentRanking>>(rankings);
    }
}

public class StubScoringService : IScoringService
{
    public Task<TournamentMatch> UpdateScoreAsync(
        TournamentMatch match, int homeScore, int awayScore, string? scoreDetails,
        CancellationToken cancellationToken = default)
    {
        match.HomeScore = homeScore;
        match.AwayScore = awayScore;
        match.ScoreDetails = scoreDetails;
        return Task.FromResult(match);
    }

    public Task<TournamentMatch> RecordWalkoverAsync(
        TournamentMatch match, Guid winnerId, string? notes,
        CancellationToken cancellationToken = default)
    {
        match.Status = Domain.Enums.MatchStatus.Walkover;
        match.WinnerId = winnerId;
        match.Notes = notes;
        return Task.FromResult(match);
    }

    public Task<TournamentMatch> RecordForfeitAsync(
        TournamentMatch match, Guid winnerId, string? notes,
        CancellationToken cancellationToken = default)
    {
        match.Status = Domain.Enums.MatchStatus.Disqualified;
        match.WinnerId = winnerId;
        match.Notes = notes;
        return Task.FromResult(match);
    }

    public Task<TournamentMatch> StartMatchAsync(
        TournamentMatch match, CancellationToken cancellationToken = default)
    {
        match.Status = Domain.Enums.MatchStatus.InProgress;
        return Task.FromResult(match);
    }

    public Task<TournamentMatch> CompleteMatchAsync(
        TournamentMatch match, CancellationToken cancellationToken = default)
    {
        match.Status = Domain.Enums.MatchStatus.Completed;
        return Task.FromResult(match);
    }
}
