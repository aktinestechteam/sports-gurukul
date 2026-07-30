using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.Finance;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.Finance;

public class CouponUsageConfiguration : IEntityTypeConfiguration<CouponUsage>
{
    public void Configure(EntityTypeBuilder<CouponUsage> builder)
    {
        builder.ToTable("CouponUsages");

        builder.HasKey(cu => cu.Id);

        builder.Property(cu => cu.DiscountAmount)
            .HasPrecision(18, 2);

        builder.HasIndex(cu => cu.CouponId)
            .HasDatabaseName("IX_CouponUsages_CouponId");

        builder.HasIndex(cu => cu.UserId)
            .HasDatabaseName("IX_CouponUsages_UserId");

        builder.HasIndex(cu => new { cu.CouponId, cu.UserId })
            .HasDatabaseName("IX_CouponUsages_CouponId_UserId");

        builder.HasOne(cu => cu.Coupon)
            .WithMany(c => c.Usages)
            .HasForeignKey(cu => cu.CouponId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(cu => cu.User)
            .WithMany()
            .HasForeignKey(cu => cu.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(cu => !cu.IsDeleted);

        builder.Ignore(cu => cu.CreatedBy);
        builder.Ignore(cu => cu.UpdatedBy);
    }
}
