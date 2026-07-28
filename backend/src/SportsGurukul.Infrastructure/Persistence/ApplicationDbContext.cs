using System.Reflection;
using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<EmailVerificationToken> EmailVerificationTokens => Set<EmailVerificationToken>();
    public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();

    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<ContactInformation> ContactInformation => Set<ContactInformation>();
    public DbSet<UserPreference> UserPreferences => Set<UserPreference>();
    public DbSet<UserFile> UserFiles => Set<UserFile>();

    public DbSet<Athlete> Athletes => Set<Athlete>();
    public DbSet<Sport> Sports => Set<Sport>();
    public DbSet<SportCategory> SportCategories => Set<SportCategory>();
    public DbSet<AthleteSport> AthleteSports => Set<AthleteSport>();
    public DbSet<Achievement> Achievements => Set<Achievement>();
    public DbSet<AthleteAchievement> AthleteAchievements => Set<AthleteAchievement>();
    public DbSet<Ranking> Rankings => Set<Ranking>();
    public DbSet<MedicalProfile> MedicalProfiles => Set<MedicalProfile>();
    public DbSet<EmergencyContact> EmergencyContacts => Set<EmergencyContact>();

    public DbSet<AthleteDocument> AthleteDocuments => Set<AthleteDocument>();
    public DbSet<DocumentVersion> DocumentVersions => Set<DocumentVersion>();
    public DbSet<DocumentAudit> DocumentAudits => Set<DocumentAudit>();

    public DbSet<Coach> Coaches => Set<Coach>();
    public DbSet<CoachSport> CoachSports => Set<CoachSport>();
    public DbSet<CoachCertification> CoachCertifications => Set<CoachCertification>();
    public DbSet<CoachExperience> CoachExperiences => Set<CoachExperience>();
    public DbSet<CoachEducation> CoachEducation => Set<CoachEducation>();
    public DbSet<CoachAvailability> CoachAvailabilities => Set<CoachAvailability>();
    public DbSet<CoachLocation> CoachLocations => Set<CoachLocation>();
    public DbSet<CoachSpecialization> CoachSpecializations => Set<CoachSpecialization>();
    public DbSet<CoachDocument> CoachDocuments => Set<CoachDocument>();
    public DbSet<CoachDocumentVersion> CoachDocumentVersions => Set<CoachDocumentVersion>();
    public DbSet<CoachDocumentAudit> CoachDocumentAudits => Set<CoachDocumentAudit>();
    public DbSet<CoachAthlete> CoachAthletes => Set<CoachAthlete>();

    public DbSet<SavedSearch> SavedSearches => Set<SavedSearch>();
    public DbSet<RecentSearch> RecentSearches => Set<RecentSearch>();

    public DbSet<SavedAcademySearch> SavedAcademySearches => Set<SavedAcademySearch>();
    public DbSet<RecentAcademySearch> RecentAcademySearches => Set<RecentAcademySearch>();
    public DbSet<AcademyView> AcademyViews => Set<AcademyView>();

    public DbSet<Academy> Academies => Set<Academy>();
    public DbSet<AcademyBranch> AcademyBranches => Set<AcademyBranch>();
    public DbSet<AcademySport> AcademySports => Set<AcademySport>();
    public DbSet<AcademyFacility> AcademyFacilities => Set<AcademyFacility>();
    public DbSet<AcademyOperatingHours> AcademyOperatingHours => Set<AcademyOperatingHours>();
    public DbSet<AcademyContact> AcademyContacts => Set<AcademyContact>();
    public DbSet<AcademySocialLink> AcademySocialLinks => Set<AcademySocialLink>();
    public DbSet<AcademyMembership> AcademyMemberships => Set<AcademyMembership>();
    public DbSet<AcademyVerification> AcademyVerifications => Set<AcademyVerification>();
    public DbSet<AcademyDocument> AcademyDocuments => Set<AcademyDocument>();
    public DbSet<AcademyGallery> AcademyGalleries => Set<AcademyGallery>();

    public DbSet<CoachAcademy> CoachAcademies => Set<CoachAcademy>();
    public DbSet<AthleteAcademy> AthleteAcademies => Set<AthleteAcademy>();

    // Training Programs & Sessions
    public DbSet<TrainingProgram> TrainingPrograms => Set<TrainingProgram>();
    public DbSet<TrainingProgramSport> TrainingProgramSports => Set<TrainingProgramSport>();
    public DbSet<TrainingBatch> TrainingBatches => Set<TrainingBatch>();
    public DbSet<TrainingSession> TrainingSessions => Set<TrainingSession>();
    public DbSet<SessionSchedule> SessionSchedules => Set<SessionSchedule>();
    public DbSet<TrainingEnrollment> TrainingEnrollments => Set<TrainingEnrollment>();
    public DbSet<Attendance> Attendances => Set<Attendance>();
    public DbSet<TrainingAssessment> TrainingAssessments => Set<TrainingAssessment>();
    public DbSet<AssessmentResult> AssessmentResults => Set<AssessmentResult>();
    public DbSet<TrainingGoal> TrainingGoals => Set<TrainingGoal>();
    public DbSet<TrainingMilestone> TrainingMilestones => Set<TrainingMilestone>();
    public DbSet<TrainingProgress> TrainingProgresses => Set<TrainingProgress>();
    public DbSet<TrainingCertificate> TrainingCertificates => Set<TrainingCertificate>();
    public DbSet<TrainingCertificate> Certificates => Set<TrainingCertificate>();
    public DbSet<TrainingMaterial> TrainingMaterials => Set<TrainingMaterial>();

    // Facility & Infrastructure
    public DbSet<Facility> Facilities => Set<Facility>();
    public DbSet<FacilityArea> FacilityAreas => Set<FacilityArea>();
    public DbSet<FacilityCourt> FacilityCourts => Set<FacilityCourt>();
    public DbSet<FacilityEquipment> FacilityEquipment => Set<FacilityEquipment>();
    public DbSet<EquipmentMaintenance> EquipmentMaintenance => Set<EquipmentMaintenance>();
    public DbSet<FacilitySchedule> FacilitySchedules => Set<FacilitySchedule>();
    public DbSet<FacilityPricing> FacilityPricing => Set<FacilityPricing>();
    public DbSet<FacilityImage> FacilityImages => Set<FacilityImage>();
    public DbSet<FacilityAmenity> FacilityAmenities => Set<FacilityAmenity>();
    public DbSet<FacilityReview> FacilityReviews => Set<FacilityReview>();

    // Booking & Scheduling
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<BookingItem> BookingItems => Set<BookingItem>();
    public DbSet<BookingParticipant> BookingParticipants => Set<BookingParticipant>();
    public DbSet<BookingSchedule> BookingSchedules => Set<BookingSchedule>();
    public DbSet<BookingRecurrence> BookingRecurrences => Set<BookingRecurrence>();
    public DbSet<BookingWaitlist> BookingWaitlists => Set<BookingWaitlist>();
    public DbSet<BookingCancellation> BookingCancellations => Set<BookingCancellation>();
    public DbSet<BookingReschedule> BookingReschedules => Set<BookingReschedule>();
    public DbSet<BookingReminder> BookingReminders => Set<BookingReminder>();
    public DbSet<BookingApproval> BookingApprovals => Set<BookingApproval>();
    public DbSet<BookingConflict> BookingConflicts => Set<BookingConflict>();
    public DbSet<BookingHistory> BookingHistories => Set<BookingHistory>();
    public DbSet<BookingAttachment> BookingAttachments => Set<BookingAttachment>();

    // Tournament Management
    public DbSet<Tournament> Tournaments => Set<Tournament>();
    public DbSet<TournamentCategory> TournamentCategories => Set<TournamentCategory>();
    public DbSet<TournamentSport> TournamentSports => Set<TournamentSport>();
    public DbSet<TournamentDivision> TournamentDivisions => Set<TournamentDivision>();
    public DbSet<TournamentVenue> TournamentVenues => Set<TournamentVenue>();
    public DbSet<TournamentCourt> TournamentCourts => Set<TournamentCourt>();
    public DbSet<TournamentStage> TournamentStages => Set<TournamentStage>();
    public DbSet<TournamentRound> TournamentRounds => Set<TournamentRound>();
    public DbSet<TournamentMatch> TournamentMatches => Set<TournamentMatch>();
    public DbSet<TournamentMatchSet> TournamentMatchSets => Set<TournamentMatchSet>();
    public DbSet<TournamentFixture> TournamentFixtures => Set<TournamentFixture>();
    public DbSet<TournamentParticipant> TournamentParticipants => Set<TournamentParticipant>();
    public DbSet<TournamentTeam> TournamentTeams => Set<TournamentTeam>();
    public DbSet<TournamentRegistration> TournamentRegistrations => Set<TournamentRegistration>();
    public DbSet<TournamentSeed> TournamentSeeds => Set<TournamentSeed>();
    public DbSet<TournamentBracket> TournamentBrackets => Set<TournamentBracket>();
    public DbSet<TournamentResult> TournamentResults => Set<TournamentResult>();
    public DbSet<TournamentRanking> TournamentRankings => Set<TournamentRanking>();
    public DbSet<TournamentAward> TournamentAwards => Set<TournamentAward>();
    public DbSet<TournamentOfficial> TournamentOfficials => Set<TournamentOfficial>();
    public DbSet<TournamentSponsor> TournamentSponsors => Set<TournamentSponsor>();
    public DbSet<TournamentDocument> TournamentDocuments => Set<TournamentDocument>();
    public DbSet<TournamentGallery> TournamentGallery_ => Set<TournamentGallery>();
    public DbSet<TournamentRule> TournamentRules => Set<TournamentRule>();

    // Event Management
    public DbSet<Event> Events => Set<Event>();
    public DbSet<EventTypeEntity> EventTypes => Set<EventTypeEntity>();
    public DbSet<EventCategory> EventCategories => Set<EventCategory>();
    public DbSet<EventSchedule> EventSchedules => Set<EventSchedule>();
    public DbSet<EventVenue> EventVenues => Set<EventVenue>();
    public DbSet<EventRegistration> EventRegistrations => Set<EventRegistration>();
    public DbSet<EventParticipant> EventParticipants => Set<EventParticipant>();
    public DbSet<EventSpeaker> EventSpeakers => Set<EventSpeaker>();
    public DbSet<EventCoach> EventCoaches => Set<EventCoach>();
    public DbSet<EventVolunteer> EventVolunteers => Set<EventVolunteer>();
    public DbSet<EventSponsor> EventSponsors => Set<EventSponsor>();
    public DbSet<EventSession> EventSessions => Set<EventSession>();
    public DbSet<EventAgenda> EventAgendas => Set<EventAgenda>();
    public DbSet<EventTicket> EventTickets => Set<EventTicket>();
    public DbSet<EventAttendance> EventAttendances => Set<EventAttendance>();
    public DbSet<EventCertificate> EventCertificates => Set<EventCertificate>();
    public DbSet<EventFeedback> EventFeedbacks => Set<EventFeedback>();
    public DbSet<EventMedia> EventMedia => Set<EventMedia>();
    public DbSet<EventDocument> EventDocuments => Set<EventDocument>();
    public DbSet<EventAnnouncement> EventAnnouncements => Set<EventAnnouncement>();

    // Event Search & Discovery
    public DbSet<EventSavedSearch> EventSavedSearches => Set<EventSavedSearch>();
    public DbSet<EventRecentSearch> EventRecentSearches => Set<EventRecentSearch>();

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
}
