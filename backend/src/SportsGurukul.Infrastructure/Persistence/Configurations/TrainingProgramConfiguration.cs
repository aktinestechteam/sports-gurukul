using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class TrainingProgramConfiguration : IEntityTypeConfiguration<TrainingProgram>
{
    public void Configure(EntityTypeBuilder<TrainingProgram> builder)
    {
        builder.ToTable("TrainingPrograms");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.ProgramCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.ProgramName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(p => p.Description)
            .HasMaxLength(2000);

        builder.Property(p => p.DifficultyLevel)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(p => p.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(p => p.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(p => p.ProgramCode)
            .IsUnique()
            .HasDatabaseName("IX_TrainingPrograms_ProgramCode");

        builder.HasIndex(p => p.ProgramName)
            .HasDatabaseName("IX_TrainingPrograms_ProgramName");

        builder.HasIndex(p => p.AcademyId)
            .HasDatabaseName("IX_TrainingPrograms_AcademyId");

        builder.HasIndex(p => p.SportId)
            .HasDatabaseName("IX_TrainingPrograms_SportId");

        builder.HasIndex(p => p.Status)
            .HasDatabaseName("IX_TrainingPrograms_Status");

        builder.HasIndex(p => p.DifficultyLevel)
            .HasDatabaseName("IX_TrainingPrograms_DifficultyLevel");

        builder.HasIndex(p => new { p.AcademyId, p.Status })
            .HasDatabaseName("IX_TrainingPrograms_AcademyId_Status");

        builder.HasIndex(p => new { p.SportId, p.Status })
            .HasDatabaseName("IX_TrainingPrograms_SportId_Status");

        builder.HasOne(p => p.Sport)
            .WithMany()
            .HasForeignKey(p => p.SportId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Academy)
            .WithMany()
            .HasForeignKey(p => p.AcademyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(p => !p.IsDeleted);

        builder.Ignore(p => p.CreatedBy);
        builder.Ignore(p => p.UpdatedBy);
    }
}
