using Microsoft.EntityFrameworkCore;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<TrainingCertificate> Certificates { get; }
    DbSet<Tournament> Tournaments { get; }
    DbSet<TournamentMatch> TournamentMatches { get; }
    DbSet<TournamentRegistration> TournamentRegistrations { get; }
    DbSet<TournamentBracket> TournamentBrackets { get; }
    DbSet<TournamentRanking> TournamentRankings { get; }
    DbSet<TournamentParticipant> TournamentParticipants { get; }
    DbSet<TournamentFixture> TournamentFixtures { get; }
    DbSet<TournamentResult> TournamentResults { get; }
    DbSet<TournamentAward> TournamentAwards { get; }
    DbSet<TournamentOfficial> TournamentOfficials { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
