using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.Commands.Prompt;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Tests.AI.Commands;

public class PublishPromptTemplateCommandHandlerTests
{
    private readonly Mock<IPromptService> _serviceMock;
    private readonly PublishPromptTemplateCommandHandler _handler;

    public PublishPromptTemplateCommandHandlerTests()
    {
        _serviceMock = new Mock<IPromptService>();
        _handler = new PublishPromptTemplateCommandHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_DelegatesToService()
    {
        var promptId = Guid.NewGuid();
        var assistantId = Guid.NewGuid();
        var command = new PublishPromptTemplateCommand(promptId, "Tone tweak", null);
        var expected = Result<PromptTemplateDto>.Success(BuildDto(promptId, assistantId, 2));

        _serviceMock.Setup(s => s.PublishAsync(It.IsAny<PublishPromptTemplateRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.CurrentVersion.Should().Be(2);
        _serviceMock.Verify(s => s.PublishAsync(
            It.Is<PublishPromptTemplateRequest>(r => r.PromptTemplateId == promptId && r.ChangeSummary == "Tone tweak"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    private static PromptTemplateDto BuildDto(Guid id, Guid assistantId, int version)
        => new(id, assistantId, "Drill prompt", null, AIPromptType.Template, "Explain {topic}",
            null, null, null, version, true, true, new List<PromptVersionDto>(), DateTime.UtcNow, null);
}

public class RollbackPromptVersionCommandHandlerTests
{
    private readonly Mock<IPromptService> _serviceMock;
    private readonly RollbackPromptVersionCommandHandler _handler;

    public RollbackPromptVersionCommandHandlerTests()
    {
        _serviceMock = new Mock<IPromptService>();
        _handler = new RollbackPromptVersionCommandHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_DelegatesToService()
    {
        var promptId = Guid.NewGuid();
        var command = new RollbackPromptVersionCommand(promptId, 1);
        var expected = Result<PromptTemplateDto>.Success(new PromptTemplateDto(
            promptId, Guid.NewGuid(), "Drill prompt", null, AIPromptType.Template, "Old template",
            null, null, null, 1, true, true, new List<PromptVersionDto>(), DateTime.UtcNow, null));

        _serviceMock.Setup(s => s.RollbackAsync(It.IsAny<RollbackPromptVersionRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.CurrentVersion.Should().Be(1);
        _serviceMock.Verify(s => s.RollbackAsync(
            It.Is<RollbackPromptVersionRequest>(r => r.VersionNumber == 1),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
