using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.Finance;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.Finance;

public class InvoiceItemConfiguration : IEntityTypeConfiguration<InvoiceItem>
{
    public void Configure(EntityTypeBuilder<InvoiceItem> builder)
    {
        builder.ToTable("InvoiceItems");

        builder.HasKey(ii => ii.Id);

        builder.Property(ii => ii.Description)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(ii => ii.UnitPrice)
            .HasPrecision(18, 2);

        builder.Property(ii => ii.TotalAmount)
            .HasPrecision(18, 2);

        builder.Property(ii => ii.ReferenceType)
            .HasMaxLength(100);

        builder.HasIndex(ii => ii.InvoiceId)
            .HasDatabaseName("IX_InvoiceItems_InvoiceId");

        builder.HasOne(ii => ii.Invoice)
            .WithMany(i => i.Items)
            .HasForeignKey(ii => ii.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(ii => !ii.IsDeleted);

        builder.Ignore(ii => ii.CreatedBy);
        builder.Ignore(ii => ii.UpdatedBy);
    }
}
