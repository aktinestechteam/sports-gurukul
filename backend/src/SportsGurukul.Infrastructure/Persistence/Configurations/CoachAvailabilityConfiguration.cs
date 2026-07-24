using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class CoachAvailabilityConfiguration : IEntityTypeConfiguration<CoachAvailability>
{
    public void Configure(EntityTypeBuilder<CoachAvailability> builder)
    {
        builder.ToTable("CoachAvailabilities");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.WeeklySchedule)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(a => a.TimeSlots)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(a => a.RowVersion)
            .IsRowVersion();

        builder.HasIndex(a => a.CoachId)
            .IsUnique()
            .HasDatabaseName("IX_CoachAvailabilities_CoachId");

        builder.HasOne(a => a.Coach)
            .WithOne(c => c.Availability)
            .HasForeignKey<CoachAvailability>(a => a.CoachId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(a => a.CreatedBy);
        builder.Ignore(a => a.UpdatedBy);

        builder.HasData(
            new CoachAvailability
            {
                Id = Guid.Parse("b2000000-0000-0000-0000-000000000001"),
                CoachId = Guid.Parse("d1000000-0000-0000-0000-000000000001"),
                WeeklySchedule = "{\"monday\":{\"start\":\"06:00\",\"end\":\"18:00\"},\"tuesday\":{\"start\":\"06:00\",\"end\":\"18:00\"},\"wednesday\":{\"start\":\"06:00\",\"end\":\"18:00\"},\"thursday\":{\"start\":\"06:00\",\"end\":\"18:00\"},\"friday\":{\"start\":\"06:00\",\"end\":\"18:00\"},\"saturday\":{\"start\":\"08:00\",\"end\":\"14:00\"}}",
                TimeSlots = "[\"06:00-08:00\",\"08:00-10:00\",\"10:00-12:00\",\"12:00-14:00\",\"14:00-16:00\",\"16:00-18:00\"]",
                OnlineAvailable = true,
                OfflineAvailable = true,
                TravelDistance = 25,
                IsDeleted = false
            }
        );
    }
}
