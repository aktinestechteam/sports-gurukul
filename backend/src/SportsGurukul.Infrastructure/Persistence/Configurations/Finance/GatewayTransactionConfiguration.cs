using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.Finance;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.Finance;

public class GatewayTransactionConfiguration : IEntityTypeConfiguration<GatewayTransaction>
{
    public void Configure(EntityTypeBuilder<GatewayTransaction> builder)
    {
        builder.ToTable("GatewayTransactions");

        builder.HasKey(gt => gt.Id);

        builder.Property(gt => gt.TransactionId)
            .HasMaxLength(200);

        builder.Property(gt => gt.Status)
            .HasMaxLength(50);

        builder.HasIndex(gt => gt.GatewayId)
            .HasDatabaseName("IX_GatewayTransactions_GatewayId");

        builder.HasIndex(gt => gt.PaymentId)
            .HasDatabaseName("IX_GatewayTransactions_PaymentId");

        builder.HasIndex(gt => gt.TransactionId)
            .HasDatabaseName("IX_GatewayTransactions_TransactionId");

        builder.HasOne(gt => gt.Gateway)
            .WithMany(g => g.GatewayTransactions)
            .HasForeignKey(gt => gt.GatewayId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(gt => gt.Payment)
            .WithMany(p => p.GatewayTransactions)
            .HasForeignKey(gt => gt.PaymentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(gt => !gt.IsDeleted);

        builder.Ignore(gt => gt.CreatedBy);
        builder.Ignore(gt => gt.UpdatedBy);
    }
}
