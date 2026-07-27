using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class TournamentBracketConfiguration : IEntityTypeConfiguration<TournamentBracket>
{
    public void Configure(EntityTypeBuilder<TournamentBracket> builder)
    {
        builder.ToTable("TournamentBrackets");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.BracketName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(b => b.BracketType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(b => b.BracketData)
            .HasMaxLength(10000);

        builder.Property(b => b.RowVersion)
            .IsRowVersion();

        builder.HasIndex(b => b.TournamentId)
            .HasDatabaseName("IX_TournamentBrackets_TournamentId");

        builder.HasIndex(b => b.DivisionId)
            .HasDatabaseName("IX_TournamentBrackets_DivisionId");

        builder.HasOne(b => b.Tournament)
            .WithMany()
            .HasForeignKey(b => b.TournamentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(b => b.Division)
            .WithMany(d => d.Brackets)
            .HasForeignKey(b => b.DivisionId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Ignore(b => b.CreatedBy);
        builder.Ignore(b => b.UpdatedBy);
    }
}
