using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.Finance;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.Finance;

public class CreditNoteConfiguration : IEntityTypeConfiguration<CreditNote>
{
    public void Configure(EntityTypeBuilder<CreditNote> builder)
    {
        builder.ToTable("CreditNotes");

        builder.HasKey(cn => cn.Id);

        builder.Property(cn => cn.CreditNoteNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(cn => cn.Reason)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(cn => cn.TotalAmount)
            .HasPrecision(18, 2);

        builder.Property(cn => cn.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.HasIndex(cn => cn.CreditNoteNumber)
            .IsUnique()
            .HasDatabaseName("IX_CreditNotes_CreditNoteNumber");

        builder.HasIndex(cn => cn.InvoiceId)
            .HasDatabaseName("IX_CreditNotes_InvoiceId");

        builder.HasOne(cn => cn.Invoice)
            .WithMany(i => i.CreditNotes)
            .HasForeignKey(cn => cn.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(cn => !cn.IsDeleted);

        builder.Ignore(cn => cn.CreatedBy);
        builder.Ignore(cn => cn.UpdatedBy);
    }
}
