using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Application.Features.AIManagement.Services;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Tests.AI.Services;

public class PromptRendererTests
{
    private readonly Mock<IPromptRepository> _repoMock = new();
    private readonly Mock<ILogger<PromptRenderer>> _loggerMock = new();
    private readonly PromptRenderer _renderer;

    public PromptRendererTests()
    {
        _renderer = new PromptRenderer(_repoMock.Object, _loggerMock.Object);
    }

    [Fact]
    public void Render_SubstitutesAllVariables()
    {
        var template = "You are a {{role}} helping {{player}}.";
        var variables = new Dictionary<string, string>
        {
            ["role"] = "coach",
            ["player"] = "Rahul",
        };

        var result = _renderer.Render(template, variables);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("You are a coach helping Rahul.");
    }

    [Fact]
    public void Render_MissingVariable_LeavesPlaceholder()
    {
        var template = "Hello {{name}}!";

        var result = _renderer.Render(template, new Dictionary<string, string>());

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("Hello {{name}}!");
    }

    [Fact]
    public void Render_SubstitutionIsCaseInsensitive()
    {
        var template = "{{Role}} is ready";
        var variables = new Dictionary<string, string> { ["role"] = "Coach" };

        var result = _renderer.Render(template, variables);

        result.Value.Should().Be("Coach is ready");
    }

    [Fact]
    public async Task ResolveAndRenderAsync_UsesDefaultTemplate()
    {
        var assistantId = Guid.NewGuid();
        var template = new PromptTemplate
        {
            Id = Guid.NewGuid(),
            AssistantId = assistantId,
            Name = "Drill",
            PromptType = AIPromptType.Template,
            TemplateText = "Explain {{topic}}",
            IsActive = true,
            IsDefault = true,
        };
        _repoMock.Setup(r => r.GetDefaultByAssistantAsync(assistantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(template);

        var result = await _renderer.ResolveAndRenderAsync(
            assistantId, AIPromptType.Template, new Dictionary<string, string> { ["topic"] = "bowling" });

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("Explain bowling");
    }

    [Fact]
    public async Task ResolveAndRenderAsync_NoTemplate_ReturnsFailure()
    {
        var assistantId = Guid.NewGuid();
        _repoMock.Setup(r => r.GetDefaultByAssistantAsync(assistantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PromptTemplate?)null);
        _repoMock.Setup(r => r.GetActiveByAssistantAsync(assistantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PromptTemplate>());

        var result = await _renderer.ResolveAndRenderAsync(
            assistantId, AIPromptType.Template, new Dictionary<string, string>());

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("template");
    }
}
