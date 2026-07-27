using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class TrainingSessionConfiguration : IEntityTypeConfiguration<TrainingSession>
{
    public void Configure(EntityTypeBuilder<TrainingSession> builder)
    {
        builder.ToTable("TrainingSessions");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.SessionCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(s => s.SessionTitle)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(s => s.SessionType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(s => s.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(s => s.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(s => s.SessionCode)
            .IsUnique()
            .HasDatabaseName("IX_TrainingSessions_SessionCode");

        builder.HasIndex(s => s.BatchId)
            .HasDatabaseName("IX_TrainingSessions_BatchId");

        builder.HasIndex(s => s.CoachId)
            .HasDatabaseName("IX_TrainingSessions_CoachId");

        builder.HasIndex(s => s.FacilityId)
            .HasDatabaseName("IX_TrainingSessions_FacilityId");

        builder.HasIndex(s => s.SessionDate)
            .HasDatabaseName("IX_TrainingSessions_SessionDate");

        builder.HasIndex(s => s.Status)
            .HasDatabaseName("IX_TrainingSessions_Status");

        builder.HasIndex(s => s.SessionType)
            .HasDatabaseName("IX_TrainingSessions_SessionType");

        builder.HasIndex(s => new { s.BatchId, s.SessionDate })
            .HasDatabaseName("IX_TrainingSessions_BatchId_SessionDate");

        builder.HasIndex(s => new { s.CoachId, s.SessionDate })
            .HasDatabaseName("IX_TrainingSessions_CoachId_SessionDate");

        builder.HasOne(s => s.Batch)
            .WithMany(b => b.Sessions)
            .HasForeignKey(s => s.BatchId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.Facility)
            .WithMany()
            .HasForeignKey(s => s.FacilityId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(s => s.Coach)
            .WithMany()
            .HasForeignKey(s => s.CoachId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(s => !s.IsDeleted);

        builder.Ignore(s => s.CreatedBy);
        builder.Ignore(s => s.UpdatedBy);
    }
}
