using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class TournamentSponsorConfiguration : IEntityTypeConfiguration<TournamentSponsor>
{
    public void Configure(EntityTypeBuilder<TournamentSponsor> builder)
    {
        builder.ToTable("TournamentSponsors");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.SponsorName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(s => s.SponsorType)
            .HasMaxLength(50);

        builder.Property(s => s.Amount)
            .HasPrecision(12, 2);

        builder.Property(s => s.ContactPerson)
            .HasMaxLength(200);

        builder.Property(s => s.ContactEmail)
            .HasMaxLength(200);

        builder.Property(s => s.ContactPhone)
            .HasMaxLength(20);

        builder.Property(s => s.LogoUrl)
            .HasMaxLength(500);

        builder.Property(s => s.Website)
            .HasMaxLength(500);

        builder.Property(s => s.RowVersion)
            .IsRowVersion();

        builder.HasIndex(s => s.TournamentId)
            .HasDatabaseName("IX_TournamentSponsors_TournamentId");

        builder.HasOne(s => s.Tournament)
            .WithMany(t => t.Sponsors)
            .HasForeignKey(s => s.TournamentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(s => s.CreatedBy);
        builder.Ignore(s => s.UpdatedBy);
    }
}
