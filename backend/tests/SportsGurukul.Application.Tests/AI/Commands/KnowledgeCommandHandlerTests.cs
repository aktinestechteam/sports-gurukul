using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.Commands.Knowledge;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Tests.AI.Commands;

public class AttachDocumentCommandHandlerTests
{
    private readonly Mock<IKnowledgeService> _serviceMock;
    private readonly AttachDocumentCommandHandler _handler;

    public AttachDocumentCommandHandlerTests()
    {
        _serviceMock = new Mock<IKnowledgeService>();
        _handler = new AttachDocumentCommandHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_DelegatesToService()
    {
        var knowledgeBaseId = Guid.NewGuid();
        var command = new AttachDocumentCommand(
            knowledgeBaseId, "Drill plan", AIKnowledgeDocumentType.Text, "Content", null,
            "/blob/drill.pdf", "application/pdf", null);
        var expected = Result<KnowledgeDocumentDto>.Success(new KnowledgeDocumentDto(
            Guid.NewGuid(), knowledgeBaseId, null, "Drill plan", AIKnowledgeDocumentType.Text,
            "ABCDEF", null, "/blob/drill.pdf", "application/pdf", 1, 500,
            AIDocumentStatus.Pending, null, DateTime.UtcNow));

        _serviceMock.Setup(s => s.AttachDocumentAsync(It.IsAny<AttachDocumentRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Title.Should().Be("Drill plan");
        _serviceMock.Verify(s => s.AttachDocumentAsync(
            It.Is<AttachDocumentRequest>(r => r.KnowledgeBaseId == knowledgeBaseId && r.StoragePath == "/blob/drill.pdf"),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}

public class RebuildKnowledgeIndexCommandHandlerTests
{
    private readonly Mock<IKnowledgeService> _serviceMock;
    private readonly RebuildKnowledgeIndexCommandHandler _handler;

    public RebuildKnowledgeIndexCommandHandlerTests()
    {
        _serviceMock = new Mock<IKnowledgeService>();
        _handler = new RebuildKnowledgeIndexCommandHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_DelegatesToService()
    {
        var knowledgeBaseId = Guid.NewGuid();
        var expected = Result<KnowledgeBaseDto>.Success(new KnowledgeBaseDto(
            knowledgeBaseId, "Drills", null, AIKnowledgeBaseType.Sports, AIResourceOwnerType.Athlete,
            Guid.NewGuid(), null, null, AIChunkingStrategy.FixedSize, 1024, 100, 1536, true,
            0, "{}", DateTime.UtcNow, null));

        _serviceMock.Setup(s => s.RebuildIndexAsync(It.IsAny<RebuildKnowledgeIndexRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _handler.Handle(new RebuildKnowledgeIndexCommand(knowledgeBaseId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(knowledgeBaseId);
        _serviceMock.Verify(s => s.RebuildIndexAsync(
            It.Is<RebuildKnowledgeIndexRequest>(r => r.KnowledgeBaseId == knowledgeBaseId),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
