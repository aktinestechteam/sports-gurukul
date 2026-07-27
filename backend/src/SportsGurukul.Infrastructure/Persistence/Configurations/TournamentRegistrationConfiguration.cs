using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class TournamentRegistrationConfiguration : IEntityTypeConfiguration<TournamentRegistration>
{
    public void Configure(EntityTypeBuilder<TournamentRegistration> builder)
    {
        builder.ToTable("TournamentRegistrations");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.RegistrationStatus)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(r => r.RegistrantName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(r => r.Email)
            .HasMaxLength(200);

        builder.Property(r => r.Phone)
            .HasMaxLength(20);

        builder.Property(r => r.FeePaid)
            .HasPrecision(10, 2);

        builder.Property(r => r.Notes)
            .HasMaxLength(2000);

        builder.Property(r => r.RowVersion)
            .IsRowVersion();

        builder.HasIndex(r => r.TournamentId)
            .HasDatabaseName("IX_TournamentRegistrations_TournamentId");

        builder.HasIndex(r => r.CategoryId)
            .HasDatabaseName("IX_TournamentRegistrations_CategoryId");

        builder.HasIndex(r => r.DivisionId)
            .HasDatabaseName("IX_TournamentRegistrations_DivisionId");

        builder.HasIndex(r => r.RegistrationStatus)
            .HasDatabaseName("IX_TournamentRegistrations_RegistrationStatus");

        builder.HasIndex(r => r.AthleteId)
            .HasDatabaseName("IX_TournamentRegistrations_AthleteId");

        builder.HasIndex(r => r.TeamId)
            .HasDatabaseName("IX_TournamentRegistrations_TeamId");

        builder.HasIndex(r => r.AcademyId)
            .HasDatabaseName("IX_TournamentRegistrations_AcademyId");

        builder.HasOne(r => r.Tournament)
            .WithMany(t => t.Registrations)
            .HasForeignKey(r => r.TournamentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Category)
            .WithMany(c => c.Registrations)
            .HasForeignKey(r => r.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(r => r.Division)
            .WithMany(d => d.Registrations)
            .HasForeignKey(r => r.DivisionId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(r => r.Athlete)
            .WithMany()
            .HasForeignKey(r => r.AthleteId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(r => r.Team)
            .WithMany(t => t.Registrations)
            .HasForeignKey(r => r.TeamId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(r => r.Academy)
            .WithMany()
            .HasForeignKey(r => r.AcademyId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Ignore(r => r.CreatedBy);
        builder.Ignore(r => r.UpdatedBy);
    }
}
