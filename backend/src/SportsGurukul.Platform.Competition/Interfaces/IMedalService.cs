using SportsGurukul.Platform.Competition.Models;

namespace SportsGurukul.Platform.Competition.Interfaces;

public interface IMedalService
{
    Task<MedalTable> GenerateMedalTableAsync(Guid tournamentId, CancellationToken cancellationToken = default);
    Task<MedalEntry?> GetParticipantMedalsAsync(Guid tournamentId, Guid participantId, CancellationToken cancellationToken = default);
    Task AwardMedalAsync(Guid tournamentId, Guid participantId, string participantName, string eventName, string sportCode, Models.Enums.MedalType medalType, CancellationToken cancellationToken = default);
}
