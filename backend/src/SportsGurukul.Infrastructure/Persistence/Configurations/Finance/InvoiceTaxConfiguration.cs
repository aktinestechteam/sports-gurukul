using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.Finance;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.Finance;

public class InvoiceTaxConfiguration : IEntityTypeConfiguration<InvoiceTax>
{
    public void Configure(EntityTypeBuilder<InvoiceTax> builder)
    {
        builder.ToTable("InvoiceTaxes");

        builder.HasKey(it => it.Id);

        builder.Property(it => it.TaxName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(it => it.TaxRate)
            .HasPrecision(18, 2);

        builder.Property(it => it.TaxAmount)
            .HasPrecision(18, 2);

        builder.HasIndex(it => it.InvoiceId)
            .HasDatabaseName("IX_InvoiceTaxes_InvoiceId");

        builder.HasOne(it => it.Invoice)
            .WithMany(i => i.Taxes)
            .HasForeignKey(it => it.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(it => !it.IsDeleted);

        builder.Ignore(it => it.CreatedBy);
        builder.Ignore(it => it.UpdatedBy);
    }
}
