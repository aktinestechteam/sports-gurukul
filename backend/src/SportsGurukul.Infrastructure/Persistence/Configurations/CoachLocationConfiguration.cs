using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class CoachLocationConfiguration : IEntityTypeConfiguration<CoachLocation>
{
    public void Configure(EntityTypeBuilder<CoachLocation> builder)
    {
        builder.ToTable("CoachLocations");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Country)
            .HasMaxLength(100);

        builder.Property(l => l.State)
            .HasMaxLength(100);

        builder.Property(l => l.City)
            .HasMaxLength(100);

        builder.Property(l => l.District)
            .HasMaxLength(100);

        builder.Property(l => l.Latitude)
            .HasPrecision(10, 8);

        builder.Property(l => l.Longitude)
            .HasPrecision(11, 8);

        builder.HasIndex(l => l.CoachId)
            .IsUnique()
            .HasDatabaseName("IX_CoachLocations_CoachId");

        builder.HasIndex(l => new { l.State, l.City })
            .HasDatabaseName("IX_CoachLocations_State_City");

        builder.HasIndex(l => l.Country)
            .HasDatabaseName("IX_CoachLocations_Country");

        builder.HasIndex(l => new { l.Latitude, l.Longitude })
            .HasDatabaseName("IX_CoachLocations_LatLon");

        builder.HasOne(l => l.Coach)
            .WithOne(c => c.Location)
            .HasForeignKey<CoachLocation>(l => l.CoachId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(l => l.CreatedBy);
        builder.Ignore(l => l.UpdatedBy);

        builder.HasData(
            new CoachLocation
            {
                Id = Guid.Parse("c2000000-0000-0000-0000-000000000001"),
                CoachId = Guid.Parse("d1000000-0000-0000-0000-000000000001"),
                Country = "India",
                State = "Maharashtra",
                City = "Mumbai",
                District = "Mumbai City",
                Latitude = 19.0760m,
                Longitude = 72.8777m,
                IsDeleted = false
            }
        );
    }
}
