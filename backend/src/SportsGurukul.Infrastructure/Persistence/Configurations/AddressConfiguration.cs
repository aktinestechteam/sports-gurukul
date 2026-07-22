using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.ToTable("Addresses");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.AddressType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(a => a.Line1)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.Line2)
            .HasMaxLength(200);

        builder.Property(a => a.City)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.State)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.Country)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.PostalCode)
            .HasMaxLength(20);

        builder.Property(a => a.Latitude)
            .HasColumnType("double precision");

        builder.Property(a => a.Longitude)
            .HasColumnType("double precision");

        builder.HasIndex(a => a.UserProfileId)
            .HasDatabaseName("IX_Addresses_UserProfileId");

        builder.HasIndex(a => a.IsDeleted)
            .HasDatabaseName("IX_Addresses_IsDeleted");

        builder.HasOne(a => a.UserProfile)
            .WithMany(up => up.Addresses)
            .HasForeignKey(a => a.UserProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(a => a.CreatedBy);
        builder.Ignore(a => a.UpdatedBy);
    }
}
