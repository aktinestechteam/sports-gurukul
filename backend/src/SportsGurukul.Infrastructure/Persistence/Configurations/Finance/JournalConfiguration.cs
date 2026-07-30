using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.Finance;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.Finance;

public class JournalConfiguration : IEntityTypeConfiguration<Journal>
{
    public void Configure(EntityTypeBuilder<Journal> builder)
    {
        builder.ToTable("Journals");

        builder.HasKey(j => j.Id);

        builder.Property(j => j.JournalNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(j => j.Description)
            .HasMaxLength(500);

        builder.Property(j => j.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(j => j.Period)
            .HasMaxLength(20);

        builder.HasIndex(j => j.JournalNumber)
            .IsUnique()
            .HasDatabaseName("IX_Journals_JournalNumber");

        builder.HasIndex(j => j.JournalDate)
            .HasDatabaseName("IX_Journals_JournalDate");

        builder.HasIndex(j => j.Status)
            .HasDatabaseName("IX_Journals_Status");

        builder.HasQueryFilter(j => !j.IsDeleted);

        builder.Ignore(j => j.CreatedBy);
        builder.Ignore(j => j.UpdatedBy);
    }
}
