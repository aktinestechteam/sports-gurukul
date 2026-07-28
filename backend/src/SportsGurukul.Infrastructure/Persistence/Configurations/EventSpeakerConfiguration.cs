using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class EventSpeakerConfiguration : IEntityTypeConfiguration<EventSpeaker>
{
    public void Configure(EntityTypeBuilder<EventSpeaker> builder)
    {
        builder.ToTable("EventSpeakers");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.SpeakerName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.Email)
            .HasMaxLength(200);

        builder.Property(e => e.Phone)
            .HasMaxLength(20);

        builder.Property(e => e.Title)
            .HasMaxLength(200);

        builder.Property(e => e.Bio)
            .HasMaxLength(2000);

        builder.Property(e => e.ProfileImageUrl)
            .HasMaxLength(500);

        builder.Property(e => e.Organization)
            .HasMaxLength(200);

        builder.Property(e => e.Role)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(e => e.Topic)
            .HasMaxLength(500);

        builder.Property(e => e.RowVersion)
            .IsRowVersion();

        builder.HasIndex(e => e.EventId)
            .HasDatabaseName("IX_EventSpeakers_EventId");

        builder.HasIndex(e => e.UserId)
            .HasDatabaseName("IX_EventSpeakers_UserId");

        builder.HasIndex(e => e.CoachId)
            .HasDatabaseName("IX_EventSpeakers_CoachId");

        builder.HasOne(e => e.Event)
            .WithMany(ev => ev.Speakers)
            .HasForeignKey(e => e.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
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
