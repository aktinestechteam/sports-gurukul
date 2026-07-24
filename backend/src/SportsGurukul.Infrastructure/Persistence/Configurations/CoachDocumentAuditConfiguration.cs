using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class CoachDocumentAuditConfiguration : IEntityTypeConfiguration<CoachDocumentAudit>
{
    public void Configure(EntityTypeBuilder<CoachDocumentAudit> builder)
    {
        builder.ToTable("CoachDocumentAudits");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.DocumentId)
            .IsRequired();

        builder.Property(a => a.Action)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(a => a.PerformedBy)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(a => a.PerformedOn)
            .IsRequired();

        builder.Property(a => a.IpAddress)
            .HasMaxLength(45);

        builder.Property(a => a.Details)
            .HasMaxLength(1000);

        builder.Ignore(a => a.CreatedBy);
        builder.Ignore(a => a.UpdatedBy);

        builder.HasOne(a => a.Document)
            .WithMany(d => d.AuditTrail)
            .HasForeignKey(a => a.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(a => a.DocumentId)
            .HasDatabaseName("IX_CoachDocumentAudits_DocumentId");

        builder.HasIndex(a => a.PerformedOn)
            .HasDatabaseName("IX_CoachDocumentAudits_PerformedOn");
    }
}
