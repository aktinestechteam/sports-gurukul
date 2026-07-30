using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.Finance;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.Finance;

public class FinancialAuditConfiguration : IEntityTypeConfiguration<FinancialAudit>
{
    public void Configure(EntityTypeBuilder<FinancialAudit> builder)
    {
        builder.ToTable("FinancialAudits");

        builder.HasKey(fa => fa.Id);

        builder.Property(fa => fa.EntityType)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(fa => fa.Action)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(fa => fa.IpAddress)
            .HasMaxLength(50);

        builder.HasIndex(fa => new { fa.EntityType, fa.EntityId })
            .HasDatabaseName("IX_FinancialAudits_Entity");

        builder.HasIndex(fa => fa.PerformedAt)
            .HasDatabaseName("IX_FinancialAudits_PerformedAt");

        builder.HasIndex(fa => fa.PerformedBy)
            .HasDatabaseName("IX_FinancialAudits_PerformedBy");

        builder.HasQueryFilter(fa => !fa.IsDeleted);

        builder.Ignore(fa => fa.CreatedBy);
        builder.Ignore(fa => fa.UpdatedBy);
    }
}
