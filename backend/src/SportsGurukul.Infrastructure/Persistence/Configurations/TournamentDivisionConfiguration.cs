using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class TournamentDivisionConfiguration : IEntityTypeConfiguration<TournamentDivision>
{
    public void Configure(EntityTypeBuilder<TournamentDivision> builder)
    {
        builder.ToTable("TournamentDivisions");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.DivisionName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(d => d.Description)
            .HasMaxLength(1000);

        builder.Property(d => d.RowVersion)
            .IsRowVersion();

        builder.HasIndex(d => d.TournamentId)
            .HasDatabaseName("IX_TournamentDivisions_TournamentId");

        builder.HasIndex(d => d.CategoryId)
            .HasDatabaseName("IX_TournamentDivisions_CategoryId");

        builder.HasOne(d => d.Tournament)
            .WithMany()
            .HasForeignKey(d => d.TournamentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(d => d.Category)
            .WithMany()
            .HasForeignKey(d => d.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Ignore(d => d.CreatedBy);
        builder.Ignore(d => d.UpdatedBy);
    }
}
