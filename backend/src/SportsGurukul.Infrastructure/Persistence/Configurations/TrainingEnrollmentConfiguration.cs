using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class TrainingEnrollmentConfiguration : IEntityTypeConfiguration<TrainingEnrollment>
{
    public void Configure(EntityTypeBuilder<TrainingEnrollment> builder)
    {
        builder.ToTable("TrainingEnrollments");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(e => e.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(e => e.BatchId)
            .HasDatabaseName("IX_TrainingEnrollments_BatchId");

        builder.HasIndex(e => e.AthleteId)
            .HasDatabaseName("IX_TrainingEnrollments_AthleteId");

        builder.HasIndex(e => e.Status)
            .HasDatabaseName("IX_TrainingEnrollments_Status");

        builder.HasIndex(e => new { e.BatchId, e.AthleteId })
            .IsUnique()
            .HasDatabaseName("IX_TrainingEnrollments_BatchId_AthleteId");

        builder.HasIndex(e => new { e.AthleteId, e.Status })
            .HasDatabaseName("IX_TrainingEnrollments_AthleteId_Status");

        builder.HasOne(e => e.Batch)
            .WithMany(b => b.Enrollments)
            .HasForeignKey(e => e.BatchId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Athlete)
            .WithMany()
            .HasForeignKey(e => e.AthleteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.Ignore(e => e.CreatedBy);
        builder.Ignore(e => e.UpdatedBy);
    }
}
