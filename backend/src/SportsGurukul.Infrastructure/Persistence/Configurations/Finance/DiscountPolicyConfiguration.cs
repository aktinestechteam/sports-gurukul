using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.Finance;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.Finance;

public class DiscountPolicyConfiguration : IEntityTypeConfiguration<DiscountPolicy>
{
    public void Configure(EntityTypeBuilder<DiscountPolicy> builder)
    {
        builder.ToTable("DiscountPolicies");

        builder.HasKey(dp => dp.Id);

        builder.Property(dp => dp.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(dp => dp.Description)
            .HasMaxLength(1000);

        builder.Property(dp => dp.DiscountType)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(dp => dp.DiscountValue)
            .HasPrecision(18, 2);

        builder.Property(dp => dp.MaxAmount)
            .HasPrecision(18, 2);

        builder.Property(dp => dp.MinOrderAmount)
            .HasPrecision(18, 2);

        builder.HasQueryFilter(dp => !dp.IsDeleted);

        builder.Ignore(dp => dp.CreatedBy);
        builder.Ignore(dp => dp.UpdatedBy);
    }
}
