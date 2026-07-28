using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class EventCoachConfiguration : IEntityTypeConfiguration<EventCoach>
{
    public void Configure(EntityTypeBuilder<EventCoach> builder)
    {
        builder.ToTable("EventCoaches");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Role)
            .HasMaxLength(100);

        builder.Property(e => e.Responsibility)
            .HasMaxLength(500);

        builder.Property(e => e.Notes)
            .HasMaxLength(2000);

        builder.Property(e => e.RowVersion)
            .IsRowVersion();

        builder.HasIndex(e => e.EventId)
            .HasDatabaseName("IX_EventCoaches_EventId");

        builder.HasIndex(e => e.CoachId)
            .HasDatabaseName("IX_EventCoaches_CoachId");

        builder.HasIndex(e => new { e.EventId, e.CoachId })
            .IsUnique()
            .HasDatabaseName("IX_EventCoaches_EventId_CoachId");

        builder.HasOne(e => e.Event)
            .WithMany(ev => ev.Coaches)
            .HasForeignKey(e => e.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Coach)
            .WithMany()
            .HasForeignKey(e => e.CoachId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.Ignore(e => e.CreatedBy);
        builder.Ignore(e => e.UpdatedBy);
    }
}
