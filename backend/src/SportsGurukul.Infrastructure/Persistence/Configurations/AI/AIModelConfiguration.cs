using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.AI;

public class AIModelConfiguration : IEntityTypeConfiguration<AIModel>
{
    public void Configure(EntityTypeBuilder<AIModel> builder)
    {
        builder.ToTable("AIModels");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.DisplayName)
            .HasMaxLength(200);

        builder.Property(e => e.Description)
            .HasMaxLength(2000);

        builder.Property(e => e.Capabilities)
            .HasConversion<string>()
            .HasMaxLength(100);

        builder.Property(e => e.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.ModelVersion)
            .HasMaxLength(50);

        builder.Property(e => e.CostPerInputToken)
            .HasPrecision(18, 8);

        builder.Property(e => e.CostPerOutputToken)
            .HasPrecision(18, 8);

        builder.Property(e => e.CostPerImageToken)
            .HasPrecision(18, 8);

        builder.Property(e => e.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(e => e.Name)
            .HasDatabaseName("IX_AIModels_Name");

        builder.HasIndex(e => e.Status)
            .HasDatabaseName("IX_AIModels_Status");

        builder.HasIndex(e => e.ProviderId)
            .HasDatabaseName("IX_AIModels_ProviderId");

        builder.HasOne(e => e.Provider)
            .WithMany(p => p.Models)
            .HasForeignKey(e => e.ProviderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.Ignore(e => e.CreatedBy);
        builder.Ignore(e => e.UpdatedBy);

        builder.HasData(
            new AIModel
            {
                Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                ProviderId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "gpt-4",
                DisplayName = "GPT-4",
                Capabilities = AIModelCapability.TextGeneration | AIModelCapability.CodeGeneration | AIModelCapability.Reasoning | AIModelCapability.FunctionCalling,
                Status = AIModelStatus.Active,
                MaxTokens = 8192,
                MaxContextLength = 32768,
                DefaultTemperature = 0.7,
                SupportsStreaming = true,
                SupportsFunctionCalling = true,
                SupportsVision = true,
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new AIModel
            {
                Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                ProviderId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "gpt-3.5-turbo",
                DisplayName = "GPT-3.5 Turbo",
                Capabilities = AIModelCapability.TextGeneration | AIModelCapability.CodeGeneration | AIModelCapability.FunctionCalling,
                Status = AIModelStatus.Active,
                MaxTokens = 4096,
                MaxContextLength = 16384,
                DefaultTemperature = 0.7,
                SupportsStreaming = true,
                SupportsFunctionCalling = true,
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new AIModel
            {
                Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                ProviderId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Name = "claude-3-opus",
                DisplayName = "Claude 3 Opus",
                Capabilities = AIModelCapability.TextGeneration | AIModelCapability.CodeGeneration | AIModelCapability.Reasoning | AIModelCapability.Vision,
                Status = AIModelStatus.Active,
                MaxTokens = 4096,
                MaxContextLength = 200000,
                DefaultTemperature = 0.7,
                SupportsStreaming = true,
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new AIModel
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                ProviderId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Name = "claude-3-sonnet",
                DisplayName = "Claude 3 Sonnet",
                Capabilities = AIModelCapability.TextGeneration | AIModelCapability.CodeGeneration | AIModelCapability.Reasoning | AIModelCapability.Vision,
                Status = AIModelStatus.Active,
                MaxTokens = 4096,
                MaxContextLength = 200000,
                DefaultTemperature = 0.7,
                SupportsStreaming = true,
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new AIModel
            {
                Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                ProviderId = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                Name = "gemini-pro",
                DisplayName = "Gemini Pro",
                Capabilities = AIModelCapability.TextGeneration | AIModelCapability.CodeGeneration | AIModelCapability.Reasoning | AIModelCapability.Vision,
                Status = AIModelStatus.Active,
                MaxTokens = 8192,
                MaxContextLength = 32768,
                DefaultTemperature = 0.7,
                SupportsStreaming = true,
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new AIModel
            {
                Id = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"),
                ProviderId = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                Name = "llama3",
                DisplayName = "Llama 3",
                Capabilities = AIModelCapability.TextGeneration | AIModelCapability.CodeGeneration,
                Status = AIModelStatus.Active,
                MaxTokens = 8192,
                MaxContextLength = 8192,
                DefaultTemperature = 0.7,
                SupportsStreaming = true,
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}
