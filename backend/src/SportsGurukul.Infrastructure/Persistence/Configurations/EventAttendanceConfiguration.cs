using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class EventAttendanceConfiguration : IEntityTypeConfiguration<EventAttendance>
{
    public void Configure(EntityTypeBuilder<EventAttendance> builder)
    {
        builder.ToTable("EventAttendances");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(e => e.Remarks)
            .HasMaxLength(1000);

        builder.Property(e => e.MarkedBy)
            .HasMaxLength(200);

        builder.Property(e => e.RowVersion)
            .IsRowVersion();

        builder.HasIndex(e => e.EventId)
            .HasDatabaseName("IX_EventAttendances_EventId");

        builder.HasIndex(e => e.SessionId)
            .HasDatabaseName("IX_EventAttendances_SessionId");

        builder.HasIndex(e => e.ParticipantId)
            .HasDatabaseName("IX_EventAttendances_ParticipantId");

        builder.HasIndex(e => e.Status)
            .HasDatabaseName("IX_EventAttendances_Status");

        builder.HasIndex(e => new { e.EventId, e.ParticipantId })
            .HasDatabaseName("IX_EventAttendances_EventId_ParticipantId");

        builder.HasIndex(e => new { e.SessionId, e.ParticipantId })
            .IsUnique()
            .HasDatabaseName("IX_EventAttendances_SessionId_ParticipantId")
            .HasFilter("\"SessionId\" IS NOT NULL");

        builder.HasOne(e => e.Event)
            .WithMany(ev => ev.Attendances)
            .HasForeignKey(e => e.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Session)
            .WithMany(s => s.Attendances)
            .HasForeignKey(e => e.SessionId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(e => e.Participant)
            .WithMany()
            .HasForeignKey(e => e.ParticipantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.Ignore(e => e.CreatedBy);
        builder.Ignore(e => e.UpdatedBy);
    }
}
