using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.Finance;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.Finance;

public class PaymentTransactionConfiguration : IEntityTypeConfiguration<PaymentTransaction>
{
    public void Configure(EntityTypeBuilder<PaymentTransaction> builder)
    {
        builder.ToTable("PaymentTransactions");

        builder.HasKey(pt => pt.Id);

        builder.Property(pt => pt.TransactionType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(pt => pt.Amount)
            .HasPrecision(18, 2);

        builder.Property(pt => pt.Status)
            .HasMaxLength(50);

        builder.Property(pt => pt.TransactionReference)
            .HasMaxLength(200);

        builder.HasIndex(pt => pt.PaymentId)
            .HasDatabaseName("IX_PaymentTransactions_PaymentId");

        builder.HasOne(pt => pt.Payment)
            .WithMany(p => p.Transactions)
            .HasForeignKey(pt => pt.PaymentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(pt => !pt.IsDeleted);

        builder.Ignore(pt => pt.CreatedBy);
        builder.Ignore(pt => pt.UpdatedBy);
    }
}
