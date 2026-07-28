using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class EventCertificateConfiguration : IEntityTypeConfiguration<EventCertificate>
{
    public void Configure(EntityTypeBuilder<EventCertificate> builder)
    {
        builder.ToTable("EventCertificates");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.CertificateNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.CertificateType)
            .HasMaxLength(100);

        builder.Property(e => e.IssuedBy)
            .HasMaxLength(200);

        builder.Property(e => e.DocumentUrl)
            .HasMaxLength(500);

        builder.Property(e => e.Notes)
            .HasMaxLength(1000);

        builder.Property(e => e.RowVersion)
            .IsRowVersion();

        builder.HasIndex(e => e.EventId)
            .HasDatabaseName("IX_EventCertificates_EventId");

        builder.HasIndex(e => e.ParticipantId)
            .HasDatabaseName("IX_EventCertificates_ParticipantId");

        builder.HasIndex(e => e.CertificateNumber)
            .IsUnique()
            .HasDatabaseName("IX_EventCertificates_CertificateNumber");

        builder.HasOne(e => e.Event)
            .WithMany(ev => ev.Certificates)
            .HasForeignKey(e => e.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Participant)
            .WithMany()
            .HasForeignKey(e => e.ParticipantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.Ignore(e => e.CreatedBy);
        builder.Ignore(e => e.UpdatedBy);
    }
}
