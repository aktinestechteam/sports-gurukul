using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class EventVenueConfiguration : IEntityTypeConfiguration<EventVenue>
{
    public void Configure(EntityTypeBuilder<EventVenue> builder)
    {
        builder.ToTable("EventVenues");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.VenueName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.Address)
            .HasMaxLength(500);

        builder.Property(e => e.City)
            .HasMaxLength(100);

        builder.Property(e => e.State)
            .HasMaxLength(100);

        builder.Property(e => e.Country)
            .HasMaxLength(100);

        builder.Property(e => e.PostalCode)
            .HasMaxLength(20);

        builder.Property(e => e.Latitude)
            .HasPrecision(10, 7);

        builder.Property(e => e.Longitude)
            .HasPrecision(10, 7);

        builder.Property(e => e.MapUrl)
            .HasMaxLength(500);

        builder.Property(e => e.Instructions)
            .HasMaxLength(2000);

        builder.Property(e => e.RowVersion)
            .IsRowVersion();

        builder.HasIndex(e => e.EventId)
            .HasDatabaseName("IX_EventVenues_EventId");

        builder.HasIndex(e => e.FacilityId)
            .HasDatabaseName("IX_EventVenues_FacilityId");

        builder.HasOne(e => e.Event)
            .WithMany(ev => ev.Venues)
            .HasForeignKey(e => e.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Facility)
            .WithMany()
            .HasForeignKey(e => e.FacilityId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.Ignore(e => e.CreatedBy);
        builder.Ignore(e => e.UpdatedBy);
    }
}
