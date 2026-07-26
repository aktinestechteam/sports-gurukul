using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class FacilityScheduleConfiguration : IEntityTypeConfiguration<FacilitySchedule>
{
    public void Configure(EntityTypeBuilder<FacilitySchedule> builder)
    {
        builder.ToTable("FacilitySchedules");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.OpeningTime)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(s => s.ClosingTime)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(s => s.Notes)
            .HasMaxLength(500);

        builder.HasIndex(s => s.FacilityId)
            .HasDatabaseName("IX_FacilitySchedules_FacilityId");

        builder.HasIndex(s => s.FacilityCourtId)
            .HasDatabaseName("IX_FacilitySchedules_FacilityCourtId");

        builder.HasIndex(s => new { s.FacilityId, s.DayOfWeek, s.FacilityCourtId })
            .IsUnique()
            .HasDatabaseName("IX_FacilitySchedules_Facility_Day_Court");

        builder.HasOne(s => s.Facility)
            .WithMany(f => f.Schedules)
            .HasForeignKey(s => s.FacilityId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.FacilityCourt)
            .WithMany()
            .HasForeignKey(s => s.FacilityCourtId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(s => s.CreatedBy);
        builder.Ignore(s => s.UpdatedBy);
    }
}
