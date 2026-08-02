using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;
using SportsGurukul.Infrastructure.Persistence;
using SportsGurukul.Infrastructure.Persistence.Repositories.AI;

namespace AI.Infrastructure.Tests.Repositories;

public class WorkflowDefinitionRepositoryTests
{
    [Fact]
    public async Task GetByIdWithDetailsAsync_IncludesExecutions()
    {
        await using var context = InMemoryDbContextFactory.Create();
        var workflow = new WorkflowDefinition { Id = Guid.NewGuid(), Name = "W", CreatedAt = DateTime.UtcNow };
        var execution = new WorkflowExecution { Id = Guid.NewGuid(), WorkflowDefinitionId = workflow.Id, CreatedAt = DateTime.UtcNow };
        context.WorkflowDefinitions.Add(workflow);
        context.WorkflowExecutions.Add(execution);
        await context.SaveChangesAsync();

        var repo = new WorkflowDefinitionRepository(context);
        var loaded = await repo.GetByIdWithDetailsAsync(workflow.Id, CancellationToken.None);

        loaded.Should().NotBeNull();
        loaded!.Executions.Should().ContainSingle();
    }

    [Fact]
    public async Task GetActiveAsync_ReturnsOnlyActive()
    {
        await using var context = InMemoryDbContextFactory.Create();
        context.WorkflowDefinitions.AddRange(
            new WorkflowDefinition { Id = Guid.NewGuid(), Name = "a", Status = WorkflowStatus.Active, CreatedAt = DateTime.UtcNow },
            new WorkflowDefinition { Id = Guid.NewGuid(), Name = "b", Status = WorkflowStatus.Draft, CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var repo = new WorkflowDefinitionRepository(context);
        var result = await repo.GetActiveAsync(CancellationToken.None);

        result.Should().ContainSingle();
    }

    [Fact]
    public async Task GetByStatusAsync_ParsesAndFilters()
    {
        await using var context = InMemoryDbContextFactory.Create();
        context.WorkflowDefinitions.AddRange(
            new WorkflowDefinition { Id = Guid.NewGuid(), Name = "a", Status = WorkflowStatus.Archived, CreatedAt = DateTime.UtcNow },
            new WorkflowDefinition { Id = Guid.NewGuid(), Name = "b", Status = WorkflowStatus.Active, CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var repo = new WorkflowDefinitionRepository(context);
        var result = await repo.GetByStatusAsync("Archived", CancellationToken.None);

        result.Should().ContainSingle();
    }
}

public class WorkflowExecutionRepositoryTests
{
    [Fact]
    public async Task GetByIdWithDetailsAsync_IncludesWorkflowDefinition()
    {
        await using var context = InMemoryDbContextFactory.Create();
        var workflow = new WorkflowDefinition { Id = Guid.NewGuid(), Name = "W", CreatedAt = DateTime.UtcNow };
        var execution = new WorkflowExecution { Id = Guid.NewGuid(), WorkflowDefinitionId = workflow.Id, CreatedAt = DateTime.UtcNow };
        context.WorkflowDefinitions.Add(workflow);
        context.WorkflowExecutions.Add(execution);
        await context.SaveChangesAsync();

        var repo = new WorkflowExecutionRepository(context);
        var loaded = await repo.GetByIdWithDetailsAsync(execution.Id, CancellationToken.None);

        loaded.Should().NotBeNull();
        loaded!.WorkflowDefinition.Should().NotBeNull();
    }

    [Fact]
    public async Task GetByWorkflowDefinitionIdAsync_ReturnsOnlyMatching()
    {
        await using var context = InMemoryDbContextFactory.Create();
        var workflowA = Guid.NewGuid();
        context.WorkflowExecutions.AddRange(
            new WorkflowExecution { Id = Guid.NewGuid(), WorkflowDefinitionId = workflowA, CreatedAt = DateTime.UtcNow },
            new WorkflowExecution { Id = Guid.NewGuid(), WorkflowDefinitionId = workflowA, CreatedAt = DateTime.UtcNow },
            new WorkflowExecution { Id = Guid.NewGuid(), WorkflowDefinitionId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var repo = new WorkflowExecutionRepository(context);
        var result = await repo.GetByWorkflowDefinitionIdAsync(workflowA, CancellationToken.None);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByStatusAsync_ParsesAndFilters()
    {
        await using var context = InMemoryDbContextFactory.Create();
        context.WorkflowExecutions.AddRange(
            new WorkflowExecution { Id = Guid.NewGuid(), Status = WorkflowExecutionStatus.Failed, CreatedAt = DateTime.UtcNow },
            new WorkflowExecution { Id = Guid.NewGuid(), Status = WorkflowExecutionStatus.Running, CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var repo = new WorkflowExecutionRepository(context);
        var result = await repo.GetByStatusAsync("Failed", CancellationToken.None);

        result.Should().ContainSingle();
    }
}

public class ToolDefinitionRepositoryTests
{
    [Fact]
    public async Task GetByIdWithDetailsAsync_IncludesExecutions()
    {
        await using var context = InMemoryDbContextFactory.Create();
        var tool = new ToolDefinition { Id = Guid.NewGuid(), Name = "T", CreatedAt = DateTime.UtcNow };
        var execution = new ToolExecution { Id = Guid.NewGuid(), ToolDefinitionId = tool.Id, CreatedAt = DateTime.UtcNow };
        context.ToolDefinitions.Add(tool);
        context.ToolExecutions.Add(execution);
        await context.SaveChangesAsync();

        var repo = new ToolDefinitionRepository(context);
        var loaded = await repo.GetByIdWithDetailsAsync(tool.Id, CancellationToken.None);

        loaded.Should().NotBeNull();
        loaded!.Executions.Should().ContainSingle();
    }

    [Fact]
    public async Task GetActiveAsync_ReturnsOnlyActive()
    {
        await using var context = InMemoryDbContextFactory.Create();
        context.ToolDefinitions.AddRange(
            new ToolDefinition { Id = Guid.NewGuid(), Name = "a", Status = ToolStatus.Active, CreatedAt = DateTime.UtcNow },
            new ToolDefinition { Id = Guid.NewGuid(), Name = "b", Status = ToolStatus.Inactive, CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var repo = new ToolDefinitionRepository(context);
        var result = await repo.GetActiveAsync(CancellationToken.None);

        result.Should().ContainSingle();
    }

    [Fact]
    public async Task GetByTypeAsync_ParsesAndFilters()
    {
        await using var context = InMemoryDbContextFactory.Create();
        context.ToolDefinitions.AddRange(
            new ToolDefinition { Id = Guid.NewGuid(), Name = "a", Type = ToolType.Api, CreatedAt = DateTime.UtcNow },
            new ToolDefinition { Id = Guid.NewGuid(), Name = "b", Type = ToolType.Function, CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var repo = new ToolDefinitionRepository(context);
        var result = await repo.GetByTypeAsync("Api", CancellationToken.None);

        result.Should().ContainSingle();
    }
}

public class ToolExecutionRepositoryTests
{
    [Fact]
    public async Task GetByIdWithDetailsAsync_IncludesRelations()
    {
        await using var context = InMemoryDbContextFactory.Create();
        var tool = new ToolDefinition { Id = Guid.NewGuid(), Name = "T", CreatedAt = DateTime.UtcNow };
        var conversation = new Conversation { Id = Guid.NewGuid(), Title = "C", CreatedAt = DateTime.UtcNow };
        var execution = new ToolExecution { Id = Guid.NewGuid(), ToolDefinitionId = tool.Id, ConversationId = conversation.Id, CreatedAt = DateTime.UtcNow };
        context.ToolDefinitions.Add(tool);
        context.Conversations.Add(conversation);
        context.ToolExecutions.Add(execution);
        await context.SaveChangesAsync();

        var repo = new ToolExecutionRepository(context);
        var loaded = await repo.GetByIdWithDetailsAsync(execution.Id, CancellationToken.None);

        loaded.Should().NotBeNull();
        loaded!.ToolDefinition.Should().NotBeNull();
        loaded.Conversation.Should().NotBeNull();
    }

    [Fact]
    public async Task GetByToolDefinitionIdAsync_ReturnsOnlyMatching()
    {
        await using var context = InMemoryDbContextFactory.Create();
        var toolA = Guid.NewGuid();
        context.ToolExecutions.AddRange(
            new ToolExecution { Id = Guid.NewGuid(), ToolDefinitionId = toolA, CreatedAt = DateTime.UtcNow },
            new ToolExecution { Id = Guid.NewGuid(), ToolDefinitionId = toolA, CreatedAt = DateTime.UtcNow },
            new ToolExecution { Id = Guid.NewGuid(), ToolDefinitionId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var repo = new ToolExecutionRepository(context);
        var result = await repo.GetByToolDefinitionIdAsync(toolA, CancellationToken.None);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByConversationIdAsync_ReturnsOnlyMatching()
    {
        await using var context = InMemoryDbContextFactory.Create();
        var convA = Guid.NewGuid();
        context.ToolExecutions.AddRange(
            new ToolExecution { Id = Guid.NewGuid(), ConversationId = convA, CreatedAt = DateTime.UtcNow },
            new ToolExecution { Id = Guid.NewGuid(), ConversationId = convA, CreatedAt = DateTime.UtcNow },
            new ToolExecution { Id = Guid.NewGuid(), ConversationId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var repo = new ToolExecutionRepository(context);
        var result = await repo.GetByConversationIdAsync(convA, CancellationToken.None);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetRecentAsync_OrdersDescendingAndLimits()
    {
        await using var context = InMemoryDbContextFactory.Create();
        var conv = Guid.NewGuid();
        context.ToolExecutions.AddRange(
            new ToolExecution { Id = Guid.NewGuid(), ConversationId = conv },
            new ToolExecution { Id = Guid.NewGuid(), ConversationId = conv },
            new ToolExecution { Id = Guid.NewGuid(), ConversationId = conv });
        await context.SaveChangesAsync();
        var all = context.ToolExecutions.ToList();
        all[0].CreatedAt = DateTime.UtcNow.AddMinutes(-5);
        all[1].CreatedAt = DateTime.UtcNow;
        all[2].CreatedAt = DateTime.UtcNow.AddMinutes(-2);
        await context.SaveChangesAsync();

        var repo = new ToolExecutionRepository(context);
        var result = await repo.GetRecentAsync(conv, 2, CancellationToken.None);

        result.Should().HaveCount(2);
        result[0].Id.Should().Be(all[1].Id);
        result[1].Id.Should().Be(all[2].Id);
    }
}
