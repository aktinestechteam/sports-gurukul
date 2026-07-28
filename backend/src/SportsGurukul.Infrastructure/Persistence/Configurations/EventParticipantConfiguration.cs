using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class EventParticipantConfiguration : IEntityTypeConfiguration<EventParticipant>
{
    public void Configure(EntityTypeBuilder<EventParticipant> builder)
    {
        builder.ToTable("EventParticipants");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.ParticipantName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.Email)
            .HasMaxLength(200);

        builder.Property(e => e.Phone)
            .HasMaxLength(20);

        builder.Property(e => e.AttendanceStatus)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(e => e.Role)
            .HasMaxLength(100);

        builder.Property(e => e.Organization)
            .HasMaxLength(200);

        builder.Property(e => e.DietaryRequirements)
            .HasMaxLength(500);

        builder.Property(e => e.SpecialNeeds)
            .HasMaxLength(500);

        builder.Property(e => e.RowVersion)
            .IsRowVersion();

        builder.HasIndex(e => e.EventId)
            .HasDatabaseName("IX_EventParticipants_EventId");

        builder.HasIndex(e => e.AthleteId)
            .HasDatabaseName("IX_EventParticipants_AthleteId");

        builder.HasIndex(e => e.UserId)
            .HasDatabaseName("IX_EventParticipants_UserId");

        builder.HasIndex(e => e.RegistrationId)
            .HasDatabaseName("IX_EventParticipants_RegistrationId");

        builder.HasIndex(e => e.AttendanceStatus)
            .HasDatabaseName("IX_EventParticipants_AttendanceStatus");

        builder.HasIndex(e => new { e.EventId, e.AthleteId })
            .HasDatabaseName("IX_EventParticipants_EventId_AthleteId");

        builder.HasOne(e => e.Event)
            .WithMany(ev => ev.Participants)
            .HasForeignKey(e => e.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Athlete)
            .WithMany()
            .HasForeignKey(e => e.AthleteId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(e => e.Registration)
            .WithMany()
            .HasForeignKey(e => e.RegistrationId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.Ignore(e => e.CreatedBy);
        builder.Ignore(e => e.UpdatedBy);
    }
}
