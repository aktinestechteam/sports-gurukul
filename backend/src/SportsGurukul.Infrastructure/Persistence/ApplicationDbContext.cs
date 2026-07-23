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
