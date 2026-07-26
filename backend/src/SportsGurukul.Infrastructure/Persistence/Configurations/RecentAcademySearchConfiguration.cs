using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class RecentAcademySearchConfiguration : IEntityTypeConfiguration<RecentAcademySearch>
{
    public void Configure(EntityTypeBuilder<RecentAcademySearch> builder)
    {
        builder.ToTable("RecentAcademySearches");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.SearchTerm)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.City)
            .HasMaxLength(100);

        builder.Property(x => x.State)
            .HasMaxLength(100);

        builder.Property(x => x.SportName)
            .HasMaxLength(100);

        builder.Ignore(x => x.CreatedBy);
        builder.Ignore(x => x.UpdatedBy);

        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasIndex(x => x.UserId)
            .HasDatabaseName("IX_RecentAcademySearches_UserId");

        builder.HasIndex(x => new { x.UserId, x.SearchedAt })
            .HasDatabaseName("IX_RecentAcademySearches_UserId_SearchedAt");
    }
}
