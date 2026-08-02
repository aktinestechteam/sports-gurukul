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

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.Description)
            .HasMaxLength(2000);

        builder.Property(e => e.AssistantType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(e => e.Personality)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(e => e.SystemPrompt)
            .HasMaxLength(8000);

        builder.Property(e => e.GreetingMessage)
            .HasMaxLength(500);

        builder.Property(e => e.AvatarUrl)
            .HasMaxLength(500);

        builder.Property(e => e.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(e => e.Name)
            .HasDatabaseName("IX_AIAssistants_Name");

        builder.HasIndex(e => e.AssistantType)
            .HasDatabaseName("IX_AIAssistants_AssistantType");

        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.Ignore(e => e.CreatedBy);
        builder.Ignore(e => e.UpdatedBy);

        builder.HasData(
            new AIAssistant
            {
                Id = Guid.Parse("99999999-9999-9999-9999-999999999999"),
                Name = "Sports Coach",
                Description = "AI-powered sports coaching assistant",
                AssistantType = AIAssistantType.Coach,
                Personality = AIAssistantPersonality.Motivational,
                SystemPrompt = "You are an expert sports coach assistant...",
                GreetingMessage = "Hello! I'm your AI sports coach. How can I help you today?",
                IsActive = true,
                IsPublic = true,
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new AIAssistant
            {
                Id = Guid.Parse("88888888-8888-8888-8888-888888888888"),
                Name = "Nutrition Advisor",
                Description = "AI-powered nutrition and diet planning assistant",
                AssistantType = AIAssistantType.Nutritionist,
                Personality = AIAssistantPersonality.Friendly,
                SystemPrompt = "You are an expert nutrition advisor...",
                GreetingMessage = "Hi! I'm your AI nutrition advisor. Let's plan your diet!",
                IsActive = true,
                IsPublic = true,
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}
