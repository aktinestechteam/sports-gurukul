using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class CoachSportConfiguration : IEntityTypeConfiguration<CoachSport>
{
    public void Configure(EntityTypeBuilder<CoachSport> builder)
    {
        builder.ToTable("CoachSports");

        builder.HasKey(cs => cs.Id);

        builder.HasIndex(cs => new { cs.CoachId, cs.SportId })
            .IsUnique()
            .HasDatabaseName("IX_CoachSports_CoachId_SportId");

        builder.HasIndex(cs => cs.CoachId)
            .HasDatabaseName("IX_CoachSports_CoachId");

        builder.HasIndex(cs => cs.SportId)
            .HasDatabaseName("IX_CoachSports_SportId");

        builder.HasOne(cs => cs.Coach)
            .WithMany(c => c.CoachSports)
            .HasForeignKey(cs => cs.CoachId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(cs => cs.Sport)
            .WithMany(s => s.CoachSports)
            .HasForeignKey(cs => cs.SportId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(cs => cs.CreatedBy);
        builder.Ignore(cs => cs.UpdatedBy);
    }
}
