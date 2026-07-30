using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.Finance;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.Finance;

public class FeeCategoryConfiguration : IEntityTypeConfiguration<FeeCategory>
{
    public void Configure(EntityTypeBuilder<FeeCategory> builder)
    {
        builder.ToTable("FeeCategories");

        builder.HasKey(fc => fc.Id);

        builder.Property(fc => fc.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(fc => fc.Description)
            .HasMaxLength(500);

        builder.Property(fc => fc.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(fc => fc.Code)
            .IsUnique()
            .HasDatabaseName("IX_FeeCategories_Code");

        builder.HasQueryFilter(fc => !fc.IsDeleted);

        builder.Ignore(fc => fc.CreatedBy);
        builder.Ignore(fc => fc.UpdatedBy);

        builder.HasData(
            new FeeCategory { Id = Guid.Parse("A1000000-0000-0000-0000-000000000001"), Name = "Registration Fee", Description = "One-time registration fee", Code = "REG", IsActive = true },
            new FeeCategory { Id = Guid.Parse("A1000000-0000-0000-0000-000000000002"), Name = "Tuition Fee", Description = "Regular training tuition fee", Code = "TUITION", IsActive = true },
            new FeeCategory { Id = Guid.Parse("A1000000-0000-0000-0000-000000000003"), Name = "Tournament Fee", Description = "Tournament participation fee", Code = "TOURNAMENT", IsActive = true },
            new FeeCategory { Id = Guid.Parse("A1000000-0000-0000-0000-000000000004"), Name = "Membership Fee", Description = "Academy membership fee", Code = "MEMBERSHIP", IsActive = true },
            new FeeCategory { Id = Guid.Parse("A1000000-0000-0000-0000-000000000005"), Name = "Facility Fee", Description = "Facility usage fee", Code = "FACILITY", IsActive = true },
            new FeeCategory { Id = Guid.Parse("A1000000-0000-0000-0000-000000000006"), Name = "Equipment Fee", Description = "Equipment rental or purchase fee", Code = "EQUIPMENT", IsActive = true }
        );
    }
}
