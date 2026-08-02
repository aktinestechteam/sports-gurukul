using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Application.Common.Interfaces.AI.Models;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.Commands.Conversation;
using SportsGurukul.Application.Features.AIManagement.Commands.Knowledge;
using SportsGurukul.Application.Features.AIManagement.Commands.Prompt;
using SportsGurukul.Application.Features.AIManagement.Commands.Workflow;
using SportsGurukul.Application.Features.AIManagement.Queries;
using System.Linq.Expressions;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace AI.Application.Tests.Commands;

public class PromptHandlerChildMappingCoverageTests
{
    private static PromptTemplate BuildWithVersions() => new()
    {
        Id = Guid.NewGuid(),
        Name = "Intro",
        Description = "desc",
        Type = PromptType.System,
        Status = PromptStatus.Draft,
        TemplateContent = "content",
        Variables = "vars",
        Tags = "tags",
        Category = "cat",
        CurrentVersion = 1,
        CreatedAt = DateTime.UtcNow,
        Versions =
        [
            new PromptVersion
            {
                Id = Guid.NewGuid(),
                PromptTemplateId = Guid.NewGuid(),
                VersionNumber = 1,
                Content = "content",
                ChangeNotes = "notes",
                Hash = "hash",
                CreatedAt = DateTime.UtcNow
            }
        ]
    };

    [Fact]
    public async Task CreatePromptTemplateHandler_PopulatedVersions_MapsChildDtos()
    {
        var template = BuildWithVersions();
        var service = new Mock<IPromptService>();
        var unitOfWork = new Mock<IUnitOfWork>();
        service.Setup(s => s.CreateAsync(It.IsAny<CreatePromptTemplateRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PromptTemplate>.Success(template));

        var result = await new CreatePromptTemplateCommandHandler(service.Object, unitOfWork.Object).Handle(
            new CreatePromptTemplateCommand("Intro", "desc", PromptType.System, "content", "vars", "tags", "cat"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Versions.Should().HaveCount(1);
        result.Value.Versions[0].Content.Should().Be("content");
        result.Value.Versions[0].ChangeNotes.Should().Be("notes");
    }

    [Fact]
    public async Task UpdatePromptTemplateHandler_PopulatedVersions_MapsChildDtos()
    {
        var template = BuildWithVersions();
        var service = new Mock<IPromptService>();
        var unitOfWork = new Mock<IUnitOfWork>();
        service.Setup(s => s.UpdateAsync(It.IsAny<UpdatePromptTemplateRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PromptTemplate>.Success(template));

        var result = await new UpdatePromptTemplateCommandHandler(service.Object, unitOfWork.Object).Handle(
            new UpdatePromptTemplateCommand(template.Id, "Intro", "desc", "content", "vars", "tags", "cat"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Versions.Should().HaveCount(1);
    }

    [Fact]
    public async Task PublishPromptTemplateHandler_PopulatedVersions_MapsChildDtos()
    {
        var template = BuildWithVersions();
        template.Status = PromptStatus.Active;
        var service = new Mock<IPromptService>();
        var unitOfWork = new Mock<IUnitOfWork>();
        service.Setup(s => s.PublishAsync(template.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PromptTemplate>.Success(template));

        var result = await new PublishPromptTemplateCommandHandler(service.Object, unitOfWork.Object).Handle(
            new PublishPromptTemplateCommand(template.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Versions.Should().HaveCount(1);
    }

    [Fact]
    public async Task ClonePromptHandler_PopulatedVersions_MapsChildDtos()
    {
        var template = BuildWithVersions();
        var service = new Mock<IPromptService>();
        var unitOfWork = new Mock<IUnitOfWork>();
        service.Setup(s => s.CloneAsync(template.Id, "Clone", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PromptTemplate>.Success(template));

        var result = await new ClonePromptCommandHandler(service.Object, unitOfWork.Object).Handle(
            new ClonePromptCommand(template.Id, "Clone"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Versions.Should().HaveCount(1);
    }

    [Fact]
    public async Task RollbackPromptHandler_PopulatedVersions_MapsChildDtos()
    {
        var template = BuildWithVersions();
        var service = new Mock<IPromptService>();
        var unitOfWork = new Mock<IUnitOfWork>();
        service.Setup(s => s.RollbackAsync(template.Id, 2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PromptTemplate>.Success(template));

        var result = await new RollbackPromptVersionCommandHandler(service.Object, unitOfWork.Object).Handle(
            new RollbackPromptVersionCommand(template.Id, 2), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Versions.Should().HaveCount(1);
    }
}

public class KnowledgeHandlerChildMappingCoverageTests
{
    private static KnowledgeBase BuildWithSources() => new()
    {
        Id = Guid.NewGuid(),
        Name = "Cricket Rules",
        Description = "desc",
        Visibility = KnowledgeBaseVisibility.Public,
        Status = KnowledgeBaseStatus.Published,
        Category = "cat",
        Tags = "tags",
        IconUrl = "icon",
        TotalSources = 1,
        TotalDocuments = 2,
        TotalSizeBytes = 1000,
        CreatedAt = DateTime.UtcNow,
        Sources =
        [
            new KnowledgeSource
            {
                Id = Guid.NewGuid(),
                KnowledgeBaseId = Guid.NewGuid(),
                Name = "Wiki",
                SourceType = KnowledgeSourceType.WebPage,
                Status = SourceStatus.Indexed,
                DocumentCount = 2,
                LastSyncAt = DateTime.UtcNow
            }
        ]
    };

    [Fact]
    public async Task CreateKnowledgeBaseHandler_PopulatedSources_MapsChildDtos()
    {
        var kb = BuildWithSources();
        var service = new Mock<IKnowledgeService>();
        var unitOfWork = new Mock<IUnitOfWork>();
        service.Setup(s => s.CreateBaseAsync(It.IsAny<CreateKnowledgeBaseRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<KnowledgeBase>.Success(kb));

        var result = await new CreateKnowledgeBaseCommandHandler(service.Object, unitOfWork.Object).Handle(
            new CreateKnowledgeBaseCommand("Cricket Rules", "desc", KnowledgeBaseVisibility.Public, "cat", "tags"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Sources!.Should().HaveCount(1);
        result.Value.Sources![0].Name.Should().Be("Wiki");
        result.Value.Sources[0].SourceType.Should().Be(KnowledgeSourceType.WebPage);
    }

    [Fact]
    public async Task UpdateKnowledgeBaseHandler_PopulatedSources_MapsChildDtos()
    {
        var kb = BuildWithSources();
        var service = new Mock<IKnowledgeService>();
        var unitOfWork = new Mock<IUnitOfWork>();
        service.Setup(s => s.UpdateBaseAsync(It.IsAny<UpdateKnowledgeBaseRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<KnowledgeBase>.Success(kb));

        var result = await new UpdateKnowledgeBaseCommandHandler(service.Object, unitOfWork.Object).Handle(
            new UpdateKnowledgeBaseCommand(kb.Id, "Updated", "desc", KnowledgeBaseVisibility.Private, "cat", "tags"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Sources.Should().HaveCount(1);
    }
}

public class ConversationHandlerChildMappingCoverageTests
{
    private static Conversation BuildWithMessages() => new()
    {
        Id = Guid.NewGuid(),
        Title = "Chat",
        AssistantId = Guid.NewGuid(),
        UserId = Guid.NewGuid(),
        Status = ConversationStatus.Active,
        ContextSummary = "summary",
        TokenCount = 100,
        MessageCount = 1,
        LastActivityAt = DateTime.UtcNow,
        Metadata = "{}",
        CreatedAt = DateTime.UtcNow,
        Assistant = new AIAssistant { Id = Guid.NewGuid(), Name = "CoachAI" },
        Messages =
        [
            new ConversationMessage
            {
                Id = Guid.NewGuid(),
                ConversationId = Guid.NewGuid(),
                Role = MessageRole.User,
                Status = MessageStatus.Sent,
                Content = "Hi",
                TokensUsed = 5,
                ToolCalls = "[]",
                ToolResults = "[]",
                ErrorMessage = null,
                Cost = 0.01m,
                LatencyMs = 10.5,
                Metadata = "{}",
                CreatedAt = DateTime.UtcNow
            }
        ]
    };

    [Fact]
    public async Task AddMessageHandler_PopulatedMessages_MapsChildDtos()
    {
        var conversation = BuildWithMessages();
        var message = new ConversationMessage
        {
            Id = Guid.NewGuid(),
            ConversationId = conversation.Id,
            Role = MessageRole.Assistant,
            Content = "gen",
            CreatedAt = DateTime.UtcNow,
            Conversation = conversation
        };
        var service = new Mock<IConversationService>();
        var unitOfWork = new Mock<IUnitOfWork>();
        service.Setup(s => s.AddMessageAsync(It.IsAny<AddMessageRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ConversationMessage>.Success(message));

        var result = await new AddMessageCommandHandler(service.Object, unitOfWork.Object).Handle(
            new AddMessageCommand(conversation.Id, MessageRole.Assistant, "gen", null), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Messages.Should().HaveCount(1);
        result.Value.Messages[0].Content.Should().Be("Hi");
        result.Value.Messages[0].Cost.Should().Be(0.01m);
    }

    [Fact]
    public async Task ArchiveConversationHandler_PopulatedMessages_MapsChildDtos()
    {
        var conversation = BuildWithMessages();
        conversation.Status = ConversationStatus.Archived;
        var service = new Mock<IConversationService>();
        var unitOfWork = new Mock<IUnitOfWork>();
        service.Setup(s => s.ArchiveAsync(conversation.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Conversation>.Success(conversation));

        var result = await new ArchiveConversationCommandHandler(service.Object, unitOfWork.Object).Handle(
            new ArchiveConversationCommand(conversation.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Messages.Should().HaveCount(1);
    }

    [Fact]
    public async Task RenameConversationHandler_PopulatedMessages_MapsChildDtos()
    {
        var conversation = BuildWithMessages();
        conversation.Title = "Renamed";
        var service = new Mock<IConversationService>();
        var unitOfWork = new Mock<IUnitOfWork>();
        service.Setup(s => s.RenameAsync(conversation.Id, "Renamed", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Conversation>.Success(conversation));

        var result = await new RenameConversationCommandHandler(service.Object, unitOfWork.Object).Handle(
            new RenameConversationCommand(conversation.Id, "Renamed"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Messages.Should().HaveCount(1);
    }

    [Fact]
    public async Task RegenerateResponseHandler_PopulatedMessages_MapsChildDtos()
    {
        var conversation = BuildWithMessages();
        var message = new ConversationMessage
        {
            Id = Guid.NewGuid(),
            ConversationId = conversation.Id,
            Content = "regenerated",
            CreatedAt = DateTime.UtcNow,
            Conversation = conversation
        };
        var service = new Mock<IConversationService>();
        var unitOfWork = new Mock<IUnitOfWork>();
        service.Setup(s => s.RegenerateResponseAsync(conversation.Id, message.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ConversationMessage>.Success(message));

        var result = await new RegenerateResponseCommandHandler(service.Object, unitOfWork.Object).Handle(
            new RegenerateResponseCommand(conversation.Id, message.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Messages.Should().HaveCount(1);
    }
}

public class UpdateWorkflowAllFieldsCoverageTests
{
    private readonly Mock<IWorkflowDefinitionRepository> _workflowRepo = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly UpdateWorkflowCommandHandler _handler;

    public UpdateWorkflowAllFieldsCoverageTests()
    {
        _handler = new UpdateWorkflowCommandHandler(_workflowRepo.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_AllFieldsProvided_UpdatesEveryFieldAndIncrementsVersion()
    {
        var workflow = new WorkflowDefinition
        {
            Id = Guid.NewGuid(),
            Name = "Old",
            Version = 1,
            CreatedAt = DateTime.UtcNow
        };
        _workflowRepo.Setup(r => r.GetByIdAsync(workflow.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(workflow);

        var result = await _handler.Handle(
            new UpdateWorkflowCommand(workflow.Id, "New", "desc", "steps", "triggers", "conds", "vars"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        var dto = result.Value!;
        dto.Name.Should().Be("New");
        dto.Description.Should().Be("desc");
        dto.Steps.Should().Be("steps");
        dto.Triggers.Should().Be("triggers");
        dto.Conditions.Should().Be("conds");
        dto.Variables.Should().Be("vars");
        dto.Version.Should().Be(2);
    }
}

public class GetModelsQueryDisplayNameCoverageTests
{
    private readonly Mock<IAIModelRepository> _repo = new();
    private readonly GetModelsQueryHandler _handler;

    public GetModelsQueryDisplayNameCoverageTests()
    {
        _handler = new GetModelsQueryHandler(_repo.Object);
    }

    [Fact]
    public async Task Handle_SearchMatchesDisplayNameOnly_FiltersByIdentifier()
    {
        var models = new List<AIModel>
        {
            new()
            {
                Id = Guid.NewGuid(), Name = "gpt-turbo", DisplayName = "Turbo Search Match",
                Status = AIModelStatus.Active, CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(), Name = "claude", DisplayName = "Other",
                Status = AIModelStatus.Active, CreatedAt = DateTime.UtcNow
            }
        };
        _repo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<AIModel, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(models);

        var result = await _handler.Handle(
            new GetModelsQuery("Search", null, null, false, 1, 50), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.TotalCount.Should().Be(1);
        result.Value.Items[0].Name.Should().Be("gpt-turbo");
    }
}

public class AuditLogQueryDateRangeCoverageTests
{
    private readonly Mock<IAIAuditLogRepository> _repo = new();
    private readonly AuditLogQueryHandler _handler;

    public AuditLogQueryDateRangeCoverageTests()
    {
        _handler = new AuditLogQueryHandler(_repo.Object);
    }

    [Fact]
    public async Task Handle_FromAndToDates_FiltersByCreatedAtRange()
    {
        var logs = new List<AIAuditLog>
        {
            new()
            {
                Id = Guid.NewGuid(), EntityType = "Conversation", EventType = AuditEventType.Create,
                Severity = AuditSeverity.Info, Action = "create", ActorId = "u1", ActorType = "User",
                Message = "created", CreatedAt = new DateTime(2024, 6, 15, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.NewGuid(), EntityType = "Conversation", EventType = AuditEventType.Update,
                Severity = AuditSeverity.Warning, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.NewGuid(), EntityType = "Conversation", EventType = AuditEventType.Delete,
                Severity = AuditSeverity.Error, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        };
        _repo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<AIAuditLog, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(logs);

        var from = new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Utc);
        var to = new DateTime(2024, 12, 31, 0, 0, 0, DateTimeKind.Utc);
        var result = await _handler.Handle(
            new AuditLogQuery(null, null, null, null, from, to, 1, 20), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.TotalCount.Should().Be(1);
        result.Value.Items[0].EventType.Should().Be(AuditEventType.Create);
    }
}


