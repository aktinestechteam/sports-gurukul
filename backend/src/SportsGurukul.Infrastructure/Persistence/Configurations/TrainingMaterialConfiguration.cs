using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class TrainingMaterialConfiguration : IEntityTypeConfiguration<TrainingMaterial>
{
    public void Configure(EntityTypeBuilder<TrainingMaterial> builder)
    {
        builder.ToTable("TrainingMaterials");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(m => m.Description)
            .HasMaxLength(2000);

        builder.Property(m => m.MaterialType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(m => m.FileUrl)
            .HasMaxLength(500)
            .IsRequired();

        builder.HasIndex(m => m.ProgramId)
            .HasDatabaseName("IX_TrainingMaterials_ProgramId");

        builder.HasIndex(m => m.SessionId)
            .HasDatabaseName("IX_TrainingMaterials_SessionId");

        builder.HasIndex(m => m.MaterialType)
            .HasDatabaseName("IX_TrainingMaterials_MaterialType");

        builder.HasIndex(m => new { m.ProgramId, m.SortOrder })
            .HasDatabaseName("IX_TrainingMaterials_ProgramId_SortOrder");

        builder.HasOne(m => m.Program)
            .WithMany(p => p.Materials)
            .HasForeignKey(m => m.ProgramId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(m => m.Session)
            .WithMany(s => s.Materials)
            .HasForeignKey(m => m.SessionId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Ignore(m => m.CreatedBy);
        builder.Ignore(m => m.UpdatedBy);
    }
}
