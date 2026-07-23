using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class SportCategoryConfiguration : IEntityTypeConfiguration<SportCategory>
{
    public void Configure(EntityTypeBuilder<SportCategory> builder)
    {
        builder.ToTable("SportCategories");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .HasMaxLength(100);

        builder.Property(c => c.Description)
            .HasMaxLength(500);

        builder.HasIndex(c => c.Name)
            .IsUnique()
            .HasDatabaseName("IX_SportCategories_Name");

        builder.Ignore(c => c.CreatedBy);
        builder.Ignore(c => c.UpdatedBy);

        builder.HasData(
            new SportCategory
            {
                Id = Guid.Parse("b1000000-0000-0000-0000-000000000001"),
                Name = "Team Sports",
                Description = "Sports played between two teams",
                IsDeleted = false
            },
            new SportCategory
            {
                Id = Guid.Parse("b1000000-0000-0000-0000-000000000002"),
                Name = "Racquet Sports",
                Description = "Sports played with racquets",
                IsDeleted = false
            },
            new SportCategory
            {
                Id = Guid.Parse("b1000000-0000-0000-0000-000000000003"),
                Name = "Individual Sports",
                Description = "Individual competitive sports",
                IsDeleted = false
            },
            new SportCategory
            {
                Id = Guid.Parse("b1000000-0000-0000-0000-000000000004"),
                Name = "Combat Sports",
                Description = "Martial arts and combat disciplines",
                IsDeleted = false
            },
            new SportCategory
            {
                Id = Guid.Parse("b1000000-0000-0000-0000-000000000005"),
                Name = "Aquatic Sports",
                Description = "Water-based sports",
                IsDeleted = false
            }
        );
    }
}
