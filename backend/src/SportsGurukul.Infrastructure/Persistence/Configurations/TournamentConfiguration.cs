using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class TournamentConfiguration : IEntityTypeConfiguration<Tournament>
{
    public void Configure(EntityTypeBuilder<Tournament> builder)
    {
        builder.ToTable("Tournaments");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.TournamentCode)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(t => t.TournamentName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(t => t.Description)
            .HasMaxLength(2000);

        builder.Property(t => t.TournamentType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(t => t.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(t => t.RegistrationType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(t => t.RegistrationFee)
            .HasPrecision(10, 2);

        builder.Property(t => t.Venue)
            .HasMaxLength(500);

        builder.Property(t => t.Rules)
            .HasMaxLength(5000);

        builder.Property(t => t.ContactEmail)
            .HasMaxLength(200);

        builder.Property(t => t.ContactPhone)
            .HasMaxLength(20);

        builder.Property(t => t.Website)
            .HasMaxLength(500);

        builder.Property(t => t.RowVersion)
            .IsRowVersion();

        builder.HasIndex(t => t.TournamentCode)
            .IsUnique()
            .HasDatabaseName("IX_Tournaments_TournamentCode");

        builder.HasIndex(t => t.AcademyId)
            .HasDatabaseName("IX_Tournaments_AcademyId");

        builder.HasIndex(t => t.SportId)
            .HasDatabaseName("IX_Tournaments_SportId");

        builder.HasIndex(t => t.Status)
            .HasDatabaseName("IX_Tournaments_Status");

        builder.HasIndex(t => t.TournamentType)
            .HasDatabaseName("IX_Tournaments_TournamentType");

        builder.HasIndex(t => new { t.StartDate, t.EndDate })
            .HasDatabaseName("IX_Tournaments_StartDate_EndDate");

        builder.HasOne(t => t.Academy)
            .WithMany()
            .HasForeignKey(t => t.AcademyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.Sport)
            .WithMany()
            .HasForeignKey(t => t.SportId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(t => t.CreatedBy);
        builder.Ignore(t => t.UpdatedBy);
    }
}
