using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class AcademyContactConfiguration : IEntityTypeConfiguration<AcademyContact>
{
    public void Configure(EntityTypeBuilder<AcademyContact> builder)
    {
        builder.ToTable("AcademyContacts");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.PrimaryContactName)
            .HasMaxLength(200);

        builder.Property(c => c.PrimaryPhone)
            .HasMaxLength(50);

        builder.Property(c => c.PrimaryEmail)
            .HasMaxLength(200);

        builder.Property(c => c.SecondaryContactName)
            .HasMaxLength(200);

        builder.Property(c => c.SecondaryPhone)
            .HasMaxLength(50);

        builder.Property(c => c.SecondaryEmail)
            .HasMaxLength(200);

        builder.Property(c => c.Address)
            .HasMaxLength(500);

        builder.Property(c => c.Country)
            .HasMaxLength(100);

        builder.Property(c => c.State)
            .HasMaxLength(100);

        builder.Property(c => c.City)
            .HasMaxLength(100);

        builder.Property(c => c.PostalCode)
            .HasMaxLength(20);

        builder.Property(c => c.Latitude)
            .HasPrecision(10, 8);

        builder.Property(c => c.Longitude)
            .HasPrecision(11, 8);

        builder.HasIndex(c => c.AcademyId)
            .IsUnique()
            .HasDatabaseName("IX_AcademyContacts_AcademyId");

        builder.HasOne(c => c.Academy)
            .WithOne(a => a.Contact)
            .HasForeignKey<AcademyContact>(c => c.AcademyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(c => c.CreatedBy);
        builder.Ignore(c => c.UpdatedBy);
    }
}
