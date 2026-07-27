using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class TrainingAssessmentConfiguration : IEntityTypeConfiguration<TrainingAssessment>
{
    public void Configure(EntityTypeBuilder<TrainingAssessment> builder)
    {
        builder.ToTable("TrainingAssessments");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.AssessmentType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(a => a.AssessmentName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(a => a.MaximumScore)
            .HasPrecision(10, 2);

        builder.Property(a => a.PassingScore)
            .HasPrecision(10, 2);

        builder.Property(a => a.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(a => a.SessionId)
            .HasDatabaseName("IX_TrainingAssessments_SessionId");

        builder.HasIndex(a => a.AssessmentType)
            .HasDatabaseName("IX_TrainingAssessments_AssessmentType");

        builder.HasIndex(a => new { a.SessionId, a.AssessmentType })
            .HasDatabaseName("IX_TrainingAssessments_SessionId_Type");

        builder.HasOne(a => a.Session)
            .WithMany(s => s.Assessments)
            .HasForeignKey(a => a.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(a => !a.IsDeleted);

        builder.Ignore(a => a.CreatedBy);
        builder.Ignore(a => a.UpdatedBy);
    }
}
