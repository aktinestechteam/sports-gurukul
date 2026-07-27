using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class TrainingProgressConfiguration : IEntityTypeConfiguration<TrainingProgress>
{
    public void Configure(EntityTypeBuilder<TrainingProgress> builder)
    {
        builder.ToTable("TrainingProgresses");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.CurrentLevel)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.CompletedPercentage)
            .HasPrecision(5, 2);

        builder.Property(p => p.OverallRating)
            .HasPrecision(5, 2);

        builder.Property(p => p.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(p => p.EnrollmentId)
            .IsUnique()
            .HasDatabaseName("IX_TrainingProgresses_EnrollmentId");

        builder.HasOne(p => p.Enrollment)
            .WithOne(e => e.Progress)
            .HasForeignKey<TrainingProgress>(p => p.EnrollmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(p => p.CreatedBy);
        builder.Ignore(p => p.UpdatedBy);
    }
}
