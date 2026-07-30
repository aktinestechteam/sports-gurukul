using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.Finance;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.Finance;

public class ReceiptConfiguration : IEntityTypeConfiguration<Receipt>
{
    public void Configure(EntityTypeBuilder<Receipt> builder)
    {
        builder.ToTable("Receipts");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.ReceiptNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(r => r.Notes)
            .HasMaxLength(500);

        builder.HasIndex(r => r.ReceiptNumber)
            .IsUnique()
            .HasDatabaseName("IX_Receipts_ReceiptNumber");

        builder.HasIndex(r => r.PaymentId)
            .HasDatabaseName("IX_Receipts_PaymentId");

        builder.HasOne(r => r.Payment)
            .WithMany(p => p.Receipts)
            .HasForeignKey(r => r.PaymentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(r => !r.IsDeleted);

        builder.Ignore(r => r.CreatedBy);
        builder.Ignore(r => r.UpdatedBy);
    }
}
