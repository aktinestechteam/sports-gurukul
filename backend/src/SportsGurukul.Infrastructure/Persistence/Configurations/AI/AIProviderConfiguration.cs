using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.AI;

public class AIProviderConfiguration : IEntityTypeConfiguration<AIProvider>
{
    public void Configure(EntityTypeBuilder<AIProvider> builder)
    {
        builder.ToTable("AIProviders");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.Description)
            .HasMaxLength(2000);

        builder.Property(e => e.Type)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(e => e.ApiBaseUrl)
            .HasMaxLength(500);

        builder.Property(e => e.ApiVersion)
            .HasMaxLength(50);

        builder.Property(e => e.MaxRetries);

        builder.Property(e => e.TimeoutSeconds);

        builder.Property(e => e.CostPerToken)
            .HasPrecision(18, 8);

        builder.Property(e => e.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(e => e.Name)
            .HasDatabaseName("IX_AIProviders_Name");

        builder.HasIndex(e => e.Type)
            .HasDatabaseName("IX_AIProviders_Type");

        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.Ignore(e => e.CreatedBy);
        builder.Ignore(e => e.UpdatedBy);

        builder.HasData(
            new AIProvider
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "OpenAI",
                Type = AIProviderType.OpenAI,
                ApiBaseUrl = "https://api.openai.com/v1",
                IsActive = true,
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new AIProvider
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = "Azure OpenAI",
                Type = AIProviderType.AzureOpenAI,
                ApiBaseUrl = "https://api.azure.com/openai",
                IsActive = true,
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new AIProvider
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Name = "Anthropic",
                Type = AIProviderType.Anthropic,
                ApiBaseUrl = "https://api.anthropic.com/v1",
                IsActive = true,
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new AIProvider
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                Name = "Google AI",
                Type = AIProviderType.Google,
                ApiBaseUrl = "https://generativelanguage.googleapis.com",
                IsActive = true,
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new AIProvider
            {
                Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                Name = "Ollama",
                Type = AIProviderType.Ollama,
                ApiBaseUrl = "http://localhost:11434",
                IsActive = true,
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new AIProvider
            {
                Id = Guid.Parse("66666666-6666-6666-6666-666666666666"),
                Name = "OpenRouter",
                Type = AIProviderType.OpenRouter,
                ApiBaseUrl = "https://openrouter.ai/api/v1",
                IsActive = true,
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}
