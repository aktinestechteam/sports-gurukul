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

    // Event Management
    DbSet<Event> Events { get; }
    DbSet<EventTypeEntity> EventTypes { get; }
    DbSet<EventCategory> EventCategories { get; }
    DbSet<EventSchedule> EventSchedules { get; }
    DbSet<EventVenue> EventVenues { get; }
    DbSet<EventRegistration> EventRegistrations { get; }
    DbSet<EventParticipant> EventParticipants { get; }
    DbSet<EventSpeaker> EventSpeakers { get; }
    DbSet<EventCoach> EventCoaches { get; }
    DbSet<EventVolunteer> EventVolunteers { get; }
    DbSet<EventSponsor> EventSponsors { get; }
    DbSet<EventSession> EventSessions { get; }
    DbSet<EventAgenda> EventAgendas { get; }
    DbSet<EventTicket> EventTickets { get; }
    DbSet<EventAttendance> EventAttendances { get; }
    DbSet<EventCertificate> EventCertificates { get; }
    DbSet<EventFeedback> EventFeedbacks { get; }
    DbSet<EventMedia> EventMedia { get; }
    DbSet<EventDocument> EventDocuments { get; }
    DbSet<EventAnnouncement> EventAnnouncements { get; }

    // Event Search & Discovery
    DbSet<EventSavedSearch> EventSavedSearches { get; }
    DbSet<EventRecentSearch> EventRecentSearches { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
