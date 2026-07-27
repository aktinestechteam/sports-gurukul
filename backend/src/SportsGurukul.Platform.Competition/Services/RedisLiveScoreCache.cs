using System.Collections.Concurrent;
using SportsGurukul.Platform.Competition.Models;
using SportsGurukul.Platform.Competition.Models.Enums;

namespace SportsGurukul.Platform.Competition.Services;

public class RedisLiveScoreCache : Interfaces.ILiveScoreCache
{
    private readonly ConcurrentDictionary<string, object> _cache = new();

    public Task<LiveMatch?> GetLiveMatchAsync(Guid matchId, CancellationToken cancellationToken = default)
    {
        var key = $"live:{matchId}";
        return Task.FromResult(_cache.TryGetValue(key, out var val) ? val as LiveMatch : null);
    }

    public Task SetLiveMatchAsync(LiveMatch match, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        _cache[$"live:{match.Id}"] = match;
        return Task.CompletedTask;
    }

    public Task RemoveLiveMatchAsync(Guid matchId, CancellationToken cancellationToken = default)
    {
        _cache.TryRemove($"live:{matchId}", out _);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<LiveMatch>> GetTournamentLiveMatchesAsync(Guid tournamentId, CancellationToken cancellationToken = default)
    {
        var key = $"tournament:{tournamentId}:live";
        var result = _cache.TryGetValue(key, out var val) && val is List<LiveMatch> list
            ? (IReadOnlyList<LiveMatch>)list
            : Array.Empty<LiveMatch>();
        return Task.FromResult(result);
    }

    public Task SetTournamentLiveMatchesAsync(Guid tournamentId, IReadOnlyList<LiveMatch> matches, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        _cache[$"tournament:{tournamentId}:live"] = matches.ToList();
        return Task.CompletedTask;
    }

    public Task<Leaderboard?> GetLeaderboardAsync(Guid tournamentId, LeaderboardType type, string? sportCode, CancellationToken cancellationToken = default)
    {
        var key = $"leaderboard:{tournamentId}:{type}:{sportCode}";
        return Task.FromResult(_cache.TryGetValue(key, out var val) ? val as Leaderboard : null);
    }

    public Task SetLeaderboardAsync(Guid tournamentId, LeaderboardType type, string? sportCode, Leaderboard leaderboard, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        _cache[$"leaderboard:{tournamentId}:{type}:{sportCode}"] = leaderboard;
        return Task.CompletedTask;
    }

    public Task<StandingsEntry[]?> GetStandingsAsync(Guid tournamentId, CancellationToken cancellationToken = default)
    {
        var key = $"standings:{tournamentId}";
        return Task.FromResult(_cache.TryGetValue(key, out var val) ? val as StandingsEntry[] : null);
    }

    public Task SetStandingsAsync(Guid tournamentId, StandingsEntry[] standings, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        _cache[$"standings:{tournamentId}"] = standings;
        return Task.CompletedTask;
    }

    public Task RemoveTournamentCacheAsync(Guid tournamentId, CancellationToken cancellationToken = default)
    {
        var keysToRemove = _cache.Keys.Where(k => k.Contains(tournamentId.ToString())).ToList();
        foreach (var key in keysToRemove) _cache.TryRemove(key, out _);
        return Task.CompletedTask;
    }
}
