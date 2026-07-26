using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class AcademySportConfiguration : IEntityTypeConfiguration<AcademySport>
{
    public void Configure(EntityTypeBuilder<AcademySport> builder)
    {
        builder.ToTable("AcademySports");

        builder.HasKey(s => s.Id);

        builder.HasIndex(s => new { s.AcademyId, s.SportId })
            .IsUnique()
            .HasDatabaseName("IX_AcademySports_AcademyId_SportId");

        builder.HasIndex(s => s.AcademyId)
            .HasDatabaseName("IX_AcademySports_AcademyId");

        builder.HasIndex(s => s.SportId)
            .HasDatabaseName("IX_AcademySports_SportId");

        builder.HasOne(s => s.Academy)
            .WithMany(a => a.AcademySports)
            .HasForeignKey(s => s.AcademyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.Sport)
            .WithMany(s => s.AcademySports)
            .HasForeignKey(s => s.SportId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(s => s.CreatedBy);
        builder.Ignore(s => s.UpdatedBy);
    }
}
