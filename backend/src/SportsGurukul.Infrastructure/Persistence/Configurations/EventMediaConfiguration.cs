using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class EventMediaConfiguration : IEntityTypeConfiguration<EventMedia>
{
    public void Configure(EntityTypeBuilder<EventMedia> builder)
    {
        builder.ToTable("EventMedia");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.MediaType)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.Description)
            .HasMaxLength(2000);

        builder.Property(e => e.Url)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(e => e.ThumbnailUrl)
            .HasMaxLength(500);

        builder.Property(e => e.FileName)
            .HasMaxLength(200);

        builder.Property(e => e.ContentType)
            .HasMaxLength(100);

        builder.Property(e => e.RowVersion)
            .IsRowVersion();

        builder.HasIndex(e => e.EventId)
            .HasDatabaseName("IX_EventMedia_EventId");

        builder.HasIndex(e => e.MediaType)
            .HasDatabaseName("IX_EventMedia_MediaType");

        builder.HasIndex(e => new { e.EventId, e.MediaType })
            .HasDatabaseName("IX_EventMedia_EventId_MediaType");

        builder.HasOne(e => e.Event)
            .WithMany(ev => ev.Media)
            .HasForeignKey(e => e.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.Ignore(e => e.CreatedBy);
        builder.Ignore(e => e.UpdatedBy);
    }
}
