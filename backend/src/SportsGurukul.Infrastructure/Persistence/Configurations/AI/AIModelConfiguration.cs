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

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(m => m.DisplayName)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(m => m.Family)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(m => m.Description)
            .HasMaxLength(1000);

        builder.Property(m => m.Version)
            .HasMaxLength(50);

        builder.Property(m => m.InputCostPerMillionTokens)
            .HasPrecision(18, 4);

        builder.Property(m => m.OutputCostPerMillionTokens)
            .HasPrecision(18, 4);

        builder.Property(m => m.Currency)
            .HasMaxLength(10);

        builder.Property(m => m.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(m => m.ProviderId)
            .HasDatabaseName("IX_AIModels_ProviderId");

        builder.HasIndex(m => new { m.Name, m.Version })
            .IsUnique()
            .HasDatabaseName("IX_AIModels_Name_Version");

        builder.HasIndex(m => m.Family)
            .HasDatabaseName("IX_AIModels_Family");

        builder.HasIndex(m => m.IsActive)
            .HasDatabaseName("IX_AIModels_IsActive");

        builder.HasOne(m => m.Provider)
            .WithMany(p => p.Models)
            .HasForeignKey(m => m.ProviderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(m => !m.IsDeleted);

        builder.Ignore(m => m.CreatedBy);
        builder.Ignore(m => m.UpdatedBy);

        builder.HasData(
            new AIModel
            {
                Id = Guid.Parse("a2000000-0000-0000-0000-000000000001"),
                ProviderId = Guid.Parse("a1000000-0000-0000-0000-000000000001"),
                Name = "gpt-4o",
                DisplayName = "GPT-4o",
                Family = AIModelFamily.Gpt,
                Description = "High-intelligence multimodal flagship model.",
                Version = "2024-08-06",
                ContextWindow = 128000,
                MaxOutputTokens = 16384,
                InputCostPerMillionTokens = 2.50m,
                OutputCostPerMillionTokens = 10.00m,
                Currency = "USD",
                SupportsChat = true,
                SupportsVision = true,
                SupportsFunctionCalling = true,
                SupportsJsonMode = true,
                SupportsStreaming = true,
                IsActive = true
            },
            new AIModel
            {
                Id = Guid.Parse("a2000000-0000-0000-0000-000000000002"),
                ProviderId = Guid.Parse("a1000000-0000-0000-0000-000000000001"),
                Name = "gpt-4o-mini",
                DisplayName = "GPT-4o Mini",
                Family = AIModelFamily.Gpt,
                Description = "Cost-efficient small model for high-volume tasks.",
                Version = "2024-07-18",
                ContextWindow = 128000,
                MaxOutputTokens = 16384,
                InputCostPerMillionTokens = 0.15m,
                OutputCostPerMillionTokens = 0.60m,
                Currency = "USD",
                SupportsChat = true,
                SupportsVision = true,
                SupportsFunctionCalling = true,
                SupportsJsonMode = true,
                SupportsStreaming = true,
                IsActive = true
            },
            new AIModel
            {
                Id = Guid.Parse("a2000000-0000-0000-0000-000000000003"),
                ProviderId = Guid.Parse("a1000000-0000-0000-0000-000000000001"),
                Name = "gpt-3.5-turbo",
                DisplayName = "GPT-3.5 Turbo",
                Family = AIModelFamily.Gpt,
                Description = "Legacy low-latency chat model.",
                Version = "0125",
                ContextWindow = 16385,
                MaxOutputTokens = 4096,
                InputCostPerMillionTokens = 0.50m,
                OutputCostPerMillionTokens = 1.50m,
                Currency = "USD",
                SupportsChat = true,
                SupportsFunctionCalling = true,
                SupportsJsonMode = true,
                SupportsStreaming = true,
                IsActive = true
            },
            new AIModel
            {
                Id = Guid.Parse("a2000000-0000-0000-0000-000000000004"),
                ProviderId = Guid.Parse("a1000000-0000-0000-0000-000000000001"),
                Name = "text-embedding-3-small",
                DisplayName = "Text Embedding 3 Small",
                Family = AIModelFamily.Embedding,
                Description = "Efficient text embedding model, 1536 dimensions.",
                Version = "1",
                InputCostPerMillionTokens = 0.02m,
                OutputCostPerMillionTokens = 0.00m,
                Currency = "USD",
                SupportsEmbeddings = true,
                SupportsStreaming = false,
                IsActive = true
            },
            new AIModel
            {
                Id = Guid.Parse("a2000000-0000-0000-0000-000000000005"),
                ProviderId = Guid.Parse("a1000000-0000-0000-0000-000000000001"),
                Name = "text-embedding-3-large",
                DisplayName = "Text Embedding 3 Large",
                Family = AIModelFamily.Embedding,
                Description = "High-quality text embedding model, 3072 dimensions.",
                Version = "1",
                InputCostPerMillionTokens = 0.13m,
                OutputCostPerMillionTokens = 0.00m,
                Currency = "USD",
                SupportsEmbeddings = true,
                SupportsStreaming = false,
                IsActive = true
            },
            new AIModel
            {
                Id = Guid.Parse("a2000000-0000-0000-0000-000000000006"),
                ProviderId = Guid.Parse("a1000000-0000-0000-0000-000000000002"),
                Name = "gpt-4o",
                DisplayName = "GPT-4o (Azure)",
                Family = AIModelFamily.Gpt,
                Description = "GPT-4o deployed on Azure OpenAI.",
                Version = "2024-11-20",
                ContextWindow = 128000,
                MaxOutputTokens = 16384,
                InputCostPerMillionTokens = 2.50m,
                OutputCostPerMillionTokens = 10.00m,
                Currency = "USD",
                SupportsChat = true,
                SupportsVision = true,
                SupportsFunctionCalling = true,
                SupportsJsonMode = true,
                SupportsStreaming = true,
                IsActive = true
            },
            new AIModel
            {
                Id = Guid.Parse("a2000000-0000-0000-0000-000000000007"),
                ProviderId = Guid.Parse("a1000000-0000-0000-0000-000000000002"),
                Name = "gpt-4o-mini",
                DisplayName = "GPT-4o Mini (Azure)",
                Family = AIModelFamily.Gpt,
                Description = "GPT-4o Mini deployed on Azure OpenAI.",
                Version = "2024-09-03",
                ContextWindow = 128000,
                MaxOutputTokens = 16384,
                InputCostPerMillionTokens = 0.15m,
                OutputCostPerMillionTokens = 0.60m,
                Currency = "USD",
                SupportsChat = true,
                SupportsVision = true,
                SupportsFunctionCalling = true,
                SupportsJsonMode = true,
                SupportsStreaming = true,
                IsActive = true
            },
            new AIModel
            {
                Id = Guid.Parse("a2000000-0000-0000-0000-000000000008"),
                ProviderId = Guid.Parse("a1000000-0000-0000-0000-000000000003"),
                Name = "claude-sonnet-4",
                DisplayName = "Claude Sonnet 4",
                Family = AIModelFamily.Claude,
                Description = "Balanced intelligence and speed for production workloads.",
                Version = "20250514",
                ContextWindow = 200000,
                MaxOutputTokens = 64000,
                InputCostPerMillionTokens = 3.00m,
                OutputCostPerMillionTokens = 15.00m,
                Currency = "USD",
                SupportsChat = true,
                SupportsVision = true,
                SupportsFunctionCalling = true,
                SupportsJsonMode = true,
                SupportsStreaming = true,
                IsActive = true
            },
            new AIModel
            {
                Id = Guid.Parse("a2000000-0000-0000-0000-000000000009"),
                ProviderId = Guid.Parse("a1000000-0000-0000-0000-000000000003"),
                Name = "claude-haiku-4-5",
                DisplayName = "Claude Haiku 4.5",
                Family = AIModelFamily.Claude,
                Description = "Fast, low-cost model for high-throughput tasks.",
                Version = "20250514",
                ContextWindow = 200000,
                MaxOutputTokens = 64000,
                InputCostPerMillionTokens = 1.00m,
                OutputCostPerMillionTokens = 5.00m,
                Currency = "USD",
                SupportsChat = true,
                SupportsVision = true,
                SupportsFunctionCalling = true,
                SupportsJsonMode = true,
                SupportsStreaming = true,
                IsActive = true
            },
            new AIModel
            {
                Id = Guid.Parse("a2000000-0000-0000-0000-00000000000a"),
                ProviderId = Guid.Parse("a1000000-0000-0000-0000-000000000004"),
                Name = "gemini-2.0-pro",
                DisplayName = "Gemini 2.0 Pro",
                Family = AIModelFamily.Gemini,
                Description = "Google's advanced multimodal reasoning model.",
                Version = "002",
                ContextWindow = 1048576,
                MaxOutputTokens = 8192,
                InputCostPerMillionTokens = 3.50m,
                OutputCostPerMillionTokens = 15.00m,
                Currency = "USD",
                SupportsChat = true,
                SupportsEmbeddings = false,
                SupportsVision = true,
                SupportsFunctionCalling = true,
                SupportsJsonMode = true,
                SupportsStreaming = true,
                IsActive = true
            },
            new AIModel
            {
                Id = Guid.Parse("a2000000-0000-0000-0000-00000000000b"),
                ProviderId = Guid.Parse("a1000000-0000-0000-0000-000000000004"),
                Name = "gemini-2.0-flash",
                DisplayName = "Gemini 2.0 Flash",
                Family = AIModelFamily.Gemini,
                Description = "Fast, cost-efficient multimodal model.",
                Version = "001",
                ContextWindow = 1048576,
                MaxOutputTokens = 8192,
                InputCostPerMillionTokens = 0.30m,
                OutputCostPerMillionTokens = 1.50m,
                Currency = "USD",
                SupportsChat = true,
                SupportsVision = true,
                SupportsFunctionCalling = true,
                SupportsJsonMode = true,
                SupportsStreaming = true,
                IsActive = true
            },
            new AIModel
            {
                Id = Guid.Parse("a2000000-0000-0000-0000-00000000000c"),
                ProviderId = Guid.Parse("a1000000-0000-0000-0000-000000000005"),
                Name = "llama3.1:8b",
                DisplayName = "Llama 3.1 8B",
                Family = AIModelFamily.Llama,
                Description = "Open-source 8B parameter model, self-hosted.",
                Version = "8b",
                ContextWindow = 131072,
                MaxOutputTokens = 8192,
                Currency = "USD",
                SupportsChat = true,
                SupportsFunctionCalling = true,
                SupportsStreaming = true,
                IsActive = true
            },
            new AIModel
            {
                Id = Guid.Parse("a2000000-0000-0000-0000-00000000000d"),
                ProviderId = Guid.Parse("a1000000-0000-0000-0000-000000000005"),
                Name = "llama3.3:70b",
                DisplayName = "Llama 3.3 70B",
                Family = AIModelFamily.Llama,
                Description = "Open-source 70B parameter model, self-hosted.",
                Version = "70b",
                ContextWindow = 131072,
                MaxOutputTokens = 8192,
                Currency = "USD",
                SupportsChat = true,
                SupportsFunctionCalling = true,
                SupportsStreaming = true,
                IsActive = true
            },
            new AIModel
            {
                Id = Guid.Parse("a2000000-0000-0000-0000-00000000000e"),
                ProviderId = Guid.Parse("a1000000-0000-0000-0000-000000000006"),
                Name = "openai/gpt-4o",
                DisplayName = "GPT-4o (OpenRouter)",
                Family = AIModelFamily.Gpt,
                Description = "GPT-4o accessible through OpenRouter gateway.",
                Version = "1",
                ContextWindow = 128000,
                MaxOutputTokens = 16384,
                InputCostPerMillionTokens = 2.50m,
                OutputCostPerMillionTokens = 10.00m,
                Currency = "USD",
                SupportsChat = true,
                SupportsVision = true,
                SupportsFunctionCalling = true,
                SupportsJsonMode = true,
                SupportsStreaming = true,
                IsActive = true
            },
            new AIModel
            {
                Id = Guid.Parse("a2000000-0000-0000-0000-00000000000f"),
                ProviderId = Guid.Parse("a1000000-0000-0000-0000-000000000006"),
                Name = "anthropic/claude-sonnet-4",
                DisplayName = "Claude Sonnet 4 (OpenRouter)",
                Family = AIModelFamily.Claude,
                Description = "Claude Sonnet 4 accessible through OpenRouter gateway.",
                Version = "1",
                ContextWindow = 200000,
                MaxOutputTokens = 64000,
                InputCostPerMillionTokens = 3.00m,
                OutputCostPerMillionTokens = 15.00m,
                Currency = "USD",
                SupportsChat = true,
                SupportsVision = true,
                SupportsFunctionCalling = true,
                SupportsJsonMode = true,
                SupportsStreaming = true,
                IsActive = true
            }
        );
    }
}
