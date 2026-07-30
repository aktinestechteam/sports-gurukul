using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.Finance;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.Finance;

public class SettlementConfiguration : IEntityTypeConfiguration<Settlement>
{
    public void Configure(EntityTypeBuilder<Settlement> builder)
    {
        builder.ToTable("Settlements");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Amount)
            .HasPrecision(18, 2);

        builder.Property(s => s.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(s => s.Reference)
            .HasMaxLength(200);

        builder.HasIndex(s => s.SettlementBatchId)
            .HasDatabaseName("IX_Settlements_SettlementBatchId");

        builder.HasIndex(s => s.PaymentId)
            .HasDatabaseName("IX_Settlements_PaymentId");

        builder.HasIndex(s => s.Status)
            .HasDatabaseName("IX_Settlements_Status");

        builder.HasOne(s => s.SettlementBatch)
            .WithMany(sb => sb.Settlements)
            .HasForeignKey(s => s.SettlementBatchId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.Payment)
            .WithMany(p => p.Settlements)
            .HasForeignKey(s => s.PaymentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(s => !s.IsDeleted);

        builder.Ignore(s => s.CreatedBy);
        builder.Ignore(s => s.UpdatedBy);
    }
}
