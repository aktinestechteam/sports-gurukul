using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.Commands.Assistant;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Tests.AI.Commands;

public class CreateAssistantCommandHandlerTests
{
    private readonly Mock<IAssistantService> _serviceMock;
    private readonly CreateAssistantCommandHandler _handler;

    public CreateAssistantCommandHandlerTests()
    {
        _serviceMock = new Mock<IAssistantService>();
        _handler = new CreateAssistantCommandHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_DelegatesToService()
    {
        var command = new CreateAssistantCommand(
            "Coach", "Coach", null, AIAssistantType.Coach, null, null, null, null,
            null, true, false, AIResourceOwnerType.Athlete, Guid.NewGuid(), null, null, null);
        var expected = Result<AssistantDto>.Success(new AssistantDto(
            Guid.NewGuid(), "Coach", "Coach", null, AIAssistantType.Coach, null, null, null,
            null, null, null, true, false, true, AIResourceOwnerType.Athlete,
            command.OwnerUserId, null, null, new List<Guid>(), new List<Guid>(),
            DateTime.UtcNow, null));

        _serviceMock.Setup(s => s.CreateAsync(It.IsAny<CreateAssistantRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _serviceMock.Verify(s => s.CreateAsync(It.IsAny<CreateAssistantRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}

public class AssignKnowledgeBaseCommandHandlerTests
{
    private readonly Mock<IAssistantService> _serviceMock;
    private readonly AssignKnowledgeBaseCommandHandler _handler;

    public AssignKnowledgeBaseCommandHandlerTests()
    {
        _serviceMock = new Mock<IAssistantService>();
        _handler = new AssignKnowledgeBaseCommandHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_DelegatesToService()
    {
        var assistantId = Guid.NewGuid();
        var knowledgeBaseId = Guid.NewGuid();
        var command = new AssignKnowledgeBaseCommand(assistantId, new List<Guid> { knowledgeBaseId }, false);
        var expected = Result<AssistantDto>.Success(new AssistantDto(
            assistantId, "Coach", "Coach", null, AIAssistantType.Coach, null, null, null,
            null, null, null, true, false, true, AIResourceOwnerType.Athlete,
            null, null, null, new List<Guid> { knowledgeBaseId }, new List<Guid>(),
            DateTime.UtcNow, null));

        _serviceMock.Setup(s => s.AssignKnowledgeBaseAsync(It.IsAny<AssignKnowledgeBaseRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.AssignedKnowledgeBaseIds.Should().Contain(knowledgeBaseId);
        _serviceMock.Verify(s => s.AssignKnowledgeBaseAsync(It.IsAny<AssignKnowledgeBaseRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}

public class PublishAssistantCommandHandlerTests
{
    private readonly Mock<IAssistantService> _serviceMock;
    private readonly PublishAssistantCommandHandler _handler;

    public PublishAssistantCommandHandlerTests()
    {
        _serviceMock = new Mock<IAssistantService>();
        _handler = new PublishAssistantCommandHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_DelegatesToService()
    {
        var assistantId = Guid.NewGuid();
        var expected = Result<AssistantDto>.Success(new AssistantDto(
            assistantId, "Coach", "Coach", null, AIAssistantType.Coach, null, null, null,
            null, null, null, true, false, true, AIResourceOwnerType.Athlete,
            null, null, null, new List<Guid>(), new List<Guid>(), DateTime.UtcNow, null));

        _serviceMock.Setup(s => s.PublishAsync(assistantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _handler.Handle(new PublishAssistantCommand(assistantId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _serviceMock.Verify(s => s.PublishAsync(assistantId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
