using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.Competition.Interfaces;
using SportsGurukul.Platform.Competition.Models;
using SportsGurukul.Platform.Competition.Models.Enums;

namespace SportsGurukul.Platform.Competition.Services;

public class LiveScoringService : ILiveScoringService
{
    private readonly MemoryMatchStore _store;
    private readonly IEnumerable<ISportRuleProvider> _sportProviders;
    private readonly ILiveUpdatePublisher _publisher;
    private readonly ILogger<LiveScoringService> _logger;

    public LiveScoringService(
        MemoryMatchStore store,
        IEnumerable<ISportRuleProvider> sportProviders,
        ILiveUpdatePublisher publisher,
        ILogger<LiveScoringService> logger)
    {
        _store = store;
        _sportProviders = sportProviders;
        _publisher = publisher;
        _logger = logger;
    }

    public Task<LiveMatch> StartMatchAsync(Guid tournamentId, Guid matchId, string sportCode, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting live match {MatchId} for tournament {TournamentId} ({Sport})", matchId, tournamentId, sportCode);

        var provider = _sportProviders.FirstOrDefault(p => p.SportCode.Equals(sportCode, StringComparison.OrdinalIgnoreCase))
            ?? throw new ArgumentException($"No sport rule provider found for sport code: {sportCode}");

        var match = new LiveMatch
        {
            Id = Guid.NewGuid(),
            TournamentId = tournamentId,
            MatchId = matchId,
            SportCode = sportCode,
            Status = LiveMatchStatus.Live,
            HomeScore = provider.CreateEmptyScore(),
            AwayScore = provider.CreateEmptyScore(),
            StartedAt = DateTime.UtcNow,
            Version = 1
        };

        _store.Set(match);
        _ = _publisher.PublishScoreUpdateAsync(match, cancellationToken);

        return Task.FromResult(match);
    }

    public async Task<LiveMatch> UpdateScoreAsync(Guid matchId, Guid participantId, int points, ScoringUnit unit, int periodNumber, string? description, CancellationToken cancellationToken = default)
    {
        var match = _store.Get(matchId) ?? throw new ArgumentException($"Live match not found: {matchId}");
        if (match.Status != LiveMatchStatus.Live)
            throw new InvalidOperationException($"Cannot update score for match in status {match.Status}");

        var provider = _sportProviders.FirstOrDefault(p => p.SportCode.Equals(match.SportCode, StringComparison.OrdinalIgnoreCase))
            ?? throw new ArgumentException($"No sport rule provider found for: {match.SportCode}");

        var eventName = match.HomeParticipantId == participantId ? match.HomeParticipantName : match.AwayParticipantName;

        var scoreEvent = new LiveScoreEvent
        {
            Id = Guid.NewGuid(),
            MatchId = matchId,
            ParticipantId = participantId,
            ParticipantName = eventName,
            Unit = unit,
            Points = points,
            PeriodNumber = periodNumber,
            Description = description,
            Timestamp = DateTime.UtcNow
        };

        match.ScoreEvents.Add(scoreEvent);

        if (match.HomeParticipantId == participantId)
        {
            match.HomeScore.TotalPoints = provider.CalculateScore(scoreEvent, match.HomeScore);
            var homePS = match.ParticipantScores.FirstOrDefault(p => p.ParticipantId == participantId);
            if (homePS != null) homePS.TotalPoints = match.HomeScore.TotalPoints;
        }
        else
        {
            match.AwayScore.TotalPoints = provider.CalculateScore(scoreEvent, match.AwayScore);
            var awayPS = match.ParticipantScores.FirstOrDefault(p => p.ParticipantId == participantId);
            if (awayPS != null) awayPS.TotalPoints = match.AwayScore.TotalPoints;
        }

        match.Version++;
        _store.Set(match);
        await _publisher.PublishScoreUpdateAsync(match, cancellationToken);

        _logger.LogInformation("Score updated for match {MatchId}: {Points} by {Participant}", matchId, points, eventName);
        return match;
    }

    public async Task<LiveMatch> UndoLastScoreAsync(Guid matchId, CancellationToken cancellationToken = default)
    {
        var match = _store.Get(matchId) ?? throw new ArgumentException($"Live match not found: {matchId}");

        var lastEvent = match.ScoreEvents.LastOrDefault(e => !e.IsUndo);
        if (lastEvent == null)
            throw new InvalidOperationException("No score events to undo");

        var undoEvent = new LiveScoreEvent
        {
            Id = Guid.NewGuid(),
            MatchId = matchId,
            ParticipantId = lastEvent.ParticipantId,
            ParticipantName = lastEvent.ParticipantName,
            Unit = lastEvent.Unit,
            Points = -lastEvent.Points,
            PeriodNumber = lastEvent.PeriodNumber,
            Description = $"Undo: {lastEvent.Description}",
            Timestamp = DateTime.UtcNow,
            IsUndo = true,
            UndoEventId = lastEvent.Id
        };

        match.ScoreEvents.Add(undoEvent);

        if (match.HomeParticipantId == lastEvent.ParticipantId)
            match.HomeScore.TotalPoints -= lastEvent.Points;
        else
            match.AwayScore.TotalPoints -= lastEvent.Points;

        match.Version++;
        _store.Set(match);
        await _publisher.PublishScoreUpdateAsync(match, cancellationToken);

        return match;
    }

    public Task<LiveMatch?> GetLiveMatchAsync(Guid matchId, CancellationToken cancellationToken = default) =>
        Task.FromResult(_store.Get(matchId));

    public Task<IReadOnlyList<LiveMatch>> GetLiveMatchesByTournamentAsync(Guid tournamentId, CancellationToken cancellationToken = default) =>
        Task.FromResult(_store.GetByTournament(tournamentId));
}
