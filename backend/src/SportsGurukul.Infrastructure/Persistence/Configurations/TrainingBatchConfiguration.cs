using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class TrainingBatchConfiguration : IEntityTypeConfiguration<TrainingBatch>
{
    public void Configure(EntityTypeBuilder<TrainingBatch> builder)
    {
        builder.ToTable("TrainingBatches");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.BatchCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(b => b.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(b => b.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(b => b.BatchCode)
            .IsUnique()
            .HasDatabaseName("IX_TrainingBatches_BatchCode");

        builder.HasIndex(b => b.ProgramId)
            .HasDatabaseName("IX_TrainingBatches_ProgramId");

        builder.HasIndex(b => b.CoachId)
            .HasDatabaseName("IX_TrainingBatches_CoachId");

        builder.HasIndex(b => b.BranchId)
            .HasDatabaseName("IX_TrainingBatches_BranchId");

        builder.HasIndex(b => b.Status)
            .HasDatabaseName("IX_TrainingBatches_Status");

        builder.HasIndex(b => new { b.ProgramId, b.Status })
            .HasDatabaseName("IX_TrainingBatches_ProgramId_Status");

        builder.HasIndex(b => new { b.CoachId, b.StartDate })
            .HasDatabaseName("IX_TrainingBatches_CoachId_StartDate");

        builder.HasOne(b => b.Program)
            .WithMany(p => p.Batches)
            .HasForeignKey(b => b.ProgramId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(b => b.Coach)
            .WithMany()
            .HasForeignKey(b => b.CoachId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.Branch)
            .WithMany()
            .HasForeignKey(b => b.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(b => !b.IsDeleted);

        builder.Ignore(b => b.CreatedBy);
        builder.Ignore(b => b.UpdatedBy);
    }
}
