using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class EventFeedbackConfiguration : IEntityTypeConfiguration<EventFeedback>
{
    public void Configure(EntityTypeBuilder<EventFeedback> builder)
    {
        builder.ToTable("EventFeedbacks");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.OverallRating)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.ContentRating)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.SpeakerRating)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.VenueRating)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.OrganizationRating)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.Comments)
            .HasMaxLength(2000);

        builder.Property(e => e.Suggestions)
            .HasMaxLength(2000);

        builder.Property(e => e.RowVersion)
            .IsRowVersion();

        builder.HasIndex(e => e.EventId)
            .HasDatabaseName("IX_EventFeedbacks_EventId");

        builder.HasIndex(e => e.ParticipantId)
            .HasDatabaseName("IX_EventFeedbacks_ParticipantId");

        builder.HasIndex(e => e.UserId)
            .HasDatabaseName("IX_EventFeedbacks_UserId");

        builder.HasIndex(e => e.OverallRating)
            .HasDatabaseName("IX_EventFeedbacks_OverallRating");

        builder.HasIndex(e => new { e.EventId, e.UserId })
            .HasDatabaseName("IX_EventFeedbacks_EventId_UserId");

        builder.HasOne(e => e.Event)
            .WithMany(ev => ev.Feedbacks)
            .HasForeignKey(e => e.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Participant)
            .WithMany()
            .HasForeignKey(e => e.ParticipantId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.Ignore(e => e.CreatedBy);
        builder.Ignore(e => e.UpdatedBy);
    }
}
