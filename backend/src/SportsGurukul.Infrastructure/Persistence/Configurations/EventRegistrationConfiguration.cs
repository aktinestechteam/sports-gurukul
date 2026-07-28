using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class EventRegistrationConfiguration : IEntityTypeConfiguration<EventRegistration>
{
    public void Configure(EntityTypeBuilder<EventRegistration> builder)
    {
        builder.ToTable("EventRegistrations");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.RegistrationNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(e => e.AmountPaid)
            .HasPrecision(10, 2);

        builder.Property(e => e.PaymentReference)
            .HasMaxLength(200);

        builder.Property(e => e.Notes)
            .HasMaxLength(2000);

        builder.Property(e => e.RejectionReason)
            .HasMaxLength(500);

        builder.Property(e => e.RowVersion)
            .IsRowVersion();

        builder.HasIndex(e => e.EventId)
            .HasDatabaseName("IX_EventRegistrations_EventId");

        builder.HasIndex(e => e.AthleteId)
            .HasDatabaseName("IX_EventRegistrations_AthleteId");

        builder.HasIndex(e => e.UserId)
            .HasDatabaseName("IX_EventRegistrations_UserId");

        builder.HasIndex(e => e.RegistrationNumber)
            .IsUnique()
            .HasDatabaseName("IX_EventRegistrations_RegistrationNumber");

        builder.HasIndex(e => e.Status)
            .HasDatabaseName("IX_EventRegistrations_Status");

        builder.HasIndex(e => new { e.EventId, e.AthleteId })
            .HasDatabaseName("IX_EventRegistrations_EventId_AthleteId");

        builder.HasIndex(e => new { e.EventId, e.Status })
            .HasDatabaseName("IX_EventRegistrations_EventId_Status");

        builder.HasOne(e => e.Event)
            .WithMany(ev => ev.Registrations)
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

        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.Ignore(e => e.CreatedBy);
        builder.Ignore(e => e.UpdatedBy);
    }
}
