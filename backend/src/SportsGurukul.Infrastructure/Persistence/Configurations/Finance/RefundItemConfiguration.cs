using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.Finance;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.Finance;

public class RefundItemConfiguration : IEntityTypeConfiguration<RefundItem>
{
    public void Configure(EntityTypeBuilder<RefundItem> builder)
    {
        builder.ToTable("RefundItems");

        builder.HasKey(ri => ri.Id);

        builder.Property(ri => ri.Description)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(ri => ri.Amount)
            .HasPrecision(18, 2);

        builder.HasIndex(ri => ri.RefundId)
            .HasDatabaseName("IX_RefundItems_RefundId");

        builder.HasOne(ri => ri.Refund)
            .WithMany(r => r.RefundItems)
            .HasForeignKey(ri => ri.RefundId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(ri => !ri.IsDeleted);

        builder.Ignore(ri => ri.CreatedBy);
        builder.Ignore(ri => ri.UpdatedBy);
    }
}
