using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.Finance;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.Finance;

public class DebitNoteConfiguration : IEntityTypeConfiguration<DebitNote>
{
    public void Configure(EntityTypeBuilder<DebitNote> builder)
    {
        builder.ToTable("DebitNotes");

        builder.HasKey(dn => dn.Id);

        builder.Property(dn => dn.DebitNoteNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(dn => dn.Reason)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(dn => dn.TotalAmount)
            .HasPrecision(18, 2);

        builder.Property(dn => dn.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.HasIndex(dn => dn.DebitNoteNumber)
            .IsUnique()
            .HasDatabaseName("IX_DebitNotes_DebitNoteNumber");

        builder.HasIndex(dn => dn.InvoiceId)
            .HasDatabaseName("IX_DebitNotes_InvoiceId");

        builder.HasOne(dn => dn.Invoice)
            .WithMany(i => i.DebitNotes)
            .HasForeignKey(dn => dn.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(dn => !dn.IsDeleted);

        builder.Ignore(dn => dn.CreatedBy);
        builder.Ignore(dn => dn.UpdatedBy);
    }
}
