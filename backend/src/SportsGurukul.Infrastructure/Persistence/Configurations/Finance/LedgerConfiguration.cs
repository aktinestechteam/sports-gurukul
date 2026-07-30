using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.Finance;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.Finance;

public class LedgerConfiguration : IEntityTypeConfiguration<Ledger>
{
    public void Configure(EntityTypeBuilder<Ledger> builder)
    {
        builder.ToTable("Ledgers");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(l => l.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(l => l.Type)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(l => l.Description)
            .HasMaxLength(500);

        builder.HasIndex(l => l.Code)
            .IsUnique()
            .HasDatabaseName("IX_Ledgers_Code");

        builder.HasQueryFilter(l => !l.IsDeleted);

        builder.Ignore(l => l.CreatedBy);
        builder.Ignore(l => l.UpdatedBy);
    }
}
