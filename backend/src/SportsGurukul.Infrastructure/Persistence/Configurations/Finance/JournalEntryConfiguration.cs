using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.Finance;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.Finance;

public class JournalEntryConfiguration : IEntityTypeConfiguration<JournalEntry>
{
    public void Configure(EntityTypeBuilder<JournalEntry> builder)
    {
        builder.ToTable("JournalEntries");

        builder.HasKey(je => je.Id);

        builder.Property(je => je.AccountCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(je => je.DebitAmount)
            .HasPrecision(18, 2);

        builder.Property(je => je.CreditAmount)
            .HasPrecision(18, 2);

        builder.Property(je => je.Description)
            .HasMaxLength(500);

        builder.HasIndex(je => je.JournalId)
            .HasDatabaseName("IX_JournalEntries_JournalId");

        builder.HasOne(je => je.Journal)
            .WithMany(j => j.Entries)
            .HasForeignKey(je => je.JournalId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(je => !je.IsDeleted);

        builder.Ignore(je => je.CreatedBy);
        builder.Ignore(je => je.UpdatedBy);
    }
}
