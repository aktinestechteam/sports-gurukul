using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class EventSessionConfiguration : IEntityTypeConfiguration<EventSession>
{
    public void Configure(EntityTypeBuilder<EventSession> builder)
    {
        builder.ToTable("EventSessions");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.SessionCode)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(e => e.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.Description)
            .HasMaxLength(2000);

        builder.Property(e => e.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(e => e.Notes)
            .HasMaxLength(2000);

        builder.Property(e => e.RowVersion)
            .IsRowVersion();

        builder.HasIndex(e => e.EventId)
            .HasDatabaseName("IX_EventSessions_EventId");

        builder.HasIndex(e => e.SessionCode)
            .IsUnique()
            .HasDatabaseName("IX_EventSessions_SessionCode");

        builder.HasIndex(e => e.Status)
            .HasDatabaseName("IX_EventSessions_Status");

        builder.HasIndex(e => new { e.EventId, e.SessionDate })
            .HasDatabaseName("IX_EventSessions_EventId_SessionDate");

        builder.HasIndex(e => e.VenueId)
            .HasDatabaseName("IX_EventSessions_VenueId");

        builder.HasIndex(e => e.SpeakerId)
            .HasDatabaseName("IX_EventSessions_SpeakerId");

        builder.HasIndex(e => e.CoachId)
            .HasDatabaseName("IX_EventSessions_CoachId");

        builder.HasOne(e => e.Event)
            .WithMany(ev => ev.Sessions)
            .HasForeignKey(e => e.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Venue)
            .WithMany()
            .HasForeignKey(e => e.VenueId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(e => e.Speaker)
            .WithMany()
            .HasForeignKey(e => e.SpeakerId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(e => e.Coach)
            .WithMany()
            .HasForeignKey(e => e.CoachId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.Ignore(e => e.CreatedBy);
        builder.Ignore(e => e.UpdatedBy);
    }
}
