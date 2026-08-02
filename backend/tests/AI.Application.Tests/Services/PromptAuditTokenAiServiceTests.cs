using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Application.Common.Interfaces.AI.Models;
using SportsGurukul.Application.Features.AIManagement.DomainEvents;
using SportsGurukul.Application.Features.AIManagement.Services;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace AI.Application.Tests.Services;

public class PromptServiceTests
{
    private readonly Mock<IPromptTemplateRepository> _templateRepo = new();
    private readonly Mock<IPromptVersionRepository> _versionRepo = new();
    private readonly Mock<IPublisher> _publisher = new();
    private readonly PromptService _service;

    public PromptServiceTests()
    {
        _service = new PromptService(
            _templateRepo.Object, _versionRepo.Object, _publisher.Object,
            NullLogger<PromptService>.Instance);
    }

    [Fact]
    public async Task CreateAsync_CreatesTemplateAndInitialVersion()
    {
        var result = await _service.CreateAsync(
            new CreatePromptTemplateRequest("Intro", "desc", PromptType.System, "content", "vars", "tags", "cat"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        var template = result.Value!;
        template.Name.Should().Be("Intro");
        template.CurrentVersion.Should().Be(1);
        template.Status.Should().Be(PromptStatus.Draft);

        _templateRepo.Verify(r => r.AddAsync(It.IsAny<PromptTemplate>(), It.IsAny<CancellationToken>()), Times.Once);
        _versionRepo.Verify(r => r.AddAsync(It.Is<PromptVersion>(v =>
            v.VersionNumber == 1 && v.PromptTemplateId == template.Id), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesFieldsAndIncrementsVersion()
    {
        var template = new PromptTemplate
        {
            Id = Guid.NewGuid(), Name = "Old", TemplateContent = "old", CurrentVersion = 2,
            Status = PromptStatus.Active
        };
        _templateRepo.Setup(r => r.GetByIdAsync(template.Id, It.IsAny<CancellationToken>())).ReturnsAsync(template);

        var result = await _service.UpdateAsync(
            new UpdatePromptTemplateRequest(template.Id, "New", null, "new content", null, null, null),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        template.Name.Should().Be("New");
        template.TemplateContent.Should().Be("new content");
        template.CurrentVersion.Should().Be(3);

        _versionRepo.Verify(r => r.AddAsync(It.Is<PromptVersion>(v =>
            v.VersionNumber == 3 && v.Content == "new content"), It.IsAny<CancellationToken>()), Times.Once);
        _templateRepo.Verify(r => r.Update(template), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_NotFound_ReturnsFailure()
    {
        _templateRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PromptTemplate?)null);

        var result = await _service.UpdateAsync(
            new UpdatePromptTemplateRequest(Guid.NewGuid(), "x", null, null, null, null, null),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Prompt template not found");
    }

    [Fact]
    public async Task PublishAsync_SetsActiveAndPublishesEvent()
    {
        _publisher.Setup(p => p.Publish(It.IsAny<PromptPublishedEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var template = new PromptTemplate { Id = Guid.NewGuid(), Name = "A", CurrentVersion = 2, Status = PromptStatus.Draft };
        _templateRepo.Setup(r => r.GetByIdWithDetailsAsync(template.Id, It.IsAny<CancellationToken>())).ReturnsAsync(template);

        var result = await _service.PublishAsync(template.Id, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        template.Status.Should().Be(PromptStatus.Active);
        _publisher.Verify(p => p.Publish(It.Is<PromptPublishedEvent>(e =>
            e.PromptTemplateId == template.Id && e.VersionNumber == 2), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RollbackAsync_Success_CreatesNewVersionFromOld()
    {
        var template = new PromptTemplate { Id = Guid.NewGuid(), TemplateContent = "current", CurrentVersion = 3 };
        var oldVersion = new PromptVersion { Id = Guid.NewGuid(), PromptTemplateId = template.Id, VersionNumber = 1, Content = "old content" };
        _templateRepo.Setup(r => r.GetByIdAsync(template.Id, It.IsAny<CancellationToken>())).ReturnsAsync(template);
        _versionRepo.Setup(r => r.GetByTemplateIdAsync(template.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync([oldVersion]);

        var result = await _service.RollbackAsync(template.Id, 1, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        template.TemplateContent.Should().Be("old content");
        template.CurrentVersion.Should().Be(4);
        _versionRepo.Verify(r => r.AddAsync(It.Is<PromptVersion>(v =>
            v.VersionNumber == 4 && v.Content == "old content" &&
            v.ChangeNotes == "Rollback to version 1"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RollbackAsync_VersionNotFound_ReturnsFailure()
    {
        var template = new PromptTemplate { Id = Guid.NewGuid(), CurrentVersion = 2 };
        _templateRepo.Setup(r => r.GetByIdAsync(template.Id, It.IsAny<CancellationToken>())).ReturnsAsync(template);
        _versionRepo.Setup(r => r.GetByTemplateIdAsync(template.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PromptVersion>());

        var result = await _service.RollbackAsync(template.Id, 5, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Version not found");
    }

    [Fact]
    public async Task CloneAsync_CreatesDraftCopyWithInitialVersion()
    {
        var source = new PromptTemplate
        {
            Id = Guid.NewGuid(), Name = "Source", Description = "desc", Type = PromptType.System,
            TemplateContent = "content", Variables = "vars", Tags = "tags", Category = "cat",
            CurrentVersion = 4, Status = PromptStatus.Active
        };
        _templateRepo.Setup(r => r.GetByIdWithDetailsAsync(source.Id, It.IsAny<CancellationToken>())).ReturnsAsync(source);

        var result = await _service.CloneAsync(source.Id, "Copy", CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        var clone = result.Value!;
        clone.Id.Should().NotBe(source.Id);
        clone.Name.Should().Be("Copy");
        clone.TemplateContent.Should().Be("content");
        clone.CurrentVersion.Should().Be(1);
        clone.Status.Should().Be(PromptStatus.Draft);

        _versionRepo.Verify(r => r.AddAsync(It.Is<PromptVersion>(v =>
            v.VersionNumber == 1 && v.PromptTemplateId == clone.Id &&
            v.ChangeNotes == $"Cloned from template {source.Id}"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CloneAsync_NotFound_ReturnsFailure()
    {
        _templateRepo.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PromptTemplate?)null);

        var result = await _service.CloneAsync(Guid.NewGuid(), "x", CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Prompt template not found");
    }

    [Fact]
    public async Task GetByIdAsync_NotFound_ReturnsFailure()
    {
        _templateRepo.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PromptTemplate?)null);

        var result = await _service.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task GetVersionsAsync_ReturnsVersions()
    {
        var versions = new List<PromptVersion> { new() { Id = Guid.NewGuid() } };
        _versionRepo.Setup(r => r.GetByTemplateIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(versions);

        var result = await _service.GetVersionsAsync(Guid.NewGuid(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Should().HaveCount(1);
    }

    [Fact]
    public async Task SearchAsync_ReturnsResults()
    {
        _templateRepo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<PromptTemplate, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PromptTemplate> { new() { Id = Guid.NewGuid() } });

        var result = await _service.SearchAsync(new SearchPromptsRequest(null, null, null, null), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Should().HaveCount(1);
    }
}

public class AuditServiceTests
{
    private readonly Mock<IAIAuditLogRepository> _auditRepo = new();
    private readonly AuditService _service;

    public AuditServiceTests()
    {
        _service = new AuditService(_auditRepo.Object, NullLogger<AuditService>.Instance);
    }

    [Fact]
    public async Task RecordAsync_CreatesAuditLog()
    {
        var entityId = Guid.NewGuid();
        var result = await _service.RecordAsync(
            new RecordAuditRequest(entityId, "Conversation", AuditEventType.Create, AuditSeverity.Info,
                "create", "u1", "User", "127.0.0.1", null, null, null, "created", "{}"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        var log = result.Value!;
        log.EntityId.Should().Be(entityId);
        log.EntityType.Should().Be("Conversation");
        log.EventType.Should().Be(AuditEventType.Create);
        log.Severity.Should().Be(AuditSeverity.Info);
        log.ActorId.Should().Be("u1");
        _auditRepo.Verify(r => r.AddAsync(It.IsAny<AIAuditLog>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByEntityAsync_DelegatesToRepository()
    {
        _auditRepo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<AIAuditLog, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AIAuditLog> { new() { Id = Guid.NewGuid() } });

        var result = await _service.GetByEntityAsync(Guid.NewGuid(), "Conversation", CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetByEventTypeAsync_DelegatesToRepository()
    {
        _auditRepo.Setup(r => r.GetByEventTypeAsync("Create", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AIAuditLog> { new() { Id = Guid.NewGuid() } });

        var result = await _service.GetByEventTypeAsync(AuditEventType.Create, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Should().HaveCount(1);
        _auditRepo.Verify(r => r.GetByEventTypeAsync("Create", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetRecentBySeverityAsync_DelegatesToRepository()
    {
        _auditRepo.Setup(r => r.GetRecentBySeverityAsync("Critical", 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AIAuditLog> { new() { Id = Guid.NewGuid() } });

        var result = await _service.GetRecentBySeverityAsync(AuditSeverity.Critical, 10, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _auditRepo.Verify(r => r.GetRecentBySeverityAsync("Critical", 10, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_DelegatesToRepository()
    {
        _auditRepo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<AIAuditLog, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AIAuditLog>());

        var result = await _service.SearchAsync(new SearchAuditRequest(null, null, null, null, null, null), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Should().BeEmpty();
    }
}

public class TokenUsageServiceTests
{
    private readonly Mock<IAITokenUsageRepository> _tokenRepo = new();
    private readonly Mock<IPublisher> _publisher = new();
    private readonly TokenUsageService _service;

    public TokenUsageServiceTests()
    {
        _service = new TokenUsageService(_tokenRepo.Object, _publisher.Object, NullLogger<TokenUsageService>.Instance);
    }

    [Fact]
    public async Task RecordUsageAsync_CreatesUsageAndPublishesEvent()
    {
        _publisher.Setup(p => p.Publish(It.IsAny<TokenUsageRecordedEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _service.RecordUsageAsync(
            new RecordTokenUsageRequest(Guid.NewGuid(), Guid.NewGuid(), "gpt-4o", "OpenAI", 10, 20, 30, 0.05m,
                "u1", "s1", "chat"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        var usage = result.Value!;
        usage.ModelName.Should().Be("gpt-4o");
        usage.PromptTokens.Should().Be(10);
        usage.CompletionTokens.Should().Be(20);
        usage.TotalTokens.Should().Be(30);
        usage.Cost.Should().Be(0.05m);

        _tokenRepo.Verify(r => r.AddAsync(It.IsAny<AITokenUsage>(), It.IsAny<CancellationToken>()), Times.Once);
        _publisher.Verify(p => p.Publish(It.Is<TokenUsageRecordedEvent>(e =>
            e.ModelName == "gpt-4o" && e.TotalTokens == 30), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByConversationAsync_ReturnsResults()
    {
        _tokenRepo.Setup(r => r.GetByConversationIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AITokenUsage> { new() { Id = Guid.NewGuid() } });

        var result = await _service.GetByConversationAsync(Guid.NewGuid(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetByUserAsync_DelegatesToRepository()
    {
        _tokenRepo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<AITokenUsage, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AITokenUsage> { new() { Id = Guid.NewGuid() } });

        var result = await _service.GetByUserAsync("u1", CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetByDateRangeAsync_DelegatesToRepository()
    {
        var from = DateTime.UtcNow.AddDays(-1);
        var to = DateTime.UtcNow;
        _tokenRepo.Setup(r => r.GetByDateRangeAsync(from, to, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AITokenUsage> { new() { Id = Guid.NewGuid() } });

        var result = await _service.GetByDateRangeAsync(from, to, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _tokenRepo.Verify(r => r.GetByDateRangeAsync(from, to, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_DelegatesToRepository()
    {
        _tokenRepo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<AITokenUsage, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AITokenUsage>());

        var result = await _service.SearchAsync(new SearchTokenUsageRequest(null, null, null, null), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Should().BeEmpty();
    }
}

public class AIServiceTests
{
    private readonly AIService _service = new();

    [Fact]
    public async Task SendMessageAsync_ReturnsNotImplementedFailure()
    {
        var result = await _service.SendMessageAsync(
            new SendMessageRequest(Guid.NewGuid(), "hi", null, null), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("AI message sending not yet implemented");
    }

    [Fact]
    public async Task GetCompletionAsync_ReturnsNotImplementedFailure()
    {
        var result = await _service.GetCompletionAsync(
            new GetCompletionRequest("prompt", null, null, null), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("AI completion not yet implemented");
    }

    [Fact]
    public async Task GetStreamingCompletionAsync_YieldsNothing()
    {
        var chunks = new List<string>();
        await foreach (var chunk in _service.GetStreamingCompletionAsync(
            new GetCompletionRequest("prompt", null, null, null), CancellationToken.None))
        {
            chunks.Add(chunk);
        }

        chunks.Should().BeEmpty();
    }
}
