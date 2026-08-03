using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Application.Features.AIManagement.Events;
using SportsGurukul.Application.Features.AIManagement.Services;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Tests.AI.Services;

public class PromptServiceTests
{
    private readonly Mock<IPromptRepository> _promptRepoMock = new();
    private readonly Mock<IAssistantRepository> _assistantRepoMock = new();
    private readonly Mock<IRepository<PromptVersion>> _versionRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IMediator> _mediatorMock = new();
    private readonly Mock<ILogger<PromptService>> _loggerMock = new();
    private readonly PromptService _service;

    public PromptServiceTests()
    {
        _service = new PromptService(
            _promptRepoMock.Object,
            _assistantRepoMock.Object,
            _versionRepoMock.Object,
            _unitOfWorkMock.Object,
            _mediatorMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task CreateAsync_MissingAssistant_ReturnsFailure()
    {
        var assistantId = Guid.NewGuid();
        _assistantRepoMock.Setup(r => r.GetByIdAsync(assistantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((AIAssistant?)null);
        var request = new CreatePromptTemplateRequest(
            assistantId, "Drill", null, AIPromptType.Template, "Explain {topic}", null, null, null, false);

        var result = await _service.CreateAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Assistant not found");
    }

    [Fact]
    public async Task CreateAsync_ValidRequest_AddsTemplateAndInitialVersion()
    {
        var assistant = new AIAssistant { Id = Guid.NewGuid(), Name = "Coach", IsActive = true };
        _assistantRepoMock.Setup(r => r.GetByIdAsync(assistant.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(assistant);
        _promptRepoMock.Setup(r => r.GetActiveByAssistantAsync(assistant.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PromptTemplate>());
        var request = new CreatePromptTemplateRequest(
            assistant.Id, "Drill", null, AIPromptType.Template, "Explain {topic}", null, null, null, true);

        var result = await _service.CreateAsync(request);

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Drill");
        result.Value.CurrentVersion.Should().Be(1);
        _versionRepoMock.Verify(r => r.AddAsync(It.Is<PromptVersion>(v => v.VersionNumber == 1), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_DefaultPrompt_UnsetsExistingDefaults()
    {
        var assistant = new AIAssistant { Id = Guid.NewGuid(), Name = "Coach", IsActive = true };
        var existingDefault = new PromptTemplate
        {
            Id = Guid.NewGuid(),
            AssistantId = assistant.Id,
            Name = "Old",
            IsDefault = true,
            IsActive = true,
            TemplateText = "old",
        };
        _assistantRepoMock.Setup(r => r.GetByIdAsync(assistant.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(assistant);
        _promptRepoMock.Setup(r => r.GetActiveByAssistantAsync(assistant.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PromptTemplate> { existingDefault });
        var request = new CreatePromptTemplateRequest(
            assistant.Id, "New", null, AIPromptType.Template, "new template", null, null, null, true);

        await _service.CreateAsync(request);

        existingDefault.IsDefault.Should().BeFalse();
        _promptRepoMock.Verify(r => r.Update(existingDefault), Times.Once);
    }

    [Fact]
    public async Task PublishAsync_MissingTemplate_ReturnsFailure()
    {
        var promptId = Guid.NewGuid();
        _promptRepoMock.Setup(r => r.GetByIdWithVersionsAsync(promptId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PromptTemplate?)null);

        var result = await _service.PublishAsync(new PublishPromptTemplateRequest(promptId, null, null));

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task PublishAsync_ValidTemplate_IncrementsVersionAndPublishesEvent()
    {
        var template = new PromptTemplate
        {
            Id = Guid.NewGuid(),
            AssistantId = Guid.NewGuid(),
            Name = "Drill",
            TemplateText = "Explain {topic}",
            CurrentVersion = 1,
        };
        _promptRepoMock.Setup(r => r.GetByIdWithVersionsAsync(template.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(template);

        var result = await _service.PublishAsync(new PublishPromptTemplateRequest(template.Id, "Tone tweak", null));

        result.IsSuccess.Should().BeTrue();
        result.Value.CurrentVersion.Should().Be(2);
        _versionRepoMock.Verify(r => r.AddAsync(It.Is<PromptVersion>(v => v.VersionNumber == 2), It.IsAny<CancellationToken>()), Times.Once);
        _mediatorMock.Verify(m => m.Publish(
            It.Is<PromptPublishedEvent>(e => e.VersionNumber == 2),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RollbackAsync_ExistingVersion_RestoresContent()
    {
        var v1 = new PromptVersion { Id = Guid.NewGuid(), VersionNumber = 1, Content = "old content" };
        var v2 = new PromptVersion { Id = Guid.NewGuid(), VersionNumber = 2, Content = "new content" };
        var template = new PromptTemplate
        {
            Id = Guid.NewGuid(),
            AssistantId = Guid.NewGuid(),
            Name = "Drill",
            TemplateText = "new content",
            CurrentVersion = 2,
        };
        template.Versions.Add(v1);
        template.Versions.Add(v2);
        _promptRepoMock.Setup(r => r.GetByIdWithVersionsAsync(template.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(template);

        var result = await _service.RollbackAsync(new RollbackPromptVersionRequest(template.Id, 1));

        result.IsSuccess.Should().BeTrue();
        result.Value.CurrentVersion.Should().Be(1);
        result.Value.TemplateText.Should().Be("old content");
        v1.IsActive.Should().BeTrue();
        v2.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task RollbackAsync_MissingVersion_ReturnsFailure()
    {
        var template = new PromptTemplate
        {
            Id = Guid.NewGuid(),
            AssistantId = Guid.NewGuid(),
            Name = "Drill",
            TemplateText = "content",
            CurrentVersion = 1,
        };
        _promptRepoMock.Setup(r => r.GetByIdWithVersionsAsync(template.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(template);

        var result = await _service.RollbackAsync(new RollbackPromptVersionRequest(template.Id, 42));

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("42");
    }
}
