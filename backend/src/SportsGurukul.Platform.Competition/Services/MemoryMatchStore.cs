using System.Collections.Concurrent;

namespace SportsGurukul.Platform.Competition.Services;

public class MemoryMatchStore
{
    private readonly ConcurrentDictionary<Guid, Models.LiveMatch> _matches = new();
    private readonly ConcurrentDictionary<Guid, List<Models.LiveMatch>> _tournamentMatches = new();

    public Models.LiveMatch? Get(Guid matchId) =>
        _matches.TryGetValue(matchId, out var match) ? match : null;

    public void Set(Models.LiveMatch match)
    {
        _matches[match.Id] = match;
        _tournamentMatches.AddOrUpdate(
            match.TournamentId,
            _ => new List<Models.LiveMatch> { match },
            (_, list) =>
            {
                lock (list)
                {
                    var existing = list.FirstOrDefault(m => m.Id == match.Id);
                    if (existing != null) list.Remove(existing);
                    list.Add(match);
                }
                return list;
            });
    }

    public IReadOnlyList<Models.LiveMatch> GetByTournament(Guid tournamentId) =>
        _tournamentMatches.TryGetValue(tournamentId, out var matches)
            ? matches.ToArray()
            : Array.Empty<Models.LiveMatch>();

    public bool Remove(Guid matchId)
    {
        if (!_matches.TryRemove(matchId, out var match)) return false;
        if (_tournamentMatches.TryGetValue(match.TournamentId, out var list))
            lock (list) { list.RemoveAll(m => m.Id == matchId); }
        return true;
    }

    public void RemoveByTournament(Guid tournamentId)
    {
        if (_tournamentMatches.TryRemove(tournamentId, out var matches))
            foreach (var match in matches)
                _matches.TryRemove(match.Id, out _);
    }
}
