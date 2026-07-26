using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class FacilityPricingConfiguration : IEntityTypeConfiguration<FacilityPricing>
{
    public void Configure(EntityTypeBuilder<FacilityPricing> builder)
    {
        builder.ToTable("FacilityPricing");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.PricingName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.HourlyRate)
            .HasPrecision(18, 2);

        builder.Property(p => p.DailyRate)
            .HasPrecision(18, 2);

        builder.Property(p => p.MonthlyRate)
            .HasPrecision(18, 2);

        builder.Property(p => p.PeakHourlyRate)
            .HasPrecision(18, 2);

        builder.Property(p => p.OffPeakHourlyRate)
            .HasPrecision(18, 2);

        builder.Property(p => p.Description)
            .HasMaxLength(500);

        builder.HasIndex(p => p.FacilityId)
            .HasDatabaseName("IX_FacilityPricing_FacilityId");

        builder.HasOne(p => p.Facility)
            .WithMany(f => f.PricingTiers)
            .HasForeignKey(p => p.FacilityId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(p => p.CreatedBy);
        builder.Ignore(p => p.UpdatedBy);
    }
}
