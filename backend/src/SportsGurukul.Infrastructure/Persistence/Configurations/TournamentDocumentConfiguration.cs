using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class TournamentDocumentConfiguration : IEntityTypeConfiguration<TournamentDocument>
{
    public void Configure(EntityTypeBuilder<TournamentDocument> builder)
    {
        builder.ToTable("TournamentDocuments");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.DocumentType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(d => d.DocumentName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(d => d.Description)
            .HasMaxLength(1000);

        builder.Property(d => d.FileUrl)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(d => d.FileName)
            .HasMaxLength(200);

        builder.Property(d => d.ContentType)
            .HasMaxLength(100);

        builder.Property(d => d.RowVersion)
            .IsRowVersion();

        builder.HasIndex(d => d.TournamentId)
            .HasDatabaseName("IX_TournamentDocuments_TournamentId");

        builder.HasIndex(d => d.DocumentType)
            .HasDatabaseName("IX_TournamentDocuments_DocumentType");

        builder.HasOne(d => d.Tournament)
            .WithMany(t => t.Documents)
            .HasForeignKey(d => d.TournamentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(d => d.CreatedBy);
        builder.Ignore(d => d.UpdatedBy);
    }
}
