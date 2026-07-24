using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class MedicalProfileConfiguration : IEntityTypeConfiguration<MedicalProfile>
{
    public void Configure(EntityTypeBuilder<MedicalProfile> builder)
    {
        builder.ToTable("MedicalProfiles");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.MedicalConditions)
            .HasMaxLength(2000);

        builder.Property(m => m.Allergies)
            .HasMaxLength(2000);

        builder.Property(m => m.Medications)
            .HasMaxLength(2000);

        builder.Property(m => m.BloodGroup)
            .HasMaxLength(20);

        builder.Property(m => m.InsuranceNumber)
            .HasMaxLength(100);

        builder.Property(m => m.DoctorName)
            .HasMaxLength(200);

        builder.Property(m => m.DoctorContact)
            .HasMaxLength(50);

        builder.Property(m => m.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(m => m.AthleteId)
            .IsUnique()
            .HasDatabaseName("IX_MedicalProfiles_AthleteId");

        builder.HasOne(m => m.Athlete)
            .WithOne(a => a.MedicalProfile)
            .HasForeignKey<MedicalProfile>(m => m.AthleteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(m => m.CreatedBy);
        builder.Ignore(m => m.UpdatedBy);
    }
}
