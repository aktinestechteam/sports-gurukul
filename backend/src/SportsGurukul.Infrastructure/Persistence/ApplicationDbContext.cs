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
