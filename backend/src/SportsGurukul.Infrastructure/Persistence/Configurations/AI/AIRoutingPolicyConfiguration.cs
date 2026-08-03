using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.AI;

public class AIRoutingPolicyConfiguration : IEntityTypeConfiguration<AIRoutingPolicy>
{
    public void Configure(EntityTypeBuilder<AIRoutingPolicy> builder)
    {
        builder.ToTable("AIRoutingPolicies");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(r => r.Description)
            .HasMaxLength(1000);

        builder.Property(r => r.RoutingStrategy)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(r => r.ConditionsJson)
            .HasMaxLength(8000);

        builder.Property(r => r.PreferredModelIdsJson)
            .HasMaxLength(4000);

        builder.Property(r => r.FallbackModelIdsJson)
            .HasMaxLength(4000);

        builder.Property(r => r.MaxCostPerRequest)
            .HasPrecision(18, 4);

        builder.Property(r => r.MetadataJson)
            .HasMaxLength(8000);

        builder.Property(r => r.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(r => r.Name)
            .IsUnique()
            .HasDatabaseName("IX_AIRoutingPolicies_Name");

        builder.HasIndex(r => r.ProviderId)
            .HasDatabaseName("IX_AIRoutingPolicies_ProviderId");

        builder.HasIndex(r => r.DefaultModelId)
            .HasDatabaseName("IX_AIRoutingPolicies_DefaultModelId");

        builder.HasIndex(r => r.RoutingStrategy)
            .HasDatabaseName("IX_AIRoutingPolicies_RoutingStrategy");

        builder.HasIndex(r => r.Priority)
            .HasDatabaseName("IX_AIRoutingPolicies_Priority");

        builder.HasIndex(r => r.IsActive)
            .HasDatabaseName("IX_AIRoutingPolicies_IsActive");

        builder.HasOne(r => r.Provider)
            .WithMany(p => p.RoutingPolicies)
            .HasForeignKey(r => r.ProviderId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(r => r.DefaultModel)
            .WithMany(m => m.DefaultRoutingPolicies)
            .HasForeignKey(r => r.DefaultModelId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(r => !r.IsDeleted);

        builder.Ignore(r => r.CreatedBy);
        builder.Ignore(r => r.UpdatedBy);
    }
}
