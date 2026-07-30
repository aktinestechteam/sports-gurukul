using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.Finance;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.Finance;

public class WalletTransactionConfiguration : IEntityTypeConfiguration<WalletTransaction>
{
    public void Configure(EntityTypeBuilder<WalletTransaction> builder)
    {
        builder.ToTable("WalletTransactions");

        builder.HasKey(wt => wt.Id);

        builder.Property(wt => wt.TransactionType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(wt => wt.Amount)
            .HasPrecision(18, 2);

        builder.Property(wt => wt.BalanceBefore)
            .HasPrecision(18, 2);

        builder.Property(wt => wt.BalanceAfter)
            .HasPrecision(18, 2);

        builder.Property(wt => wt.Reference)
            .HasMaxLength(200);

        builder.Property(wt => wt.Description)
            .HasMaxLength(500);

        builder.HasIndex(wt => wt.WalletId)
            .HasDatabaseName("IX_WalletTransactions_WalletId");

        builder.HasIndex(wt => new { wt.WalletId, wt.CreatedAt })
            .HasDatabaseName("IX_WalletTransactions_WalletId_CreatedAt");

        builder.HasOne(wt => wt.Wallet)
            .WithMany(w => w.Transactions)
            .HasForeignKey(wt => wt.WalletId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(wt => !wt.IsDeleted);

        builder.Ignore(wt => wt.CreatedBy);
        builder.Ignore(wt => wt.UpdatedBy);
    }
}
