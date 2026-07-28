using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class EventScheduleConfiguration : IEntityTypeConfiguration<EventSchedule>
{
    public void Configure(EntityTypeBuilder<EventSchedule> builder)
    {
        builder.ToTable("EventSchedules");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Title)
            .HasMaxLength(200);

        builder.Property(e => e.Description)
            .HasMaxLength(2000);

        builder.Property(e => e.RecurrenceRule)
            .HasMaxLength(500);

        builder.Property(e => e.RowVersion)
            .IsRowVersion();

        builder.HasIndex(e => e.EventId)
            .HasDatabaseName("IX_EventSchedules_EventId");

        builder.HasIndex(e => new { e.EventId, e.ScheduleDate })
            .HasDatabaseName("IX_EventSchedules_EventId_ScheduleDate");

        builder.HasOne(e => e.Event)
            .WithMany(ev => ev.Schedules)
            .HasForeignKey(e => e.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.Ignore(e => e.CreatedBy);
        builder.Ignore(e => e.UpdatedBy);
    }
}
