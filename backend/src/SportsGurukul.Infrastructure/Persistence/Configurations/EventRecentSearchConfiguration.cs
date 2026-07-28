using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class EventRecentSearchConfiguration : IEntityTypeConfiguration<EventRecentSearch>
{
    public void Configure(EntityTypeBuilder<EventRecentSearch> builder)
    {
        builder.ToTable("EventRecentSearches");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.SearchTerm)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(e => e.SportName)
            .HasMaxLength(100);

        builder.Property(e => e.City)
            .HasMaxLength(100);

        builder.Property(e => e.State)
            .HasMaxLength(100);

        builder.Property(e => e.EventType)
            .HasMaxLength(100);

        builder.Property(e => e.FiltersJson)
            .HasMaxLength(4000);

        builder.Property(e => e.RowVersion)
            .IsRowVersion();

        builder.HasIndex(e => e.UserId)
            .HasDatabaseName("IX_EventRecentSearches_UserId");

        builder.HasIndex(e => e.SearchedAt)
            .HasDatabaseName("IX_EventRecentSearches_SearchedAt");

        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
