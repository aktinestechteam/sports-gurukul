using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class EventAnnouncementConfiguration : IEntityTypeConfiguration<EventAnnouncement>
{
    public void Configure(EntityTypeBuilder<EventAnnouncement> builder)
    {
        builder.ToTable("EventAnnouncements");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.Message)
            .HasMaxLength(5000)
            .IsRequired();

        builder.Property(e => e.Priority)
            .HasMaxLength(20);

        builder.Property(e => e.RowVersion)
            .IsRowVersion();

        builder.HasIndex(e => e.EventId)
            .HasDatabaseName("IX_EventAnnouncements_EventId");

        builder.HasIndex(e => e.IsPublished)
            .HasDatabaseName("IX_EventAnnouncements_IsPublished");

        builder.HasIndex(e => new { e.EventId, e.IsPublished })
            .HasDatabaseName("IX_EventAnnouncements_EventId_IsPublished");

        builder.HasOne(e => e.Event)
            .WithMany(ev => ev.Announcements)
            .HasForeignKey(e => e.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.Ignore(e => e.CreatedBy);
        builder.Ignore(e => e.UpdatedBy);
    }
}
