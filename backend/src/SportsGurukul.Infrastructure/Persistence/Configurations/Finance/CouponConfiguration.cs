using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.Finance;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.Finance;

public class CouponConfiguration : IEntityTypeConfiguration<Coupon>
{
    public void Configure(EntityTypeBuilder<Coupon> builder)
    {
        builder.ToTable("Coupons");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Code)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.Type)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(c => c.Value)
            .HasPrecision(18, 2);

        builder.Property(c => c.MinOrderAmount)
            .HasPrecision(18, 2);

        builder.Property(c => c.MaxDiscountAmount)
            .HasPrecision(18, 2);

        builder.HasIndex(c => c.Code)
            .IsUnique()
            .HasDatabaseName("IX_Coupons_Code");

        builder.HasIndex(c => new { c.IsActive, c.ValidFrom, c.ValidTo })
            .HasDatabaseName("IX_Coupons_ActiveValidity");

        builder.HasQueryFilter(c => !c.IsDeleted);

        builder.Ignore(c => c.CreatedBy);
        builder.Ignore(c => c.UpdatedBy);
    }
}
