using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.Finance;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.Finance;

public class LedgerEntryConfiguration : IEntityTypeConfiguration<LedgerEntry>
{
    public void Configure(EntityTypeBuilder<LedgerEntry> builder)
    {
        builder.ToTable("LedgerEntries");

        builder.HasKey(le => le.Id);

        builder.Property(le => le.DebitAmount)
            .HasPrecision(18, 2);

        builder.Property(le => le.CreditAmount)
            .HasPrecision(18, 2);

        builder.Property(le => le.Reference)
            .HasMaxLength(200);

        builder.Property(le => le.Description)
            .HasMaxLength(500);

        builder.HasIndex(le => le.LedgerId)
            .HasDatabaseName("IX_LedgerEntries_LedgerId");

        builder.HasIndex(le => le.EntryDate)
            .HasDatabaseName("IX_LedgerEntries_EntryDate");

        builder.HasOne(le => le.Ledger)
            .WithMany(l => l.Entries)
            .HasForeignKey(le => le.LedgerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(le => !le.IsDeleted);

        builder.Ignore(le => le.CreatedBy);
        builder.Ignore(le => le.UpdatedBy);
    }
}
