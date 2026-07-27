using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.Competition.Interfaces;
using SportsGurukul.Platform.Competition.Models;
using SportsGurukul.Platform.Competition.Models.Enums;

namespace SportsGurukul.Platform.Competition.Services;

public class MedalService : IMedalService
{
    private readonly ConcurrentDictionary<Guid, List<MedalEntry>> _medals = new();
    private readonly ILogger<MedalService> _logger;

    public MedalService(ILogger<MedalService> logger)
    {
        _logger = logger;
    }

    public Task AwardMedalAsync(Guid tournamentId, Guid participantId, string participantName, string eventName, string sportCode, MedalType medalType, CancellationToken cancellationToken = default)
    {
        var entries = _medals.GetOrAdd(tournamentId, _ => new List<MedalEntry>());

        lock (entries)
        {
            var entry = entries.FirstOrDefault(e => e.ParticipantId == participantId);
            if (entry == null)
            {
                entry = new MedalEntry { ParticipantId = participantId, ParticipantName = participantName };
                entries.Add(entry);
            }

            entry.Medals.Add(new MedalDetail
            {
                Type = medalType,
                EventName = eventName,
                SportCode = sportCode,
                AchievedAt = DateTime.UtcNow
            });

            entry.TotalMedals = entry.Medals.Count;
            entry.GoldCount = entry.Medals.Count(m => m.Type == MedalType.Gold);
            entry.SilverCount = entry.Medals.Count(m => m.Type == MedalType.Silver);
            entry.BronzeCount = entry.Medals.Count(m => m.Type == MedalType.Bronze);
            entry.TotalPoints = entry.GoldCount * 3 + entry.SilverCount * 2 + entry.BronzeCount * 1;
        }

        _logger.LogInformation("Medal {Type} awarded to {Participant} in tournament {TournamentId}", medalType, participantName, tournamentId);
        return Task.CompletedTask;
    }

    public Task<MedalEntry?> GetParticipantMedalsAsync(Guid tournamentId, Guid participantId, CancellationToken cancellationToken = default)
    {
        var entries = _medals.TryGetValue(tournamentId, out var list) ? list : new List<MedalEntry>();
        return Task.FromResult(entries.FirstOrDefault(e => e.ParticipantId == participantId));
    }

    public Task<MedalTable> GenerateMedalTableAsync(Guid tournamentId, CancellationToken cancellationToken = default)
    {
        var entries = _medals.TryGetValue(tournamentId, out var list) ? list : new List<MedalEntry>();
        var sorted = entries.OrderByDescending(e => e.GoldCount).ThenByDescending(e => e.SilverCount).ThenByDescending(e => e.BronzeCount).ToList();

        var table = new MedalTable
        {
            TournamentId = tournamentId,
            Entries = sorted,
            GeneratedAt = DateTime.UtcNow
        };

        return Task.FromResult(table);
    }
}
