using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.Finance;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.Finance;

public class InvoicePaymentConfiguration : IEntityTypeConfiguration<InvoicePayment>
{
    public void Configure(EntityTypeBuilder<InvoicePayment> builder)
    {
        builder.ToTable("InvoicePayments");

        builder.HasKey(ip => ip.Id);

        builder.Property(ip => ip.AmountApplied)
            .HasPrecision(18, 2);

        builder.HasIndex(ip => ip.InvoiceId)
            .HasDatabaseName("IX_InvoicePayments_InvoiceId");

        builder.HasIndex(ip => ip.PaymentId)
            .HasDatabaseName("IX_InvoicePayments_PaymentId");

        builder.HasOne(ip => ip.Invoice)
            .WithMany(i => i.InvoicePayments)
            .HasForeignKey(ip => ip.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ip => ip.Payment)
            .WithMany(p => p.InvoicePayments)
            .HasForeignKey(ip => ip.PaymentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(ip => !ip.IsDeleted);

        builder.Ignore(ip => ip.CreatedBy);
        builder.Ignore(ip => ip.UpdatedBy);
    }
}
