using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Application.Common.Interfaces.AI.Models;
using SportsGurukul.Application.Features.AIManagement.Services;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace AI.Application.Tests.Services;

public class GetByIdSuccessPathCoverageTests
{
    [Fact]
    public async Task AgentGetByIdAsync_ActiveAgent_ReturnsEntity()
    {
        var agentRepo = new Mock<IAgentDefinitionRepository>();
        var agent = new AgentDefinition { Id = Guid.NewGuid(), Name = "Analyst", CreatedAt = DateTime.UtcNow };
        agentRepo.Setup(r => r.GetByIdWithDetailsAsync(agent.Id, It.IsAny<CancellationToken>())).ReturnsAsync(agent);
        var service = new AgentService(agentRepo.Object, new Mock<IWorkflowDefinitionRepository>().Object,
            new Mock<IPublisher>().Object, NullLogger<AgentService>.Instance);

        var result = await service.GetByIdAsync(agent.Id, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(agent.Id);
    }

    [Fact]
    public async Task AssistantGetByIdAsync_ActiveAssistant_ReturnsEntity()
    {
        var assistantRepo = new Mock<IAIAssistantRepository>();
        var assistant = new AIAssistant { Id = Guid.NewGuid(), Name = "Coach", CreatedAt = DateTime.UtcNow };
        assistantRepo.Setup(r => r.GetByIdWithDetailsAsync(assistant.Id, It.IsAny<CancellationToken>())).ReturnsAsync(assistant);
        var service = new AssistantService(assistantRepo.Object, new Mock<IKnowledgeBaseRepository>().Object,
            new Mock<IToolDefinitionRepository>().Object, new Mock<IPublisher>().Object,
            NullLogger<AssistantService>.Instance);

        var result = await service.GetByIdAsync(assistant.Id, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(assistant.Id);
    }

    [Fact]
    public async Task KnowledgeBaseGetByIdAsync_ActiveBase_ReturnsEntity()
    {
        var baseRepo = new Mock<IKnowledgeBaseRepository>();
        var kb = new KnowledgeBase { Id = Guid.NewGuid(), Name = "Rules", CreatedAt = DateTime.UtcNow };
        baseRepo.Setup(r => r.GetByIdWithDetailsAsync(kb.Id, It.IsAny<CancellationToken>())).ReturnsAsync(kb);
        var service = new KnowledgeService(baseRepo.Object, new Mock<IKnowledgeDocumentRepository>().Object,
            new Mock<IKnowledgeSourceRepository>().Object, new Mock<IPublisher>().Object,
            NullLogger<KnowledgeService>.Instance);

        var result = await service.GetBaseByIdAsync(kb.Id, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(kb.Id);
    }

    [Fact]
    public async Task PromptTemplateGetByIdAsync_ActiveTemplate_ReturnsEntity()
    {
        var templateRepo = new Mock<IPromptTemplateRepository>();
        var template = new PromptTemplate { Id = Guid.NewGuid(), Name = "Intro", CreatedAt = DateTime.UtcNow };
        templateRepo.Setup(r => r.GetByIdWithDetailsAsync(template.Id, It.IsAny<CancellationToken>())).ReturnsAsync(template);
        var service = new PromptService(templateRepo.Object, new Mock<IPromptVersionRepository>().Object,
            new Mock<IPublisher>().Object, NullLogger<PromptService>.Instance);

        var result = await service.GetByIdAsync(template.Id, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(template.Id);
    }
}

public class AgentServiceIsDeletedGuardTests
{
    private readonly Mock<IAgentDefinitionRepository> _agentRepo = new();
    private readonly Mock<IWorkflowDefinitionRepository> _workflowRepo = new();
    private readonly Mock<IPublisher> _publisher = new();
    private readonly AgentService _service;

    public AgentServiceIsDeletedGuardTests()
    {
        _service = new AgentService(
            _agentRepo.Object, _workflowRepo.Object, _publisher.Object,
            NullLogger<AgentService>.Instance);
    }

    [Fact]
    public async Task EnableAsync_DeletedAgent_ReturnsFailure()
    {
        var agent = new AgentDefinition { Id = Guid.NewGuid(), IsDeleted = true, CreatedAt = DateTime.UtcNow };
        _agentRepo.Setup(r => r.GetByIdAsync(agent.Id, It.IsAny<CancellationToken>())).ReturnsAsync(agent);

        var result = await _service.EnableAsync(agent.Id, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Agent not found");
        _agentRepo.Verify(r => r.Update(It.IsAny<AgentDefinition>()), Times.Never);
    }

    [Fact]
    public async Task DisableAsync_DeletedAgent_ReturnsFailure()
    {
        var agent = new AgentDefinition { Id = Guid.NewGuid(), IsDeleted = true, CreatedAt = DateTime.UtcNow };
        _agentRepo.Setup(r => r.GetByIdAsync(agent.Id, It.IsAny<CancellationToken>())).ReturnsAsync(agent);

        var result = await _service.DisableAsync(agent.Id, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Agent not found");
        _agentRepo.Verify(r => r.Update(It.IsAny<AgentDefinition>()), Times.Never);
    }

    [Fact]
    public async Task GetByIdAsync_DeletedAgent_ReturnsFailure()
    {
        var agent = new AgentDefinition { Id = Guid.NewGuid(), IsDeleted = true, CreatedAt = DateTime.UtcNow };
        _agentRepo.Setup(r => r.GetByIdWithDetailsAsync(agent.Id, It.IsAny<CancellationToken>())).ReturnsAsync(agent);

        var result = await _service.GetByIdAsync(agent.Id, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Agent not found");
    }
}

public class AssistantServiceIsDeletedGuardTests
{
    private readonly Mock<IAIAssistantRepository> _assistantRepo = new();
    private readonly Mock<IKnowledgeBaseRepository> _kbRepo = new();
    private readonly Mock<IToolDefinitionRepository> _toolRepo = new();
    private readonly Mock<IPublisher> _publisher = new();
    private readonly AssistantService _service;

    public AssistantServiceIsDeletedGuardTests()
    {
        _service = new AssistantService(
            _assistantRepo.Object, _kbRepo.Object, _toolRepo.Object, _publisher.Object,
            NullLogger<AssistantService>.Instance);
    }

    [Fact]
    public async Task PublishAsync_DeletedAssistant_ReturnsFailure()
    {
        var assistant = new AIAssistant { Id = Guid.NewGuid(), IsDeleted = true, CreatedAt = DateTime.UtcNow };
        _assistantRepo.Setup(r => r.GetByIdAsync(assistant.Id, It.IsAny<CancellationToken>())).ReturnsAsync(assistant);

        var result = await _service.PublishAsync(assistant.Id, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Assistant not found");
        _assistantRepo.Verify(r => r.Update(It.IsAny<AIAssistant>()), Times.Never);
    }

    [Fact]
    public async Task ArchiveAsync_DeletedAssistant_ReturnsFailure()
    {
        var assistant = new AIAssistant { Id = Guid.NewGuid(), IsDeleted = true, CreatedAt = DateTime.UtcNow };
        _assistantRepo.Setup(r => r.GetByIdAsync(assistant.Id, It.IsAny<CancellationToken>())).ReturnsAsync(assistant);

        var result = await _service.ArchiveAsync(assistant.Id, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Assistant not found");
        _assistantRepo.Verify(r => r.Update(It.IsAny<AIAssistant>()), Times.Never);
    }

    [Fact]
    public async Task GetByIdAsync_DeletedAssistant_ReturnsFailure()
    {
        var assistant = new AIAssistant { Id = Guid.NewGuid(), IsDeleted = true, CreatedAt = DateTime.UtcNow };
        _assistantRepo.Setup(r => r.GetByIdWithDetailsAsync(assistant.Id, It.IsAny<CancellationToken>())).ReturnsAsync(assistant);

        var result = await _service.GetByIdAsync(assistant.Id, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Assistant not found");
    }
}

public class ConversationServiceIsDeletedGuardTests
{
    private readonly Mock<IConversationRepository> _conversationRepo = new();
    private readonly Mock<IConversationMessageRepository> _messageRepo = new();
    private readonly Mock<IConversationMemoryRepository> _memoryRepo = new();
    private readonly Mock<IPublisher> _publisher = new();
    private readonly ConversationService _service;

    public ConversationServiceIsDeletedGuardTests()
    {
        _service = new ConversationService(
            _conversationRepo.Object, _messageRepo.Object, _memoryRepo.Object,
            _publisher.Object, NullLogger<ConversationService>.Instance);
    }

    [Fact]
    public async Task ArchiveAsync_DeletedConversation_ReturnsFailure()
    {
        var conversation = new Conversation { Id = Guid.NewGuid(), IsDeleted = true, CreatedAt = DateTime.UtcNow };
        _conversationRepo.Setup(r => r.GetByIdAsync(conversation.Id, It.IsAny<CancellationToken>())).ReturnsAsync(conversation);

        var result = await _service.ArchiveAsync(conversation.Id, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Conversation not found");
        _conversationRepo.Verify(r => r.Update(It.IsAny<Conversation>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_DeletedConversation_ReturnsFailure()
    {
        var conversation = new Conversation { Id = Guid.NewGuid(), IsDeleted = true, CreatedAt = DateTime.UtcNow };
        _conversationRepo.Setup(r => r.GetByIdAsync(conversation.Id, It.IsAny<CancellationToken>())).ReturnsAsync(conversation);

        var result = await _service.DeleteAsync(conversation.Id, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Conversation not found");
        _conversationRepo.Verify(r => r.Update(It.IsAny<Conversation>()), Times.Never);
    }

    [Fact]
    public async Task SummarizeAsync_DeletedConversation_ReturnsFailure()
    {
        var conversation = new Conversation { Id = Guid.NewGuid(), IsDeleted = true, CreatedAt = DateTime.UtcNow };
        _conversationRepo.Setup(r => r.GetByIdAsync(conversation.Id, It.IsAny<CancellationToken>())).ReturnsAsync(conversation);

        var result = await _service.SummarizeAsync(conversation.Id, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Conversation not found");
        _conversationRepo.Verify(r => r.Update(It.IsAny<Conversation>()), Times.Never);
    }
}

public class PromptServiceIsDeletedGuardTests
{
    private readonly Mock<IPromptTemplateRepository> _templateRepo = new();
    private readonly Mock<IPromptVersionRepository> _versionRepo = new();
    private readonly Mock<IPublisher> _publisher = new();
    private readonly PromptService _service;

    public PromptServiceIsDeletedGuardTests()
    {
        _service = new PromptService(
            _templateRepo.Object, _versionRepo.Object, _publisher.Object,
            NullLogger<PromptService>.Instance);
    }

    [Fact]
    public async Task PublishAsync_DeletedTemplate_ReturnsFailure()
    {
        var template = new PromptTemplate { Id = Guid.NewGuid(), IsDeleted = true, CreatedAt = DateTime.UtcNow };
        _templateRepo.Setup(r => r.GetByIdWithDetailsAsync(template.Id, It.IsAny<CancellationToken>())).ReturnsAsync(template);

        var result = await _service.PublishAsync(template.Id, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Prompt template not found");
        _templateRepo.Verify(r => r.Update(It.IsAny<PromptTemplate>()), Times.Never);
    }

    [Fact]
    public async Task RollbackAsync_DeletedTemplate_ReturnsFailure()
    {
        var template = new PromptTemplate { Id = Guid.NewGuid(), IsDeleted = true, CreatedAt = DateTime.UtcNow };
        _templateRepo.Setup(r => r.GetByIdAsync(template.Id, It.IsAny<CancellationToken>())).ReturnsAsync(template);

        var result = await _service.RollbackAsync(template.Id, 1, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Prompt template not found");
        _versionRepo.Verify(r => r.AddAsync(It.IsAny<PromptVersion>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetByIdAsync_DeletedTemplate_ReturnsFailure()
    {
        var template = new PromptTemplate { Id = Guid.NewGuid(), IsDeleted = true, CreatedAt = DateTime.UtcNow };
        _templateRepo.Setup(r => r.GetByIdWithDetailsAsync(template.Id, It.IsAny<CancellationToken>())).ReturnsAsync(template);

        var result = await _service.GetByIdAsync(template.Id, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Prompt template not found");
    }
}

public class KnowledgeServiceIsDeletedGuardTests
{
    private readonly Mock<IKnowledgeBaseRepository> _baseRepo = new();
    private readonly Mock<IKnowledgeDocumentRepository> _documentRepo = new();
    private readonly Mock<IKnowledgeSourceRepository> _sourceRepo = new();
    private readonly Mock<IPublisher> _publisher = new();
    private readonly KnowledgeService _service;

    public KnowledgeServiceIsDeletedGuardTests()
    {
        _service = new KnowledgeService(
            _baseRepo.Object, _documentRepo.Object, _sourceRepo.Object, _publisher.Object,
            NullLogger<KnowledgeService>.Instance);
    }

    [Fact]
    public async Task GetBaseByIdAsync_DeletedBase_ReturnsFailure()
    {
        var kb = new KnowledgeBase { Id = Guid.NewGuid(), IsDeleted = true, CreatedAt = DateTime.UtcNow };
        _baseRepo.Setup(r => r.GetByIdWithDetailsAsync(kb.Id, It.IsAny<CancellationToken>())).ReturnsAsync(kb);

        var result = await _service.GetBaseByIdAsync(kb.Id, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Knowledge base not found");
    }

    [Fact]
    public async Task AttachDocumentAsync_DeletedBase_ReturnsFailure()
    {
        var kb = new KnowledgeBase { Id = Guid.NewGuid(), IsDeleted = true, CreatedAt = DateTime.UtcNow };
        _baseRepo.Setup(r => r.GetByIdAsync(kb.Id, It.IsAny<CancellationToken>())).ReturnsAsync(kb);

        var result = await _service.AttachDocumentAsync(kb.Id, Guid.NewGuid(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Knowledge base not found");
        _baseRepo.Verify(r => r.Update(It.IsAny<KnowledgeBase>()), Times.Never);
    }

    [Fact]
    public async Task AttachDocumentAsync_DeletedDocument_ReturnsFailure()
    {
        var kb = new KnowledgeBase { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow };
        var document = new KnowledgeDocument { Id = Guid.NewGuid(), IsDeleted = true, CreatedAt = DateTime.UtcNow };
        _baseRepo.Setup(r => r.GetByIdAsync(kb.Id, It.IsAny<CancellationToken>())).ReturnsAsync(kb);
        _documentRepo.Setup(r => r.GetByIdAsync(document.Id, It.IsAny<CancellationToken>())).ReturnsAsync(document);

        var result = await _service.AttachDocumentAsync(kb.Id, document.Id, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Document not found");
        _baseRepo.Verify(r => r.Update(It.IsAny<KnowledgeBase>()), Times.Never);
    }

    [Fact]
    public async Task DetachDocumentAsync_DeletedBase_ReturnsFailure()
    {
        var kb = new KnowledgeBase { Id = Guid.NewGuid(), IsDeleted = true, CreatedAt = DateTime.UtcNow };
        _baseRepo.Setup(r => r.GetByIdAsync(kb.Id, It.IsAny<CancellationToken>())).ReturnsAsync(kb);

        var result = await _service.DetachDocumentAsync(kb.Id, Guid.NewGuid(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Knowledge base not found");
        _baseRepo.Verify(r => r.Update(It.IsAny<KnowledgeBase>()), Times.Never);
    }

    [Fact]
    public async Task DetachDocumentAsync_DeletedDocument_ReturnsFailure()
    {
        var kb = new KnowledgeBase { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow };
        var document = new KnowledgeDocument { Id = Guid.NewGuid(), IsDeleted = true, CreatedAt = DateTime.UtcNow };
        _baseRepo.Setup(r => r.GetByIdAsync(kb.Id, It.IsAny<CancellationToken>())).ReturnsAsync(kb);
        _documentRepo.Setup(r => r.GetByIdAsync(document.Id, It.IsAny<CancellationToken>())).ReturnsAsync(document);

        var result = await _service.DetachDocumentAsync(kb.Id, document.Id, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Document not found");
        _baseRepo.Verify(r => r.Update(It.IsAny<KnowledgeBase>()), Times.Never);
    }
}
