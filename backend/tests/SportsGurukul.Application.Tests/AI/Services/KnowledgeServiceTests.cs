using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Application.Features.AIManagement.Events;
using SportsGurukul.Application.Features.AIManagement.Services;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Tests.AI.Services;

public class KnowledgeServiceTests
{
    private readonly Mock<IKnowledgeBaseRepository> _knowledgeBaseRepoMock = new();
    private readonly Mock<IRepository<KnowledgeDocument>> _documentRepoMock = new();
    private readonly Mock<IRepository<AIModel>> _modelRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IMediator> _mediatorMock = new();
    private readonly Mock<ILogger<KnowledgeService>> _loggerMock = new();
    private readonly KnowledgeService _service;

    public KnowledgeServiceTests()
    {
        _service = new KnowledgeService(
            _knowledgeBaseRepoMock.Object,
            _documentRepoMock.Object,
            _modelRepoMock.Object,
            _unitOfWorkMock.Object,
            _mediatorMock.Object,
            _loggerMock.Object);
    }

    private static KnowledgeBase BuildKnowledgeBase() => new()
    {
        Id = Guid.NewGuid(),
        Name = "Drills",
        KnowledgeBaseType = AIKnowledgeBaseType.Sports,
        OwnerType = AIResourceOwnerType.Academy,
        OwnerUserId = Guid.NewGuid(),
        ChunkingStrategy = AIChunkingStrategy.FixedSize,
        ChunkSize = 1024,
        ChunkOverlap = 100,
        IsActive = true,
    };

    [Fact]
    public async Task CreateAsync_InvalidEmbeddingModel_ReturnsFailure()
    {
        var modelId = Guid.NewGuid();
        _modelRepoMock.Setup(r => r.GetByIdAsync(modelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((AIModel?)null);
        var request = new CreateKnowledgeBaseRequest(
            "Drills", null, AIKnowledgeBaseType.Sports, AIResourceOwnerType.Academy, null,
            modelId, null, AIChunkingStrategy.FixedSize, 1024, 100, null);

        var result = await _service.CreateAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("embedding");
    }

    [Fact]
    public async Task CreateAsync_ValidRequest_AddsKnowledgeBase()
    {
        var request = new CreateKnowledgeBaseRequest(
            "Drills", null, AIKnowledgeBaseType.Sports, AIResourceOwnerType.Academy, null,
            null, null, AIChunkingStrategy.FixedSize, 1024, 100, null);

        var result = await _service.CreateAsync(request);

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Drills");
        result.Value.DocumentCount.Should().Be(0);
        _knowledgeBaseRepoMock.Verify(r => r.AddAsync(It.IsAny<KnowledgeBase>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AttachDocumentAsync_MissingKnowledgeBase_ReturnsFailure()
    {
        var knowledgeBaseId = Guid.NewGuid();
        _knowledgeBaseRepoMock.Setup(r => r.GetByIdAsync(knowledgeBaseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((KnowledgeBase?)null);

        var result = await _service.AttachDocumentAsync(new AttachDocumentRequest(
            knowledgeBaseId, "Drill", AIKnowledgeDocumentType.Text, "content", null, null, null, null));

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Knowledge base not found");
    }

    [Fact]
    public async Task AttachDocumentAsync_ValidRequest_ComputesHashAndSetsPending()
    {
        var knowledgeBase = BuildKnowledgeBase();
        _knowledgeBaseRepoMock.Setup(r => r.GetByIdAsync(knowledgeBase.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(knowledgeBase);

        var result = await _service.AttachDocumentAsync(new AttachDocumentRequest(
            knowledgeBase.Id, "Drill", AIKnowledgeDocumentType.Text, "content", null, null, null, null));

        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(AIDocumentStatus.Pending);
        result.Value.ContentHash.Should().NotBeNullOrEmpty();
        result.Value.ContentHash.Should().HaveLength(64);
        _documentRepoMock.Verify(r => r.AddAsync(It.Is<KnowledgeDocument>(d => d.ContentHash == result.Value.ContentHash), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DetachDocumentAsync_MissingDocument_ReturnsFailure()
    {
        var knowledgeBase = BuildKnowledgeBase();
        _knowledgeBaseRepoMock.Setup(r => r.GetByIdWithDetailsAsync(knowledgeBase.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(knowledgeBase);

        var result = await _service.DetachDocumentAsync(knowledgeBase.Id, Guid.NewGuid());

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Document not found");
    }

    [Fact]
    public async Task DetachDocumentAsync_ExistingDocument_SoftDeletes()
    {
        var knowledgeBase = BuildKnowledgeBase();
        var document = new KnowledgeDocument
        {
            Id = Guid.NewGuid(),
            KnowledgeBaseId = knowledgeBase.Id,
            Title = "Drill",
            ContentHash = "ABCDEF",
            Status = AIDocumentStatus.Indexed,
        };
        knowledgeBase.Documents.Add(document);
        _knowledgeBaseRepoMock.Setup(r => r.GetByIdWithDetailsAsync(knowledgeBase.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(knowledgeBase);

        var result = await _service.DetachDocumentAsync(knowledgeBase.Id, document.Id);

        result.IsSuccess.Should().BeTrue();
        document.IsDeleted.Should().BeTrue();
        _documentRepoMock.Verify(r => r.Update(document), Times.Once);
    }

    [Fact]
    public async Task RebuildIndexAsync_ResetsDocumentsAndStatistics()
    {
        var knowledgeBase = BuildKnowledgeBase();
        var processed = new KnowledgeDocument
        {
            Id = Guid.NewGuid(),
            KnowledgeBaseId = knowledgeBase.Id,
            Title = "Drill",
            ContentHash = "ABCDEF",
            Status = AIDocumentStatus.Indexed,
            ProcessedAt = DateTime.UtcNow.AddDays(-1),
        };
        var deleted = new KnowledgeDocument
        {
            Id = Guid.NewGuid(),
            KnowledgeBaseId = knowledgeBase.Id,
            Title = "Old",
            ContentHash = "123456",
            Status = AIDocumentStatus.Indexed,
            IsDeleted = true,
        };
        knowledgeBase.Documents.Add(processed);
        knowledgeBase.Documents.Add(deleted);
        _knowledgeBaseRepoMock.Setup(r => r.GetByIdWithDetailsAsync(knowledgeBase.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(knowledgeBase);

        var result = await _service.RebuildIndexAsync(new RebuildKnowledgeIndexRequest(knowledgeBase.Id));

        result.IsSuccess.Should().BeTrue();
        processed.Status.Should().Be(AIDocumentStatus.Pending);
        processed.ProcessedAt.Should().BeNull();
        result.Value.DocumentCount.Should().Be(1);
        knowledgeBase.StatisticsJson.Should().Contain("\"documentCount\":1");
        _documentRepoMock.Verify(r => r.Update(processed), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_PublishesUpdatedEvent()
    {
        var knowledgeBase = BuildKnowledgeBase();
        _knowledgeBaseRepoMock.Setup(r => r.GetByIdWithDetailsAsync(knowledgeBase.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(knowledgeBase);

        var result = await _service.UpdateAsync(new UpdateKnowledgeBaseRequest(
            knowledgeBase.Id, "New name", null, null, null, null, null, null, null, null, null, null));

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("New name");
        _mediatorMock.Verify(m => m.Publish(
            It.Is<KnowledgeBaseUpdatedEvent>(e => e.KnowledgeBaseId == knowledgeBase.Id),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
