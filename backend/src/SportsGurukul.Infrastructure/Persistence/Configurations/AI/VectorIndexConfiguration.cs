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

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.Description)
            .HasMaxLength(2000);

        builder.Property(e => e.IndexType)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.DistanceMetric)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.IndexConfiguration)
            .HasMaxLength(4000);

        builder.Property(e => e.TableName)
            .HasMaxLength(200);

        builder.Property(e => e.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(e => e.Name)
            .HasDatabaseName("IX_VectorIndexes_Name");

        builder.HasIndex(e => e.Status)
            .HasDatabaseName("IX_VectorIndexes_Status");

        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.Ignore(e => e.CreatedBy);
        builder.Ignore(e => e.UpdatedBy);
    }
}
