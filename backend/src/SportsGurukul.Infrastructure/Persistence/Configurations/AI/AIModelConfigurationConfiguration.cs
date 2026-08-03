using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.AI;
using AIModelConfigurationEntity = SportsGurukul.Domain.Entities.AI.AIModelConfiguration;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.AI;

public class AIModelConfigurationConfiguration : IEntityTypeConfiguration<AIModelConfigurationEntity>
{
    public void Configure(EntityTypeBuilder<AIModelConfigurationEntity> builder)
    {
        builder.ToTable("AIModelConfigurations");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(c => c.Description)
            .HasMaxLength(1000);

        builder.Property(c => c.ApiKeyEncrypted)
            .HasMaxLength(2000);

        builder.Property(c => c.ApiVersion)
            .HasMaxLength(50);

        builder.Property(c => c.BaseUrlOverride)
            .HasMaxLength(500);

        builder.Property(c => c.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(c => c.ProviderId)
            .HasDatabaseName("IX_AIModelConfigurations_ProviderId");

        builder.HasIndex(c => c.ModelId)
            .HasDatabaseName("IX_AIModelConfigurations_ModelId");

        builder.HasIndex(c => c.AssistantId)
            .HasDatabaseName("IX_AIModelConfigurations_AssistantId");

        builder.HasIndex(c => c.AgentDefinitionId)
            .HasDatabaseName("IX_AIModelConfigurations_AgentDefinitionId");

        builder.HasIndex(c => new { c.AssistantId, c.Name })
            .IsUnique()
            .HasDatabaseName("IX_AIModelConfigurations_AssistantId_Name");

        builder.HasIndex(c => c.IsActive)
            .HasDatabaseName("IX_AIModelConfigurations_IsActive");

        builder.HasOne(c => c.Provider)
            .WithMany(p => p.ModelConfigurations)
            .HasForeignKey(c => c.ProviderId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(c => c.Model)
            .WithMany(m => m.ModelConfigurations)
            .HasForeignKey(c => c.ModelId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(c => c.Assistant)
            .WithMany(a => a.ModelConfigurations)
            .HasForeignKey(c => c.AssistantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.AgentDefinition)
            .WithMany(a => a.ModelConfigurations)
            .HasForeignKey(c => c.AgentDefinitionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(c => !c.IsDeleted);

        builder.Ignore(c => c.CreatedBy);
        builder.Ignore(c => c.UpdatedBy);
    }
}
