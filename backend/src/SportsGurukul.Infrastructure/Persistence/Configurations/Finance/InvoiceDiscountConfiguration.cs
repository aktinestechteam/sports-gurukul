using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.Finance;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.Finance;

public class InvoiceDiscountConfiguration : IEntityTypeConfiguration<InvoiceDiscount>
{
    public void Configure(EntityTypeBuilder<InvoiceDiscount> builder)
    {
        builder.ToTable("InvoiceDiscounts");

        builder.HasKey(id => id.Id);

        builder.Property(id => id.DiscountName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(id => id.DiscountType)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(id => id.DiscountValue)
            .HasPrecision(18, 2);

        builder.Property(id => id.DiscountAmount)
            .HasPrecision(18, 2);

        builder.HasIndex(id => id.InvoiceId)
            .HasDatabaseName("IX_InvoiceDiscounts_InvoiceId");

        builder.HasOne(id => id.Invoice)
            .WithMany(i => i.Discounts)
            .HasForeignKey(id => id.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(id => !id.IsDeleted);

        builder.Ignore(id => id.CreatedBy);
        builder.Ignore(id => id.UpdatedBy);
    }
}
