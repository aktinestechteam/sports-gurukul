using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.AI;

public class AIAssistantConfiguration : IEntityTypeConfiguration<AIAssistant>
{
    public void Configure(EntityTypeBuilder<AIAssistant> builder)
    {
        builder.ToTable("AIAssistants");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(a => a.DisplayName)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(a => a.Description)
            .HasMaxLength(1000);

        builder.Property(a => a.AssistantType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(a => a.OwnerType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(a => a.AvatarUrl)
            .HasMaxLength(1000);

        builder.Property(a => a.GuardrailsJson)
            .HasMaxLength(8000);

        builder.Property(a => a.MetadataJson)
            .HasMaxLength(8000);

        builder.Property(a => a.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(a => a.Name)
            .IsUnique()
            .HasDatabaseName("IX_AIAssistants_Name");

        builder.HasIndex(a => a.AssistantType)
            .HasDatabaseName("IX_AIAssistants_AssistantType");

        builder.HasIndex(a => a.OwnerType)
            .HasDatabaseName("IX_AIAssistants_OwnerType");

        builder.HasIndex(a => a.OwnerUserId)
            .HasDatabaseName("IX_AIAssistants_OwnerUserId");

        builder.HasIndex(a => a.ModelId)
            .HasDatabaseName("IX_AIAssistants_ModelId");

        builder.HasIndex(a => a.IsActive)
            .HasDatabaseName("IX_AIAssistants_IsActive");

        builder.HasOne(a => a.Model)
            .WithMany()
            .HasForeignKey(a => a.ModelId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(a => !a.IsDeleted);

        builder.Ignore(a => a.CreatedBy);
        builder.Ignore(a => a.UpdatedBy);
    }
}
