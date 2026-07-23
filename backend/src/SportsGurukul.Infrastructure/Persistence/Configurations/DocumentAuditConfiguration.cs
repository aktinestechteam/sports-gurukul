using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class DocumentAuditConfiguration : IEntityTypeConfiguration<DocumentAudit>
{
    public void Configure(EntityTypeBuilder<DocumentAudit> builder)
    {
        builder.ToTable("DocumentAudits");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Action)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(a => a.IpAddress)
            .HasMaxLength(45);

        builder.Property(a => a.Details)
            .HasMaxLength(500);

        builder.HasIndex(a => a.DocumentId)
            .HasDatabaseName("IX_DocumentAudits_DocumentId");

        builder.HasIndex(a => a.PerformedOn)
            .HasDatabaseName("IX_DocumentAudits_PerformedOn");

        builder.HasOne(a => a.Document)
            .WithMany(d => d.AuditTrail)
            .HasForeignKey(a => a.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(a => a.CreatedBy);
        builder.Ignore(a => a.UpdatedBy);
    }
}
