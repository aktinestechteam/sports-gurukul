using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.Finance;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.Finance;

public class TaxConfigurationConfiguration : IEntityTypeConfiguration<TaxConfiguration>
{
    public void Configure(EntityTypeBuilder<TaxConfiguration> builder)
    {
        builder.ToTable("TaxConfigurations");

        builder.HasKey(tc => tc.Id);

        builder.Property(tc => tc.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(tc => tc.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(tc => tc.Rate)
            .HasPrecision(5, 2);

        builder.Property(tc => tc.Type)
            .HasMaxLength(50);

        builder.Property(tc => tc.Description)
            .HasMaxLength(500);

        builder.HasIndex(tc => tc.Code)
            .IsUnique()
            .HasDatabaseName("IX_TaxConfigurations_Code");

        builder.HasIndex(tc => new { tc.IsActive, tc.EffectiveFrom, tc.EffectiveTo })
            .HasDatabaseName("IX_TaxConfigurations_ActiveValidity");

        builder.HasQueryFilter(tc => !tc.IsDeleted);

        builder.Ignore(tc => tc.CreatedBy);
        builder.Ignore(tc => tc.UpdatedBy);

        builder.HasData(
            new TaxConfiguration { Id = Guid.Parse("C3000000-0000-0000-0000-000000000001"), Name = "GST 5%", Code = "GST5", Rate = 5.00m, Type = "GST", IsActive = true, EffectiveFrom = new DateTime(2024, 7, 1, 0, 0, 0, DateTimeKind.Utc), Description = "Goods and Services Tax 5%" },
            new TaxConfiguration { Id = Guid.Parse("C3000000-0000-0000-0000-000000000002"), Name = "GST 12%", Code = "GST12", Rate = 12.00m, Type = "GST", IsActive = true, EffectiveFrom = new DateTime(2024, 7, 1, 0, 0, 0, DateTimeKind.Utc), Description = "Goods and Services Tax 12%" },
            new TaxConfiguration { Id = Guid.Parse("C3000000-0000-0000-0000-000000000003"), Name = "GST 18%", Code = "GST18", Rate = 18.00m, Type = "GST", IsActive = true, EffectiveFrom = new DateTime(2024, 7, 1, 0, 0, 0, DateTimeKind.Utc), Description = "Goods and Services Tax 18%" },
            new TaxConfiguration { Id = Guid.Parse("C3000000-0000-0000-0000-000000000004"), Name = "GST 28%", Code = "GST28", Rate = 28.00m, Type = "GST", IsActive = true, EffectiveFrom = new DateTime(2024, 7, 1, 0, 0, 0, DateTimeKind.Utc), Description = "Goods and Services Tax 28%" }
        );
    }
}
