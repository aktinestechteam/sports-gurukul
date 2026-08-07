using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Application.Features.AIManagement.Services;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Tests.AI.Services;

public class AssistantServiceTests
{
    private readonly Mock<IAssistantRepository> _assistantRepoMock = new();
    private readonly Mock<IRepository<AIModel>> _modelRepoMock = new();
    private readonly Mock<IKnowledgeBaseRepository> _knowledgeBaseRepoMock = new();
    private readonly Mock<IRepository<ToolDefinition>> _toolRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<ILogger<AssistantService>> _loggerMock = new();
    private readonly AssistantService _service;

    public AssistantServiceTests()
    {
        _service = new AssistantService(
            _assistantRepoMock.Object,
            _modelRepoMock.Object,
            _knowledgeBaseRepoMock.Object,
            _toolRepoMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    private static AIAssistant BuildAssistant() => new()
    {
        Id = Guid.NewGuid(),
        Name = "Coach",
        DisplayName = "Coach",
        AssistantType = AIAssistantType.Coach,
        IsActive = true,
        OwnerType = AIResourceOwnerType.Athlete,
        OwnerUserId = Guid.NewGuid(),
    };

    [Fact]
    public async Task CreateAsync_InvalidModel_ReturnsFailure()
    {
        var modelId = Guid.NewGuid();
        _modelRepoMock.Setup(r => r.GetByIdAsync(modelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((AIModel?)null);
        var request = new CreateAssistantRequest(
            "Coach", "Coach", null, AIAssistantType.Coach, null, modelId, null, null,
            null, true, false, AIResourceOwnerType.Athlete, null, null, null, null);

        var result = await _service.CreateAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("model");
    }

    [Fact]
    public async Task CreateAsync_ValidRequest_AddsAssistant()
    {
        var request = new CreateAssistantRequest(
            "Coach", "Coach", null, AIAssistantType.Coach, null, null, null, null,
            null, true, false, AIResourceOwnerType.Athlete, Guid.NewGuid(), null, null, null);

        var result = await _service.CreateAsync(request);

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Coach");
        _assistantRepoMock.Verify(r => r.AddAsync(It.IsAny<AIAssistant>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task PublishAsync_MissingAssistant_ReturnsFailure()
    {
        var assistantId = Guid.NewGuid();
        _assistantRepoMock.Setup(r => r.GetByIdAsync(assistantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((AIAssistant?)null);

        var result = await _service.PublishAsync(assistantId);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Assistant not found");
    }

    [Fact]
    public async Task PublishAsync_ValidAssistant_ActivatesAndSetsFlag()
    {
        var assistant = BuildAssistant();
        _assistantRepoMock.Setup(r => r.GetByIdAsync(assistant.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(assistant);

        var result = await _service.PublishAsync(assistant.Id);

        result.IsSuccess.Should().BeTrue();
        result.Value.IsActive.Should().BeTrue();
        assistant.IsActive.Should().BeTrue();
        assistant.MetadataJson.Should().Contain("\"published\":true");
        _assistantRepoMock.Verify(r => r.Update(assistant), Times.Once);
    }

    [Fact]
    public async Task AssignKnowledgeBaseAsync_MissingKnowledgeBase_ReturnsFailure()
    {
        var assistant = BuildAssistant();
        _assistantRepoMock.Setup(r => r.GetByIdAsync(assistant.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(assistant);
        _knowledgeBaseRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<KnowledgeBase, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<KnowledgeBase>());
        var request = new AssignKnowledgeBaseRequest(assistant.Id, new List<Guid> { Guid.NewGuid() }, false);

        var result = await _service.AssignKnowledgeBaseAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("knowledge base");
    }

    [Fact]
    public async Task AssignKnowledgeBaseAsync_ValidRequest_WritesMetadata()
    {
        var assistant = BuildAssistant();
        var knowledgeBaseId = Guid.NewGuid();
        _assistantRepoMock.Setup(r => r.GetByIdAsync(assistant.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(assistant);
        _knowledgeBaseRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<KnowledgeBase, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<KnowledgeBase> { new KnowledgeBase { Id = knowledgeBaseId, Name = "Drills" } });
        var request = new AssignKnowledgeBaseRequest(assistant.Id, new List<Guid> { knowledgeBaseId }, false);

        var result = await _service.AssignKnowledgeBaseAsync(request);

        result.IsSuccess.Should().BeTrue();
        result.Value.AssignedKnowledgeBaseIds.Should().Contain(knowledgeBaseId);
        assistant.MetadataJson.Should().Contain(knowledgeBaseId.ToString());
    }

    [Fact]
    public async Task AssignToolsAsync_ValidRequest_AppendsToExistingTools()
    {
        var assistant = BuildAssistant();
        var existingToolId = Guid.NewGuid();
        var newToolId = Guid.NewGuid();
        AssistantAssignmentStoreTest.SetToolIds(assistant, new[] { existingToolId });
        _assistantRepoMock.Setup(r => r.GetByIdAsync(assistant.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(assistant);
        _toolRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<ToolDefinition, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ToolDefinition> { new ToolDefinition { Id = newToolId, Name = "Weather" } });
        var request = new AssignToolsRequest(assistant.Id, new List<Guid> { newToolId }, false);

        var result = await _service.AssignToolsAsync(request);

        result.IsSuccess.Should().BeTrue();
        result.Value.AssignedToolIds.Should().Contain(existingToolId);
        result.Value.AssignedToolIds.Should().Contain(newToolId);
    }

    [Fact]
    public async Task AssignToolsAsync_ClearExisting_ReplacesTools()
    {
        var assistant = BuildAssistant();
        var existingToolId = Guid.NewGuid();
        var newToolId = Guid.NewGuid();
        AssistantAssignmentStoreTest.SetToolIds(assistant, new[] { existingToolId });
        _assistantRepoMock.Setup(r => r.GetByIdAsync(assistant.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(assistant);
        _toolRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<ToolDefinition, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ToolDefinition> { new ToolDefinition { Id = newToolId, Name = "Weather" } });
        var request = new AssignToolsRequest(assistant.Id, new List<Guid> { newToolId }, true);

        var result = await _service.AssignToolsAsync(request);

        result.IsSuccess.Should().BeTrue();
        result.Value.AssignedToolIds.Should().ContainSingle();
        result.Value.AssignedToolIds.Should().Contain(newToolId);
    }
}

internal static class AssistantAssignmentStoreTest
{
    public static void SetToolIds(AIAssistant assistant, IEnumerable<Guid> ids)
        => SportsGurukul.Application.Features.AIManagement.AssistantAssignmentStore.SetToolIds(assistant, ids);
}
