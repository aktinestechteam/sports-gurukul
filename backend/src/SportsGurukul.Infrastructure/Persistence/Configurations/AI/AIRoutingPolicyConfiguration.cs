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

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.Description)
            .HasMaxLength(2000);

        builder.Property(e => e.Strategy)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(e => e.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.ProviderIds)
            .HasMaxLength(2000);

        builder.Property(e => e.ModelIds)
            .HasMaxLength(2000);

        builder.Property(e => e.Rules)
            .HasMaxLength(4000);

        builder.Property(e => e.FallbackPolicy)
            .HasMaxLength(4000);

        builder.Property(e => e.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(e => e.Name)
            .HasDatabaseName("IX_AIRoutingPolicies_Name");

        builder.HasIndex(e => e.Status)
            .HasDatabaseName("IX_AIRoutingPolicies_Status");

        builder.HasIndex(e => e.Strategy)
            .HasDatabaseName("IX_AIRoutingPolicies_Strategy");

        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.Ignore(e => e.CreatedBy);
        builder.Ignore(e => e.UpdatedBy);
    }
}
