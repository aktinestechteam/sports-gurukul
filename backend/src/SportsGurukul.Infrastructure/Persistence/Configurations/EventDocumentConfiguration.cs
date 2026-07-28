using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class EventDocumentConfiguration : IEntityTypeConfiguration<EventDocument>
{
    public void Configure(EntityTypeBuilder<EventDocument> builder)
    {
        builder.ToTable("EventDocuments");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.DocumentType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(e => e.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.Description)
            .HasMaxLength(2000);

        builder.Property(e => e.Url)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(e => e.FileName)
            .HasMaxLength(200);

        builder.Property(e => e.ContentType)
            .HasMaxLength(100);

        builder.Property(e => e.RowVersion)
            .IsRowVersion();

        builder.HasIndex(e => e.EventId)
            .HasDatabaseName("IX_EventDocuments_EventId");

        builder.HasIndex(e => e.DocumentType)
            .HasDatabaseName("IX_EventDocuments_DocumentType");

        builder.HasIndex(e => new { e.EventId, e.DocumentType })
            .HasDatabaseName("IX_EventDocuments_EventId_DocumentType");

        builder.HasOne(e => e.Event)
            .WithMany(ev => ev.Documents)
            .HasForeignKey(e => e.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.Ignore(e => e.CreatedBy);
        builder.Ignore(e => e.UpdatedBy);
    }
}
