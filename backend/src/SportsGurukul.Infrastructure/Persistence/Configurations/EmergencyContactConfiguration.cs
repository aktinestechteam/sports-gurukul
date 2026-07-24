using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class EmergencyContactConfiguration : IEntityTypeConfiguration<EmergencyContact>
{
    public void Configure(EntityTypeBuilder<EmergencyContact> builder)
    {
        builder.ToTable("EmergencyContacts");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .HasMaxLength(200);

        builder.Property(e => e.Relationship)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(e => e.Phone)
            .HasMaxLength(50);

        builder.Property(e => e.Email)
            .HasMaxLength(200);

        builder.Property(e => e.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(e => e.AthleteId)
            .IsUnique()
            .HasDatabaseName("IX_EmergencyContacts_AthleteId");

        builder.HasOne(e => e.Athlete)
            .WithOne(a => a.EmergencyContact)
            .HasForeignKey<EmergencyContact>(e => e.AthleteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(e => e.CreatedBy);
        builder.Ignore(e => e.UpdatedBy);
    }
}
