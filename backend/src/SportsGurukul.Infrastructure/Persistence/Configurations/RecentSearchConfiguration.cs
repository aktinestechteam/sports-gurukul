using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class RecentSearchConfiguration : IEntityTypeConfiguration<RecentSearch>
{
    public void Configure(EntityTypeBuilder<RecentSearch> builder)
    {
        builder.ToTable("RecentSearches");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.QueryText)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(s => s.FiltersJson)
            .HasMaxLength(4000)
            .IsRequired();

        builder.Property(s => s.ResultCount)
            .HasDefaultValue(0);

        builder.Property(s => s.SearchedAt)
            .IsRequired();

        builder.HasIndex(s => s.UserId)
            .HasDatabaseName("IX_RecentSearches_UserId");

        builder.HasIndex(s => new { s.UserId, s.SearchedAt })
            .HasDatabaseName("IX_RecentSearches_UserId_SearchedAt");

        builder.HasOne(s => s.User)
            .WithMany()
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(s => s.CreatedBy);
        builder.Ignore(s => s.UpdatedBy);
    }
}
