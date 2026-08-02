using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Application.Common.Interfaces.AI.Models;
using SportsGurukul.Application.Features.AIManagement.DomainEvents;
using SportsGurukul.Application.Features.AIManagement.Services;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace AI.Application.Tests.Services;

public class AssistantServiceTests
{
    private readonly Mock<IAIAssistantRepository> _assistantRepo = new();
    private readonly Mock<IKnowledgeBaseRepository> _kbRepo = new();
    private readonly Mock<IToolDefinitionRepository> _toolRepo = new();
    private readonly Mock<IPublisher> _publisher = new();
    private readonly AssistantService _service;

    public AssistantServiceTests()
    {
        _service = new AssistantService(
            _assistantRepo.Object, _kbRepo.Object, _toolRepo.Object, _publisher.Object,
            NullLogger<AssistantService>.Instance);
    }

    [Fact]
    public async Task CreateAsync_CreatesActiveAssistant()
    {
        var result = await _service.CreateAsync(
            new CreateAssistantRequest("Coach", "desc", AIAssistantType.Coach,
                AIAssistantPersonality.Enthusiastic, "prompt", "hello", true),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        var assistant = result.Value!;
        assistant.Name.Should().Be("Coach");
        assistant.AssistantType.Should().Be(AIAssistantType.Coach);
        assistant.Personality.Should().Be(AIAssistantPersonality.Enthusiastic);
        assistant.SystemPrompt.Should().Be("prompt");
        assistant.IsPublic.Should().BeTrue();
        assistant.IsActive.Should().BeTrue();

        _assistantRepo.Verify(r => r.AddAsync(It.IsAny<AIAssistant>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesNonNullFields()
    {
        var assistant = new AIAssistant { Id = Guid.NewGuid(), Name = "Old", IsActive = true };
        _assistantRepo.Setup(r => r.GetByIdAsync(assistant.Id, It.IsAny<CancellationToken>())).ReturnsAsync(assistant);

        var result = await _service.UpdateAsync(
            new UpdateAssistantRequest(assistant.Id, "New", null, null,
                AIAssistantPersonality.Motivational, null, "hi", false),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        assistant.Name.Should().Be("New");
        assistant.Personality.Should().Be(AIAssistantPersonality.Motivational);
        assistant.GreetingMessage.Should().Be("hi");
        assistant.IsPublic.Should().BeFalse();
        assistant.Description.Should().BeNull();
        _assistantRepo.Verify(r => r.Update(assistant), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_NotFound_ReturnsFailure()
    {
        _assistantRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AIAssistant?)null);

        var result = await _service.UpdateAsync(
            new UpdateAssistantRequest(Guid.NewGuid(), "x", null, null, null, null, null, null),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Assistant not found");
    }

    [Fact]
    public async Task PublishAsync_SetsActive()
    {
        var assistant = new AIAssistant { Id = Guid.NewGuid(), IsActive = false };
        _assistantRepo.Setup(r => r.GetByIdAsync(assistant.Id, It.IsAny<CancellationToken>())).ReturnsAsync(assistant);

        var result = await _service.PublishAsync(assistant.Id, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        assistant.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task ArchiveAsync_SetsInactive()
    {
        var assistant = new AIAssistant { Id = Guid.NewGuid(), IsActive = true };
        _assistantRepo.Setup(r => r.GetByIdAsync(assistant.Id, It.IsAny<CancellationToken>())).ReturnsAsync(assistant);

        var result = await _service.ArchiveAsync(assistant.Id, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        assistant.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task AssignKnowledgeBaseAsync_Success()
    {
        var assistant = new AIAssistant { Id = Guid.NewGuid() };
        var kb = new KnowledgeBase { Id = Guid.NewGuid() };
        _assistantRepo.Setup(r => r.GetByIdAsync(assistant.Id, It.IsAny<CancellationToken>())).ReturnsAsync(assistant);
        _kbRepo.Setup(r => r.GetByIdAsync(kb.Id, It.IsAny<CancellationToken>())).ReturnsAsync(kb);

        var result = await _service.AssignKnowledgeBaseAsync(assistant.Id, kb.Id, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task AssignKnowledgeBaseAsync_AssistantNotFound_ReturnsFailure()
    {
        _assistantRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AIAssistant?)null);

        var result = await _service.AssignKnowledgeBaseAsync(Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Assistant not found");
    }

    [Fact]
    public async Task AssignKnowledgeBaseAsync_KnowledgeBaseNotFound_ReturnsFailure()
    {
        var assistant = new AIAssistant { Id = Guid.NewGuid() };
        _assistantRepo.Setup(r => r.GetByIdAsync(assistant.Id, It.IsAny<CancellationToken>())).ReturnsAsync(assistant);
        _kbRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((KnowledgeBase?)null);

        var result = await _service.AssignKnowledgeBaseAsync(assistant.Id, Guid.NewGuid(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Knowledge base not found");
    }

    [Fact]
    public async Task AssignToolsAsync_Success()
    {
        var assistant = new AIAssistant { Id = Guid.NewGuid() };
        _assistantRepo.Setup(r => r.GetByIdAsync(assistant.Id, It.IsAny<CancellationToken>())).ReturnsAsync(assistant);

        var result = await _service.AssignToolsAsync(assistant.Id, [Guid.NewGuid(), Guid.NewGuid()], CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _assistantRepo.Verify(r => r.Update(assistant), Times.Once);
    }

    [Fact]
    public async Task AssignToolsAsync_NotFound_ReturnsFailure()
    {
        _assistantRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AIAssistant?)null);

        var result = await _service.AssignToolsAsync(Guid.NewGuid(), [], CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task GetByIdAsync_NotFound_ReturnsFailure()
    {
        _assistantRepo.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AIAssistant?)null);

        var result = await _service.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task SearchAsync_ReturnsResults()
    {
        _assistantRepo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<AIAssistant, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AIAssistant> { new() { Id = Guid.NewGuid() } });

        var result = await _service.SearchAsync(new SearchAssistantsRequest(null, null, null, null), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Should().HaveCount(1);
    }
}

public class KnowledgeServiceTests
{
    private readonly Mock<IKnowledgeBaseRepository> _baseRepo = new();
    private readonly Mock<IKnowledgeDocumentRepository> _documentRepo = new();
    private readonly Mock<IKnowledgeSourceRepository> _sourceRepo = new();
    private readonly Mock<IPublisher> _publisher = new();
    private readonly KnowledgeService _service;

    public KnowledgeServiceTests()
    {
        _service = new KnowledgeService(
            _baseRepo.Object, _documentRepo.Object, _sourceRepo.Object, _publisher.Object,
            NullLogger<KnowledgeService>.Instance);
    }

    [Fact]
    public async Task CreateBaseAsync_CreatesDraftKnowledgeBase()
    {
        var result = await _service.CreateBaseAsync(
            new CreateKnowledgeBaseRequest("KB", "desc", KnowledgeBaseVisibility.Public, "cat", "tags"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        var kb = result.Value!;
        kb.Name.Should().Be("KB");
        kb.Visibility.Should().Be(KnowledgeBaseVisibility.Public);
        kb.Status.Should().Be(KnowledgeBaseStatus.Draft);
        _baseRepo.Verify(r => r.AddAsync(It.IsAny<KnowledgeBase>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateBaseAsync_UpdatesAndPublishesEvent()
    {
        _publisher.Setup(p => p.Publish(It.IsAny<KnowledgeBaseUpdatedEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var kb = new KnowledgeBase { Id = Guid.NewGuid(), Name = "Old", Visibility = KnowledgeBaseVisibility.Private };
        _baseRepo.Setup(r => r.GetByIdAsync(kb.Id, It.IsAny<CancellationToken>())).ReturnsAsync(kb);

        var result = await _service.UpdateBaseAsync(
            new UpdateKnowledgeBaseRequest(kb.Id, "New", null, KnowledgeBaseVisibility.Public, "cat", "tags"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        kb.Name.Should().Be("New");
        kb.Visibility.Should().Be(KnowledgeBaseVisibility.Public);
        _publisher.Verify(p => p.Publish(It.IsAny<KnowledgeBaseUpdatedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateBaseAsync_NotFound_ReturnsFailure()
    {
        _baseRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((KnowledgeBase?)null);

        var result = await _service.UpdateBaseAsync(
            new UpdateKnowledgeBaseRequest(Guid.NewGuid(), "x", null, null, null, null),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Knowledge base not found");
    }

    [Fact]
    public async Task AttachDocumentAsync_IncrementsDocumentCount()
    {
        var kb = new KnowledgeBase { Id = Guid.NewGuid(), TotalDocuments = 0 };
        var doc = new KnowledgeDocument { Id = Guid.NewGuid() };
        _baseRepo.Setup(r => r.GetByIdAsync(kb.Id, It.IsAny<CancellationToken>())).ReturnsAsync(kb);
        _documentRepo.Setup(r => r.GetByIdAsync(doc.Id, It.IsAny<CancellationToken>())).ReturnsAsync(doc);

        var result = await _service.AttachDocumentAsync(kb.Id, doc.Id, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        kb.TotalDocuments.Should().Be(1);
    }

    [Fact]
    public async Task AttachDocumentAsync_DocumentNotFound_ReturnsFailure()
    {
        var kb = new KnowledgeBase { Id = Guid.NewGuid() };
        _baseRepo.Setup(r => r.GetByIdAsync(kb.Id, It.IsAny<CancellationToken>())).ReturnsAsync(kb);
        _documentRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((KnowledgeDocument?)null);

        var result = await _service.AttachDocumentAsync(kb.Id, Guid.NewGuid(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Document not found");
    }

    [Fact]
    public async Task DetachDocumentAsync_DecrementsDocumentCount()
    {
        var kb = new KnowledgeBase { Id = Guid.NewGuid(), TotalDocuments = 2 };
        var doc = new KnowledgeDocument { Id = Guid.NewGuid() };
        _baseRepo.Setup(r => r.GetByIdAsync(kb.Id, It.IsAny<CancellationToken>())).ReturnsAsync(kb);
        _documentRepo.Setup(r => r.GetByIdAsync(doc.Id, It.IsAny<CancellationToken>())).ReturnsAsync(doc);

        var result = await _service.DetachDocumentAsync(kb.Id, doc.Id, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        kb.TotalDocuments.Should().Be(1);
    }

    [Fact]
    public async Task DetachDocumentAsync_NeverGoesBelowZero()
    {
        var kb = new KnowledgeBase { Id = Guid.NewGuid(), TotalDocuments = 0 };
        var doc = new KnowledgeDocument { Id = Guid.NewGuid() };
        _baseRepo.Setup(r => r.GetByIdAsync(kb.Id, It.IsAny<CancellationToken>())).ReturnsAsync(kb);
        _documentRepo.Setup(r => r.GetByIdAsync(doc.Id, It.IsAny<CancellationToken>())).ReturnsAsync(doc);

        var result = await _service.DetachDocumentAsync(kb.Id, doc.Id, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        kb.TotalDocuments.Should().Be(0);
    }

    [Fact]
    public async Task RebuildIndexAsync_Success()
    {
        var kb = new KnowledgeBase { Id = Guid.NewGuid() };
        _baseRepo.Setup(r => r.GetByIdAsync(kb.Id, It.IsAny<CancellationToken>())).ReturnsAsync(kb);

        var result = await _service.RebuildIndexAsync(kb.Id, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _baseRepo.Verify(r => r.Update(kb), Times.Once);
    }

    [Fact]
    public async Task RebuildIndexAsync_NotFound_ReturnsFailure()
    {
        _baseRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((KnowledgeBase?)null);

        var result = await _service.RebuildIndexAsync(Guid.NewGuid(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Knowledge base not found");
    }

    [Fact]
    public async Task GetBaseByIdAsync_NotFound_ReturnsFailure()
    {
        _baseRepo.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((KnowledgeBase?)null);

        var result = await _service.GetBaseByIdAsync(Guid.NewGuid(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task SearchBasesAsync_ReturnsResults()
    {
        _baseRepo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<KnowledgeBase, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<KnowledgeBase> { new() { Id = Guid.NewGuid() } });

        var result = await _service.SearchBasesAsync(new SearchKnowledgeBasesRequest(null, null, null, null), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Should().HaveCount(1);
    }
}
