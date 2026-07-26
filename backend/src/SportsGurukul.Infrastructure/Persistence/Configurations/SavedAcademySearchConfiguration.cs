using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class SavedAcademySearchConfiguration : IEntityTypeConfiguration<SavedAcademySearch>
{
    public void Configure(EntityTypeBuilder<SavedAcademySearch> builder)
    {
        builder.ToTable("SavedAcademySearches");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.SearchName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.SearchTerm)
            .HasMaxLength(500);

        builder.Property(x => x.Name)
            .HasMaxLength(200);

        builder.Property(x => x.City)
            .HasMaxLength(100);

        builder.Property(x => x.State)
            .HasMaxLength(100);

        builder.Property(x => x.Country)
            .HasMaxLength(100);

        builder.Property(x => x.District)
            .HasMaxLength(100);

        builder.Property(x => x.PinCode)
            .HasMaxLength(20);

        builder.Property(x => x.SportName)
            .HasMaxLength(100);

        builder.Property(x => x.SportCategory)
            .HasMaxLength(100);

        builder.Property(x => x.FacilityType)
            .HasMaxLength(100);

        builder.Ignore(x => x.CreatedBy);
        builder.Ignore(x => x.UpdatedBy);

        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasIndex(x => x.UserId)
            .HasDatabaseName("IX_SavedAcademySearches_UserId");

        builder.HasIndex(x => new { x.UserId, x.SearchName })
            .IsUnique()
            .HasDatabaseName("IX_SavedAcademySearches_UserId_SearchName");
    }
}
