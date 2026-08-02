using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;
using SportsGurukul.Infrastructure.Persistence;
using SportsGurukul.Infrastructure.Persistence.Repositories.AI;

namespace AI.Infrastructure.Tests.Repositories;

public class AgentDefinitionRepositoryTests
{
    [Fact]
    public async Task GetByIdWithDetailsAsync_IncludesRelations()
    {
        await using var context = InMemoryDbContextFactory.Create();
        var assistant = new AIAssistant { Id = Guid.NewGuid(), Name = "Coach", CreatedAt = DateTime.UtcNow };
        var agent = new AgentDefinition { Id = Guid.NewGuid(), Name = "Agent", AssistantId = assistant.Id, CreatedAt = DateTime.UtcNow };
        var execution = new AgentExecution { Id = Guid.NewGuid(), AgentDefinitionId = agent.Id, CreatedAt = DateTime.UtcNow };
        context.AIAssistants.Add(assistant);
        context.AgentDefinitions.Add(agent);
        context.AgentExecutions.Add(execution);
        await context.SaveChangesAsync();

        var repo = new AgentDefinitionRepository(context);
        var loaded = await repo.GetByIdWithDetailsAsync(agent.Id, CancellationToken.None);

        loaded.Should().NotBeNull();
        loaded!.Assistant.Should().NotBeNull();
        loaded.Executions.Should().ContainSingle();
    }

    [Fact]
    public async Task GetActiveAsync_ReturnsOnlyActive()
    {
        await using var context = InMemoryDbContextFactory.Create();
        context.AgentDefinitions.AddRange(
            new AgentDefinition { Id = Guid.NewGuid(), Name = "a", Status = AgentStatus.Active, CreatedAt = DateTime.UtcNow },
            new AgentDefinition { Id = Guid.NewGuid(), Name = "b", Status = AgentStatus.Draft, CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var repo = new AgentDefinitionRepository(context);
        var result = await repo.GetActiveAsync(CancellationToken.None);

        result.Should().ContainSingle();
    }

    [Fact]
    public async Task GetByAssistantIdAsync_ReturnsOnlyMatching()
    {
        await using var context = InMemoryDbContextFactory.Create();
        var assistantA = Guid.NewGuid();
        var assistantB = Guid.NewGuid();
        context.AgentDefinitions.AddRange(
            new AgentDefinition { Id = Guid.NewGuid(), Name = "a", AssistantId = assistantA, CreatedAt = DateTime.UtcNow },
            new AgentDefinition { Id = Guid.NewGuid(), Name = "b", AssistantId = assistantA, CreatedAt = DateTime.UtcNow },
            new AgentDefinition { Id = Guid.NewGuid(), Name = "c", AssistantId = assistantB, CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var repo = new AgentDefinitionRepository(context);
        var result = await repo.GetByAssistantIdAsync(assistantA, CancellationToken.None);

        result.Should().HaveCount(2);
    }
}

public class AgentExecutionRepositoryTests
{
    [Fact]
    public async Task GetByIdWithDetailsAsync_IncludesAgentDefinition()
    {
        await using var context = InMemoryDbContextFactory.Create();
        var agent = new AgentDefinition { Id = Guid.NewGuid(), Name = "Agent", CreatedAt = DateTime.UtcNow };
        var execution = new AgentExecution { Id = Guid.NewGuid(), AgentDefinitionId = agent.Id, CreatedAt = DateTime.UtcNow };
        context.AgentDefinitions.Add(agent);
        context.AgentExecutions.Add(execution);
        await context.SaveChangesAsync();

        var repo = new AgentExecutionRepository(context);
        var loaded = await repo.GetByIdWithDetailsAsync(execution.Id, CancellationToken.None);

        loaded.Should().NotBeNull();
        loaded!.AgentDefinition.Should().NotBeNull();
    }

    [Fact]
    public async Task GetByAgentDefinitionIdAsync_ReturnsOnlyMatching()
    {
        await using var context = InMemoryDbContextFactory.Create();
        var agentA = Guid.NewGuid();
        context.AgentExecutions.AddRange(
            new AgentExecution { Id = Guid.NewGuid(), AgentDefinitionId = agentA, CreatedAt = DateTime.UtcNow },
            new AgentExecution { Id = Guid.NewGuid(), AgentDefinitionId = agentA, CreatedAt = DateTime.UtcNow },
            new AgentExecution { Id = Guid.NewGuid(), AgentDefinitionId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var repo = new AgentExecutionRepository(context);
        var result = await repo.GetByAgentDefinitionIdAsync(agentA, CancellationToken.None);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByStatusAsync_ParsesStatusAndFilters()
    {
        await using var context = InMemoryDbContextFactory.Create();
        context.AgentExecutions.AddRange(
            new AgentExecution { Id = Guid.NewGuid(), Status = AgentExecutionStatus.Completed, CreatedAt = DateTime.UtcNow },
            new AgentExecution { Id = Guid.NewGuid(), Status = AgentExecutionStatus.Running, CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var repo = new AgentExecutionRepository(context);
        var result = await repo.GetByStatusAsync("Completed", CancellationToken.None);

        result.Should().ContainSingle();
        result[0].Status.Should().Be(AgentExecutionStatus.Completed);
    }
}

public class AIAssistantRepositoryTests
{
    [Fact]
    public async Task GetByIdWithDetailsAsync_IncludesRelations()
    {
        await using var context = InMemoryDbContextFactory.Create();
        var assistant = new AIAssistant { Id = Guid.NewGuid(), Name = "Coach", CreatedAt = DateTime.UtcNow };
        var conversation = new Conversation { Id = Guid.NewGuid(), AssistantId = assistant.Id, CreatedAt = DateTime.UtcNow };
        var agent = new AgentDefinition { Id = Guid.NewGuid(), Name = "A", AssistantId = assistant.Id, CreatedAt = DateTime.UtcNow };
        context.AIAssistants.Add(assistant);
        context.Conversations.Add(conversation);
        context.AgentDefinitions.Add(agent);
        await context.SaveChangesAsync();

        var repo = new AIAssistantRepository(context);
        var loaded = await repo.GetByIdWithDetailsAsync(assistant.Id, CancellationToken.None);

        loaded.Should().NotBeNull();
        loaded!.Conversations.Should().ContainSingle();
        loaded.AgentDefinitions.Should().ContainSingle();
    }

    [Fact]
    public async Task GetActiveAsync_ReturnsOnlyIsActive()
    {
        await using var context = InMemoryDbContextFactory.Create();
        context.AIAssistants.AddRange(
            new AIAssistant { Id = Guid.NewGuid(), Name = "a", IsActive = true, CreatedAt = DateTime.UtcNow },
            new AIAssistant { Id = Guid.NewGuid(), Name = "b", IsActive = false, CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var repo = new AIAssistantRepository(context);
        var result = await repo.GetActiveAsync(CancellationToken.None);

        result.Should().ContainSingle();
    }

    [Fact]
    public async Task GetByTypeAsync_ParsesTypeAndFilters()
    {
        await using var context = InMemoryDbContextFactory.Create();
        context.AIAssistants.AddRange(
            new AIAssistant { Id = Guid.NewGuid(), Name = "a", AssistantType = AIAssistantType.Coach, CreatedAt = DateTime.UtcNow },
            new AIAssistant { Id = Guid.NewGuid(), Name = "b", AssistantType = AIAssistantType.General, CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var repo = new AIAssistantRepository(context);
        var result = await repo.GetByTypeAsync("Coach", CancellationToken.None);

        result.Should().ContainSingle();
        result[0].AssistantType.Should().Be(AIAssistantType.Coach);
    }

    [Fact]
    public async Task GetPublicAsync_ReturnsOnlyPublic()
    {
        await using var context = InMemoryDbContextFactory.Create();
        context.AIAssistants.AddRange(
            new AIAssistant { Id = Guid.NewGuid(), Name = "a", IsPublic = true, CreatedAt = DateTime.UtcNow },
            new AIAssistant { Id = Guid.NewGuid(), Name = "b", IsPublic = false, CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var repo = new AIAssistantRepository(context);
        var result = await repo.GetPublicAsync(CancellationToken.None);

        result.Should().ContainSingle();
    }
}
