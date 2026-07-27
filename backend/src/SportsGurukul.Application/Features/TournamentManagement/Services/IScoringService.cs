using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.TournamentManagement.Services;

public interface IScoringService
{
    Task<TournamentMatch> UpdateScoreAsync(
        TournamentMatch match,
        int homeScore,
        int awayScore,
        string? scoreDetails,
        CancellationToken cancellationToken = default);

    Task<TournamentMatch> RecordWalkoverAsync(
        TournamentMatch match,
        Guid winnerId,
        string? notes,
        CancellationToken cancellationToken = default);

    Task<TournamentMatch> RecordForfeitAsync(
        TournamentMatch match,
        Guid winnerId,
        string? notes,
        CancellationToken cancellationToken = default);

    Task<TournamentMatch> StartMatchAsync(
        TournamentMatch match,
        CancellationToken cancellationToken = default);

    Task<TournamentMatch> CompleteMatchAsync(
        TournamentMatch match,
        CancellationToken cancellationToken = default);
}
