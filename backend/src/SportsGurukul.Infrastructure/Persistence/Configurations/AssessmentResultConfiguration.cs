using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class AssessmentResultConfiguration : IEntityTypeConfiguration<AssessmentResult>
{
    public void Configure(EntityTypeBuilder<AssessmentResult> builder)
    {
        builder.ToTable("AssessmentResults");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Score)
            .HasPrecision(10, 2);

        builder.Property(r => r.Remarks)
            .HasMaxLength(500);

        builder.HasIndex(r => r.AssessmentId)
            .HasDatabaseName("IX_AssessmentResults_AssessmentId");

        builder.HasIndex(r => r.AthleteId)
            .HasDatabaseName("IX_AssessmentResults_AthleteId");

        builder.HasIndex(r => new { r.AssessmentId, r.AthleteId })
            .IsUnique()
            .HasDatabaseName("IX_AssessmentResults_AssessmentId_AthleteId");

        builder.HasIndex(r => r.IsPassed)
            .HasDatabaseName("IX_AssessmentResults_IsPassed");

        builder.HasOne(r => r.Assessment)
            .WithMany(a => a.Results)
            .HasForeignKey(r => r.AssessmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Athlete)
            .WithMany()
            .HasForeignKey(r => r.AthleteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(r => r.CreatedBy);
        builder.Ignore(r => r.UpdatedBy);
    }
}
