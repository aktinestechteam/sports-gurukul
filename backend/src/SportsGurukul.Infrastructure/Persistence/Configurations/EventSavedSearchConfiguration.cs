using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class EventSavedSearchConfiguration : IEntityTypeConfiguration<EventSavedSearch>
{
    public void Configure(EntityTypeBuilder<EventSavedSearch> builder)
    {
        builder.ToTable("EventSavedSearches");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.SearchName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.SearchTerm)
            .HasMaxLength(500);

        builder.Property(e => e.SportName)
            .HasMaxLength(100);

        builder.Property(e => e.AcademyName)
            .HasMaxLength(200);

        builder.Property(e => e.CoachName)
            .HasMaxLength(200);

        builder.Property(e => e.SpeakerName)
            .HasMaxLength(200);

        builder.Property(e => e.VenueName)
            .HasMaxLength(200);

        builder.Property(e => e.City)
            .HasMaxLength(100);

        builder.Property(e => e.State)
            .HasMaxLength(100);

        builder.Property(e => e.Country)
            .HasMaxLength(100);

        builder.Property(e => e.EventType)
            .HasMaxLength(100);

        builder.Property(e => e.Category)
            .HasMaxLength(100);

        builder.Property(e => e.SkillLevel)
            .HasMaxLength(50);

        builder.Property(e => e.AgeGroup)
            .HasMaxLength(50);

        builder.Property(e => e.Language)
            .HasMaxLength(50);

        builder.Property(e => e.SortBy)
            .HasMaxLength(50);

        builder.Property(e => e.MinPrice)
            .HasPrecision(10, 2);

        builder.Property(e => e.MaxPrice)
            .HasPrecision(10, 2);

        builder.Property(e => e.RowVersion)
            .IsRowVersion();

        builder.HasIndex(e => e.UserId)
            .HasDatabaseName("IX_EventSavedSearches_UserId");

        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
