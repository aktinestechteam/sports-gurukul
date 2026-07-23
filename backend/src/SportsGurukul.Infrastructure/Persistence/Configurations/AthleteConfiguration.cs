using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class AthleteConfiguration : IEntityTypeConfiguration<Athlete>
{
    public void Configure(EntityTypeBuilder<Athlete> builder)
    {
        builder.ToTable("Athletes");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.AthleteCode)
            .HasMaxLength(50);

        builder.Property(a => a.Height)
            .HasMaxLength(20);

        builder.Property(a => a.Weight)
            .HasMaxLength(20);

        builder.Property(a => a.Biography)
            .HasMaxLength(2000);

        builder.Property(a => a.CurrentLevel)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(a => a.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(a => a.BloodGroup)
            .HasConversion<string?>()
            .HasMaxLength(20);

        builder.Property(a => a.DominantHand)
            .HasConversion<string?>()
            .HasMaxLength(20);

        builder.Property(a => a.DominantFoot)
            .HasConversion<string?>()
            .HasMaxLength(20);

        builder.Property(a => a.RowVersion)
            .IsRowVersion();

        builder.HasIndex(a => a.UserId)
            .IsUnique()
            .HasDatabaseName("IX_Athletes_UserId");

        builder.HasIndex(a => a.AthleteCode)
            .IsUnique()
            .HasDatabaseName("IX_Athletes_AthleteCode");

        builder.HasIndex(a => a.Status)
            .HasDatabaseName("IX_Athletes_Status");

        builder.HasOne(a => a.User)
            .WithOne()
            .HasForeignKey<Athlete>(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.MedicalProfile)
            .WithOne(m => m.Athlete)
            .HasForeignKey<MedicalProfile>(m => m.AthleteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.EmergencyContact)
            .WithOne(e => e.Athlete)
            .HasForeignKey<EmergencyContact>(e => e.AthleteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.Ranking)
            .WithOne(r => r.Athlete)
            .HasForeignKey<Ranking>(r => r.AthleteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(a => a.CreatedBy);
        builder.Ignore(a => a.UpdatedBy);
    }
}
