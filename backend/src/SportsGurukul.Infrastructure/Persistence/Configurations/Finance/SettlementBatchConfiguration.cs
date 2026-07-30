using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.Finance;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.Finance;

public class SettlementBatchConfiguration : IEntityTypeConfiguration<SettlementBatch>
{
    public void Configure(EntityTypeBuilder<SettlementBatch> builder)
    {
        builder.ToTable("SettlementBatches");

        builder.HasKey(sb => sb.Id);

        builder.Property(sb => sb.BatchNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(sb => sb.TotalAmount)
            .HasPrecision(18, 2);

        builder.Property(sb => sb.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.HasIndex(sb => sb.BatchNumber)
            .IsUnique()
            .HasDatabaseName("IX_SettlementBatches_BatchNumber");

        builder.HasQueryFilter(sb => !sb.IsDeleted);

        builder.Ignore(sb => sb.CreatedBy);
        builder.Ignore(sb => sb.UpdatedBy);
    }
}
