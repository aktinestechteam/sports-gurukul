using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class RankingConfiguration : IEntityTypeConfiguration<Ranking>
{
    public void Configure(EntityTypeBuilder<Ranking> builder)
    {
        builder.ToTable("Rankings");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.CurrentRank)
            .HasMaxLength(50);

        builder.Property(r => r.StateRank)
            .HasMaxLength(50);

        builder.Property(r => r.NationalRank)
            .HasMaxLength(50);

        builder.Property(r => r.InternationalRank)
            .HasMaxLength(50);

        builder.Property(r => r.RankingAuthority)
            .HasMaxLength(200);

        builder.Property(r => r.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(r => r.AthleteId)
            .IsUnique()
            .HasDatabaseName("IX_Rankings_AthleteId");

        builder.HasOne(r => r.Athlete)
            .WithOne(a => a.Ranking)
            .HasForeignKey<Ranking>(r => r.AthleteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(r => r.CreatedBy);
        builder.Ignore(r => r.UpdatedBy);
    }
}
