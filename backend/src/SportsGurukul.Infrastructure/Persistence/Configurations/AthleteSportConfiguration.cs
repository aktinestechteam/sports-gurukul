using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class AthleteSportConfiguration : IEntityTypeConfiguration<AthleteSport>
{
    public void Configure(EntityTypeBuilder<AthleteSport> builder)
    {
        builder.ToTable("AthleteSports");

        builder.HasKey(a => a.Id);

        builder.HasIndex(a => new { a.AthleteId, a.SportId })
            .IsUnique()
            .HasDatabaseName("IX_AthleteSports_AthleteId_SportId");

        builder.HasIndex(a => a.AthleteId)
            .HasDatabaseName("IX_AthleteSports_AthleteId");

        builder.HasIndex(a => a.SportId)
            .HasDatabaseName("IX_AthleteSports_SportId");

        builder.HasOne(a => a.Athlete)
            .WithMany(a => a.AthleteSports)
            .HasForeignKey(a => a.AthleteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.Sport)
            .WithMany(s => s.AthleteSports)
            .HasForeignKey(a => a.SportId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(a => a.CreatedBy);
        builder.Ignore(a => a.UpdatedBy);
    }
}
