using Microsoft.EntityFrameworkCore;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;
using SportsGurukul.Infrastructure.Persistence;

namespace AI.Infrastructure.Tests;

public class ApplicationDbContextInMemorySmokeTests
{
    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task AddAndQueryConversation_WorksWithInMemory()
    {
        await using var context = CreateContext();
        var conversation = new Conversation
        {
            Id = Guid.NewGuid(),
            Title = "Hello",
            UserId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow
        };
        context.Conversations.Add(conversation);
        await context.SaveChangesAsync();

        var loaded = await context.Conversations.FirstOrDefaultAsync(c => c.Id == conversation.Id);
        loaded.Should().NotBeNull();
        loaded!.Title.Should().Be("Hello");
    }

    [Fact]
    public async Task AddAndQueryAiModel_WorksWithInMemory()
    {
        await using var context = CreateContext();
        var model = new AIModel
        {
            Id = Guid.NewGuid(),
            Name = "gpt-4",
            ProviderId = Guid.NewGuid(),
            Status = AIModelStatus.Active,
            MaxTokens = 4096,
            CreatedAt = DateTime.UtcNow
        };
        context.AIModels.Add(model);
        await context.SaveChangesAsync();

        var loaded = await context.AIModels.FirstOrDefaultAsync(m => m.Id == model.Id);
        loaded.Should().NotBeNull();
        loaded!.Status.Should().Be(AIModelStatus.Active);
    }
}
