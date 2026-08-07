using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.AI;

public class VectorIndexConfiguration : IEntityTypeConfiguration<VectorIndex>
{
    public void Configure(EntityTypeBuilder<VectorIndex> builder)
    {
        builder.ToTable("VectorIndexes");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(v => v.Description)
            .HasMaxLength(1000);

        builder.Property(v => v.Provider)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(v => v.DistanceMetric)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(v => v.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(v => v.IndexName)
            .HasMaxLength(200);

        builder.Property(v => v.ConfigurationJson)
            .HasMaxLength(8000);

        builder.Property(v => v.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(v => v.Name)
            .IsUnique()
            .HasDatabaseName("IX_VectorIndexes_Name");

        builder.HasIndex(v => v.Provider)
            .HasDatabaseName("IX_VectorIndexes_Provider");

        builder.HasIndex(v => v.Status)
            .HasDatabaseName("IX_VectorIndexes_Status");

        builder.HasIndex(v => v.IsActive)
            .HasDatabaseName("IX_VectorIndexes_IsActive");

        builder.HasQueryFilter(v => !v.IsDeleted);

        builder.Ignore(v => v.CreatedBy);
        builder.Ignore(v => v.UpdatedBy);
    }
}
