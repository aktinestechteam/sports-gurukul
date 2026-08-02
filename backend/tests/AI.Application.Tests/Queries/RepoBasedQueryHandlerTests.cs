using System.Linq.Expressions;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.Queries;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace AI.Application.Tests.Queries;

public class SearchPromptsQueryHandlerTests
{
    private readonly Mock<IPromptTemplateRepository> _repo = new();
    private readonly SearchPromptsQueryHandler _handler;

    public SearchPromptsQueryHandlerTests()
    {
        _handler = new SearchPromptsQueryHandler(_repo.Object);
    }

    [Fact]
    public async Task Handle_FiltersByNameTypeStatusAndCategory()
    {
        var prompts = new List<PromptTemplate>
        {
            new() { Id = Guid.NewGuid(), Name = "Cricket Intro", Description = "intro", Type = PromptType.System, Status = PromptStatus.Active, Category = "Cricket", CurrentVersion = 1, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Bowling", Type = PromptType.User, Status = PromptStatus.Inactive, Category = "Cricket", CurrentVersion = 1, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Basketball", Type = PromptType.System, Status = PromptStatus.Active, Category = "Hoops", CurrentVersion = 1, CreatedAt = DateTime.UtcNow }
        };
        _repo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<PromptTemplate, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(prompts);

        var result = await _handler.Handle(
            new SearchPromptsQuery("cricket", PromptType.System, PromptStatus.Active, "cricket", 1, 20),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.TotalCount.Should().Be(1);
        result.Value.Items.Should().ContainSingle();
        result.Value.Items[0].Name.Should().Be("Cricket Intro");
        result.Value.Items[0].Status.Should().Be(PromptStatus.Active);
    }

    [Fact]
    public async Task Handle_NoFilters_ReturnsAllPaged()
    {
        var prompts = new List<PromptTemplate>();
        for (var i = 0; i < 25; i++)
        {
            prompts.Add(new PromptTemplate
            {
                Id = Guid.NewGuid(), Name = $"P{i}", Type = PromptType.System, Status = PromptStatus.Draft,
                CurrentVersion = 1, CreatedAt = DateTime.UtcNow
            });
        }
        _repo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<PromptTemplate, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(prompts);

        var result = await _handler.Handle(new SearchPromptsQuery(null, null, null, null, 2, 20), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.TotalCount.Should().Be(25);
        result.Value.Items.Should().HaveCount(5);
        result.Value.Page.Should().Be(2);
    }
}

public class SearchAgentsQueryHandlerTests
{
    private readonly Mock<IAgentDefinitionRepository> _repo = new();
    private readonly SearchAgentsQueryHandler _handler;

    public SearchAgentsQueryHandlerTests()
    {
        _handler = new SearchAgentsQueryHandler(_repo.Object);
    }

    [Fact]
    public async Task Handle_FiltersBySearchTermStatusAndAssistant()
    {
        var assistantId = Guid.NewGuid();
        var agents = new List<AgentDefinition>
        {
            new() { Id = Guid.NewGuid(), Name = "Analyst One", Description = "data", Status = AgentStatus.Active, AssistantId = assistantId, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Coach", Status = AgentStatus.Inactive, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Analyst Two", Status = AgentStatus.Active, CreatedAt = DateTime.UtcNow }
        };
        _repo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<AgentDefinition, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(agents);

        var result = await _handler.Handle(
            new SearchAgentsQuery("analyst", AgentStatus.Active, assistantId, 1, 20),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.TotalCount.Should().Be(1);
        result.Value.Items.Should().ContainSingle();
        result.Value.Items[0].Name.Should().Be("Analyst One");
    }

    [Fact]
    public async Task Handle_NoMatches_ReturnsEmpty()
    {
        _repo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<AgentDefinition, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AgentDefinition>());

        var result = await _handler.Handle(new SearchAgentsQuery("zzz", null, null, 1, 20), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.TotalCount.Should().Be(0);
        result.Value.Items.Should().BeEmpty();
    }
}

public class SearchAssistantsQueryHandlerTests
{
    private readonly Mock<IAIAssistantRepository> _repo = new();
    private readonly SearchAssistantsQueryHandler _handler;

    public SearchAssistantsQueryHandlerTests()
    {
        _handler = new SearchAssistantsQueryHandler(_repo.Object);
    }

    [Fact]
    public async Task Handle_FiltersByTypeActiveAndPublic()
    {
        var assistants = new List<AIAssistant>
        {
            new() { Id = Guid.NewGuid(), Name = "Coach A", AssistantType = AIAssistantType.Coach, IsActive = true, IsPublic = true, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Coach B", AssistantType = AIAssistantType.Coach, IsActive = true, IsPublic = false, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Analyst", AssistantType = AIAssistantType.Analyst, IsActive = true, IsPublic = true, CreatedAt = DateTime.UtcNow }
        };
        _repo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<AIAssistant, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(assistants);

        var result = await _handler.Handle(
            new SearchAssistantsQuery("coach", AIAssistantType.Coach, true, true, 1, 20),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.TotalCount.Should().Be(1);
        result.Value.Items[0].Name.Should().Be("Coach A");
        result.Value.Items[0].Personality.Should().Be(AIAssistantPersonality.Professional);
    }
}

public class SearchConversationsQueryHandlerTests
{
    private readonly Mock<IConversationRepository> _repo = new();
    private readonly SearchConversationsQueryHandler _handler;

    public SearchConversationsQueryHandlerTests()
    {
        _handler = new SearchConversationsQueryHandler(_repo.Object);
    }

    [Fact]
    public async Task Handle_FiltersByTermAssistantUserAndDates()
    {
        var assistantId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var from = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var to = new DateTime(2024, 12, 31, 0, 0, 0, DateTimeKind.Utc);
        var conversations = new List<Conversation>
        {
            new()
            {
                Id = Guid.NewGuid(), Title = "Match Analysis", AssistantId = assistantId, UserId = userId,
                Status = ConversationStatus.Active, MessageCount = 3,
                CreatedAt = new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Utc),
                Assistant = new AIAssistant { Id = Guid.NewGuid(), Name = "Coach" }
            },
            new()
            {
                Id = Guid.NewGuid(), Title = "Other", AssistantId = assistantId, UserId = userId,
                Status = ConversationStatus.Active, CreatedAt = new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        };
        _repo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Conversation, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(conversations);

        var result = await _handler.Handle(
            new SearchConversationsQuery("match", assistantId, userId, ConversationStatus.Active, from, to, 1, 20),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.TotalCount.Should().Be(1);
        result.Value.Items[0].Title.Should().Be("Match Analysis");
        result.Value.Items[0].AssistantName.Should().Be("Coach");
    }
}

public class SearchKnowledgeBasesQueryHandlerTests
{
    private readonly Mock<IKnowledgeBaseRepository> _repo = new();
    private readonly SearchKnowledgeBasesQueryHandler _handler;

    public SearchKnowledgeBasesQueryHandlerTests()
    {
        _handler = new SearchKnowledgeBasesQueryHandler(_repo.Object);
    }

    [Fact]
    public async Task Handle_FiltersByVisibilityStatusAndCategory()
    {
        var bases = new List<KnowledgeBase>
        {
            new() { Id = Guid.NewGuid(), Name = "Cricket Rules", Visibility = KnowledgeBaseVisibility.Public, Status = KnowledgeBaseStatus.Published, Category = "Cricket", TotalDocuments = 2, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Cricket Stats", Visibility = KnowledgeBaseVisibility.Public, Status = KnowledgeBaseStatus.Draft, Category = "Cricket", CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Soccer", Visibility = KnowledgeBaseVisibility.Private, Status = KnowledgeBaseStatus.Published, Category = "Soccer", CreatedAt = DateTime.UtcNow }
        };
        _repo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<KnowledgeBase, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(bases);

        var result = await _handler.Handle(
            new SearchKnowledgeBasesQuery("cricket", KnowledgeBaseVisibility.Public, KnowledgeBaseStatus.Published, "cricket", 1, 20),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.TotalCount.Should().Be(1);
        result.Value.Items[0].Name.Should().Be("Cricket Rules");
        result.Value.Items[0].TotalDocuments.Should().Be(2);
    }
}

public class SearchWorkflowsQueryHandlerTests
{
    private readonly Mock<IWorkflowDefinitionRepository> _repo = new();
    private readonly SearchWorkflowsQueryHandler _handler;

    public SearchWorkflowsQueryHandlerTests()
    {
        _handler = new SearchWorkflowsQueryHandler(_repo.Object);
    }

    [Fact]
    public async Task Handle_FiltersByNameAndStatus()
    {
        var workflows = new List<WorkflowDefinition>
        {
            new() { Id = Guid.NewGuid(), Name = "Daily Digest", Status = WorkflowStatus.Active, Version = 1, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Nightly", Status = WorkflowStatus.Inactive, Version = 2, CreatedAt = DateTime.UtcNow }
        };
        _repo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<WorkflowDefinition, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(workflows);

        var result = await _handler.Handle(
            new SearchWorkflowsQuery("digest", WorkflowStatus.Active, 1, 20),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.TotalCount.Should().Be(1);
        result.Value.Items[0].Name.Should().Be("Daily Digest");
        result.Value.Items[0].Version.Should().Be(1);
    }
}

public class GetModelsQueryHandlerTests
{
    private readonly Mock<IAIModelRepository> _repo = new();
    private readonly GetModelsQueryHandler _handler;

    public GetModelsQueryHandlerTests()
    {
        _handler = new GetModelsQueryHandler(_repo.Object);
    }

    [Fact]
    public async Task Handle_FiltersBySearchProviderAndActiveOnly()
    {
        var providerId = Guid.NewGuid();
        var models = new List<AIModel>
        {
            new()
            {
                Id = Guid.NewGuid(), ProviderId = providerId, Name = "gpt-4o", DisplayName = "GPT-4o",
                Status = AIModelStatus.Active, SupportsStreaming = true, CreatedAt = DateTime.UtcNow,
                Provider = new AIProvider { Id = providerId, Name = "OpenAI" }
            },
            new()
            {
                Id = Guid.NewGuid(), ProviderId = providerId, Name = "gpt-3.5", DisplayName = "GPT-3.5",
                Status = AIModelStatus.Inactive, CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(), ProviderId = Guid.NewGuid(), Name = "gpt-4o", Status = AIModelStatus.Active,
                CreatedAt = DateTime.UtcNow
            }
        };
        _repo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<AIModel, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(models);

        var result = await _handler.Handle(
            new GetModelsQuery("gpt-4o", providerId, null, true, 1, 50),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.TotalCount.Should().Be(1);
        result.Value.Items[0].Name.Should().Be("gpt-4o");
        result.Value.Items[0].ProviderName.Should().Be("OpenAI");
    }

    [Fact]
    public async Task Handle_DeletedModelsAreExcluded()
    {
        var models = new List<AIModel>
        {
            new() { Id = Guid.NewGuid(), Name = "m1", Status = AIModelStatus.Active, IsDeleted = true, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "m2", Status = AIModelStatus.Active, CreatedAt = DateTime.UtcNow }
        };
        _repo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<AIModel, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<AIModel, bool>> predicate, CancellationToken _) =>
                models.Where(predicate.Compile()).ToList());

        var result = await _handler.Handle(new GetModelsQuery(null, null, null, false, 1, 50), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.TotalCount.Should().Be(1);
        result.Value.Items[0].Name.Should().Be("m2");
    }
}

public class TokenUsageQueryHandlerTests
{
    private readonly Mock<IAITokenUsageRepository> _repo = new();
    private readonly TokenUsageQueryHandler _handler;

    public TokenUsageQueryHandlerTests()
    {
        _handler = new TokenUsageQueryHandler(_repo.Object);
    }

    [Fact]
    public async Task Handle_FiltersByConversationUserAndDates()
    {
        var conversationId = Guid.NewGuid();
        var from = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var to = new DateTime(2024, 12, 31, 0, 0, 0, DateTimeKind.Utc);
        var usages = new List<AITokenUsage>
        {
            new()
            {
                Id = Guid.NewGuid(), ConversationId = conversationId, UserId = "user-1", ModelName = "gpt-4o",
                TotalTokens = 100, Cost = 1.5m, RequestType = "chat", CreatedAt = new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.NewGuid(), ConversationId = conversationId, UserId = "user-1", ModelName = "gpt-4o",
                TotalTokens = 50, CreatedAt = new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        };
        _repo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<AITokenUsage, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(usages);

        var result = await _handler.Handle(
            new TokenUsageQuery(conversationId, "user-1", from, to, 1, 20),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.TotalCount.Should().Be(2);
        result.Value.Items[0].TotalTokens.Should().Be(100);
        result.Value.Items[0].Cost.Should().Be(1.5m);
        result.Value.Items[0].RequestType.Should().Be("chat");
    }
}

public class AuditLogQueryHandlerTests
{
    private readonly Mock<IAIAuditLogRepository> _repo = new();
    private readonly AuditLogQueryHandler _handler;

    public AuditLogQueryHandlerTests()
    {
        _handler = new AuditLogQueryHandler(_repo.Object);
    }

    [Fact]
    public async Task Handle_FiltersByAllCriteria()
    {
        var entityId = Guid.NewGuid();
        var logs = new List<AIAuditLog>
        {
            new()
            {
                Id = Guid.NewGuid(), EntityId = entityId, EntityType = "Conversation", EventType = AuditEventType.Create,
                Severity = AuditSeverity.Info, Action = "create", ActorId = "u1", ActorType = "User",
                Message = "created", CreatedAt = new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.NewGuid(), EntityId = entityId, EntityType = "Conversation", EventType = AuditEventType.Update,
                Severity = AuditSeverity.Warning, CreatedAt = new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        };
        _repo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<AIAuditLog, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(logs);

        var result = await _handler.Handle(
            new AuditLogQuery("conversation", entityId, AuditEventType.Create, AuditSeverity.Info, null, null, 1, 20),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.TotalCount.Should().Be(1);
        result.Value.Items[0].EventType.Should().Be(AuditEventType.Create);
        result.Value.Items[0].Severity.Should().Be(AuditSeverity.Info);
        result.Value.Items[0].ActorId.Should().Be("u1");
    }
}

public class ConversationHistoryQueryHandlerTests
{
    private readonly Mock<IConversationRepository> _conversationRepo = new();
    private readonly Mock<IConversationMessageRepository> _messageRepo = new();
    private readonly ConversationHistoryQueryHandler _handler;

    public ConversationHistoryQueryHandlerTests()
    {
        _handler = new ConversationHistoryQueryHandler(_conversationRepo.Object, _messageRepo.Object);
    }

    [Fact]
    public async Task Handle_ConversationNotFound_ReturnsFailure()
    {
        _conversationRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Conversation?)null);

        var id = Guid.NewGuid();
        var result = await _handler.Handle(new ConversationHistoryQuery(id), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be($"Conversation {id} not found");
    }

    [Fact]
    public async Task Handle_Success_MapsMessages()
    {
        var conversationId = Guid.NewGuid();
        var conversation = new Conversation { Id = conversationId, CreatedAt = DateTime.UtcNow };
        var messages = new List<ConversationMessage>
        {
            new()
            {
                Id = Guid.NewGuid(), ConversationId = conversationId, Role = MessageRole.User,
                Status = MessageStatus.Sent, Content = "hello", TokensUsed = 3, CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(), ConversationId = conversationId, Role = MessageRole.Assistant,
                Status = MessageStatus.Sent, Content = "hi", CreatedAt = DateTime.UtcNow
            }
        };
        _conversationRepo.Setup(r => r.GetByIdAsync(conversationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(conversation);
        _messageRepo.Setup(r => r.GetByConversationIdAsync(conversationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(messages);

        var result = await _handler.Handle(new ConversationHistoryQuery(conversationId, 1, 50), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.TotalCount.Should().Be(2);
        result.Value.Items[0].Role.Should().Be(MessageRole.User);
        result.Value.Items[0].TokensUsed.Should().Be(3);
    }
}
