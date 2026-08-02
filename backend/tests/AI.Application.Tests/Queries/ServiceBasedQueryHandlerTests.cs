using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.Queries;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace AI.Application.Tests.Queries;

public class AgentQueryHandlerTests
{
    private readonly Mock<IAgentService> _service = new();
    private readonly AgentQueryHandler _handler;

    public AgentQueryHandlerTests()
    {
        _handler = new AgentQueryHandler(_service.Object);
    }

    [Fact]
    public async Task Handle_Success_MapsAgentDto()
    {
        var agent = new AgentDefinition
        {
            Id = Guid.NewGuid(),
            Name = "Analyst",
            Status = AgentStatus.Active,
            MaxIterations = 5,
            RequiresApproval = true,
            CreatedAt = DateTime.UtcNow,
            Assistant = new AIAssistant { Id = Guid.NewGuid(), Name = "Coach" }
        };
        _service.Setup(s => s.GetByIdAsync(agent.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AgentDefinition>.Success(agent));

        var result = await _handler.Handle(new AgentQuery(agent.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        var dto = result.Value!;
        dto.Id.Should().Be(agent.Id);
        dto.Name.Should().Be("Analyst");
        dto.AssistantName.Should().Be("Coach");
        dto.MaxIterations.Should().Be(5);
        dto.RequiresApproval.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Failure_ReturnsFailure()
    {
        _service.Setup(s => s.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AgentDefinition>.Failure("Agent not found"));

        var result = await _handler.Handle(new AgentQuery(Guid.NewGuid()), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Agent not found");
    }
}

public class AssistantQueryHandlerTests
{
    private readonly Mock<IAssistantService> _service = new();
    private readonly AssistantQueryHandler _handler;

    public AssistantQueryHandlerTests()
    {
        _handler = new AssistantQueryHandler(_service.Object);
    }

    [Fact]
    public async Task Handle_Success_MapsAssistantDto()
    {
        var assistant = new AIAssistant
        {
            Id = Guid.NewGuid(),
            Name = "Coach",
            AssistantType = AIAssistantType.Coach,
            Personality = AIAssistantPersonality.Enthusiastic,
            IsActive = true,
            IsPublic = true,
            MaxHistoryLength = 50,
            CreatedAt = DateTime.UtcNow
        };
        _service.Setup(s => s.GetByIdAsync(assistant.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AIAssistant>.Success(assistant));

        var result = await _handler.Handle(new AssistantQuery(assistant.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        var dto = result.Value!;
        dto.Id.Should().Be(assistant.Id);
        dto.Name.Should().Be("Coach");
        dto.AssistantType.Should().Be(AIAssistantType.Coach);
        dto.Personality.Should().Be(AIAssistantPersonality.Enthusiastic);
        dto.IsActive.Should().BeTrue();
        dto.IsPublic.Should().BeTrue();
        dto.MaxHistoryLength.Should().Be(50);
    }

    [Fact]
    public async Task Handle_Failure_ReturnsFailure()
    {
        _service.Setup(s => s.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AIAssistant>.Failure("Assistant not found"));

        var result = await _handler.Handle(new AssistantQuery(Guid.NewGuid()), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Assistant not found");
    }
}

public class GetConversationQueryHandlerTests
{
    private readonly Mock<IConversationService> _service = new();
    private readonly GetConversationQueryHandler _handler;

    public GetConversationQueryHandlerTests()
    {
        _handler = new GetConversationQueryHandler(_service.Object);
    }

    [Fact]
    public async Task Handle_Success_MapsConversationDtoWithMessages()
    {
        var conversation = new Conversation
        {
            Id = Guid.NewGuid(),
            Title = "T",
            Status = ConversationStatus.Active,
            MessageCount = 2,
            CreatedAt = DateTime.UtcNow,
            Assistant = new AIAssistant { Id = Guid.NewGuid(), Name = "Coach" },
            Messages =
            [
                new ConversationMessage
                {
                    Id = Guid.NewGuid(),
                    ConversationId = Guid.NewGuid(),
                    Role = MessageRole.User,
                    Status = MessageStatus.Sent,
                    Content = "hi",
                    CreatedAt = DateTime.UtcNow
                }
            ]
        };
        _service.Setup(s => s.GetByIdAsync(conversation.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Conversation>.Success(conversation));

        var result = await _handler.Handle(new GetConversationQuery(conversation.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        var dto = result.Value!;
        dto.Id.Should().Be(conversation.Id);
        dto.Title.Should().Be("T");
        dto.AssistantName.Should().Be("Coach");
        dto.Messages.Should().HaveCount(1);
        dto.Messages[0].Content.Should().Be("hi");
    }

    [Fact]
    public async Task Handle_Failure_ReturnsFailure()
    {
        _service.Setup(s => s.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Conversation>.Failure("Conversation not found"));

        var result = await _handler.Handle(new GetConversationQuery(Guid.NewGuid()), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Conversation not found");
    }
}

public class KnowledgeBaseQueryHandlerTests
{
    private readonly Mock<IKnowledgeService> _service = new();
    private readonly KnowledgeBaseQueryHandler _handler;

    public KnowledgeBaseQueryHandlerTests()
    {
        _handler = new KnowledgeBaseQueryHandler(_service.Object);
    }

    [Fact]
    public async Task Handle_Success_MapsKnowledgeBaseDtoWithSources()
    {
        var kb = new KnowledgeBase
        {
            Id = Guid.NewGuid(),
            Name = "Cricket",
            Visibility = KnowledgeBaseVisibility.Public,
            Status = KnowledgeBaseStatus.Published,
            TotalDocuments = 3,
            CreatedAt = DateTime.UtcNow,
            Sources =
            [
                new KnowledgeSource
                {
                    Id = Guid.NewGuid(),
                    KnowledgeBaseId = Guid.NewGuid(),
                    Name = "Wikipedia",
                    SourceType = KnowledgeSourceType.WebPage,
                    Status = SourceStatus.Indexed,
                    DocumentCount = 2,
                    LastSyncAt = DateTime.UtcNow
                }
            ]
        };
        _service.Setup(s => s.GetBaseByIdAsync(kb.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<KnowledgeBase>.Success(kb));

        var result = await _handler.Handle(new KnowledgeBaseQuery(kb.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        var dto = result.Value!;
        dto.Id.Should().Be(kb.Id);
        dto.Name.Should().Be("Cricket");
        dto.Status.Should().Be(KnowledgeBaseStatus.Published);
        dto.Sources.Should().HaveCount(1);
        dto.Sources![0].Name.Should().Be("Wikipedia");
        dto.Sources[0].SourceType.Should().Be(KnowledgeSourceType.WebPage);
    }

    [Fact]
    public async Task Handle_Failure_ReturnsFailure()
    {
        _service.Setup(s => s.GetBaseByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<KnowledgeBase>.Failure("Knowledge base not found"));

        var result = await _handler.Handle(new KnowledgeBaseQuery(Guid.NewGuid()), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }
}

public class PromptQueryHandlerTests
{
    private readonly Mock<IPromptService> _service = new();
    private readonly PromptQueryHandler _handler;

    public PromptQueryHandlerTests()
    {
        _handler = new PromptQueryHandler(_service.Object);
    }

    [Fact]
    public async Task Handle_Success_MapsPromptTemplateDtoWithVersions()
    {
        var template = new PromptTemplate
        {
            Id = Guid.NewGuid(),
            Name = "Intro",
            Type = PromptType.System,
            Status = PromptStatus.Active,
            TemplateContent = "content",
            CurrentVersion = 2,
            CreatedAt = DateTime.UtcNow,
            Versions =
            [
                new PromptVersion
                {
                    Id = Guid.NewGuid(),
                    PromptTemplateId = Guid.NewGuid(),
                    VersionNumber = 1,
                    Content = "v1",
                    CreatedAt = DateTime.UtcNow
                }
            ]
        };
        _service.Setup(s => s.GetByIdAsync(template.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PromptTemplate>.Success(template));

        var result = await _handler.Handle(new PromptQuery(template.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        var dto = result.Value!;
        dto.Id.Should().Be(template.Id);
        dto.Name.Should().Be("Intro");
        dto.CurrentVersion.Should().Be(2);
        dto.TemplateContent.Should().Be("content");
        dto.Versions.Should().HaveCount(1);
        dto.Versions[0].VersionNumber.Should().Be(1);
    }

    [Fact]
    public async Task Handle_Failure_ReturnsFailure()
    {
        _service.Setup(s => s.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PromptTemplate>.Failure("Prompt template not found"));

        var result = await _handler.Handle(new PromptQuery(Guid.NewGuid()), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }
}

public class WorkflowQueryHandlerTests
{
    private readonly Mock<IWorkflowService> _service = new();
    private readonly WorkflowQueryHandler _handler;

    public WorkflowQueryHandlerTests()
    {
        _handler = new WorkflowQueryHandler(_service.Object);
    }

    [Fact]
    public async Task Handle_Success_MapsWorkflowDto()
    {
        var workflow = new WorkflowDefinition
        {
            Id = Guid.NewGuid(),
            Name = "Flow",
            Status = WorkflowStatus.Active,
            Steps = "steps",
            Version = 2,
            CreatedAt = DateTime.UtcNow
        };
        _service.Setup(s => s.GetByIdAsync(workflow.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<WorkflowDefinition>.Success(workflow));

        var result = await _handler.Handle(new WorkflowQuery(workflow.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        var dto = result.Value!;
        dto.Id.Should().Be(workflow.Id);
        dto.Name.Should().Be("Flow");
        dto.Steps.Should().Be("steps");
        dto.Version.Should().Be(2);
    }

    [Fact]
    public async Task Handle_Failure_ReturnsFailure()
    {
        _service.Setup(s => s.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<WorkflowDefinition>.Failure("Workflow not found"));

        var result = await _handler.Handle(new WorkflowQuery(Guid.NewGuid()), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Workflow not found");
    }
}
