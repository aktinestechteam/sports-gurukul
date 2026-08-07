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

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.DisplayName)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(p => p.Description)
            .HasMaxLength(1000);

        builder.Property(p => p.ProviderType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(p => p.BaseUrl)
            .HasMaxLength(500);

        builder.Property(p => p.AuthType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(p => p.DefaultApiVersion)
            .HasMaxLength(50);

        builder.Property(p => p.ConfigurationSchemaJson)
            .HasMaxLength(8000);

        builder.Property(p => p.IconUrl)
            .HasMaxLength(1000);

        builder.Property(p => p.WebsiteUrl)
            .HasMaxLength(1000);

        builder.Property(p => p.DocumentationUrl)
            .HasMaxLength(1000);

        builder.Property(p => p.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(p => p.Name)
            .IsUnique()
            .HasDatabaseName("IX_AIProviders_Name");

        builder.HasIndex(p => p.ProviderType)
            .HasDatabaseName("IX_AIProviders_ProviderType");

        builder.HasIndex(p => p.IsActive)
            .HasDatabaseName("IX_AIProviders_IsActive");

        builder.HasQueryFilter(p => !p.IsDeleted);

        builder.Ignore(p => p.CreatedBy);
        builder.Ignore(p => p.UpdatedBy);

        builder.HasData(
            new AIProvider
            {
                Id = Guid.Parse("a1000000-0000-0000-0000-000000000001"),
                Name = "openai",
                DisplayName = "OpenAI",
                Description = "OpenAI GPT and embedding models.",
                ProviderType = AIProviderType.OpenAi,
                BaseUrl = "https://api.openai.com/v1",
                AuthType = AIAuthType.ApiKey,
                SupportsChat = true,
                SupportsEmbeddings = true,
                SupportsVision = true,
                SupportsFunctionCalling = true,
                IsActive = true,
                WebsiteUrl = "https://openai.com",
                DocumentationUrl = "https://platform.openai.com/docs"
            },
            new AIProvider
            {
                Id = Guid.Parse("a1000000-0000-0000-0000-000000000002"),
                Name = "azure-openai",
                DisplayName = "Azure OpenAI",
                Description = "OpenAI models hosted on Microsoft Azure.",
                ProviderType = AIProviderType.AzureOpenAi,
                BaseUrl = "https://{resource}.openai.azure.com",
                AuthType = AIAuthType.ApiKey,
                SupportsChat = true,
                SupportsEmbeddings = true,
                SupportsVision = true,
                SupportsFunctionCalling = true,
                IsActive = true,
                WebsiteUrl = "https://azure.microsoft.com/products/ai-services/openai-service",
                DocumentationUrl = "https://learn.microsoft.com/azure/ai-services/openai"
            },
            new AIProvider
            {
                Id = Guid.Parse("a1000000-0000-0000-0000-000000000003"),
                Name = "anthropic",
                DisplayName = "Anthropic Claude",
                Description = "Anthropic Claude models.",
                ProviderType = AIProviderType.Anthropic,
                BaseUrl = "https://api.anthropic.com/v1",
                AuthType = AIAuthType.BearerToken,
                SupportsChat = true,
                SupportsVision = true,
                SupportsFunctionCalling = true,
                IsActive = true,
                WebsiteUrl = "https://www.anthropic.com",
                DocumentationUrl = "https://docs.anthropic.com"
            },
            new AIProvider
            {
                Id = Guid.Parse("a1000000-0000-0000-0000-000000000004"),
                Name = "google",
                DisplayName = "Google Gemini",
                Description = "Google Gemini models.",
                ProviderType = AIProviderType.Google,
                BaseUrl = "https://generativelanguage.googleapis.com/v1",
                AuthType = AIAuthType.ApiKey,
                SupportsChat = true,
                SupportsEmbeddings = true,
                SupportsVision = true,
                SupportsFunctionCalling = true,
                IsActive = true,
                WebsiteUrl = "https://deepmind.google/technologies/gemini",
                DocumentationUrl = "https://ai.google.dev"
            },
            new AIProvider
            {
                Id = Guid.Parse("a1000000-0000-0000-0000-000000000005"),
                Name = "ollama",
                DisplayName = "Ollama",
                Description = "Self-hosted local open-source models via Ollama.",
                ProviderType = AIProviderType.Ollama,
                BaseUrl = "http://localhost:11434",
                AuthType = AIAuthType.None,
                SupportsChat = true,
                SupportsEmbeddings = true,
                SupportsVision = true,
                SupportsFunctionCalling = true,
                IsActive = true,
                WebsiteUrl = "https://ollama.com",
                DocumentationUrl = "https://github.com/ollama/ollama"
            },
            new AIProvider
            {
                Id = Guid.Parse("a1000000-0000-0000-0000-000000000006"),
                Name = "openrouter",
                DisplayName = "OpenRouter",
                Description = "Unified gateway to multiple AI model providers.",
                ProviderType = AIProviderType.OpenRouter,
                BaseUrl = "https://openrouter.ai/api/v1",
                AuthType = AIAuthType.ApiKey,
                SupportsChat = true,
                SupportsEmbeddings = true,
                SupportsVision = true,
                SupportsFunctionCalling = true,
                IsActive = true,
                WebsiteUrl = "https://openrouter.ai",
                DocumentationUrl = "https://openrouter.ai/docs"
            }
        );
    }
}
