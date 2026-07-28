using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class EventSponsorConfiguration : IEntityTypeConfiguration<EventSponsor>
{
    public void Configure(EntityTypeBuilder<EventSponsor> builder)
    {
        builder.ToTable("EventSponsors");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.SponsorName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.ContactPerson)
            .HasMaxLength(200);

        builder.Property(e => e.ContactEmail)
            .HasMaxLength(200);

        builder.Property(e => e.ContactPhone)
            .HasMaxLength(20);

        builder.Property(e => e.Website)
            .HasMaxLength(500);

        builder.Property(e => e.LogoUrl)
            .HasMaxLength(500);

        builder.Property(e => e.ContributionAmount)
            .HasPrecision(12, 2);

        builder.Property(e => e.ContributionDescription)
            .HasMaxLength(2000);

        builder.Property(e => e.Tier)
            .HasMaxLength(50);

        builder.Property(e => e.Notes)
            .HasMaxLength(2000);

        builder.Property(e => e.RowVersion)
            .IsRowVersion();

        builder.HasIndex(e => e.EventId)
            .HasDatabaseName("IX_EventSponsors_EventId");

        builder.HasOne(e => e.Event)
            .WithMany(ev => ev.Sponsors)
            .HasForeignKey(e => e.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.Ignore(e => e.CreatedBy);
        builder.Ignore(e => e.UpdatedBy);
    }
}
