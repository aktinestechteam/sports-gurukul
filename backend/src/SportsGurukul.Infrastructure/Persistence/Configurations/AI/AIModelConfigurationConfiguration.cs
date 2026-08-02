using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.AI;

public class AIModelConfigurationConfiguration : IEntityTypeConfiguration<Domain.Entities.AI.AIModelConfiguration>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.AI.AIModelConfiguration> builder)
    {
        builder.ToTable("AIModelConfigurations");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.DisplayName)
            .HasMaxLength(200);

        builder.Property(e => e.Temperature)
            .HasPrecision(3, 2);

        builder.Property(e => e.TopP)
            .HasPrecision(3, 2);

        builder.Property(e => e.FrequencyPenalty)
            .HasPrecision(3, 2);

        builder.Property(e => e.PresencePenalty)
            .HasPrecision(3, 2);

        builder.Property(e => e.StopSequences)
            .HasMaxLength(2000);

        builder.Property(e => e.ModelParameters)
            .HasMaxLength(4000);

        builder.Property(e => e.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(e => e.ModelId)
            .HasDatabaseName("IX_AIModelConfigurations_ModelId");

        builder.HasIndex(e => e.IsDefault)
            .HasDatabaseName("IX_AIModelConfigurations_IsDefault");

        builder.HasOne(e => e.Model)
            .WithMany(m => m.ModelConfigurations)
            .HasForeignKey(e => e.ModelId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.Ignore(e => e.CreatedBy);
        builder.Ignore(e => e.UpdatedBy);
    }
}
